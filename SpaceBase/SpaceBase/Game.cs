namespace SpaceBase.Models
{
    public interface IGame
    {
        public int VictoryThreshold { get; }
    }

    public sealed class Game : PropertyChangedBase, IGame
    {
        private readonly ObservableCollection<Player> _players;
        private bool _isGameOver;
        private readonly int _maxNumRounds;
        private int _roundNumber;
        private int _turnNumber;
        private int _activePlayerID;
        private readonly Random _randomNumberGenerator;

        public event EventHandler<EventArgs>? PreDiceRollEvent;
        public event DiceRollEventHandler<DiceRollEventArgs>? DiceRollEvent;
        public event EventHandler<EventArgs>? BuyEvent;
        public event TurnOverEvent<EventArgs>? TurnOverEvent;
        public event RoundOverEventHandler<RoundOverEventArgs>? RoundOverEvent;
        public event GameOverEventHandler<GameOverEventArgs>? GameOverEvent;

        public Game() : this(Constants.MinNumPlayers) { }

        public Game(int numPlayers) : this(numPlayers, Constants.MaxNumRounds) { }

        private Game(int numPlayers, int maxNumRounds)
        {
            if (numPlayers < Constants.MinNumPlayers || numPlayers > Constants.MaxNumPlayers)
                throw new ArgumentException($"The number of players must be between {Constants.MinNumPlayers} and {Constants.MaxNumPlayers}.");

            VictoryThreshold = Constants.VictoryThreshold;
            _players = [];

            var humanPlayer = new HumanPlayer(1);
            humanPlayer.PropertyChanged += Player_PropertyChanged;
            Players.Add(humanPlayer);

            for (int i = 1; i < numPlayers; ++i)
            {
                var player = new ComputerPlayer(i + 1);
                player.PropertyChanged += Player_PropertyChanged;

                Players.Add(player);
            }

            Level1Deck = [];
            Level2Deck = [];
            Level3Deck = [];
            Level1Cards = [];
            Level2Cards = [];
            Level3Cards = [];

            _isGameOver = false;
            _maxNumRounds = maxNumRounds;
            _roundNumber = 0;
            _turnNumber = 0;
            _activePlayerID = 0;
            _randomNumberGenerator = new Random(1);

            RoundOverEvent += MaxNumRoundsHandler;
        }

        #region Properties

        public int VictoryThreshold { get; init; }

        /// <summary>
        /// The collection of players.
        /// </summary>
        public ObservableCollection<Player> Players { get => _players; }

        public Stack<Card> Level1Deck { get; }
        public Stack<Card> Level2Deck { get; }
        public Stack<Card> Level3Deck { get; }

        public ObservableCollection<Card> Level1Cards { get; }
        public ObservableCollection<Card> Level2Cards { get; }
        public ObservableCollection<Card> Level3Cards { get; }

        /// <summary>
        /// The current turn number.
        /// </summary>
        /// <remarks>A turn is defined as the full set of a player's actions from the <see cref="Game.PreDiceRollEvent"/> to the <see cref="Game.TurnOverEvent"/>.</remarks>
        public int TurnNumber { get => _turnNumber; }

        /// <summary>
        /// The current round number.
        /// </summary>
        /// <remarks>A round is defined as full cycle from the beginning of the starting player's turn to the end of the ending player's turn.</remarks>
        public int RoundNumber { get => _roundNumber; set => SetProperty(ref _roundNumber, value); }

        /// <summary>
        /// The ID of the player rolling the dice and going to receive stationed card effects.
        /// </summary>
        /// <remarks>This ID will be updated at the end of each turn.</remarks>
        public int ActivePlayerID { get => _activePlayerID; set => SetProperty(ref _activePlayerID, value); }

        #endregion Properties

        #region Public methods

        /// <summary>
        /// Loads all card information from the database.
        /// </summary>
        public async Task LoadCards()
        {
            try
            {
                DataAccessLayer dataAccessLayer = new();
                List<Card> cards = await dataAccessLayer.GetCards();

                if (cards.Count < Constants.MaxSectorID)
                    throw new Exception($"The database has less than {Constants.MaxSectorID} cards.");

                int i = 0;

                // First load the initial cards for each player.

                for (; i < Constants.MaxSectorID; i++)
                {
                    foreach (Player player in Players)
                        player.AddCard(cards[i]);
                }

                foreach (Player player in Players)
                    player.AddCardToSectorEvent += AddCardToSectorHandler;

                // Now load the decks.

                for (; i < cards.Count; i++)
                {
                    Card card = cards[i];
                    if (card.Level == 1)
                    {
                        if (Level1Cards.Count >= 6) Level1Deck.Push(card);
                        else Level1Cards.Add(card);
                    }
                    else if (card.Level == 2)
                    {
                        if (Level2Cards.Count >= 6) Level2Deck.Push(card);
                        else Level2Cards.Add(card);
                    }
                    else if (card.Level == 3)
                    {
                        if (Level3Cards.Count >= 6) Level3Deck.Push(card);
                        else Level3Cards.Add(card);
                    }
                }
            }
            catch (Exception ex)
            {
                Trace.WriteLine($"Error loading the cards from the database: {ex.Message}");
            }
        }

        /// <summary>
        /// Initialize members and start the game.
        /// </summary>
        public async Task StartGame()
        {
            if (Players.Count < 2) return;

            foreach (var player in Players)
                player.AddCredits(5);

            // TODO Each player draws a card. Player order is determined by highest cost

            RoundNumber = 1;
            _turnNumber = 1;
            ActivePlayerID = 1; // TODO Just set human player to first player for now

            await PlayGame();
        }

        #endregion Public methods

        /// <summary>
        /// Start the game loop.
        /// </summary>
        private async Task PlayGame()
        {
            while (!_isGameOver || _turnNumber != 1)
            {
                // Broadcast PreDiceRollEvent

                if (PreDiceRollEvent != null) await Task.Run(() => PreDiceRollEvent.Invoke(this, new EventArgs()));

                await RollDice();

                if (BuyEvent != null) await Task.Run(() => BuyEvent.Invoke(this, new EventArgs()));

                // Broadcast PlayerMoveEvent
                //   Current player can choose to buy and/or use charge cubes
                //   Other players can choose to use charge cubes

                Players[ActivePlayerID - 1].ResetCredits();

                if (ActivePlayerID < Players.Count)
                    ++ActivePlayerID;
                else
                    ActivePlayerID = 1;

                if (_turnNumber < Players.Count)
                {
                    ++_turnNumber;
                }
                else
                {
                    _turnNumber = 1;
                    RoundOverEvent?.Invoke(this, new RoundOverEventArgs(RoundNumber++));
                }

                TurnOverEvent?.Invoke(this, new EventArgs());
            }

            int curr = 0;
            var victoryPlayerIDs = new List<int>();
            foreach (var player in Players)
            {
                if (player.VictoryPoints > curr)
                {
                    victoryPlayerIDs.Clear();
                    victoryPlayerIDs.Add(player.ID);
                }
                else if (player.VictoryPoints == curr)
                {
                    victoryPlayerIDs.Add(player.ID);
                }
            }

            GameOverEvent?.Invoke(this, new GameOverEventArgs(victoryPlayerIDs));
        }

        /// <summary>
        /// Selects two random numbers for the two die and invokes the DiceRollEvent.
        /// </summary>
        private async Task RollDice()
        {
            int dice1 = (_randomNumberGenerator.Next() % 6) + 1;
            int dice2 = (_randomNumberGenerator.Next() % 6) + 1;

            if (DiceRollEvent != null)
                await Task.Run(() => DiceRollEvent.Invoke(this, new DiceRollEventArgs(dice1, dice2, ActivePlayerID)));
        }

        /// <summary>
        /// Handler to decide when the game is over.
        /// </summary>
        /// <param name="sender">The player.</param>
        /// <param name="e">The event args describing the changed property.</param>
        private void Player_PropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            if (sender is not Player player || e.PropertyName != nameof(Player.VictoryPoints))
                return;

            if (player.VictoryPoints >= VictoryThreshold)
                _isGameOver = true;
        }

        /// <summary>
        /// Mark the game as over after a specified amount of rounds.
        /// </summary>
        private void MaxNumRoundsHandler(object _, RoundOverEventArgs args)
        {
            if (args.EndingRoundNumber == _maxNumRounds)
                _isGameOver = true;
        }

        /// <summary>
        /// Draws a new card from the appropriate deck and replaces the added card.
        /// </summary>
        /// <param name="sender">The player.</param>
        /// <param name="args">The arguments describing the added card.</param>
        private void AddCardToSectorHandler(object sender, AddCardToSectorEventArgs args)
        {
            if (sender == null || args == null)
                return;

            // Draw the next card from the stack of the appropriate level.

            static void DrawCard(Stack<Card> stackOfCards, ObservableCollection<Card> visibleRowOfCards, Card addedCard)
            {
                int index = visibleRowOfCards.IndexOf(addedCard);
                if (index == -1)
                    return;

                if (stackOfCards.TryPop(out Card? card) && card != null)
                    visibleRowOfCards[index] = card;
                else
                    visibleRowOfCards[index] = Utilities.NullLevelCard;
            }

            if (args.AddedCard is ColonyCard)
                return;

            Card? addedCard = args.AddedCard as Card;
            Debug.Assert(addedCard != null);

            int cardLevel = addedCard.Level;

            if (cardLevel == 1)
            {
                DrawCard(Level1Deck, Level1Cards, addedCard);
            }
            else if (cardLevel == 2)
            {
                DrawCard(Level2Deck, Level2Cards, addedCard);
            }
            else
            {
                DrawCard(Level3Deck, Level3Cards, addedCard);
            }
        }
    }
}
