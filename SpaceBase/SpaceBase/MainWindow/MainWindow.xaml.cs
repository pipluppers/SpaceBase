using System.Windows.Media.Imaging;

namespace SpaceBase
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public const int Left1 = 21;
        private readonly List<Border> _borders;

        public MainWindow()
        {
            InitializeComponent();
            Title = Constants.GameTitle;
            Icon = new BitmapImage(new Uri(Constants.IconPath));

            _borders = [];
        }

        private void CardControl_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (sender is not CardControl)
                return;

            e.Handled = true;
        }

        private void CardControl_MouseMove(object sender, MouseEventArgs e)
        {
            if (sender is not CardControl cardControl || cardControl.DataContext is not Card card || e.LeftButton != MouseButtonState.Pressed)
                return;

            if (DataContext is not MainWindowViewModel viewModel || !viewModel.CanDragCards || card.Cost > viewModel.HumanPlayer.Credits)
                return;

            Border? border = _borders[card.SectorID - 1];
            Debug.Assert(border != null);

            try
            {
                border.BorderThickness = new Thickness(5);
                border.BorderBrush = Brushes.Yellow;
                for (int i = 0; i < 12; ++i) if (i != card.SectorID - 1) _borders[i].Opacity = 0.5;

                DragDrop.DoDragDrop(cardControl, Utilities.Serialize(card), DragDropEffects.Move);
            }
            catch (Exception ex)
            {
                Trace.WriteLine($"Failed to drag/drop: {ex.Message}");
            }
            finally
            {
                border.BorderThickness = new Thickness(1);
                border.BorderBrush = Brushes.Black;
                for (int i = 0; i < 12; ++i) if (i != card.SectorID - 1) _borders[i].Opacity = 1.0;
            }
        }

        /// <summary>
        /// Adds the dragged card to the sector at the dropped location.
        /// </summary>
        /// <param name="sender">The dropped location.</param>
        /// <param name="e">The arguments with the dragged card.</param>
        private void Border_Drop(object sender, DragEventArgs e)
        {
            if (sender is not Border border || e.Source is not CardControl)
                return;

            if (border.DataContext is not Sector sector)
                return;

            string serializedString = (string)e.Data.GetData(DataFormats.Text);

            Card? card = JsonSerializer.Deserialize<Card>(serializedString);

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
                player.AddCard(card);
                player.ResetCredits();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"STRAUGHT TO JAIL. {ex.Message}");
            }
        }

        /// <summary>
        /// Cache all of the borders to quickly update the UI during drag/drop.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SectorItemsControl_Loaded(object sender, RoutedEventArgs e)
        {
            if (sender is not ItemsControl itemsControl)
                return;

            for (int i = 0; i < itemsControl.Items.Count; ++i)
            {
                var container = itemsControl.ItemContainerGenerator.ContainerFromItem(itemsControl.Items[i]);
                if (container != null)
                {
                    ContentPresenter? contentPresenter = Utilities.FindVisualChild<ContentPresenter>(container);
                    if (contentPresenter != null)
                    {
                        Border? border = Utilities.FindVisualChild<Border>(contentPresenter);

                        if (border != null)
                            _borders.Add(border);
                    }
                }
            }
        }

        /// <summary>
        /// Close the application.
        /// </summary>
        private void MainGameWindow_Closed(object sender, EventArgs e)
        {
            Application.Current.Shutdown();
        }
    }
}
