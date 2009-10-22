using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PokerServer
{
    public class CardDeck
    {
        #region Fields

        private Dictionary<string, Card> remainingCards = new Dictionary<string, Card>();
        private List<Card> allCards = new List<Card>();
        private Queue<Card> deck = new Queue<Card>();

        #endregion

        #region Properties

        #endregion

        public CardDeck()
        {
            allCards.Clear();
            for (int s = 0; s < (int)CardSuite.Count; s++)
            {
                for (int v = (int)CardValue.Ace; v <= (int)CardValue.King; v++)
                {
                    allCards.Add(new Card(v, s));
                }
            }
        }

        #region Events and Delegates

        #endregion

        #region Event Handlers

        #endregion

        #region Private Methods

        #endregion

        #region Public Methods

        public void Shuffle()
        {
            remainingCards.Clear();
            deck.Clear();

            List<Card> pile = new List<Card>(allCards);
            while (pile.Count > 0)
            {
                int i = new Random().Next(pile.Count);
                Card c = pile[i];
                deck.Enqueue(c);
                remainingCards.Add(c.ToString(), c);
                pile.RemoveAt(i);
            }
        }

        public Card Draw()
        {
            if (deck.Count == 0 || remainingCards.Count == 0)
                Shuffle();

            Card c = deck.Dequeue();
            if (remainingCards.ContainsKey(c.ToString()))
                remainingCards.Remove(c.ToString());

            return c;
        }

        #endregion
    }
}
