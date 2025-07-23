namespace SpaceBase.Models
{
    public sealed class Game : PropertyChangedBase
    {
        private readonly ObservableCollection<Player> _players;
        private bool _isGameOver;
        private int _victoryThreshold;
        private readonly int _maxNumRounds;
        private int _roundNumber;
        private int _activePlayerID;
        private readonly DiceRollService _diceRollService;

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

            _victoryThreshold = Constants.VictoryThreshold;
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
            ColonyCards = [];

            _isGameOver = false;
            _maxNumRounds = maxNumRounds;
            _roundNumber = 0;
            _activePlayerID = 0;
            _diceRollService = new DiceRollService();

            RoundOverEvent += MaxNumRoundsHandler;
        }

        #region Properties

        /// <summary>
        /// The collection of players.
        /// </summary>
        public ObservableCollection<Player> Players { get => _players; }

        public Stack<IStandardCard> Level1Deck { get; }
        public Stack<IStandardCard> Level2Deck { get; }
        public Stack<IStandardCard> Level3Deck { get; }

        public ObservableCollection<IStandardCard> Level1Cards { get; }
        public ObservableCollection<IStandardCard> Level2Cards { get; }
        public ObservableCollection<IStandardCard> Level3Cards { get; }
        public ObservableCollection<IColonyCard> ColonyCards { get; }

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
        /// Updates the amount of points required to win the game.
        /// </summary>
        /// <param name="victoryThreshold">The new required amount of points to win.</param>
        /// <exception cref="InvalidOperationException">The value must be greater than 0.</exception>
        public void UpdateVictoryThreshold(int victoryThreshold)
        {
            if (victoryThreshold < 1)
                throw new InvalidOperationException("Players must score at least 1 point to win.");

            _victoryThreshold = victoryThreshold;
        }

        /// <summary>
        /// Initialize members and start the game.
        /// </summary>
        public async Task StartGame()
        {
            if (Players.Count < 2) return;

            foreach (var player in Players)
                PlayerResourcesService.AddCredits(player, 5);

            // TODO Each player draws a card. Player order is determined by highest cost

            RoundNumber = 1;
            ActivePlayerID = 1; // TODO Just set human player to first player for now

            await PlayGame();
        }

        #endregion Public methods

        /// <summary>
        /// Start the game loop.
        /// </summary>
        private async Task PlayGame()
        {
            while (!_isGameOver)
            {
                // Broadcast PreDiceRollEvent

                if (PreDiceRollEvent != null) await Task.Run(() => PreDiceRollEvent.Invoke(this, new EventArgs()));

                await RollDice();

                if (BuyEvent != null) await Task.Run(() => BuyEvent.Invoke(this, new EventArgs()));

                // Broadcast PlayerMoveEvent
                //   Current player can choose to buy and/or use charge cubes
                //   Other players can choose to use charge cubes

                PlayerResourcesService.ResetCredits(Players[ActivePlayerID - 1]);

                UpdateActivePlayer();

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
        /// Simulates the dice roll and notifies any listeners.
        /// </summary>
        private async Task RollDice()
        {
            if (DiceRollEvent == null)
                return;

            DiceRollResult result = _diceRollService.RollDice();
            await Task.Run(() => DiceRollEvent.Invoke(this, new DiceRollEventArgs(result.Dice1, result.Dice2, ActivePlayerID)));
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

            if (player.VictoryPoints >= _victoryThreshold)
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
        /// If the added card is a standard card, then draw a new card from the appropriate deck and replaces the added card.
        /// If the added card is a colony card, then remove it from the collection of colony cards and increase the player's victory points.
        /// </summary>
        /// <param name="sender">The player.</param>
        /// <param name="args">The arguments describing the added card.</param>
        internal void AddCardToSectorHandler(object sender, AddCardToSectorEventArgs args)
        {
            if (sender is not Player player || args == null)
                return;

            // Draw the next card from the stack of the appropriate level.

            static void DrawCard(Stack<IStandardCard> stackOfCards, ObservableCollection<IStandardCard> visibleRowOfCards, IStandardCard addedCard)
            {
                int index = visibleRowOfCards.IndexOf(addedCard);
                Debug.Assert(index >= 0);

                if (stackOfCards.TryPop(out IStandardCard? card) && card != null)
                    visibleRowOfCards[index] = card;
                else
                    visibleRowOfCards[index] = Utilities.NullLevelCard;
            }

            if (args.AddedCard is IStandardCard card)
            {
                int cardLevel = card.Level;
                Debug.Assert(cardLevel > 0 && cardLevel < 4);

                if (cardLevel == 1)
                    DrawCard(Level1Deck, Level1Cards, card);
                else if (cardLevel == 2)
                    DrawCard(Level2Deck, Level2Cards, card);
                else if (cardLevel == 3)
                    DrawCard(Level3Deck, Level3Cards, card);
            }
            else if (args.AddedCard is IColonyCard colonyCard)
            {
                ColonyCards[colonyCard.SectorID - 1] = Utilities.NullColonyCard;
                PlayerResourcesService.AddVictoryPoints(player, colonyCard.Amount);
            }
        }

        /// <summary>
        /// If the active player is not the last player, move the counter to next player. Otherwise, move the counter to the first player and broadcast the RoundOverEvent.
        /// </summary>
        private void UpdateActivePlayer()
        {
            if (ActivePlayerID < Players.Count)
            {
                ++ActivePlayerID;
            }
            else
            {
                ActivePlayerID = 1;
                RoundOverEvent?.Invoke(this, new RoundOverEventArgs(RoundNumber++));
            }
        }

    }
}
