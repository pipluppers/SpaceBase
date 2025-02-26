namespace SpaceBase
{
    /// <summary>
    /// Interaction logic for DiceSectorControl.xaml
    /// </summary>
    public partial class DiceSectorControl : UserControl
    {
        public DiceSectorControl()
        {
            InitializeComponent();

            Dice2Value = -1;
        }

        public readonly static DependencyProperty Dice1ValueProperty =
            DependencyProperty.Register("Dice1Value", typeof(int), typeof(DiceSectorControl), new PropertyMetadata());

        /// <summary>
        /// The value of the first dice.
        /// </summary>
        public int Dice1Value { get => (int)GetValue(Dice1ValueProperty); set { SetValue(Dice1ValueProperty, value); } }

        public readonly static DependencyProperty Dice2ValueProperty =
            DependencyProperty.Register("Dice2Value", typeof(int), typeof(DiceSectorControl), new PropertyMetadata());

        /// <summary>
        /// The value will be -1 if this control is not referencing the sum. Otherwise, it will be the value of the second dice.
        /// </summary>
        public int Dice2Value { get => (int)GetValue(Dice2ValueProperty); set { SetValue(Dice2ValueProperty, value); } }

        /// <summary>
        /// If the control is made visible, update the text showing the sector number.
        /// </summary>
        /// <param name="sender">The dice sector control.</param>
        /// <param name="e">True if visible. Otherwise, false.</param>
        private void DiceSectorControl_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (!(bool)e.NewValue)
                return;

            int valueText = Dice2Value < 0 ? Dice1Value : Dice1Value + Dice2Value;
            SectorTextBlock.Text = $"Sector {valueText}";
        }
    }
}
