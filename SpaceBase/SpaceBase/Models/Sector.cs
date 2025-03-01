namespace SpaceBase.Models
{
    public class Sector : PropertyChangedBase
    {
        private readonly int _id;
        private Card? _stationedCard;
        private readonly ObservableCollection<Card> _deployedCards;

        public Sector(int id, Card? card)
        {
            if (card != null && id != card.SectorID)
                throw new ArgumentException("The card has sector ID {card.SectorID} which cannot be added to sector {ID}.");

            _id = id;
            _stationedCard = card;
            _deployedCards = [];
        }

        public int ID { get => _id; }
        public Card? StationedCard { get => _stationedCard; private set => SetProperty(ref _stationedCard, value); }
        public ObservableCollection<Card> DeployedCards => _deployedCards;

        /// <summary>
        /// Deploys the current card and sets the current card to the provided one.
        /// </summary>
        /// <param name="card">The new stationed card.</param>
        /// <exception cref="ArgumentException">The ID of the sector and card do not match.</exception>
        public void AddCard(Card card)
        {
            if (ID != card.SectorID)
                throw new ArgumentException($"The card has sector ID {card.SectorID} which cannot be added to sector {ID}.");

            if (StationedCard != null)
                DeployedCards.Add(StationedCard);

            StationedCard = card;
        }
    }
}
