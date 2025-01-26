namespace SpaceBase.Models
{
    public class Game
    {
        private readonly List<Player> _players;
        private List<Card> _sector1Cards = [];
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

            _players = new List<Player>(numPlayers);
            for (int i = 0; i < numPlayers; ++i)
            {
                var player = new Player(i + 1);
                player.PlayerReachedVictoryThresholdEvent += BeginGameOverRoutine;
                DiceRollEvent += player.ChooseDiceRoll;

                _players.Add(player);
            }
        }

        #region Properties

        public List<Player> Players { get => _players; }

        public int TurnNumber { get => _turnNumber; }

        public int RoundNumber { get => _roundNumber; }

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
            Trace.WriteLine("Not implemented");
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

                // Reset current player's gold to income if applicable

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
