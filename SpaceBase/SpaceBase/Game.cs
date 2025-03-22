﻿namespace SpaceBase.Models
{
    public class Game : PropertyChangedBase
    {
        private readonly ObservableCollection<Player> _players;
        private readonly ObservableCollection<Card> _level1Cards;
        private readonly ObservableCollection<Card> _level2Cards;
        private readonly ObservableCollection<Card> _level3Cards;
        private readonly int _maxNumRounds;
        private int _roundNumber;
        private int _turnNumber;
        private bool _isGameOver = false;
        private readonly Random _random;
        private int _activePlayerID;

        public event EventHandler<EventArgs>? PreDiceRollEvent;
        public event DiceRollEventHandler<DiceRollEventArgs>? DiceRollEvent;
        public event EventHandler<EventArgs>? BuyEvent;
        public event TurnOverEvent<TurnOverEventArgs>? TurnOverEvent;
        public event RoundOverEventHandler<RoundOverEventArgs>? RoundOverEvent;
        public event GameOverEventHandler<GameOverEventArgs>? GameOverEvent;

        public Game() : this(Constants.MinNumPlayers) { }

        public Game(int numPlayers) : this(numPlayers, Constants.MaxNumRounds) { }

        private Game(int numPlayers, int maxNumRounds)
        {
            if (numPlayers < Constants.MinNumPlayers || numPlayers > Constants.MaxNumPlayers)
                throw new ArgumentException($"The number of players must be between {Constants.MinNumPlayers} and {Constants.MaxNumPlayers}.");

            _random = new Random(1);
            ActivePlayerID = 0;

            _maxNumRounds = maxNumRounds;
            RoundOverEvent += MaxNumRoundsHandler;

            Level1Deck = [];
            Level2Deck = [];
            Level3Deck = [];
            _level1Cards = [];
            _level2Cards = [];
            _level3Cards = [];

            _players = [];

            var humanPlayer = new HumanPlayer(1);
            humanPlayer.PlayerReachedVictoryThresholdEvent += BeginGameOverRoutine;
            Players.Add(humanPlayer);

            for (int i = 1; i < numPlayers; ++i)
            {
                var player = new ComputerPlayer(i + 1);
                player.PlayerReachedVictoryThresholdEvent += BeginGameOverRoutine;

                Players.Add(player);
            }

            CacheCards();
        }

        #region Properties

        /// <summary>
        /// The random number generator for the dice rolls.
        /// </summary>
        public Random Random { get => _random; init => _random = value; }

        public ObservableCollection<Player> Players { get => _players; }

        public int ActivePlayerID { get => _activePlayerID; set => SetProperty(ref _activePlayerID, value); }

        public int TurnNumber { get => _turnNumber; }

        public int RoundNumber { get => _roundNumber; }

        public ObservableCollection<Card> Level1Cards { get => _level1Cards; }
        public ObservableCollection<Card> Level2Cards { get => _level2Cards; }
        public ObservableCollection<Card> Level3Cards { get => _level3Cards; }

        public Stack<Card> Level1Deck { get; }
        public Stack<Card> Level2Deck { get; }
        public Stack<Card> Level3Deck { get; }

        #endregion Properties

        /// <summary>
        /// Prepare to play the game by loading cards into memory and initializing members.
        /// </summary>
        public async Task StartGame()
        {
            if (Players.Count < 2) return;

            foreach (var player in Players)
                player.AddCredits(5);

            // TODO Each player draws a card. Player order is determined by highest cost

            _roundNumber = 1;
            _turnNumber = 1;
            ActivePlayerID = 1; // TODO Just set human player to first player for now

            await PlayGame();
        }

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
                    RoundOverEvent?.Invoke(this, new RoundOverEventArgs(_roundNumber++));
                }

                TurnOverEvent?.Invoke(this, new TurnOverEventArgs());
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

        private void BeginGameOverRoutine(object sender, PlayerReachedVictoryThresholdEventArgs args)
        {
            _isGameOver = true;
        }

        private void MaxNumRoundsHandler(object sender, RoundOverEventArgs args)
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

            int cardLevel = args.AddedCard.Level;

            if (cardLevel == 1)
            {
                DrawCard(Level1Deck, Level1Cards, args.AddedCard);
            }
            else if (cardLevel == 2)
            {
                DrawCard(Level2Deck, Level2Cards, args.AddedCard);
            }
            else
            {
                DrawCard(Level3Deck, Level3Cards, args.AddedCard);
            }
        }

        /// <summary>
        /// Reads the database to store all card information.
        /// </summary>
        private void CacheCards()
        {
            try
            {
                DataAccessLayer dataAccessLayer = new();
                List<Card> cards = dataAccessLayer.GetCards();

                if (cards.Count < Constants.MaxSectorID)
                    throw new Exception($"The database has less than {Constants.MaxSectorID} cards.");

                int i = 0;

                for (; i < Constants.MaxSectorID; i++)
                {
                    foreach (Player player in Players)
                        player.AddCard(cards[i]);
                }

                foreach (Player player in Players)
                    player.AddCardToSectorEvent += AddCardToSectorHandler;

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
        /// Reads environment variables and returns the SqlConnection object.
        /// </summary>
        /// <returns>The connection object.</returns>
        /// <exception cref="InvalidOperationException">There was an error reading the environment variables or getting the connection.</exception>
        private static SqlConnection GetSqlConnection()
        {
            string? server = Environment.GetEnvironmentVariable(Constants.ServerEnvironmentVariable, EnvironmentVariableTarget.User);
            string? database = Environment.GetEnvironmentVariable(Constants.DatabaseEnvironmentVariable, EnvironmentVariableTarget.User);

            if (string.IsNullOrEmpty(server) || string.IsNullOrEmpty(database))
                throw new InvalidOperationException($"Error loading the server {server} or database {database}.");

            string connectionString = $"{Constants.ServerKey}={server};{Constants.DatabaseKey}={database};";

            // TODO  Get Certificate Authority signed certificate and remove the assignment below
            connectionString += "Encrypt=False;Trusted_Connection=True";

            return new SqlConnection(connectionString);
        }

        /// <summary>
        /// Creates a card based on the current row in the table.
        /// </summary>
        /// <param name="reader">The iterator over the current row in the table.</param>
        /// <returns>A card based on the table row.</returns>
        private static Card CreateCard(SqlDataReader reader)
        {
            int level = reader.GetInt32(0);
            int sectorID = reader.GetInt32(1);
            int cost = reader.GetInt32(2);
            int effect = reader.GetInt32(3);
            int effectAmount = reader.GetInt32(4);
            int? secondaryEffectAmount = !reader.IsDBNull(5) ? reader.GetInt32(5) : null;
            int deployedEffect = reader.GetInt32(6);
            int deployedEffectAmount = reader.GetInt32(7);
            int? secondaryDeployedEffectAmount = !reader.IsDBNull(8) ? reader.GetInt32(7) : null;

            if (reader.IsDBNull(9))
            {
                return new Card(level, sectorID, cost,
                    (ActionType)effect, effectAmount, secondaryEffectAmount,
                    (ActionType)deployedEffect, deployedEffectAmount, secondaryDeployedEffectAmount);
            }
            else
            {
                int chargeEffect = reader.GetInt32(9);
                int requiredChargeCubes = reader.GetInt32(10);
                int chargeCubeLimit = reader.GetInt32(11);
                int chargeCardType = reader.GetInt32(12);
                int deployedChargeEffect = reader.GetInt32(13);
                int deployedRequiredChargeCubes = reader.GetInt32(14);
                int deployedChargeCubeLimit = reader.GetInt32(15);
                int deployedChargeCardType = reader.GetInt32(16);

                return new ChargeCard(level, sectorID, cost,
                    (ActionType)effect, effectAmount, secondaryEffectAmount,
                    (ActionType)deployedEffect, deployedEffectAmount, secondaryDeployedEffectAmount,
                    (ChargeActionType)chargeEffect, requiredChargeCubes, chargeCubeLimit, (ChargeCardType)chargeCardType,
                    (ChargeActionType)deployedChargeEffect, deployedRequiredChargeCubes, deployedChargeCubeLimit, (ChargeCardType)deployedChargeCardType);
            }
        }

        /// <summary>
        /// Populate the starting cards on each player's boards and subscribe to adding card events.
        /// </summary>
        /// <param name="reader">The reader object over the data.</param>
        private void PopulatePlayerStartingCards(SqlDataReader reader)
        {
            for (int i = 0; i < 12; ++i)
            {
                reader.Read();

                var card = CreateCard(reader);

                foreach (var player in Players)
                    player.AddCard(card);
            }

            foreach (var player in Players)
                player.AddCardToSectorEvent += AddCardToSectorHandler;
        }

        /// <summary>
        /// Selects two random numbers for the two die and invokes the DiceRollEvent.
        /// </summary>
        private async Task RollDice()
        {
            int dice1 = (_random.Next() % 6) + 1;
            int dice2 = (_random.Next() % 6) + 1;

            // TODO: Currently, the current player's ID is always 1. This will change later on.
            if (DiceRollEvent != null)
                await Task.Run(() => DiceRollEvent.Invoke(this, new DiceRollEventArgs(dice1, dice2, ActivePlayerID)));
        }
    }
}
