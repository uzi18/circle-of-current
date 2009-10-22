using System;
using System.Net;
using System.Net.Sockets;
using System.IO;
using System.Threading;
using System.Diagnostics;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using NetUtilities;
using LogUtilities;

namespace PokerServer
{
    public class LobbyPlayer
    {
        #region Fields

        private Server lobby;

        private NetTunnel tunnel;
        private string name;
        private int bank;
        private List<CGamePlayer> gamePlayers = new List<CGamePlayer>();

        #endregion

        #region Properties

        public NetTunnel Tunnel
        {
            get { return tunnel; }
        }

        public int Bank
        {
            get { return bank; }
        }

        public int TotalChips
        {
            get
            {
                return bank + ChipsInPlay;
            }
        }

        public int ChipsInPlay
        {
            get
            {
                int cnt = 0;
                List<CGamePlayer> tmpList = new List<CGamePlayer>(gamePlayers);
                foreach (CGamePlayer i in tmpList)
                    cnt += i.Wallet;
                return cnt;
            }
        }

        public bool Connected
        {
            get { return tunnel.Connected; }
        }

        public string Name
        {
            get { return name; }
        }

        public List<CGamePlayer> GameRoomPlayers
        {
            get { return gamePlayers; }
        }

        #endregion

        public LobbyPlayer(string username, NetTunnel tunnel, int bank, Server lobby)
        {
            this.lobby = lobby;
            this.name = username;
            this.bank = bank;
            this.tunnel = tunnel;
            this.tunnel.Identifier = username + " (LobbyPlayer)";
            this.tunnel.OnMessageReceivedEvent += new NetTunnel.OnMessageReceivedCall(tunnel_OnMessageReceivedEvent);
            this.tunnel.OnUnexpectedDisconnectEvent += new NetTunnel.OnUnexpectedDisconnectCall(tunnel_OnUnexpectedDisconnectEvent);
        }

        #region Delegates and Events

        #endregion

        #region Event Handlers

        private bool tunnel_OnMessageReceivedEvent(string msg)
        {
            if (msg.StartsWith("chat:"))
            {
                lobby.OnGlobalChatEvent(name, msg.Substring("chat:".Length));
                return true;
            }
            else if (msg.StartsWith("join:"))
            {
                int gameID = -1;
                string gameIDStr = msg.Substring("join:".Length);
                if (int.TryParse(gameIDStr, out gameID))
                {
                    string ticket = lobby.OnJoinGameRequestEvent(this, gameID);
                    if (string.IsNullOrEmpty(ticket))
                        this.tunnel.SendMessage("ticket:invalid");
                    else
                        this.tunnel.SendMessage("ticket:" + ticket);
                }
                else
                {
                    Logger.Log(LogType.Error, name + " tried to join with non-integer gameID");

                    this.tunnel.SendMessage("ticket:invalid");
                }

                return true;
            }
            else if (msg == "quit")
            {
                Logger.Log(LogType.Event, name + " has quit");

                Quit();

                return true;
            }
            else if (msg == "reqgames")
            {
                Logger.Log(LogType.Debug, name + " requested game list");
                lobby.OnRequestGameListEvent(this);

                return true;
            }
            else if (msg == "reqbank")
            {
                Logger.Log(LogType.Debug, name + " requested bank amount");

                tunnel.SendMessage("bank:" + this.bank.ToString("0"));

                return true;
            }

            return false;
        }

        private void tunnel_OnUnexpectedDisconnectEvent(NetTunnel tunnel)
        {
            Logger.Log(LogType.Debug, "Unexpected Disconnect Event from LobbyPlayer" + name);

            Quit();
        }

        #endregion

        #region Private Methods

        private void Quit()
        {
            lobby.RemoveLobbyPlayer(this);
            Destroy();
        }

        #endregion

        #region Public Methods

        public bool Chat(string name, string msg)
        {
            return tunnel.SendMessage(String.Format("chat:{0}: {1}", name, msg));
        }

        public void SetBank(int amount)
        {
            bank = amount;
            tunnel.SendMessage("bank:" + bank.ToString("0"));

            List<CGamePlayer> tmpList = new List<CGamePlayer>(gamePlayers);
            foreach (CGamePlayer i in tmpList)
                i.SetBank(bank);
        }

        public void Destroy()
        {
            Logger.Log(LogType.Event, this.name + " has left");

            List<CGamePlayer> tmpList = new List<CGamePlayer>(gamePlayers);
            foreach (CGamePlayer i in tmpList)
                i.Quit();

            Database.SaveBank(this.Name, this.bank);

            tunnel.Destroy();
        }

        public override string ToString()
        {
            return String.Format(
                "LobbyPlayer: Name: {0}",
                name
                );
        }

        #endregion
    }
}
