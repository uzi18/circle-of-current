using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Diagnostics;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

using NetUtilities;
using LogUtilities;

namespace PokerServer
{
    public class Server
    {
        #region Fields

        private const int QUITTIMEOUT = 1000;

        private const int MAX_GAME_NUM = 10;

        private Socket listener;
        
        private Dictionary<string, LobbyPlayer> lobbyList = new Dictionary<string,LobbyPlayer>();
        private ManualResetEvent denyLobbyListAccess = new ManualResetEvent(true);

        private Dictionary<int, CGameRoom> roomList = new Dictionary<int,CGameRoom>();
        private Dictionary<string, CGameRoom> ticketRoomList = new Dictionary<string,CGameRoom>();
        private Dictionary<string, LobbyPlayer> ticketPlayerList = new Dictionary<string,LobbyPlayer>();

        private Thread workerThread;
        private volatile bool quitThread = false;
        private volatile bool hasQuit = false;

        #endregion

        #region Properties

        public string LocalEndPoint
        {
            get
            {
                return listener.LocalEndPoint.ToString();
            }
        }

        #endregion

        public Server(int port)
        {
            roomList.Add(0, new TexasHoldem("test", 500, 1000, this));

            listener = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            listener.Bind(new IPEndPoint(IPAddress.Any, port));
            listener.Listen(Properties.Settings.Default.serverbacklog);

            workerThread = new Thread(new ThreadStart(BackgroundThread));
            workerThread.IsBackground = true;
            workerThread.Priority = ThreadPriority.Lowest;

            OnChatEvent += new OnChatCall(DefaultOnChatEvent);
            
            listener.BeginAccept(new AsyncCallback(AcceptCallback), listener);
            workerThread.Start();
        }

        #region Event Handlers

        private void AcceptCallback(IAsyncResult ar)
        {
            try
            {
                Socket listener = (Socket)ar.AsyncState;
                Socket client = listener.EndAccept(ar);

                Logger.Log(LogType.Event, "New connection to server");

                NetTunnel tunnel = new NetTunnel(client);

                string conType = tunnel.WaitMessage();

                if (string.IsNullOrEmpty(conType))
                {
                    Logger.Log(LogType.Error, "Did not specify connection type");
                    return;
                }
                else if (conType == "login")
                {
                    NewLobbyPlayer(tunnel);
                }
                else if (conType.StartsWith("joingame:"))
                {
                    NewGamePlayer(tunnel, conType.Substring("joingame:".Length));
                }
                else if (conType == "testgame")
                {
                    NewTestGame(tunnel);
                }
                else if (conType == "register")
                {
                    RegisterNewPlayer(tunnel);
                }
                else if (conType == "forgot")
                {
                    ResetPassword(tunnel);
                }
            }
            catch (Exception ex)
            {
                Logger.Log("From Server's AcceptCallback while handling new connection", ex);
            }

            try
            {
                listener.BeginAccept(new AsyncCallback(AcceptCallback), listener);
            }
            catch (Exception ex)
            {
                Logger.Log("From Server's AcceptCallback while restarting the listener", ex);
            }
        }

        private void NewTestGame(NetTunnel tunnel)
        {
            TexasHoldem game = new TexasHoldem("Admin Test Game", 500, 1500, this);
            LobbyPlayer lob = new LobbyPlayer("Admin", new NetTunnel(), 9999, this);
            game.JoinGame(lob, tunnel);
        }

        private void ResetPassword(NetTunnel tunnel)
        {
            string username = tunnel.WaitMessage();
            string email = tunnel.WaitMessage();

            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(email))
                return;

            if (Database.ResetPassword(username, email))
            {
                tunnel.SendMessage("ok");
            }
            else
            {
                tunnel.SendMessage("fail");
            }
        }

        private void RegisterNewPlayer(NetTunnel tunnel)
        {
            string username = tunnel.WaitMessage();
            string email = tunnel.WaitMessage();

            RegistrationResult res = Database.RegisterNewPlayer(username, email);
            if (res == RegistrationResult.Successful)
                tunnel.SendMessage("ok");
            else
                tunnel.SendMessage("fail:" + Enum.GetName(typeof(RegistrationResult), res));
        }

        public void OnGlobalChatEvent(string name, string msg)
        {
            Logger.Log(LogType.Event, "Broadcasting chat from " + name);
            List<LobbyPlayer> broadcastList = new List<LobbyPlayer>(lobbyList.Values);
            foreach (LobbyPlayer lp in broadcastList)
            {
                lp.Chat(name, msg);
            }

            OnChatEvent(name, msg);
        }

        public string OnJoinGameRequestEvent(LobbyPlayer lobbyPlayer, int gameID)
        {
            Logger.Log(LogType.Event, "Player " + lobbyPlayer.Name + " requested to joing gameID " + gameID.ToString("X"));

            if (roomList.ContainsKey(gameID) && lobbyList.ContainsKey(lobbyPlayer.Name))
            {
                string guid = roomList[gameID].GetTicket(lobbyPlayer, ticketPlayerList.Keys.ToList());

                if (guid != null)
                {
                    ticketRoomList.Add(guid, roomList[gameID]);
                    ticketPlayerList.Add(guid, lobbyPlayer);

                    Logger.Log(LogType.Event, "Generated ticket for " + lobbyPlayer.Name);

                    return guid;
                }
                else
                    Logger.Log(LogType.Error, "Player " + lobbyPlayer.Name + " tried to join an room he's already in");
            }
            else
                Logger.Log(LogType.Error, "Player " + lobbyPlayer.Name + " tried to join non-existing game or player does not exist");

            return null;
        }

        public void OnRequestGameListEvent(LobbyPlayer lobbyPlayer)
        {
            lobbyPlayer.Tunnel.SendMessage("clrgames");

            for (int i = 0, j = 0; i < roomList.Count; j++)
            {
                if (roomList.ContainsKey(j))
                {
                    CGameRoom room = roomList[j];
                    lobbyPlayer.Tunnel.SendMessage(
                        String.Format(
                            "gameinfo:{0:0},{1},{2}/{3},{4}",
                            j,
                            room.Name,
                            room.PlayerCount,
                            room.MaxSeatNum,
                            room.ChipsInPlay
                        )
                    );

                    i++;
                }
            }
        }

        #endregion

        #region Events and Delegates

        #endregion

        #region Public Methods

        public void RemoveLobbyPlayer(LobbyPlayer lobbyPlayer)
        {
            if (lobbyList.ContainsKey(lobbyPlayer.Name))
            {
                lobbyList.Remove(lobbyPlayer.Name);
            }
        }

        public void Destroy()
        {
            List<LobbyPlayer> tmpList = new List<LobbyPlayer>(lobbyList.Values);

            foreach (LobbyPlayer i in tmpList)
                i.Destroy();

            Logger.Log(LogType.Event, "Server being destroyed");
            quitThread = true;

            Stopwatch sw = new Stopwatch();
            sw.Start();

            while (sw.ElapsedMilliseconds < QUITTIMEOUT && hasQuit == false && workerThread.IsAlive) ;

            if (workerThread.IsAlive || hasQuit == false)
            {
                workerThread.Abort();
            }
        }

        public void RemoveGame(CGameRoom room)
        {
            for (int i = 0; i < MAX_GAME_NUM; i++)
            {
                if (roomList.ContainsKey(i))
                {
                    if (roomList[i] == room)
                    {
                        roomList.Remove(i);
                    }
                }
            }
        }

        #endregion

        #region Manager Access Methods

        private delegate void ListBoxModifyCall(ListBox listbox, string[] entries);

        private void ListBoxFill(ListBox listbox, string[] entries)
        {
            if (listbox.InvokeRequired)
            {
                listbox.Invoke(new ListBoxModifyCall(ListBoxFill), new object[] { listbox, entries, });
            }
            else
            {
                listbox.Items.Clear();
                listbox.Items.AddRange(entries);
            }
        }

        public void FillPlayerListBox(ListBox listbox)
        {
            List<string> entries = new List<string>();
            List<LobbyPlayer> tmpList = new List<LobbyPlayer>(lobbyList.Values);
            foreach (LobbyPlayer lp in tmpList)
            {
                entries.Add(lp.Name);
            }

            ListBoxFill(listbox, entries.ToArray());
        }

        private delegate void TextBoxModifyCall(TextBox textbox, string text);

        private void TextBoxSet(TextBox textbox, string text)
        {
            if (textbox.InvokeRequired)
            {
                textbox.Invoke(new TextBoxModifyCall(TextBoxSet), new object[] { textbox, text, });
            }
            else
            {
                textbox.Text = text;
            }
        }

        public void GetLobbyPlayerInfo(string name, TextBox textbox)
        {
            if (lobbyList.ContainsKey(name))
            {
                LobbyPlayer player = lobbyList[name];
                TextBoxSet(textbox, String.Format(
                    "Name: {0}\r\nBank: {1:0}\r\nChips In Play: {2:0}",
                    name, player.Bank, player.ChipsInPlay
                    ));
            }
            else
            {
                TextBoxSet(textbox, name + " is not in list");
            }
        }

        public delegate void OnChatCall(string name, string msg);
        public event OnChatCall OnChatEvent;

        private void DefaultOnChatEvent(string name, string msg)
        {
        }

        #endregion

        #region Private Methods

        private void NewGamePlayer(NetTunnel tunnel, string ticket)
        {
            if (string.IsNullOrEmpty(ticket))
            {
                Logger.Log(LogType.Error, "Received an empty ticket");
                tunnel.Destroy();
                return;
            }

            if (ticketRoomList.ContainsKey(ticket) && ticketPlayerList.ContainsKey(ticket))
            {
                LobbyPlayer lobbyPlayer = ticketPlayerList[ticket];
                CGameRoom room = ticketRoomList[ticket];

                room.JoinGame(lobbyPlayer, tunnel);

                ticketPlayerList.Remove(ticket);
                ticketRoomList.Remove(ticket);

                Logger.Log(LogType.Event, "Player " + lobbyPlayer.Name + " joined " + room.Name);
            }
            else
            {
                Logger.Log(LogType.Error, "Received an invalid ticket");
                tunnel.Destroy();
                return;
            }
        }

        private void NewLobbyPlayer(NetTunnel tunnel)
        {
            string username = tunnel.WaitMessage();
            string hashedPass = tunnel.WaitMessage();

            if (string.IsNullOrEmpty(username))
            {
                Logger.Log(LogType.Error, "Blank username");
                tunnel.Destroy();
                return;
            }

            if (string.IsNullOrEmpty(hashedPass))
            {
                Logger.Log(LogType.Error, "Blank password");
                tunnel.Destroy();
                return;
            }

            LobbyPlayer lobbyPlayer = Database.Login(username, hashedPass, tunnel, this);

            if (lobbyPlayer == null)
            {
                tunnel.SendMessage("fail");
                tunnel.Destroy();
                Logger.Log(LogType.Event, "Bad login attempt (bad credentials) from " + username);
                return;
            }
            else if (lobbyList.ContainsKey(lobbyPlayer.Name))
            {
                if (lobbyList[lobbyPlayer.Name].Tunnel.SendMessage("alreadyin"))
                {
                    tunnel.SendMessage("fail");
                    tunnel.Destroy();
                    Logger.Log(LogType.Event, "Bad login attempt (existing player) from " + username);
                    return;
                }
                else
                {
                    tunnel.SendMessage("ok");
                }
            }
            else
            {
                tunnel.SendMessage("ok");
            }

            lobbyList.Add(lobbyPlayer.Name, lobbyPlayer);

            Logger.Log(LogType.Event, "Added player to lobby " + lobbyPlayer.Name);
        }

        private void BackgroundThread()
        {
            try
            {
                Logger.Log(LogType.Event, "Server BackgroundThread started");

                hasQuit = false;

                while (quitThread == false)
                {
                    try
                    {
                        List<LobbyPlayer> tmpList = new List<LobbyPlayer>(lobbyList.Values);
                        foreach (LobbyPlayer lp in tmpList)
                        {
                            if (lp.Connected == false)
                            {
                                lp.Destroy();
                                lobbyList.Remove(lp.Name);
                            }
                        }
                    }
                    catch (ThreadAbortException ex)
                    {

                    }
                    catch (Exception ex)
                    {
                        Logger.Log("From Server's BackgroundWorker", ex);
                    }

                    Thread.Sleep(500);
                }

                hasQuit = true;

                Logger.Log(LogType.Event, "Server BackgroundThread ended");
            }
            catch (ThreadAbortException ex)
            {

            }
            catch (Exception ex)
            {
                hasQuit = true;
                Logger.Log("Thread Failure from Server's BackgroundWorker", ex);
            }
        }

        #endregion
    }
}
