using System;
using System.Threading;
using System.Diagnostics;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using NetUtilities;
using LogUtilities;

namespace PokerServer
{
    public class CGameRoom
    {
        #region Fields

        protected int MAX_SEAT_NUM;
        protected int BUY_IN;
        protected int MAX_BUY_IN;
        protected bool AUTO_BUY_IN;
        protected Type GAME_TYPE;

        private Server ownerLobby;

        protected Dictionary<string, CGamePlayer> spectators = new Dictionary<string, CGamePlayer>();
        protected Dictionary<int, CGamePlayer> table = new Dictionary<int, CGamePlayer>();
        protected Dictionary<CGamePlayer, int> tableR = new Dictionary<CGamePlayer, int>();

        protected string name = "";

        protected Thread gameWorker;
        protected bool shutdown = false;
        protected bool hasquit = false;

        #endregion

        #region Properties

        public string Name
        {
            get { return name; }
        }

        public int ChipsInPlay
        {
            get
            {
                List<CGamePlayer> tmpList = new List<CGamePlayer>(table.Values);
                int cnt = 0;
                foreach (CGamePlayer i in tmpList)
                    cnt += i.Wallet;

                return cnt;
            }
        }

        public int PlayerCount
        {
            get { return table.Count; }
        }

        public int MaxSeatNum
        {
            get { return MAX_SEAT_NUM; }
        }

        public int BuyIn
        {
            get { return BUY_IN; }
        }

        public int MaxBuyIn
        {
            get { return MAX_BUY_IN; }
        }

        public bool AutoBuyIn
        {
            get { return AUTO_BUY_IN; }
        }

        public Type GameType
        {
            get { return GAME_TYPE; }
        }

        #endregion

        #region Constructor

        protected CGameRoom(string name, Server lobby)
        {
            this.name = name;
            this.ownerLobby = lobby;

            gameWorker = new Thread(new ThreadStart(GameWorker));
            gameWorker.IsBackground = true;
            gameWorker.Priority = ThreadPriority.Lowest;
        }

        #endregion

        #region Delegates and Events

        #endregion

        #region Event Handlers

        #endregion

        #region Public Methods

        public void BroadcastChat(string name, string msg)
        {
            Logger.Log(LogType.Debug, "Broadcasting Chat");

            List<CGamePlayer> tmpList = new List<CGamePlayer>(spectators.Values);

            foreach (CGamePlayer i in tmpList)
                i.Tunnel.SendMessage(String.Format("chat:{0}:{1}", name, msg));
        }

        public bool JoinGame(LobbyPlayer lobbyPlayer, NetTunnel tunnel)
        {
            string pname = lobbyPlayer.Name;
            if (spectators.ContainsKey(pname))
            {
                // should trigger unexpected disconnect and remove itself from list if disconnected
                if (spectators[pname].Tunnel.Ping())
                {
                    Logger.Log(LogType.Error, pname + " is already a spectator when he suppied the correct ticket to join " + name);

                    tunnel.SendMessage("fail");
                    return false;
                }
            }

            CGamePlayer newPlayer = null;

            if (GAME_TYPE == typeof(TexasHoldem))
                newPlayer = new THPlayer(lobbyPlayer, tunnel, this);
            else
            {
                Logger.Log(LogType.Error, "unknown game type");
                tunnel.SendMessage("fail");
                return false;
            }

            spectators.Add(pname, newPlayer);

            Logger.Log(LogType.Debug, pname + " joined game " + name);

            newPlayer.Tunnel.SendMessage(newPlayer.Name);
            newPlayer.Tunnel.SendMessage(this.name);
            newPlayer.Tunnel.SendMessage(BUY_IN.ToString("0"));
            newPlayer.Tunnel.SendMessage(MAX_BUY_IN.ToString("0"));

            newPlayer.Status = PlayerStatus.Standing;

            SendAllSeatInfo(newPlayer);

            return true;
        }

        public void LeaveGame(CGamePlayer player)
        {
            Logger.Log(LogType.Debug, player.Name + " is leaving game " + name);

            string pname = player.Name;
            if (spectators.ContainsKey(pname))
                spectators.Remove(pname);
            if (tableR.ContainsKey(player))
            {
                int seat = tableR[player];
                tableR.Remove(player);
                if (table.ContainsKey(seat))
                    table.Remove(seat);

                BroadcastSeatInfo(seat);
            }
        }

        public bool SitDown(CGamePlayer player, int seatnum)
        {
            if (seatnum >= MAX_SEAT_NUM)
            {
                Logger.Log(LogType.Error, player.Name + " tried to sit in an invalid seat in " + name);
                return false;
            }

            if (table.ContainsKey(seatnum))
            {
                Logger.Log(LogType.Error, player.Name + " tried to sit in occupied seat in " + name);
                return false;
            }

            if (player.Bank < BUY_IN)
            {
                Logger.Log(LogType.Error, player.Name + " tried to sit with a low bank amount in " + name);
                return false;
            }

            // note that the player's OnMsgRx event sends the "fail" if returned false
            // but this SitDown method sends the "ok"

            table.Add(seatnum, player);
            tableR.Add(player, seatnum);

            player.Status = PlayerStatus.SittingOut;

            Logger.Log(LogType.Debug, player.Name + " is now sitting in " + seatnum.ToString("0") + " in " + name);

            player.Tunnel.SendMessage("sit:ok");

            BroadcastSeatInfo(seatnum);

            return true;
        }

        public void StandUp(CGamePlayer player)
        {
            if (tableR.ContainsKey(player))
            {
                Logger.Log(LogType.Debug, player.Name + " stood up in " + name);

                int seat = tableR[player];
                table.Remove(seat);
                tableR.Remove(player);

                player.DepositWallet();

                BroadcastSeatInfo(seat);
            }
            else
            {
                Logger.Log(LogType.Error, player.Name + " tried to stand without sitting in " + name);
            }
        }

        public bool SitIn(CGamePlayer player)
        {
            if (tableR.ContainsKey(player))
            {
                player.Status = PlayerStatus.Playing;

                BroadcastSeatInfo(tableR[player]);

                return true;
            }

            return false;
        }

        public bool SitOut(CGamePlayer player)
        {
            if (tableR.ContainsKey(player))
            {
                player.Status = PlayerStatus.SittingOut;

                BroadcastSeatInfo(tableR[player]);

                return true;
            }

            return false;
        }

        public string GetTicket(LobbyPlayer lobbyPlayer, List<string> existingGUIDList)
        {
            if (spectators.ContainsKey(lobbyPlayer.Name))
            {
                if (spectators[lobbyPlayer.Name].Tunnel.Ping())
                {
                    Logger.Log(LogType.Error, lobbyPlayer.Name + " tried to get a ticket from " + name + " while already in the room");

                    return null;
                }
            }

            string guid = Guid.NewGuid().ToString();

            while (existingGUIDList.Contains(guid))
                guid = Guid.NewGuid().ToString();

            return guid;
        }

        public virtual void ShutDown()
        {
            Logger.Log(LogType.Debug, name + " is shutting down");

            List<CGamePlayer> tmpList = new List<CGamePlayer>(spectators.Values);

            foreach (CGamePlayer i in tmpList)
                i.Destroy();

            ownerLobby.RemoveGame(this);
        }

        #endregion

        #region Private Methods

        protected virtual void GameLoop()
        {
            throw new NotImplementedException("Please Override GameLoop");
        }

        private void GameWorker()
        {
            try
            {
                while (shutdown == false)
                {
                    try
                    {
                        GameLoop();

                        Thread.Sleep(500);
                    }
                    catch (ThreadAbortException)
                    {
                        Logger.Log(LogType.Error, "GameRoom Worker Thread Aborting(1) " + name);
                    }
                    catch (Exception ex)
                    {
                        Logger.Log("From GameRoom Worker " + name, ex);
                    }
                }

                hasquit = true;
                ShutDown();
            }
            catch (ThreadAbortException)
            {
                Logger.Log(LogType.Error, "GameRoom Worker Thread Aborting(2) " + name);
                hasquit = true;
                ShutDown();
            }
            catch (Exception ex)
            {
                Logger.Log("Thread Failure From GameRoom Worker " + name, ex);
            }
        }

        protected void SendAllSeatInfo(CGamePlayer receiver)
        {
            for (int i = 0; i < this.MaxSeatNum; i++)
                SendSeatInfo(receiver, i);
        }

        public void BroadcastSeatInfo(CGamePlayer player)
        {
            if (tableR.ContainsKey(player))
                BroadcastSeatInfo(tableR[player]);
        }

        protected void BroadcastSeatInfo(int seatnum)
        {
            List<CGamePlayer> tmpList = new List<CGamePlayer>(spectators.Values);

            foreach (CGamePlayer i in tmpList)
                SendSeatInfo(i, seatnum);
        }

        protected void BroadcastAllSeatInfo()
        {
            List<CGamePlayer> tmpList = new List<CGamePlayer>(spectators.Values);

            foreach (CGamePlayer i in tmpList)
                SendAllSeatInfo(i);
        }

        protected virtual void SendSeatInfo(CGamePlayer receiver, int seatnum)
        {
            if (table.ContainsKey(seatnum))
            {
                CGamePlayer player = table[seatnum];
                receiver.Tunnel.SendMessage(String.Format("seatinfo:{0:0},{1},{2:0},{3}", seatnum, player.Name, player.Wallet, Enum.GetName(typeof(PlayerStatus), player.Status)));
            }
            else
            {
                receiver.Tunnel.SendMessage(String.Format("seatinfo:{0:0},{1},{2:0},{3}", seatnum, "", 0, "Empty"));
            }
        }

        #endregion

        #region Public Static Methods

        #endregion

        #region Private Static Methods

        #endregion
    }
}
