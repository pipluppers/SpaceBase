namespace SpaceBase.Models
{
    public class Player
    {
        private readonly Board _board = new();
        private int _gold = 5;
        private int _income = 0;
        private int _victoryPoints = 0;
        private int _chargeCubes = 0;
        private bool _isCurrentPlayer = false;

        public Player(int id)
        {
            ID = id;
        }

        public event GameOverEventHandler<GameOverEventArgs>? GameOverEvent;

        public Board Board { get => _board; }
        public int ID { get; }
        public int Gold { get => _gold; private set => _gold = value; }
        public int Income { get => _income; private set => _income = value; }
        public int VictoryPoints { get => _victoryPoints; private set => _victoryPoints = value; }
        public int ChargeCubes { get => _chargeCubes; set => _chargeCubes = value; }

        /// <summary>
        /// Adds the specified amount of gold to the player's gold pool.
        /// </summary>
        /// 
        /// <param name="gold">The amount of gold to add.</param>
        /// <exception cref="NotSupportedException"><paramref name="gold"/> cannot be negative.</exception>
        public void AddGold(int gold)
        {
            if (gold < 0) throw new NotSupportedException("This API cannot be used to subtract or reset gold.");

            Gold += gold;
        }

        /// <summary>
        /// Adds the specified amount of income to the player's income pool.
        /// </summary>
        /// 
        /// <param name="income">The amount of income to add.</param>
        /// <exception cref="NotSupportedException"><paramref name="income"/> cannot be negative.</exception>
        public void AddIncome(int income)
        {
            if (income < 0) throw new NotSupportedException("Income cannot be removed");

            Income += income;
        }

        /// <summary>
        /// Adds the specified amount of victory points to the player's income pool.
        /// </summary>
        /// 
        /// <param name="victoryPoints">The amount of victory points to add.</param>
        /// <remarks>
        /// Fires the <see cref="SpaceBase.GameOverEventHandler{GameOverEventArgs}"/> event after reaching the victory threshold.
        /// </remarks>
        public void AddVictoryPoints(int victoryPoints)
        {
            // Note: There is a card that removes victory points.

            VictoryPoints += victoryPoints;

            if (VictoryPoints >= Constants.VictoryThreshold)
                GameOverEvent?.Invoke(this, new GameOverEventArgs(ID));
        }

        public void UpdateCurrentPlayer(bool isCurrentPlayer)
        {
            _isCurrentPlayer = isCurrentPlayer;
        }

        public void ChooseDiceRoll(int dice1, int dice2)
        {
            // TODO Display popup screen for user to click individual sectors or sum sector

            //string? userText = string.Empty;
            //int choice = 0;

            //Console.Write($"Dice roll 1: {dice1}, Dice roll 2: {dice2}\nClick '1' to choose both or '2' to choose the sum: ");
            //userText = Console.ReadLine();

            //if (!int.TryParse(userText, out choice) || choice != 1 || choice != 2)
            //{
            //    int remainingAttempts = 10;
            //    while (remainingAttempts-- >= 0)
            //    {
            //        Console.Write($"Invalid choice. Click '1' to choose both or '2' to choose the sum: ");
            //        userText = Console.ReadLine();

            //        if (int.TryParse(userText, out choice))
            //            break;
            //    }
            //}
        }

        /// <summary>
        /// Resets the player's gold to their cinom
        /// </summary>
        public void ResetGold()
        {
            Gold = Income;
        }
    }
}
