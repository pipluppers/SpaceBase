namespace SpaceBaseApplication.MainWindow
{
    /// <summary>
    /// Interaction logic for SectorView.xaml
    /// </summary>
    public partial class SectorView : UserControl
    {
        public SectorView()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Adds the dragged card to the sector at the dropped location.
        /// If the ID of the card does not match the sector or if the sector has a colony card stationed, then no-op.
        /// </summary>
        /// <param name="sender">The sector that received the card.</param>
        /// <param name="e">The arguments with the dragged card.</param>
        private void Border_Drop(object sender, DragEventArgs e)
        {
            if (sender is not Border border || e.Source is not CardControl)
                return;

            if (border.DataContext is not Sector sector || sector.StationedCard is IColonyCard)
                return;

            string serializedString = (string)e.Data.GetData(DataFormats.Text);

            CardBase? card;
            if (serializedString.Contains("Level"))
            {
                card = JsonSerializer.Deserialize<Card>(serializedString);
            }
            else
            {
                card = JsonSerializer.Deserialize<ColonyCard>(serializedString);
            }

            if (card == null)
                return;

            if (card.SectorID != sector.ID)
                return;

            Grid? grid = Utilities.FindAncestor<Grid>(border, 2);
            if (grid == null)
                return;

            if (grid.DataContext is not Player player)
                return;

            try
            {
                player.BuyCard(card);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"STRAUGHT TO JAIL. {ex.Message}");
            }
        }
    }
}
