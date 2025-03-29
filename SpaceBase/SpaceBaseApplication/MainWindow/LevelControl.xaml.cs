namespace SpaceBaseApplication.MainWindow
{
    /// <summary>
    /// Interaction logic for LevelControl.xaml
    /// </summary>
    public partial class LevelControl : UserControl
    {
        private MainWindow? _mainWindow;

        public LevelControl()
        {
            InitializeComponent();
            _mainWindow = null;
        }

        public static readonly DependencyProperty DisplayedCardsProperty =
            DependencyProperty.Register(nameof(DisplayedCards), typeof(ObservableCollection<Card>), typeof(LevelControl), new PropertyMetadata(Callback));

        /// <summary>
        /// The set of cards to be displayed for each level and from which the players will be buying from.
        /// </summary>
        public ObservableCollection<Card> DisplayedCards { get => (ObservableCollection<Card>)GetValue(DisplayedCardsProperty); set => SetValue(DisplayedCardsProperty, value); }

        /// <summary>
        /// Set the ItemsSource for the ItemsControl.
        /// </summary>
        /// <param name="d">The ItemsControl.</param>
        /// <param name="e">The argument with the set of cards to buy from.</param>
        private static void Callback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            LevelControl levelControl = (LevelControl)d;

            if (e.NewValue is ObservableCollection<Card> cards)
            {
                levelControl.LevelItemsControl.ItemsSource = cards;
            }
        }

        /// <summary>
        /// Start the drag/drop.
        /// </summary>
        /// <param name="sender">The CardControl.</param>
        /// <param name="e">The mouse event args.</param>
        private void CardControl_MouseMove(object sender, MouseEventArgs e)
        {
            Debug.Assert(_mainWindow != null);
            _mainWindow.CardControl_MouseMove(sender, e);
        }

        /// <summary>
        /// Store a reference to the MainWindow.
        /// </summary>
        /// <param name="sender">This control.</param>
        private void LevelControl_Loaded(object sender, RoutedEventArgs _)
        {
            if (sender is not LevelControl levelControl)
                return;

            _mainWindow = Utilities.FindAncestor<MainWindow>(levelControl);
            Debug.Assert(_mainWindow != null);
        }
    }
}
