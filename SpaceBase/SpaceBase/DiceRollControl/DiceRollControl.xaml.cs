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

        public static readonly DependencyProperty Dice1Property =
            DependencyProperty.Register("Dice1", typeof(int), typeof(DiceRollControl), new PropertyMetadata());

        public int Dice1 { get => (int)GetValue(Dice1Property); set => SetValue(Dice1Property, value); }

        public static readonly DependencyProperty Dice2Property =
            DependencyProperty.Register("Dice2", typeof(int), typeof(DiceRollControl), new PropertyMetadata());

        public int Dice2 { get => (int)GetValue(Dice2Property); set => SetValue(Dice2Property, value); }

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
            e.Handled = true;
        }
    }
}
