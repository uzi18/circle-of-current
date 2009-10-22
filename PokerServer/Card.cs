using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PokerServer
{
    public enum CardValue : int
    {
        Ace = 1,
        Two,
        Three,
        Four,
        Five,
        Six,
        Seven,
        Eight,
        Nine,
        Ten,
        Jack,
        Queen,
        King
    }

    public enum CardSuite : int
    {
        Heart = 0,
        Diamond,
        Club,
        Spade,
        Count
    }

    public struct Card
    {
        private CardValue v;
        private CardSuite s;

        public CardValue Value
        {
            get { return v; }
            set { v = value; }
        }

        public CardSuite Suite
        {
            get { return s; }
            set { s = value; }
        }

        public Card(CardValue value, CardSuite suite)
        {
            v = value;
            s = suite;
        }

        public Card(int value, int suite)
        {
            v = (CardValue)value;
            s = (CardSuite)suite;
        }

        public override string ToString()
        {
            return String.Format(
                "{0} of {1}",
                Enum.GetName(typeof(CardValue), v),
                Enum.GetName(typeof(CardSuite), s)
                );
        }
    }
}
