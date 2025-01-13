namespace SpaceBase.Models
{
    public class Sector
    {
        private readonly int _id;
        private Card _card;
        private readonly List<Card> _deployedCards;

        internal Sector(int id, Card card)
        {
            _id = id;
            _card = card;
            _deployedCards = [];
        }

        public int ID => _id;
        public Card Card { get => _card; set => _card = value; }
        public List<Card> DeployedCards => _deployedCards;
    }
}
