namespace SpaceBase.Models
{
    public class Board
    {
        private readonly ObservableCollection<Sector> _sectors;

        public Board()
        {
            _sectors =
            [
                new(1, null),
                new(2, null),
                new(3, null),
                new(4, null),
                new(5, null),
                new(6, null),
                new(7, null),
                new(8, null),
                new(9, null),
                new(10, null),
                new(11, null),
                new(12, null),
            ];
        }

        public ObservableCollection<Sector> Sectors { get => _sectors; }
    }
}
