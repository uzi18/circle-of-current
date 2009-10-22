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
    public enum PlayerStatus
    {
        Sitting,
        Standing,
        SittingOut
    }

    public class GamePlayer
    {
        #region Fields

        private LobbyPlayer lobbyPlayer;
        private GameRoom gameRoom;

        private string name;
        private NetTunnel tunnel;
        private int wallet = 0;
        private int bank;

        private PlayerStatus status = PlayerStatus.Standing;

        #endregion

        #region Properties

        public string Name
        {
            get { return name; }
        }

        public NetTunnel Tunnel
        {
            get { return tunnel; }
        }

        public int Bank
        {
            get { return bank; }
        }

        public int Wallet
        {
            get { return wallet; }
        }

        public PlayerStatus Status
        {
            get { return status; }
            set { status = value; }
        }

        #endregion

        private GamePlayer(LobbyPlayer lobbyPlayer, NetTunnel tunnel, GameRoom room)
        {
            this.name = lobbyPlayer.Name;
            this.tunnel = tunnel;
            this.bank = lobbyPlayer.Bank;

            this.lobbyPlayer = lobbyPlayer;
            this.gameRoom = room;

            this.tunnel.OnMessageReceivedEvent += new NetTunnel.OnMessageReceivedCall(tunnel_OnMessageReceivedEvent);
            this.tunnel.OnUnexpectedDisconnectEvent += new NetTunnel.OnUnexpectedDisconnectCall(tunnel_OnUnexpectedDisconnectEvent);
        }

        private void tunnel_OnUnexpectedDisconnectEvent(NetTunnel tunnel)
        {
            this.QuitRoom();
        }

        private bool tunnel_OnMessageReceivedEvent(string msg)
        {
            if (msg.StartsWith("sit:"))
            {
                int seatnum = int.MaxValue;
                if (int.TryParse(msg.Substring("sit:".Length), out seatnum))
                {
                    if (gameRoom.OnAttemptSitEvent(this, seatnum))
                    {
                        Logger.Log(LogType.Event, name + " sat down");
                        this.status = PlayerStatus.Sitting;
                    }
                    else
                    {
                        tunnel.SendMessage("fail");
                        Logger.Log(LogType.Error, name + " failed to sit");
                    }
                }
                else
                {
                    Logger.Log(LogType.Error, name + " sent a malformed message '" + msg + "'");
                }

                return true;
            }
            else if (msg.StartsWith("chat:"))
            {
                gameRoom.OnChatEvent(name, msg.Substring("chat:".Length));
                return true;
            }
            else if (msg == "stand")
            {
                gameRoom.OnStandEvent(this);
                return true;
            }
            else if (msg == "reqbank")
            {
                this.SetBank(this.bank);
                return true;
            }
            else if (msg == "quit")
            {
                QuitRoom();

                return true;
            }
            else if (msg.StartsWith("withdraw:"))
            {
                int amount = int.MaxValue;
                if (int.TryParse(msg.Substring("withdraw:".Length), out amount))
                {
                    WithdrawBank(amount);
                    gameRoom.BroadcastSeatInfo(this);
                }

                return true;
            }

            return false;
        }

        #region Events and Delegates

        #endregion

        #region Event Handlers

        #endregion

        #region Private Methods

        #endregion

        #region Public Methods

        public bool Chat(string name, string msg)
        {
            return tunnel.SendMessage(String.Format("chat:{0}: {1}", name, msg));
        }

        public void Destroy()
        {
            QuitRoom();
        }

        public void WithdrawBank(int withdrawAmount)
        {
            if (this.bank >= withdrawAmount)
            {
                this.bank -= withdrawAmount;
                this.wallet += withdrawAmount;
            }
            else
            {
                this.bank = 0;
                this.wallet = this.gameRoom.BuyIn;
            }

            this.lobbyPlayer.SetBank(this.bank);
            this.tunnel.SendMessage("bank:" + this.bank.ToString("0"));
            this.tunnel.SendMessage("wallet:" + this.wallet.ToString("0"));
        }

        public void DepositBank(int depositAmount)
        {
            if (depositAmount <= this.wallet)
            {
                this.bank += depositAmount;
                this.wallet -= depositAmount;
            }
            else
            {
                this.bank += this.wallet;
                this.wallet = 0;
            }

            this.lobbyPlayer.SetBank(this.bank);
            this.tunnel.SendMessage("bank:" + this.bank.ToString("0"));
            this.tunnel.SendMessage("wallet:" + this.wallet.ToString("0"));
        }

        public void SetBank(int amount)
        {
            this.bank = amount;
            this.tunnel.SendMessage("bank:" + this.bank.ToString("0"));
        }

        public void QuitRoom()
        {
            this.bank += this.wallet;
            this.lobbyPlayer.SetBank(this.bank);

            gameRoom.OnQuitEvent(this);

            //lobbyPlayer.GameRoomPlayers.Remove(this);

            tunnel.Destroy();
        }

        #endregion

        public static GamePlayer GamePlayerFromLobbyPlayer(LobbyPlayer player, NetTunnel tunnel, GameRoom room)
        {
            return new GamePlayer(player, tunnel, room);
        }
    }
}
