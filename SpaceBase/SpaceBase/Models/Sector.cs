namespace SpaceBase.Models
{
    /// <summary>
    /// Represents an object containing stationed and deployed cards.
    /// </summary>
    public sealed class Sector : PropertyChangedBase
    {
        private ICard? _stationedCard;
        private readonly ObservableCollection<IStandardCard> _deployedCards;

        public Sector(int id, ICard? card)
        {
            if (card != null && id != card.SectorID)
                throw new ArgumentException($"The card has sector ID {card.SectorID} which cannot be added to sector {ID}.");

            ID = id;
            _stationedCard = card;
            _deployedCards = [];
        }

        /// <summary>
        /// The ID of the sector.
        /// </summary>
        public int ID { get; }

        /// <summary>
        /// The card currently being stationed in the sector.
        /// </summary>
        public ICard? StationedCard { get => _stationedCard; private set => SetProperty(ref _stationedCard, value); }

        /// <summary>
        /// The cards that are deployed in the sector. This can not include colony cards.
        /// </summary>
        public ObservableCollection<IStandardCard> DeployedCards => _deployedCards;

        /// <summary>
        /// Deploys the currently stationed card and sets the stationed card to the provided one.
        /// </summary>
        /// <param name="card">The new stationed card. This can be a standard card or a colony card.</param>
        /// <exception cref="ArgumentException">The ID of the sector and card do not match.</exception>
        /// <exception cref="InvalidOperationException">The sector currently has a colony card stationed which cannot be deployed.</exception>
        public void AddCard(ICard card)
        {
            if (ID != card.SectorID)
                throw new ArgumentException($"The card has sector ID {card.SectorID} which cannot be added to sector {ID}.");

            if (StationedCard is IColonyCard)
                throw new InvalidOperationException($"Sector {ID} currently has a colony card stationed that cannot be deployed.");

            if (StationedCard is IStandardCard standardCard)
            {
                DeployedCards.Add(standardCard);
                //Card? nonColonyCard = StationedCard as Card;
                //Debug.Assert(nonColonyCard != null);
                //DeployedCards.Add(nonColonyCard);
            }

            StationedCard = card;
        }
    }
}
