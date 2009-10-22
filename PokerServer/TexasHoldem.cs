using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PokerServer
{
    public class TexasHoldem : CGameRoom
    {
        #region Fields

        #endregion

        #region Properties

        #endregion

        #region Constructor

        public TexasHoldem(string name, int buyIn, int maxBuyIn, Server lobby) : base(name, lobby)
        {
            this.BUY_IN = buyIn;
            this.MAX_BUY_IN = maxBuyIn;
            this.AUTO_BUY_IN = buyIn <= 50 ? true : false;
            this.MAX_SEAT_NUM = 10;
            this.GAME_TYPE = this.GetType();

            this.name += " (Texas Holdem)";

            this.gameWorker.Start();
        }

        #endregion

        #region Delegates and Events

        #endregion

        #region Event Handlers

        #endregion

        #region Public Methods

        #endregion

        #region Private Methods

        protected override void GameLoop()
        {
            
        }

        #endregion

        #region Public Static Methods

        #endregion

        #region Private Static Methods

        #endregion
    }
}
