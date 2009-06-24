using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Diagnostics;
using System.Threading;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace PokerCoCServer
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            Application.Run(new MainMonitor());
        }
    }

    class GamePlayer
    {
        public string name;
        public TcpClient tc;
        public bool active;
        public int chips;

        public GamePlayer(TcpClient tc_, string n, int c)
        {
            tc = tc_;
            name = n;
            chips = c;
            active = false;
        }
    }

    class GameRoom
    {
        public string name;
        public int id;

        public GamePlayer[] player;

        public int chips
        {
            get
            {
                int j = 0;
                foreach (GamePlayer i in player)
                {
                    j += i.chips;
                }
                return j;
            }
        }

        public int player_cnt
        {
            get
            {
                int j = 0;
                foreach(GamePlayer i in player)
                {
                    if (i.active)
                    {
                        j++;
                    }
                }
                return j;
            }
        }

        public Thread game_work_thread;
        
        public GameRoom(string n, int i)
        {
            name = n;
            id = i;
            player = new GamePlayer[10];
            for (int j = 0; j < 10; j++)
            {
                player[j] = new GamePlayer(null, null, 0);
            }
        }

        private void game_work()
        {
            while (player_cnt > 0)
            {
            }
        }

        public bool AddPlayer(GamePlayer gp, int seat)
        {
            for (int i = seat; i < player.Length + seat; i++)
            {
                if (player[i % player.Length].active == false)
                {
                    player[i % player.Length] = gp;
                    player[i % player.Length].active = true;
                    if (player_cnt == 1)
                    {
                        game_work_thread = new Thread(new ThreadStart(game_work));
                        game_work_thread.Start();
                    }
                    return true;
                }
            }
            return false;

        }
    }

    struct LobbyListEntry
    {
        public TcpClient tc;
        public string name;
        public int i;
    }

    class Lobby
    {
        public TcpListener tcpL;
        public ArrayList lobby_list = new ArrayList();
        public ArrayList game_rooms = new ArrayList();

        public volatile int timeout;
        public volatile int room_management_speed;
        public volatile int connection_check_speed;
        public volatile int request_check_speed;

        public Thread room_man_thread;
        public Thread request_handle_thread;
        public Thread connection_check_thread;

        public static ManualResetEvent TcpClientConnected = new ManualResetEvent(false);

        public Lobby()
        {
            timeout = 100;
            room_management_speed = 2000;
            connection_check_speed = 1000;
            request_check_speed = 10;

            tcpL = new TcpListener(5000);
            tcpL.Start();
            TcpClientConnected.Reset();

            tcpL.BeginAcceptTcpClient(new AsyncCallback(NewClientEvent), tcpL);

            request_handle_thread = new Thread(new ThreadStart(RequestListener));
            request_handle_thread.Start();

            room_man_thread = new Thread(new ThreadStart(RoomManager));
            room_man_thread.Start();

            connection_check_thread = new Thread(new ThreadStart(CheckConnections));
            connection_check_thread.Start();
        }

        delegate void RoomMakerUnmakerCallback();

        private void RoomMakerUnmaker()
        {
            bool empty_found = false;
            for (int i = 0; i < game_rooms.Count; i++)
            {
                if (empty_found == false && ((GameRoom)game_rooms[i]).player_cnt == 0)
                {
                    empty_found = true;
                }
                else if (((GameRoom)game_rooms[i]).player_cnt == 0)
                {
                    //((GameRoom)game_rooms[i]).game_work_thread.Abort();
                    game_rooms.RemoveAt(i);
                    i = 0;
                }
                Thread.Sleep(0);
            }
            if (empty_found == false)
            {
                GameRoom gr = new GameRoom("Empty Room", game_rooms.Count);
                game_rooms.Add(gr);
            }
        }

        private void RoomManager()
        {
            while (true)
            {
                RoomMakerUnmakerCallback cb = new RoomMakerUnmakerCallback(RoomMakerUnmaker);
                cb.Invoke();

                Thread.Sleep(room_management_speed);
            }
        }

        delegate void RestartTcpListeningCallback();

        private void RestartTcpListening()
        {
            tcpL.BeginAcceptTcpClient(new AsyncCallback(NewClientEvent), tcpL);
        }

        private void NewClientEvent(IAsyncResult ar)
        {
            TcpListener listener = (TcpListener)ar.AsyncState;
            TcpClient client = listener.EndAcceptTcpClient(ar);
            TcpClientConnected.Set();
            string c;
            NetTunnel nt = new NetTunnel(client, timeout);
            int err = nt.Receive(out c);
            if (err == 0)
            {
                if (c == "joinlobby")
                {
                    string n;
                    err = nt.Receive(out n);
                    LobbyListEntry lle = new LobbyListEntry();
                    lle.name = n;
                    lle.tc = client;
                    err = nt.Send("Test String");

                    string ip_str = client.Client.LocalEndPoint.ToString();

                    lle.i = lobby_list.Count;

                    lobby_list.Add(lle);
                }
                else if (c == "joingame")
                {
                    string game_id;
                    string n;
                    string seat;
                    err = nt.Receive(out game_id);
                    err = nt.Receive(out n);
                    err = nt.Receive(out seat);
                    bool success = ((GameRoom)game_rooms[int.Parse(game_id)]).AddPlayer(new GamePlayer(client, n, 0), int.Parse(seat));
                    if (success)
                    {
                        err = nt.Send("You have joined " + game_id);
                    }
                    else
                    {
                        err = nt.Send("joined failed" + game_id);
                    }
                }

            }

            RestartTcpListeningCallback d = new RestartTcpListeningCallback(RestartTcpListening);
            d.Invoke();
        }

        delegate void RequestHandlerCallback();

        private void RequestHandler()
        {
            for (int i = 0; i < lobby_list.Count; i++)
            {
                while (((LobbyListEntry)lobby_list[i]).tc.Available > 0)
                {
                    string c;
                    TcpClient tc = ((LobbyListEntry)lobby_list[i]).tc;
                    NetTunnel nt = new NetTunnel(tc, timeout);
                    int err = nt.Receive(out c);
                    if (err == 0)
                    {
                        if (c == "requestroomlist")
                        {
                            for (int j = 0; j < game_rooms.Count; j++)
                            {
                                err = nt.Send(((GameRoom)game_rooms[j]).id.ToString() + "," + ((GameRoom)game_rooms[j]).name + "," + ((GameRoom)game_rooms[j]).player_cnt + "," + ((GameRoom)game_rooms[j]).chips);
                            }
                            err = nt.Send("endaddgame");
                            //MessageBox.Show("request received, " + Convert.ToString(err));
                        }
                        else if (c == "requsers")
                        {
                            string game_id;
                            err = nt.Receive(out game_id);
                            for (int j = 0; j < ((GameRoom)game_rooms[int.Parse(game_id)]).player.Length; j++)
                            {
                                if (((GameRoom)game_rooms[int.Parse(game_id)]).player[j].active)
                                {
                                    err = nt.Send(((GameRoom)game_rooms[int.Parse(game_id)]).player[j].name + "," + ((GameRoom)game_rooms[int.Parse(game_id)]).player[j].chips);
                                }
                            }
                            err = nt.Send("endplayerlist");
                        }
                    }
                    Thread.Sleep(0);
                }
                Thread.Sleep(0);
            }
        }

        private void RequestListener()
        {
            while (true)
            {
                RequestHandlerCallback d = new RequestHandlerCallback(RequestHandler);
                d.Invoke();

                Thread.Sleep(request_check_speed);
            }
        }

        delegate void LobbyConnectionCheckerCallback();

        public void LobbyConnectionChecker()
        {
            for (int i = 0; i < lobby_list.Count; i++)
            {
                if (((LobbyListEntry)lobby_list[i]).tc.Client.Connected == false)
                {
                    int to_remove = ((LobbyListEntry)lobby_list[i]).i;
                    lobby_list.RemoveAt(i);
                    i = -1;
                }
                else
                {
                    try
                    {
                        ((LobbyListEntry)lobby_list[i]).tc.GetStream().WriteByte(0);
                    }
                    catch (Exception ex)
                    {
                        int to_remove = ((LobbyListEntry)lobby_list[i]).i;
                        lobby_list.RemoveAt(i);
                        i = -1;
                    }
                }
                Thread.Sleep(0);
            }
        }

        private void CheckConnections()
        {
            while (true)
            {
                LobbyConnectionCheckerCallback d = new LobbyConnectionCheckerCallback(LobbyConnectionChecker);
                d.Invoke();

                Thread.Sleep(connection_check_speed);
            }
        }
    }
}
