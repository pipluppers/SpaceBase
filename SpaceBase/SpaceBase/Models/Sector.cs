namespace SpaceBase.Models
{
    public class Sector
    {
        private readonly int _id;
        private Card? _activeCard;
        private readonly List<Card> _deployedCards;

        public Sector(int id, Card? card)
        {
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
        public void AddCard(Card card)
        {
            if (ActiveCard != null)
                DeployedCards.Add(ActiveCard);

            ActiveCard = card;
        }
    }
}
