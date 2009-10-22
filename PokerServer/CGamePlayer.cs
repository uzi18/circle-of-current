using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using NetUtilities;
using LogUtilities;

namespace PokerServer
{
    public enum PlayerStatus
    {
        Standing,
        Playing,
        SittingOut,
        Empty,
    }

    public class CGamePlayer
    {
        #region Fields

        private LobbyPlayer lobbyEntity;
        private CGameRoom gameRoom;

        private NetTunnel tunnel;

        private string name = "";
        private int bank = 0;
        private int wallet = 0;
        private PlayerStatus status = PlayerStatus.Standing;

        #endregion

        #region Properties

        public string Name
        {
            get { return name; }
        }

        public int Bank
        {
            get { return bank; }
        }

        public int Wallet
        {
            get { return wallet; }
        }

        public NetTunnel Tunnel
        {
            get { return tunnel; }
        }

        public LobbyPlayer LobbyEntity
        {
            get { return lobbyEntity; }
        }

        public PlayerStatus Status
        {
            get { return status; }
            set { status = value; }
        }

        #endregion

        #region Constructor

        protected CGamePlayer(LobbyPlayer lobbyPlayer, NetTunnel tunnel, CGameRoom gameRoom)
        {
            this.name = lobbyPlayer.Name;
            this.tunnel = tunnel;
            this.tunnel.Identifier = this.name + " (GamePlayer)";
            this.bank = lobbyPlayer.Bank;

            this.lobbyEntity = lobbyPlayer;
            this.lobbyEntity.GameRoomPlayers.Add(this);

            this.gameRoom = gameRoom;

            this.tunnel.OnUnexpectedDisconnectEvent += new NetTunnel.OnUnexpectedDisconnectCall(OnUnexpectedDisconnectEvent);
        }

        #endregion

        #region Delegates and Events

        #endregion

        #region Event Handlers

        protected virtual bool OnMessageReceivedEvent(string msg)
        {
            if (msg.StartsWith("chat:"))
            {
                gameRoom.BroadcastChat(name, msg.Substring("chat:".Length));

                return true;
            }
            else if (msg == "reqbank")
            {
                Logger.Log(LogType.Debug, name + " requested bank amount");

                SetBank(bank);

                return true;
            }
            else if (msg == "quit")
            {
                Logger.Log(LogType.Debug, name + " is quitting game room");

                Quit();

                return true;
            }
            else if (msg.StartsWith("sit:"))
            {
                int i = 0;
                if (int.TryParse(msg.Substring("sit:".Length), out i))
                {
                    Logger.Log(LogType.Debug, name + " is trying to sit in " + gameRoom.Name);

                    if (gameRoom.SitDown(this, i) == false)
                    {
                        // ok was sent inside gameRoom.SitDown along with all seat info
                        tunnel.SendMessage("fail");
                    }
                }
                else
                {
                    Logger.Log(LogType.Error, name + " sent sit request with bad integer");

                    tunnel.SendMessage("sit:fail");
                }

                return true;
            }
            else if (msg == "stand")
            {
                Logger.Log(LogType.Debug, name + " stood up in " + gameRoom.Name);

                DepositBank(wallet);
                gameRoom.StandUp(this);

                return true;
            }
            else if (msg.StartsWith("sitin"))
            {
                Logger.Log(LogType.Debug, name + " is starting to play in " + gameRoom.Name);

                gameRoom.SitIn(this);

                return true;
            }
            else if (msg.StartsWith("sitout"))
            {
                Logger.Log(LogType.Debug, name + " is sitting out in " + gameRoom.Name);

                gameRoom.SitOut(this);

                return true;
            }
            else if (msg == "refill")
            {
                Logger.Log(LogType.Debug, name + " requested a refill");

                int chipsInPlay = lobbyEntity.ChipsInPlay;
                int totalChips = lobbyEntity.Bank + chipsInPlay;
                if (totalChips < gameRoom.BuyIn)
                {
                    lobbyEntity.SetBank(gameRoom.BuyIn - chipsInPlay);
                }

                return true;
            }
            else if (msg.StartsWith("withdraw:"))
            {
                Logger.Log(LogType.Debug, name + " is trying to withdraw");

                int i = 0;
                if (int.TryParse(msg.Substring("withdraw:".Length), out i))
                {
                    WithdrawBank(i);
                }
                else
                {
                    SetBank(bank);

                    Logger.Log(LogType.Error, name + " tried to withdraw a non-integer amount");
                }

                return true;
            }
            else if (msg.StartsWith("deposit:"))
            {
                Logger.Log(LogType.Debug, name + " is trying to deposit");

                int i = 0;
                if (int.TryParse(msg.Substring("deposit:".Length), out i))
                {
                    DepositBank(i);
                }
                else
                {
                    SetBank(bank);

                    Logger.Log(LogType.Error, name + " tried to deposit a non-integer amount");
                }

                return true;
            }

            return false;
        }

        private void OnUnexpectedDisconnectEvent(NetTunnel tunnel)
        {
            Logger.Log(LogType.Debug, "Unexpected Disconnect Event from GamePlayer " + name);
            Quit();
        }

        #endregion

        #region Public Methods

        #endregion

        #region Private Methods

        private void WithdrawBank(int withdrawAmount)
        {
            if (status != PlayerStatus.SittingOut)
            {
                Logger.Log(LogType.Error, name + " tried to make a withdraw while not sitting out");

                return;
            }

            if (bank < withdrawAmount)
            {
                if (wallet + bank <= gameRoom.MaxBuyIn)
                {
                    wallet += bank;
                    bank = 0;
                }
                else
                {
                    WithdrawBank(gameRoom.MaxBuyIn - wallet);
                    return;
                }
            }
            else
            {
                if (wallet + withdrawAmount <= gameRoom.MaxBuyIn)
                {
                    wallet += withdrawAmount;
                    bank -= withdrawAmount;
                }
                else
                {
                    WithdrawBank(gameRoom.MaxBuyIn - wallet);
                    return;
                }
            }

            lobbyEntity.SetBank(bank);
            UpdateBankWallet();
        }

        private void DepositBank(int depositAmount)
        {
            if (wallet < depositAmount)
            {
                bank += wallet;
                wallet = 0;
            }
            else
            {
                wallet -= depositAmount;
                bank += depositAmount;
            }

            lobbyEntity.SetBank(bank);
            UpdateBankWallet();
        }

        protected void UpdateBankWallet()
        {
            tunnel.SendMessage("bank:" + bank.ToString("0"));
            tunnel.SendMessage("wallet:" + wallet.ToString("0"));
            gameRoom.BroadcastSeatInfo(this);
        }

        #endregion

        #region Public Static Methods

        public void DepositWallet()
        {
            DepositBank(wallet);
        }

        public void SetBank(int amount)
        {
            bank = amount;
            UpdateBankWallet();
        }

        public void Quit()
        {
            DepositBank(wallet);
            gameRoom.LeaveGame(this);
            lobbyEntity.GameRoomPlayers.Remove(this);
            tunnel.Destroy();
        }

        public void Destroy()
        {
            DepositBank(wallet);
            tunnel.Destroy();
        }

        #endregion

        #region Private Static Methods

        #endregion
    }
}
