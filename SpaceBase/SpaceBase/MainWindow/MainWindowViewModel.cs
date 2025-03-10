namespace SpaceBase
{
    public class MainWindowViewModel : ViewModelBase
    {
        private readonly MainWindow _mainWindow;
        public HumanPlayer _humanPlayer;
        private bool _showDiceRollControl;
        private int _dice1;
        private int _dice2;
        private bool _canDragCards;

        private readonly RelayCommand _rollDiceCommand;
        private readonly RelayCommand _dontBuyCommand;

        public MainWindowViewModel()
        {
            Game = new Game();
            _humanPlayer = (HumanPlayer)Game.Players.First(p => p is HumanPlayer);
            _showDiceRollControl = false;
            _canDragCards = false;

            _rollDiceCommand = new RelayCommand(RollDice, () => CanRollDice);
            _dontBuyCommand = new RelayCommand(DontBuy, () => CanDragCards);

            _mainWindow = new MainWindow() { DataContext = this };

            WaitForPlayerInput = false;

            SubscribeToEvents();
        }

        #region Properties

        public Game Game { get; }

        public HumanPlayer HumanPlayer { get => _humanPlayer; set => SetProperty(ref _humanPlayer, value); }

        public bool IsHumanPlayerActive { get => HumanPlayer != null && Game != null && HumanPlayer.ID == Game.CurrentPlayerID; }

        public bool ShowDiceRollControl
        {
            get => _showDiceRollControl;
            set => SetProperty(ref _showDiceRollControl, value);
        }

        private bool _canRollDice = true;
        public bool CanRollDice { get => _canRollDice; set => SetProperty(ref _canRollDice, value); }

        public int Dice1 { get => _dice1; set => SetProperty(ref _dice1, value); }
        public int Dice2 { get => _dice2; set => SetProperty(ref _dice2, value); }

        public bool WaitForPlayerInput { get; set; }

        /// <summary>
        /// True if the user is allowed to drag cards. Otherwise, false.
        /// </summary>
        public bool CanDragCards { get => _canDragCards; set => SetProperty(ref _canDragCards, value); }

        #endregion Properties

        #region Events

        public event UpdateAvailableMovesFromDiceRollEventHandler<DiceRollEventArgs>? UpdateAvailableMovesFromDiceRollEventHandler;

        #endregion Events

        /// <summary>
        /// Show the main window.
        /// </summary>
        public void Show() => _mainWindow.Show();

        /// <summary>
        /// Start the game.
        /// </summary>
        public async Task StartGame()
        {
            WaitForPlayerInput = true;
            await Game.StartGame();
        }

        #region Event handlers

        /// <summary>
        /// Subscribe events to the appropriate handlers.
        /// </summary>
        private void SubscribeToEvents()
        {
            HumanPlayer.AddCardToSectorEvent += HumanPlayer_AddCardToSectorEvent;

            Game.PreDiceRollEventHandler += Game_PreDiceRollEventHandler;
            Game.DiceRollEventHandler += Game_DiceRollEventHandler;
            Game.BuyEventHandler += Game_BuyEventHandler;

            Game.TurnOverEventHandler += Game_TurnOverEventHandler;
        }

        private void HumanPlayer_AddCardToSectorEvent(object sender, AddCardToSectorEventArgs e)
        {
            WaitForPlayerInput = false;
        }

        /// <summary>
        /// Enables the button to roll dice and wait until player has clicked the button.
        /// </summary>
        /// <param name="sender">The game.</param>
        /// <param name="e">Unused event arguments.</param>
        private void Game_PreDiceRollEventHandler(object? sender, EventArgs e)
        {
            CanRollDice = true;

            while (WaitForPlayerInput) { }

            CanRollDice = false;
        }

        /// <summary>
        /// Show the dice control with the dice rolls.
        /// </summary>
        /// <param name="sender">The game.</param>
        /// <param name="e">The arguments describing the dice roll event.</param>
        private void Game_DiceRollEventHandler(object sender, DiceRollEventArgs e)
        {
            WaitForPlayerInput = true;

            Dice1 = e.Dice1;
            Dice2 = e.Dice2;

            UpdateAvailableMovesFromDiceRollEventHandler?.Invoke(this, e);

            while (WaitForPlayerInput) { }
        }

        private void Game_BuyEventHandler(object? sender, EventArgs e)
        {
            if (IsHumanPlayerActive)
            {
                WaitForPlayerInput = true;
                CanDragCards = true;

                while (WaitForPlayerInput) { }

                CanDragCards = false;
            }
        }

        /// <summary>
        /// Loop until the user interacts with the appropriate control based on the current state of the game.
        /// </summary>
        /// <remarks>This function should be spinning on a worker thread, not the UI thread.</remarks>
        /// <param name="sender">The game.</param>
        /// <param name="e">Unused arguments.</param>
        private void Game_WaitForPlayerInputEventHandler(object? sender, EventArgs e)
        {
            while (WaitForPlayerInput) { }
        }

        /// <summary>
        /// Set current state back to waiting for player input.
        /// </summary>
        /// <param name="sender">The game.</param>
        /// <param name="e">Unused arguments.</param>
        private void Game_TurnOverEventHandler(object sender, TurnOverEventArgs e)
        {
            WaitForPlayerInput = true;
            NotifyPropertyChanged(nameof(IsHumanPlayerActive));
        }

        #endregion Event handlers

        #region Commands

        /// <summary>
        /// Starts the round with the dice roll.
        /// </summary>
        public ICommand RollDiceCommand { get => _rollDiceCommand; }
        private void RollDice()
        {
            WaitForPlayerInput = false;
        }

        public ICommand DontBuyCommand { get => _dontBuyCommand; }
        private void DontBuy()
        {
            WaitForPlayerInput = false;
        }

        #endregion Commands
    }
}
