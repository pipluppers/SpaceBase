namespace SpaceBaseApplication.MainWindow
{
    /// <summary>
    /// Interaction logic for PlayerBoardControl.xaml
    /// </summary>
    public partial class PlayerBoardControl : UserControl
    {
        private MainWindow? _mainWindow;

        public PlayerBoardControl()
        {
            InitializeComponent();
            _mainWindow = null;
        }

        public static readonly DependencyProperty PlayerProperty =
            DependencyProperty.Register(nameof(Player), typeof(Player), typeof(PlayerBoardControl), new PropertyMetadata(Callback));

        public Player Player { get => (Player)GetValue(PlayerProperty); set => SetValue(PlayerProperty, value); }

        private static void Callback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is not PlayerBoardControl playerBoardControl || e.NewValue is not Player player)
                return;

            playerBoardControl.DataContext = player;
            playerBoardControl.Player = player;
        }

        /// <summary>
        /// Store a reference to the parent window and load all of the borders and deployed areas into the MainWindow.
        /// </summary>
        /// <param name="sender">This control.</param>
        private void PlayerBoardControl_Loaded(object sender, RoutedEventArgs _)
        {
            if (sender is not PlayerBoardControl playerBoardControl)
                return;

            if (Utilities.FindAncestor<MainWindow>(playerBoardControl) is not MainWindow mainWindow)
                return;

            _mainWindow = mainWindow;

            if (Player == null || Player.ID != 1 || _mainWindow == null)
                return;

            StoreBordersAndDeployedAreaIntoMainWindow();
        }

        /// <summary>
        /// Load all of the borders and deployed areas of the board into memory to quickly update the UI during drag/drop.
        /// </summary>
        private void StoreBordersAndDeployedAreaIntoMainWindow()
        {
            if (_mainWindow == null)
                return;

            for (int i = 0; i < SectorItemsControl.Items.Count; ++i)
            {
                var container = SectorItemsControl.ItemContainerGenerator.ContainerFromItem(SectorItemsControl.Items[i]);
                if (container != null)
                {
                    ContentPresenter? contentPresenter = Utilities.FindVisualChild<ContentPresenter>(container);
                    if (contentPresenter != null)
                    {
                        ItemsControl? deployedItemsControl = Utilities.FindVisualChild<ItemsControl>(contentPresenter);
                        if (deployedItemsControl != null)
                            _mainWindow.DeployedSectorItemsControls.Add(deployedItemsControl);

                        Border? border = Utilities.FindVisualChild<Border>(contentPresenter);
                        if (border != null)
                            _mainWindow.SectorViewBorders.Add(border);
                    }
                }
            }
        }
    }
}
