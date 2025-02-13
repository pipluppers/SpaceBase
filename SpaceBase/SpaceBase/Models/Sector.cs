namespace SpaceBase.Models
{
    public class Sector
    {
        private readonly int _id;
        private Card? _activeCard;
        private readonly List<Card> _deployedCards;

        public Sector(int id, Card? card)
        {
            if (card != null && id != card.SectorID)
                throw new ArgumentException("The card has sector ID {card.SectorID} which cannot be added to sector {ID}.");

            _id = id;
            _activeCard = card;
            _deployedCards = [];
        }

        public int ID { get => _id; }
        public Card? ActiveCard { get => _activeCard; private set => _activeCard = value; }
        public List<Card> DeployedCards => _deployedCards;

        /// <summary>
        /// Deploys the current card and sets the current card to the provided one.
        /// </summary>
        /// <param name="card">The new active card.</param>
        /// <exception cref="ArgumentException">The ID of the sector and card do not match.</exception>
        public void AddCard(Card card)
        {
            if (ID != card.SectorID)
                throw new ArgumentException($"The card has sector ID {card.SectorID} which cannot be added to sector {ID}.");

            if (ActiveCard != null)
                DeployedCards.Add(ActiveCard);

            ActiveCard = card;
        }
    }
}
