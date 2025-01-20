namespace SpaceBase.Models
{
    public class Player
    {
        private readonly Board _board = new();
        private int _gold = 0;
        private int _income = 0;
        private int _victoryPoints = 0;
        private int _chargeCubes = 0;
        private bool _isCurrentPlayer = false;

        public Player() { }

        public Board Board { get => _board; }
        public int Gold { get => _gold; set => _gold = value; }
        public int Income { get => _income; set => _income = value; }
        public int VictoryPoints { get => _victoryPoints; set => _victoryPoints = value; }
        public int ChargeCubes { get => _chargeCubes; set => _chargeCubes = value; }

        public void UpdateCurrentPlayer(bool isCurrentPlayer)
        {
            _isCurrentPlayer = isCurrentPlayer;
        }

        public void ResetGold()
        {
            Gold = Income;
        }
    }
}
