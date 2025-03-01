namespace SpaceBase.Models
{
    public abstract class Player(int id) : PropertyChangedBase
    {
        private readonly Board _board = new();
        private int _credits = 5;
        private int _income = 0;
        private int _victoryPoints = 0;
        private int _chargeCubes = 0;
        private bool _isCurrentPlayer = false;

        public event PlayerReachedVictoryThresholdEvent<PlayerReachedVictoryThresholdEventArgs>? PlayerReachedVictoryThresholdEvent;
        public event AddCardToSectorEvent<AddCardToSectorEventArgs>? AddCardToSectorEvent;

        public Board Board { get => _board; }
        public int ID { get; } = id;
        public int Credits { get => _credits; private set => SetProperty(ref _credits, value); }
        public int Income { get => _income; private set => SetProperty(ref _income, value); }
        public int VictoryPoints { get => _victoryPoints; private set => SetProperty(ref _victoryPoints, value); }
        public int ChargeCubes { get => _chargeCubes; set => _chargeCubes = value; }

        /// <summary>
        /// Adds the specified amount of credits to the player's credits pool.
        /// </summary>
        /// <param name="credits">The amount of credits to add.</param>
        /// <exception cref="NotSupportedException"><paramref name="credits"/> cannot be negative.</exception>
        public void AddCredits(int credits)
        {
            if (credits < 0) throw new NotSupportedException("This API cannot be used to subtract or reset credits.");

            Credits += credits;
        }

        /// <summary>
        /// Adds the specified amount of income to the player's income pool.
        /// </summary>
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
        /// <param name="victoryPoints">The amount of victory points to add.</param>
        /// <remarks>Fires the <see cref="SpaceBase.GameOverEventHandler{GameOverEventArgs}"/> event after reaching the victory threshold.</remarks>
        public void AddVictoryPoints(int victoryPoints)
        {
            // Note: There is a card that removes victory points.

            VictoryPoints += victoryPoints;

            if (VictoryPoints >= Constants.VictoryThreshold)
                PlayerReachedVictoryThresholdEvent?.Invoke(this, new PlayerReachedVictoryThresholdEventArgs(ID));
        }

        /// <summary>
        /// Gets the sector with the given ID.
        /// </summary>
        /// <param name="sectorID">The ID of the sector to get.</param>
        /// <returns>The sector with the given ID.</returns>
        /// <exception cref="ArgumentOutOfRangeException">The ID of the card is invalid.</exception>
        public Sector GetSector(int sectorID)
        {
            if (sectorID < Constants.MinSectorID || sectorID > Constants.MaxSectorID)
                throw new ArgumentOutOfRangeException(nameof(sectorID), $"The input sector ID must be between {Constants.MinSectorID} and {Constants.MaxSectorID}");

            return Board.Sectors[sectorID - 1];
        }

        /// <summary>
        /// Adds the card at the sectorID referenced by the card.
        /// </summary>
        /// <param name="card">The card to add.</param>
        /// <exception cref="ArgumentOutOfRangeException">The ID of the card is invalid.</exception>
        public void AddCard(Card card)
        {
            if (card.SectorID < Constants.MinSectorID || card.SectorID > Constants.MaxSectorID)
                throw new ArgumentOutOfRangeException(nameof(card), $"The input sector ID of the card must be between {Constants.MinSectorID} and {Constants.MaxSectorID}");

            GetSector(card.SectorID).AddCard(card);

            AddCardToSectorEvent?.Invoke(this, new AddCardToSectorEventArgs(card));
        }

        /// <summary>
        /// Activates the effect of the stationed card at the given sector.
        /// </summary>
        /// <param name="sectorID">The ID of the sector whose card to activate.</param>
        /// <exception cref="ArgumentOutOfRangeException">The ID is invalid.</exception>
        public void ActivateCardEffect(int sectorID)
        {
            GetSector(sectorID).StationedCard?.ActivateStationedEffect(this);
        }

        /// <summary>
        /// Activates the effects of all deployed cards at the given sector.
        /// </summary>
        /// <param name="sectorID">The ID of the sector whose cards to activate.</param>
        /// <exception cref="ArgumentOutOfRangeException">The ID is invalid.</exception>
        public void ActivateDeployedCardsEffect(int sectorID)
        {
            var deployedCards = GetSector(sectorID).DeployedCards;
            foreach (var deployedCard in deployedCards)
                deployedCard.ActivateDeployedEffect(this);
        }

        public void UpdateCurrentPlayer(bool isCurrentPlayer)
        {
            _isCurrentPlayer = isCurrentPlayer;
        }

        /// <summary>
        /// Resets the player's credits to their income.
        /// </summary>
        public void ResetCredits()
        {
            Credits = Income;
        }

        #region Overridable methods

        public abstract void ChooseDiceRoll(object sender, DiceRollEventArgs args);

        #endregion Overridable methods
    }

    public class HumanPlayer(int id) : Player(id)
    {
        public override void ChooseDiceRoll(object sender, DiceRollEventArgs args)
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
    }

    public class ComputerPlayer(int id) : Player(id)
    {
        public override void ChooseDiceRoll(object sender, DiceRollEventArgs args)
        {
            // TODO
        }
    }
}
