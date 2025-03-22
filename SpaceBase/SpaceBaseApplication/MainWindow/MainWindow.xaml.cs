namespace SpaceBaseApplication.MainWindow
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public const int Left1 = 21;
        private readonly List<Border> _sectorViewBorders;
        private readonly List<ItemsControl> _deployedSectorItemsControls;
        private DiceRollEventArgs _currentDiceRollArgs;

        public MainWindow()
        {
            InitializeComponent();
            Title = Constants.GameTitle;
            Icon = new BitmapImage(new Uri(Constants.IconPath));

            _sectorViewBorders = [];
            _deployedSectorItemsControls = [];
            _currentDiceRollArgs = new DiceRollEventArgs(0, 0, 0);

            DataContextChanged += MainWindow_DataContextChanged;
        }

        public List<Border> SectorViewBorders { get => _sectorViewBorders; }
        public List<ItemsControl> DeployedSectorItemsControls { get => _deployedSectorItemsControls; }

        /// <summary>
        /// Adds necessary event handlers.
        /// </summary>
        private void MainWindow_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (e.OldValue is MainWindowViewModel oldViewModel)
            {
                oldViewModel.HelpDiceRollEvent -= ViewModel_HelpDiceRollEventHandler;
                oldViewModel.RemoveHelpDiceRollEffectsEvent -= ViewModel_RemoveHelpDiceRollEffectsEvent;
            }

            if (e.NewValue is MainWindowViewModel newViewModel)
            {
                newViewModel.HelpDiceRollEvent += ViewModel_HelpDiceRollEventHandler;
                newViewModel.RemoveHelpDiceRollEffectsEvent += ViewModel_RemoveHelpDiceRollEffectsEvent;
            }
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
                    ItemsControl itemsControl = _deployedSectorItemsControls[i];

                    if (i == e.Dice1 - 1 || i == e.Dice2 - 1)
                    {
                        var borderHighlightAdorner = new BorderIndividualRewardAdorner(border);
                        AdornerLayer layer = AdornerLayer.GetAdornerLayer(border);
                        layer.Add(borderHighlightAdorner);

                        border.MouseDown += SectorView_MouseDown;

                        var deployedBorderHighlightAdorner = new BorderIndividualRewardAdorner(itemsControl);
                        AdornerLayer deployedLayer = AdornerLayer.GetAdornerLayer(itemsControl);
                        deployedLayer.Add(deployedBorderHighlightAdorner);

                        if (e.ActivePlayerID != 1)
                            border.Opacity = 0.5;
                        else
                            itemsControl.Opacity = 0.5;
                    }
                    else if (i == e.Dice1 + e.Dice2 - 1)
                    {
                        var borderHighlightAdorner = new BorderSumRewardAdorner(border);
                        AdornerLayer layer = AdornerLayer.GetAdornerLayer(border);
                        layer.Add(borderHighlightAdorner);

                        border.MouseDown += SectorView_MouseDown;

                        var deployedBorderHighlightAdorner = new BorderSumRewardAdorner(itemsControl);
                        AdornerLayer deployedLayer = AdornerLayer.GetAdornerLayer(itemsControl);
                        deployedLayer.Add(deployedBorderHighlightAdorner);

                        if (e.ActivePlayerID != 1)
                            border.Opacity = 0.5;
                        else
                            itemsControl.Opacity = 0.5;
                    }
                    else
                    {
                        border.Opacity = 0.5;
                        itemsControl.Opacity = 0.5;
                    }
                }
            }, System.Windows.Threading.DispatcherPriority.Input);

            return;
        }

        /// <summary>
        /// Activates the effects of the clicked sectors.
        /// </summary>
        /// <remarks>This handler will also choose the cards for the computer players.</remarks>
        /// <param name="sender">The sector view reprsenting the clicked sector.</param>
        private void SectorView_MouseDown(object sender, MouseButtonEventArgs _)
        {
            if (sender is not Border sectorBorder || sectorBorder.DataContext is not Sector sector || DataContext is not MainWindowViewModel viewModel)
                return;

            int dice1 = _currentDiceRollArgs.Dice1, dice2 = _currentDiceRollArgs.Dice2;

            if (sector.ID != dice1 && sector.ID != dice2 && sector.ID != dice1 + dice2)
                return;

            HumanPlayer player = viewModel.HumanPlayer;

            Utilities.ActivateCurrentPlayerCardEffects(player, sector.ID, dice1, dice2, _currentDiceRollArgs.ActivePlayerID);
            Utilities.ChooseComputerPlayersSectors(viewModel.Game.Players, dice1, dice2, _currentDiceRollArgs.ActivePlayerID);

            RemoveHelpDiceRollEffects(dice1, dice2);

            viewModel.WaitForPlayerDiceRollSelection = false;
        }

        /// <summary>
        /// Removes adorners, event handlers, and transparencies.
        /// </summary>
        /// <param name="e">The dice roll event arguments.</param>
        private void ViewModel_RemoveHelpDiceRollEffectsEvent(object _, DiceRollEventArgs e)
        {
            RemoveHelpDiceRollEffects(e.Dice1, e.Dice2);
        }

        /// <summary>
        /// Removes adorners, event handlers, and transparencies.
        /// </summary>
        /// <param name="dice1">The value of the first sector to remove adorners and event handlers.</param>
        /// <param name="dice2">The value of the second sector to remove adorners and event handlers.</param>
        private void RemoveHelpDiceRollEffects(int dice1, int dice2)
        {
            for (int i = 0; i < _sectorViewBorders.Count; ++i)
            {
                Border border = _sectorViewBorders[i];
                ItemsControl itemsControl = _deployedSectorItemsControls[i];

                if (i == dice1 - 1 || i == dice2 - 1 || i == (dice1 + dice2 - 1))
                {
                    Utilities.RemoveAllAdorners(border);
                    Utilities.RemoveAllAdorners(itemsControl);
                    border.MouseDown -= SectorView_MouseDown;
                }

                border.Opacity = 1.0;
                itemsControl.Opacity = 1.0;
            }
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
