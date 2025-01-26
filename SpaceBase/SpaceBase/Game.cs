namespace SpaceBase
{
    public class Game
    {
        private readonly List<Player> _players;
        private List<Card> _sector1Cards = [];
        private List<Card> _sector2Cards = [];
        private List<Card> _sector3Cards = [];
        private List<Card> _sectorFinalCards = [];
        private int _turnCount = 0;
        private int _currentPlayer = 0;
        private bool _isGameOver = false;

        internal event DiceRollEventHandler<DiceRollEventArgs>? DiceRollEvent;

        public Game(int numPlayers)
        {
            _players = new List<Player>(numPlayers);
            for (int i = 0; i < numPlayers; ++i)
            {
                var player = new Player(i + 1);
                player.GameOverEvent += BeginGameOverRoutine;
                DiceRollEvent += player.ChooseDiceRoll;

                _players.Add(player);
            }
        }

        public void StartGame()
        {
            if (_players.Count < 2) return;

            CacheCards();

            // TODO Each player draws a card. Player order is determined by highest cost

            _turnCount = 1;
            _currentPlayer = 0;

            PlayGame();
        }

        private void BeginGameOverRoutine(object sender, GameOverEventArgs args)
        {
            _isGameOver = true;
        }

        /// <summary>
        /// Reads the database to store all card information.
        /// </summary>
        private void CacheCards()
        {
            throw new NotImplementedException();
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
                ++_turnCount;
            }
        }

        private void RollDice()
        {
            Random random = new();
            int dice1 = random.Next() % 6;
            int dice2 = random.Next() % 6;

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
