namespace SpaceBase.Services
{
    public static class CardLoadingService
    {
        /// <summary>
        /// Loads all card information from the database to the provided game.
        /// </summary>
        /// <param name="game">The game to load the cards to.</param>
        public static async Task LoadCards(Game game)
        {
            try
            {
                DataAccessLayer dataAccessLayer = new();
                List<ICard> cards = await dataAccessLayer.GetCards();

                if (cards.Count < Constants.MaxSectorID)
                    throw new Exception($"The database has less than {Constants.MaxSectorID} cards.");

                int i;

                // First load the initial cards for each player.

                for (i = 0; i < Constants.MaxSectorID; i++)
                {
                    foreach (Player player in game.Players)
                        player.AddCard(cards[i]);
                }

                foreach (Player player in game.Players)
                    player.AddCardToSectorEvent += game.AddCardToSectorHandler;

                // Now load the decks.

                List<ICard> nonPlayerCards = cards[Constants.MaxSectorID..];
                Utilities.Shuffle(nonPlayerCards);

                for (i = 0; i < nonPlayerCards.Count; i++)
                {
                    if (nonPlayerCards[i] is IStandardCard card)
                    {
                        // TODO Add support for charge cards
                        if (card is IChargeCard)
                            continue;

                        if (card.Level == 1)
                        {
                            if (game.Level1Cards.Count >= 6)
                                game.Level1Deck.Push(card);
                            else
                                game.Level1Cards.Add(card);
                        }
                        else if (card.Level == 2)
                        {
                            if (game.Level2Cards.Count >= 6)
                                game.Level2Deck.Push(card);
                            else
                                game.Level2Cards.Add(card);
                        }
                        else if (card.Level == 3)
                        {
                            if (game.Level3Cards.Count >= 6)
                                game.Level3Deck.Push(card);
                            else
                                game.Level3Cards.Add(card);
                        }
                    }
                    else if (nonPlayerCards[i] is IColonyCard colonyCard)
                    {
                        game.ColonyCards.Add(colonyCard);
                    }
                }
            }
            catch (Exception ex)
            {
                Trace.WriteLine($"Error loading the cards from the database: {ex.Message}");
            }
        }
    }
}
