namespace SpaceBase.Models
{
    public class Card
    {
        private readonly int _sectorID;
        private readonly int _cost;

        public Card(int sectorID, int cost)
        {
            if (sectorID < 1 || sectorID > 12)
                throw new NotSupportedException("The sector must be between 1 and 12 inclusive.");

            _sectorID = sectorID;
            _cost = cost;
        }

        public int SectorID { get => _sectorID; }
        public int Cost { get => _cost; }

        public delegate void Effect();

        public void ActivateActiveEffect()
        {

        }

        public void ActivateDeployedEffect()
        {

        }
    }
}
