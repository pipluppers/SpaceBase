namespace SpaceBase.Models
{
    /// <summary>
    /// Represents an object containing sectors.
    /// </summary>
    public sealed class Board
    {
        public Board()
        {
            Sectors =
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

        /// <summary>
        /// The list of sectors.
        /// </summary>
        public ObservableCollection<Sector> Sectors { get; }
    }
}
