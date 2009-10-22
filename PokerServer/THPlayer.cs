using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using NetUtilities;
using LogUtilities;

namespace PokerServer
{
    public class THPlayer : CGamePlayer
    {
        #region Fields

        #endregion

        #region Properties

        #endregion

        #region Constructor

        public THPlayer(LobbyPlayer lobbyPlayer, NetTunnel tunnel, CGameRoom gameRoom) : base(lobbyPlayer, tunnel, gameRoom)
        {
            tunnel.OnMessageReceivedEvent += new NetTunnel.OnMessageReceivedCall(OnMessageReceivedEvent);
        }

        #endregion

        #region Events and Delegates

        #endregion

        #region Event Handlers

        protected override bool OnMessageReceivedEvent(string msg)
        {
            return base.OnMessageReceivedEvent(msg);
        }

        #endregion

        #region Private Methods

        #endregion

        #region Public Methods

        #endregion

        #region Static Fields and Properties

        #endregion

        #region Public Static Methods

        #endregion

        #region Private Static Methods

        #endregion
    }
}
