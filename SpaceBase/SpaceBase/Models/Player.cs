namespace SpaceBase.Models
{
    public abstract class Player(int id) : PropertyChangedBase
    {
        private readonly Board _board = new();
        private int _credits = 0;
        private int _income = 0;
        private int _victoryPoints = 0;
        private int _chargeCubes = 0;

        public event AddCardToSectorEvent<AddCardToSectorEventArgs>? AddCardToSectorEvent;

        public Board Board { get => _board; }
        public int ID { get; } = id;
        public int Credits { get => _credits; internal set => SetProperty(ref _credits, value); }
        public int Income { get => _income; internal set => SetProperty(ref _income, value); }
        public int VictoryPoints { get => _victoryPoints; internal set => SetProperty(ref _victoryPoints, value); }
        public int ChargeCubes { get => _chargeCubes; set => _chargeCubes = value; }

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
        public void AddCard(ICard card)
        {
            if (card.SectorID < Constants.MinSectorID || card.SectorID > Constants.MaxSectorID)
                throw new ArgumentOutOfRangeException(nameof(card), $"The input sector ID of the card must be between {Constants.MinSectorID} and {Constants.MaxSectorID}");

            Sector sector = GetSector(card.SectorID);
            AddCard(sector, card);

            AddCardToSectorEvent?.Invoke(this, new AddCardToSectorEventArgs(card));
        }

        /// <summary>
        /// Adds the card to the provided sector.
        /// </summary>
        /// <param name="sector">The sector to get the card.</param>
        /// <param name="card">The card to add.</param>
        private static void AddCard(Sector sector, ICard card)
        {
            sector.AddCard(card);
        }

        /// <summary>
        /// Adds the card at the sectorID referenced by the card and then reduces the player's credits to 0.
        /// </summary>
        /// <param name="card">The card to add.</param>
        /// <exception cref="ArgumentOutOfRangeException">The ID of the card is invalid.</exception>
        /// <exception cref="InvalidOperationException">The player does not have enough credits to purchase this card.</exception>
        public void BuyCard(ICard card)
        {
            BuyCard(card, true);
        }

        /// <summary>
        /// Adds the card at the sectorID referenced by the card and either reduces the player's credits by the cost of the card or reduces them to 0.
        /// </summary>
        /// <param name="card">The card to add.</param>
        /// <param name="removeAllCredits">If true, the player's credits will reduce to 0. Otherwise, the player's credits will reduce by the cost of the card.</param>
        /// <exception cref="ArgumentOutOfRangeException">The ID of the card is invalid.</exception>
        /// <exception cref="InvalidOperationException">The player does not have enough credits to purchase this card.</exception>
        public void BuyCard(ICard card, bool removeAllCredits)
        {
            if (Credits < card.Cost)
                throw new InvalidOperationException("The player does not have enough credits to purchase this card.");

            AddCard(card);

            Credits = removeAllCredits ? 0 : Credits - card.Cost;
        }

        /// <summary>
        /// Activates the effect of the stationed card at the given sector.
        /// </summary>
        /// <param name="sectorID">The ID of the sector whose card to activate.</param>
        /// <exception cref="ArgumentOutOfRangeException">The ID is invalid.</exception>
        public void ActivateCardEffect(int sectorID)
        {
            ICard? stationedCard = GetSector(sectorID).StationedCard;
            Debug.Assert(stationedCard != null);

            CardActivationService.ActivateStationedEffect(stationedCard, this);
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
                CardActivationService.ActivateDeployedEffect(deployedCard, this);
        }

    }

    public sealed class HumanPlayer(int id) : Player(id)
    {
    }

    public sealed class ComputerPlayer(int id) : Player(id)
    {
    }
}
