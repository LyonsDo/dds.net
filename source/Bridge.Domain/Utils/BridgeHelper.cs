﻿using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Bridge.Domain.Utils
{
    public class BridgeHelper
    {
        public static BridgeGame GetGameFromPbn(string pbnHand)
        {
            var hands = new Dictionary<PlayerPosition, Deck>();
            var pbnhands = pbnHand.Split(':', ' ');
            var side = new PlayerPosition(pbnhands.First());
            var declarer = side;
            for (var i = 1; i < 5; i++)
            {
                hands.Add(side, GetDeck(pbnhands[i]));
                side = GetNextPlayerPosition(side);
            }

            return new BridgeGame(hands, declarer);
        }

        public static PlayerPosition GetNextPlayerPosition(PlayerPosition currentSide)
        {
            if (currentSide.Order == 3)
                return PlayerPosition.North;

            return new PlayerPosition(currentSide.Order + 1);
        }

        public static string ToPbn(BridgeGame game)
        {
            var sb = new StringBuilder();
            sb.Append(game.Declarer.FirstLetter);
            sb.Append(":");

            var side = game.Declarer;
            for (var i = 1; i < 5; i++)
            {
                var deck = game.GameState[side];
                sb.Append(DeckToPbnHand(deck));
                if (i != 4)
                    sb.Append(' ');
                side = GetNextPlayerPosition(side);
            }

            return sb.ToString();
        }

        private static string DeckToPbnHand(Deck deck)
        {
            var sb = new StringBuilder();

            foreach (var suit in Suit.Suits)
            {
                foreach (var card in deck.Cards.Where(x => x.Suit == suit))
                {
                    sb.Append(card.Rank.ShortName);
                }
                sb.Append(".");
            }


            return sb.ToString().TrimEnd('.');
        }

        private static Deck GetDeck(string pbnHand)
        {
            var deck = new Deck();
            var list = new List<Card>();
            var suits = pbnHand.Split('.');
            for (int i = 0; i < 4; i++)
            {
                list.AddRange(ReadPbnCard(Suit.Suits[i], suits[i]));
            }
            deck.Cards = list;

            return deck;
        }

        private static IEnumerable<Card> ReadPbnCard(Suit suit, string cardString)
        {
            return cardString.Aggregate(new List<Card>(), (cards, card) =>
            {
                cards.Add(new Card(new Rank(card), suit));
                return cards;
            });
        }

        public static Card GetCard(string card)
        {
            return new Card(new Rank(card[1]),Suit.Suits.Find(x=> x.ShortName == new string(card[0],1)) );
        }
    }
}
