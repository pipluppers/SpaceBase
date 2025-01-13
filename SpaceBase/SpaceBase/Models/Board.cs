namespace SpaceBase.Models
{
    public class Board
    {
        private readonly ObservableCollection<Sector> _sectors;

        public Board()
        {
            _sectors =
            [
                new(1, new Card(1, 0)),
                new(2, new Card(2, 0)),
                new(3, new Card(3, 0)),
                new(4, new Card(4, 0)),
                new(5, new Card(5, 0)),
                new(6, new Card(6, 0)),
                new(7, new Card(7, 0)),
                new(8, new Card(8, 0)),
                new(9, new Card(9, 0)),
                new(10, new Card(10, 0)),
                new(11, new Card(11, 0)),
                new(12, new Card(12, 0)),
            ];
        }

        public ObservableCollection<Sector> Sectors { get => _sectors; }
    }
}
