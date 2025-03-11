﻿using System.Windows.Documents;
using System.Windows.Media.Imaging;

namespace SpaceBase
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public const int Left1 = 21;
        private readonly List<Border> _sectorViewBorders;
        private DiceRollEventArgs _currentDiceRollArgs;

        public MainWindow()
        {
            InitializeComponent();
            Title = Constants.GameTitle;
            Icon = new BitmapImage(new Uri(Constants.IconPath));

            _sectorViewBorders = [];
            _currentDiceRollArgs = new DiceRollEventArgs(0, 0, 0);

            DataContextChanged += MainWindow_DataContextChanged;
        }

        /// <summary>
        /// Adds necessary event handlers.
        /// </summary>
        private void MainWindow_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (e.OldValue is MainWindowViewModel oldViewModel)
                oldViewModel.HelpDiceRollEvent -= ViewModel_HelpDiceRollEventHandler;

            if (e.NewValue is MainWindowViewModel newViewModel)
                newViewModel.HelpDiceRollEvent += ViewModel_HelpDiceRollEventHandler;
        }

        /// <summary>
        /// Adds click event to sectors referenced by individual dice rolls or the sum. Also adds colored border and makes irrelevant sectors more transparent.
        /// </summary>
        /// <param name="sender">The view model.</param>
        /// <param name="e">The arguments describing the dice roll.</param>
        private async void ViewModel_HelpDiceRollEventHandler(object sender, DiceRollEventArgs e)
        {
            if (_sectorViewBorders == null)
                return;

            _currentDiceRollArgs = e;

            // Adorners can only be added on the UI thread
            await Dispatcher.BeginInvoke(() =>
            {
                for (int i = 0; i < _sectorViewBorders.Count; ++i)
                {
                    Border border = _sectorViewBorders[i];

                    if (i == e.Dice1 - 1 || i == e.Dice2 - 1)
                    {
                        var borderHighlightAdorner = new BorderIndividualRewardAdorner(border);
                        AdornerLayer layer = AdornerLayer.GetAdornerLayer(border);
                        layer.Add(borderHighlightAdorner);

                        border.MouseDown += SectorView_MouseDown;
                    }
                    else if (i == e.Dice1 + e.Dice2 - 1)
                    {
                        var borderHighlightAdorner = new BorderSumRewardAdorner(border);
                        AdornerLayer layer = AdornerLayer.GetAdornerLayer(border);
                        layer.Add(borderHighlightAdorner);

                        border.MouseDown += SectorView_MouseDown;
                    }
                    else
                    {
                        border.Opacity = 0.5;
                    }
                }
            }, System.Windows.Threading.DispatcherPriority.Input);

            return;
        }

        /// <summary>
        /// Activates the effects of the clicked sectors.
        /// </summary>
        /// <param name="sender">The sector view reprsenting the clicked sector.</param>
        private void SectorView_MouseDown(object sender, MouseButtonEventArgs _)
        {
            if (sender is not Border border || border.DataContext is not Sector sector || DataContext is not MainWindowViewModel viewModel)
                return;

            static void ActivateEffect(Player player, int sectorID, int currentPlayerID)
            {
                if (player.ID == currentPlayerID) player.ActivateCardEffect(sectorID);
                else player.ActivateDeployedCardsEffect(sectorID);
            };

            int dice1 = _currentDiceRollArgs.Dice1, dice2 = _currentDiceRollArgs.Dice2, currentPlayerID = _currentDiceRollArgs.CurrentPlayerID;

            HumanPlayer player = viewModel.HumanPlayer;

            if (sector.ID == dice1 || sector.ID == dice2)
            {
                ActivateEffect(player, dice1, currentPlayerID);
                ActivateEffect(player, dice2, currentPlayerID);
            }
            else if (sector.ID == dice1 + dice2)
            {
                ActivateEffect(player, dice1 + dice2, currentPlayerID);
            }
            else
            {
                Debug.Assert(false);
            }

            // Remove adorners, event handlers, and transparencies

            for (int i = 0; i < _sectorViewBorders.Count; ++i)
            {
                Border border2 = _sectorViewBorders[i];

                if (i == dice1 - 1 || i == dice2 - 1 || i == (dice1 + dice2 - 1))
                {
                    Utilities.RemoveAllAdorners(border2);
                    border2.MouseDown -= SectorView_MouseDown;
                }
                else
                {
                    border2.Opacity = 1.0;
                }
            }

            viewModel.WaitForPlayerInput = false;
        }

        private void CardControl_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (sender is not CardControl)
                return;

            e.Handled = true;
        }

        /// <summary>
        /// Start the drag/drop.
        /// </summary>
        /// <param name="sender">The CardControl.</param>
        /// <param name="e">The mouse event args.</param>
        private void CardControl_MouseMove(object sender, MouseEventArgs e)
        {
            if (sender is not CardControl cardControl || cardControl.DataContext is not Card card || e.LeftButton != MouseButtonState.Pressed)
                return;

            if (DataContext is not MainWindowViewModel viewModel || !viewModel.CanDragCards || card.Cost > viewModel.HumanPlayer.Credits)
                return;

            Border? border = _sectorViewBorders[card.SectorID - 1];
            Debug.Assert(border != null);

            var borderHighlightAdorner = new BorderDragAdorner(border);
            AdornerLayer layer = AdornerLayer.GetAdornerLayer(border);

            try
            {
                layer.Add(borderHighlightAdorner);

                for (int i = 0; i < 12; ++i) if (i != card.SectorID - 1) _sectorViewBorders[i].Opacity = 0.5;

                DragDrop.DoDragDrop(cardControl, Utilities.Serialize(card), DragDropEffects.Move);
            }
            catch (Exception ex)
            {
                Trace.WriteLine($"Failed to drag/drop: {ex.Message}");
            }
            finally
            {
                layer.Remove(borderHighlightAdorner);

                for (int i = 0; i < 12; ++i) if (i != card.SectorID - 1) _sectorViewBorders[i].Opacity = 1.0;
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
                            _sectorViewBorders.Add(border);
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

    /// <summary>
    /// Adorner to add a yellow highlight around a border to help user pick a sector to drag a card to.
    /// </summary>
    public class BorderDragAdorner : Adorner
    {
        private readonly Pen _highlightedPen;

        public BorderDragAdorner(UIElement adornedElement) : base(adornedElement)
        {
            _highlightedPen = new Pen(Brushes.Yellow, 5);
            _highlightedPen.Freeze();
        }

        protected override void OnRender(DrawingContext drawingContext)
        {
            var adornedElementBoundingBox = new Rect(AdornedElement.RenderSize);
            drawingContext.DrawRectangle(null, _highlightedPen, adornedElementBoundingBox);
        }
    }

    /// <summary>
    /// Adorner to add a orange highlight around a border to help user pick a sector to gain rewards from after a dice roll.
    /// </summary>
    public class BorderIndividualRewardAdorner : Adorner
    {
        private readonly Pen _highlightedPen;

        public BorderIndividualRewardAdorner(UIElement adornedElement) : base(adornedElement)
        {
            _highlightedPen = new Pen(Brushes.Orange, 5);
            _highlightedPen.Freeze();
        }

        protected override void OnRender(DrawingContext drawingContext)
        {
            var adornedElementBoundingBox = new Rect(AdornedElement.RenderSize);
            drawingContext.DrawRectangle(null, _highlightedPen, adornedElementBoundingBox);
        }
    }

    /// <summary>
    /// Adorner to add a purple highlight around a border to help user pick a sector to gain rewards from after a dice roll.
    /// </summary>
    public class BorderSumRewardAdorner : Adorner
    {
        private readonly Pen _highlightedPen;

        public BorderSumRewardAdorner(UIElement adornedElement) : base(adornedElement)
        {
            _highlightedPen = new Pen(Brushes.Purple, 5);
            _highlightedPen.Freeze();
        }

        protected override void OnRender(DrawingContext drawingContext)
        {
            var adornedElementBoundingBox = new Rect(AdornedElement.RenderSize);
            drawingContext.DrawRectangle(null, _highlightedPen, adornedElementBoundingBox);
        }
    }

}
