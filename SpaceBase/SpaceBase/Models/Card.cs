namespace SpaceBase.Models
{
    public class Card(int sectorID, int cost)
    {
        private readonly int _sectorID = sectorID;
        private readonly int _cost = cost;

        public int SectorID { get => _sectorID; }
        public int Cost { get => _cost; }
    }
}
