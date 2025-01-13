namespace SpaceBase
{
    public class Game
    {
        private readonly List<Player> _players;
        private bool _gameOver = false;
        private List<Card> _sector1Cards = [];
        private List<Card> _sector2Cards = [];
        private List<Card> _sector3Cards = [];
        private List<Card> _sectorFinalCards = [];
        private int _turnCount = 0;
        private int _currentPlayer = 0;

        public Game(int numPlayers)
        {
            _players = new List<Player>(numPlayers);
        }

        public void StartGame()
        {
            if (_players.Count < 2) return;

            CacheCards();

            _turnCount = 1;
            _currentPlayer = 0;

            while (!IsGameOver())
            {

                UpdateNextPlayer();
                ++_turnCount;
            }
        }

        /// <summary>
        /// Reads the database to store all card information.
        /// </summary>
        private void CacheCards()
        {
            throw new NotImplementedException();
        }

        private void RollDice()
        {

        }

        /// <summary>
        /// Checks if the game is over. If any player has over the victory threshold amount of points, then the game is over.
        /// </summary>
        /// 
        /// <returns>True if game is over. Else false.</returns>
        /// 
        /// <remarks>
        /// The standard victory threshold if 40 points but players may modify this as they wish at the beginning of the game.
        /// </remarks>
        private bool IsGameOver()
        {
            // Prevent infinite game
            if (_turnCount >= 100)
                return true;

            foreach (var player in _players)
            {
                if (player.VictoryPoints >= Constants.VictoryThreshold)
                    return true;
            }

            return false;
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
