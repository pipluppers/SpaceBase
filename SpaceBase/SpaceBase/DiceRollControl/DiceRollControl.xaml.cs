namespace SpaceBase
{
    /// <summary>
    /// Interaction logic for DiceRollControl.xaml
    /// </summary>
    public partial class DiceRollControl : UserControl
    {
        public DiceRollControl()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Switches back to the screen with the cards and boards.
        /// </summary>
        /// <param name="sender">The grid that was clicked on.</param>
        /// <param name="e">The args.</param>
        private void Grid_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (sender is not Grid grid || grid.DataContext is not MainWindowViewModel viewModel)
                return;

            viewModel.ShowDiceRollControl = false;
            viewModel.WaitForPlayerInput = false;
            viewModel.IsMainWindowActive = true;
            viewModel.IsIndividualDieChosen = string.Equals(grid.Name, "IndividualGrid", StringComparison.OrdinalIgnoreCase);
            e.Handled = true;
        }
    }
}
