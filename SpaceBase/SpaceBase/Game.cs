using System.Windows.Media.Effects;
using System.Windows.Media.Media3D;

namespace SpaceBase.Models
{
    public class Game
    {
        private readonly List<Player> _players;
        private readonly ObservableCollection<Card> _sector1Cards;
        private List<Card> _sector2Cards = [];
        private List<Card> _sector3Cards = [];
        private List<Card> _sectorFinalCards = [];
        private int _maxNumRounds;
        private int _roundNumber;
        private int _turnNumber;
        private int _currentPlayer = 0;
        private bool _isGameOver = false;

        public event DiceRollEventHandler<DiceRollEventArgs>? DiceRollEvent;
        public event RoundOverEventHandler<RoundOverEventArgs>? RoundOverEvent;
        public event GameOverEventHandler<GameOverEventArgs>? GameOverEvent;

        public Game() : this(Constants.MinNumPlayers) { }

        public Game(int numPlayers) : this(numPlayers, Constants.MaxNumRounds) { }

        public Game(int numPlayers, int maxNumRounds)
        {
            if (numPlayers < Constants.MinNumPlayers || numPlayers > Constants.MaxNumPlayers)
                throw new ArgumentException($"The number of players must be between {Constants.MinNumPlayers} and {Constants.MaxNumPlayers}.");

            _maxNumRounds = maxNumRounds;
            RoundOverEvent += MaxNumRoundsHandler;

            _sector1Cards = [];

            _players = new List<Player>(numPlayers);

            var humanPlayer = new HumanPlayer(1);
            humanPlayer.PlayerReachedVictoryThresholdEvent += BeginGameOverRoutine;
            DiceRollEvent += humanPlayer.ChooseDiceRoll;
            _players.Add(humanPlayer);

            for (int i = 1; i < numPlayers; ++i)
            {
                var player = new ComputerPlayer(i + 1);
                player.PlayerReachedVictoryThresholdEvent += BeginGameOverRoutine;
                DiceRollEvent += player.ChooseDiceRoll;

                _players.Add(player);
            }
        }

        #region Properties

        public List<Player> Players { get => _players; }

        public int TurnNumber { get => _turnNumber; }

        public int RoundNumber { get => _roundNumber; }

        public ObservableCollection<Card> Sector1Cards { get => _sector1Cards; }

        #endregion Properties

        public void StartGame()
        {
            if (_players.Count < 2) return;

            CacheCards();

            // TODO Each player draws a card. Player order is determined by highest cost

            _roundNumber = 1;
            _turnNumber = 1;
            _currentPlayer = 0;

            PlayGame();
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

                int sectorID, cost, effect, effectAmount, deployedEffect, deployedEffectAmount;
                int? secondaryEffectAmount, secondaryDeployedEffectAmount;
                for (int i = 0; i < 6; ++i)
                {
                    reader.Read();

                    sectorID = reader.GetInt32(0);
                    cost = reader.GetInt32(1);
                    effect = reader.GetInt32(2);
                    effectAmount = reader.GetInt32(3);
                    secondaryEffectAmount = !reader.IsDBNull(4) ? reader.GetInt32(4) : null;
                    deployedEffect = reader.GetInt32(5);
                    deployedEffectAmount = reader.GetInt32(6);
                    secondaryDeployedEffectAmount = !reader.IsDBNull(7) ? reader.GetInt32(7) : null;

                    _sector1Cards.Add(new Card(sectorID, cost,
                        (ActionType)effect, effectAmount, secondaryEffectAmount,
                        (ActionType)deployedEffect, deployedEffectAmount, secondaryDeployedEffectAmount));
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
        /// Populate the starting cards on each player's boards.
        /// </summary>
        /// <param name="reader">The reader object over the data.</param>
        private void PopulatePlayerStartingCards(SqlDataReader reader)
        {
            int sectorID, cost, effect, effectAmount, deployedEffect, deployedEffectAmount;
            int? secondaryEffectAmount, secondaryDeployedEffectAmount;
            for (int i = 0; i < 12; ++i)
            {
                reader.Read();

                sectorID = reader.GetInt32(0);
                cost = reader.GetInt32(1);
                effect = reader.GetInt32(2);
                effectAmount = reader.GetInt32(3);
                secondaryEffectAmount = !reader.IsDBNull(4) ? reader.GetInt32(4) : null;
                deployedEffect = reader.GetInt32(5);
                deployedEffectAmount = reader.GetInt32(6);
                secondaryDeployedEffectAmount = !reader.IsDBNull(7) ? reader.GetInt32(7) : null;

                foreach (var player in _players)
                {
                    player.Board.Sectors[i].AddCard(new Card(sectorID, cost,
                        (ActionType)effect, effectAmount, secondaryEffectAmount,
                        (ActionType)deployedEffect, deployedEffectAmount, secondaryDeployedEffectAmount));
                }
            }
        }

        private void PlayGame()
        {
            while (!_isGameOver)
            {
                // Broadcast PreDiceRollEvent

                RollDice();

                // Broadcast PlayerMoveEvent
                //   Current player can choose to buy and/or use charge cubes
                //   Other players can choose to use charge cubes

                // Reset current player's credits to income if applicable

                UpdateNextPlayer();

                if (_turnNumber < _players.Count)
                    ++_turnNumber;
                else
                {
                    _turnNumber = 1;
                    RoundOverEvent?.Invoke(this, new RoundOverEventArgs(_roundNumber++));
                }
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

        private void RollDice()
        {
            Random random = new();
            int dice1 = (random.Next() % 6) + 1;
            int dice2 = (random.Next() % 6) + 1;

            DiceRollEvent?.Invoke(this, new DiceRollEventArgs(dice1, dice2));
        }

        /// <summary>
        /// Updates the state of the game to the next player.
        /// </summary>
        private void UpdateNextPlayer()
        {
            _players[_currentPlayer].UpdateCurrentPlayer(false);

            if (_currentPlayer < _players.Count - 1)
                ++_currentPlayer;
            else
                _currentPlayer = 0;

            _players[_currentPlayer].UpdateCurrentPlayer(true);
        }
    }
}
