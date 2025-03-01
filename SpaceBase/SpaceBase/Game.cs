namespace SpaceBase.Models
{
    public class Game
    {
        private readonly List<Player> _players;
        private readonly ObservableCollection<Card?> _level1Cards;
        private readonly ObservableCollection<Card?> _level2Cards;
        private readonly ObservableCollection<Card?> _level3Cards;
        private readonly int _maxNumRounds;
        private int _roundNumber;
        private int _turnNumber;
        private bool _isGameOver = false;
        private readonly Random _random;

        public event DiceRollEventHandler<DiceRollEventArgs>? DiceRollEventHandler;
        public event EventHandler<EventArgs>? PreDiceRollEventHandler;
        public event EventHandler<EventArgs>? BuyEventHandler;
        public event TurnOverEvent<TurnOverEventArgs>? TurnOverEventHandler;
        public event RoundOverEventHandler<RoundOverEventArgs>? RoundOverEvent;
        public event GameOverEventHandler<GameOverEventArgs>? GameOverEvent;

        public Game() : this(Constants.MinNumPlayers) { }

        public Game(int numPlayers) : this(numPlayers, Constants.MaxNumRounds) { }

        private Game(int numPlayers, int maxNumRounds)
        {
            if (numPlayers < Constants.MinNumPlayers || numPlayers > Constants.MaxNumPlayers)
                throw new ArgumentException($"The number of players must be between {Constants.MinNumPlayers} and {Constants.MaxNumPlayers}.");

            _random = new Random(1);
            CurrentPlayerID = 0;

            _maxNumRounds = maxNumRounds;
            RoundOverEvent += MaxNumRoundsHandler;

            Level1Deck = [];
            Level2Deck = [];
            Level3Deck = [];
            _level1Cards = [];
            _level2Cards = [];
            _level3Cards = [];

            _players = new List<Player>(numPlayers);

            var humanPlayer = new HumanPlayer(1);
            humanPlayer.PlayerReachedVictoryThresholdEvent += BeginGameOverRoutine;
            DiceRollEventHandler += humanPlayer.ChooseDiceRoll;
            _players.Add(humanPlayer);

            for (int i = 1; i < numPlayers; ++i)
            {
                var player = new ComputerPlayer(i + 1);
                player.PlayerReachedVictoryThresholdEvent += BeginGameOverRoutine;
                DiceRollEventHandler += player.ChooseDiceRoll;

                _players.Add(player);
            }

            CacheCards();
        }

        #region Properties

        /// <summary>
        /// The random number generator for the dice rolls.
        /// </summary>
        public Random Random { get => _random; init => _random = value; }

        public List<Player> Players { get => _players; }

        public int CurrentPlayerID { get; set; }

        public int TurnNumber { get => _turnNumber; }

        public int RoundNumber { get => _roundNumber; }

        public ObservableCollection<Card?> Level1Cards { get => _level1Cards; }
        public ObservableCollection<Card?> Level2Cards { get => _level2Cards; }
        public ObservableCollection<Card?> Level3Cards { get => _level3Cards; }

        public Stack<Card> Level1Deck { get; }
        public Stack<Card> Level2Deck { get; }
        public Stack<Card> Level3Deck { get; }

        #endregion Properties

        /// <summary>
        /// Prepare to play the game by loading cards into memory and initializing members.
        /// </summary>
        public async Task StartGame()
        {
            if (_players.Count < 2) return;

            // TODO Each player draws a card. Player order is determined by highest cost

            _roundNumber = 1;
            _turnNumber = 1;
            CurrentPlayerID = 1; // TODO Just set human player to first player for now

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

                if (PreDiceRollEventHandler != null) await Task.Run(() => PreDiceRollEventHandler.Invoke(this, new EventArgs()));

                await RollDice();

                if (BuyEventHandler != null) await Task.Run(() => BuyEventHandler.Invoke(this, new EventArgs()));

                // Broadcast PlayerMoveEvent
                //   Current player can choose to buy and/or use charge cubes
                //   Other players can choose to use charge cubes

                // Reset current player's credits to income if applicable

                if (CurrentPlayerID < _players.Count)
                    ++CurrentPlayerID;
                else
                    CurrentPlayerID = 1;

                //UpdateNextPlayer();

                if (_turnNumber < _players.Count)
                {
                    ++_turnNumber;
                }
                else
                {
                    _turnNumber = 1;
                    RoundOverEvent?.Invoke(this, new RoundOverEventArgs(_roundNumber++));
                }

                TurnOverEventHandler?.Invoke(this, new TurnOverEventArgs());
            }

            int curr = 0;
            var victoryPlayerIDs = new List<int>();
            foreach (var player in _players)
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

            // Find the card
            int cardLevel = args.AddedCard.Level;

            if (cardLevel == 1)
            {
                int index = Level1Cards.IndexOf(args.AddedCard);
                if (index == -1)
                    return;

                if (Level1Deck.TryPop(out Card? card) && card != null)
                    Level1Cards[index] = card;
                else
                    Level1Cards[index] = null;
            }
            else if (cardLevel == 2)
            {
                int index = Level2Cards.IndexOf(args.AddedCard);
                if (index == -1)
                    return;

                if (Level2Deck.TryPop(out Card? card) && card != null)
                    Level2Cards[index] = card;
                else
                    Level2Cards[index] = null;
            }
            else
            {
                int index = Level3Cards.IndexOf(args.AddedCard);
                if (index == -1)
                    return;

                if (Level3Deck.TryPop(out Card? card) && card != null)
                    Level3Cards[index] = card;
                else
                    Level3Cards[index] = null;
            }
        }

        /// <summary>
        /// Reads the database to store all card information.
        /// </summary>
        private void CacheCards()
        {
            try
            {
                using var connection = GetSqlConnection();
                connection.Open();

                string? table = Environment.GetEnvironmentVariable(Constants.CardsTableEnvironmentVariable, EnvironmentVariableTarget.User);
                string queryString = $"SELECT * FROM {table}";

                using var command = new SqlCommand(queryString, connection);
                using SqlDataReader reader = command.ExecuteReader();

                PopulatePlayerStartingCards(reader);

                while (true)
                {
                    if (!reader.Read())
                        break;

                    var card = CreateCard(reader);

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

                return new ChargeCard(level, sectorID, cost,
                    (ActionType)effect, effectAmount, secondaryEffectAmount,
                    (ActionType)deployedEffect, deployedEffectAmount, secondaryDeployedEffectAmount,
                    (ActionType)chargeEffect, requiredChargeCubes, chargeCubeLimit, (ChargeCardType)chargeCardType);
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

                _players.ForEach((player) => player.AddCard(card));
            }

            _players.ForEach((player) => player.AddCardToSectorEvent += AddCardToSectorHandler);
        }

        /// <summary>
        /// Selects two random numbers for the two die and invokes the DiceRollEventHandler.
        /// </summary>
        private async Task RollDice()
        {
            int dice1 = (_random.Next() % 6) + 1;
            int dice2 = (_random.Next() % 6) + 1;

            if (DiceRollEventHandler != null)
                await Task.Run(() => DiceRollEventHandler.Invoke(this, new DiceRollEventArgs(dice1, dice2)));
        }

        /// <summary>
        /// Updates the state of the game to the next player.
        /// </summary>
        private void UpdateNextPlayer()
        {
            _players[CurrentPlayerID].UpdateCurrentPlayer(false);

            if (CurrentPlayerID < _players.Count - 1)
                ++CurrentPlayerID;
            else
                CurrentPlayerID = 0;

            _players[CurrentPlayerID].UpdateCurrentPlayer(true);
        }
    }
}
