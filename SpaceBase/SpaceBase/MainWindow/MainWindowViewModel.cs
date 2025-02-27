namespace SpaceBase
{
    public class MainWindowViewModel : ViewModelBase
    {
        private readonly MainWindow _mainWindow;
        private bool _showDiceRollControl;
        private int _dice1;
        private int _dice2;

        private readonly RelayCommand _startRoundCommand;
        private readonly RelayCommand _showMainWindowCommand;
        private readonly RelayCommand _returnToDiceControl;

        public MainWindowViewModel()
        {
            Game = new Game();
            _humanPlayer = (HumanPlayer)Game.Players.First(p => p is HumanPlayer);
            _showDiceRollControl = false;

            _startRoundCommand = new RelayCommand(StartRound, () => true);
            _showMainWindowCommand = new RelayCommand(ShowMainWindow, () => true);
            _returnToDiceControl = new RelayCommand(ReturnToDiceControl, () => true);

            _mainWindow = new MainWindow() { DataContext = this };

            WaitForPlayerInput = false;

            SubscribeToEvents();
        }

        #region Properties

        public Game Game { get; }

        public HumanPlayer _humanPlayer;
        public HumanPlayer HumanPlayer { get => _humanPlayer; set => SetProperty(ref _humanPlayer, value); }

        public bool ShowDiceRollControl
        {
            get => _showDiceRollControl;
            set => SetProperty(ref _showDiceRollControl, value);
        }

        private bool _isMainWindowActive = true;
        public bool IsMainWindowActive
        {
            get => _isMainWindowActive;
            set => SetProperty(ref _isMainWindowActive, value);
        }

        private bool _isIndividualDieChosen;
        public bool IsIndividualDieChosen
        {
            get => _isIndividualDieChosen;
            set
            {
                _isIndividualDieChosen = value;
                if (value)
                {
                    HumanPlayer.ActivateCardEffect(Dice1);
                    HumanPlayer.ActivateCardEffect(Dice2);
                }
                else
                {
                    HumanPlayer.ActivateCardEffect(Dice1 + Dice2);
                }
            }
        }

        public int Dice1 { get => _dice1; set => SetProperty(ref _dice1, value); }
        public int Dice2 { get => _dice2; set => SetProperty(ref _dice2, value); }

        public bool WaitForPlayerInput { get; set; }

        #endregion Properties

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
            Game.WaitForPlayerInputEventHandler += Game_WaitForPlayerInputEventHandler;
            Game.DiceRollEventHandler += Game_DiceRollEventHandler;
            Game.TurnOverEventHandler += Game_TurnOverEventHandler;
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
        /// Show the dice control with the dice rolls.
        /// </summary>
        /// <param name="sender">The game.</param>
        /// <param name="e">The arguments describing the dice roll event.</param>
        private void Game_DiceRollEventHandler(object sender, DiceRollEventArgs e)
        {
            Dice1 = e.Dice1;
            Dice2 = e.Dice2;
            ShowDiceRollControl = true;
            WaitForPlayerInput = true;
        }

        /// <summary>
        /// Set current state back to waiting for player input.
        /// </summary>
        /// <param name="sender">The game.</param>
        /// <param name="e">Unused arguments.</param>
        private void Game_TurnOverEventHandler(object sender, TurnOverEventArgs e)
        {
            WaitForPlayerInput = true;
        }

        #endregion Event handlers

        #region Commands

        /// <summary>
        /// Starts the round with the dice roll.
        /// </summary>
        public ICommand StartRoundCommand { get => _startRoundCommand; }
        private void StartRound()
        {
            WaitForPlayerInput = false;
        }

        /// <summary>
        /// Temporarily display the main window.
        /// </summary>
        /// <remarks>
        /// The only available action in this state is to go back to the dice control.
        /// This is used for when players want to see the board before making their decision.
        /// </remarks>
        public ICommand ShowMainWindowCommand { get => _showMainWindowCommand; }
        private void ShowMainWindow()
        {
            ShowDiceRollControl = false;
            IsMainWindowActive = false;
        }

        /// <summary>
        /// Display the dice control screen.
        /// </summary>
        public ICommand ReturnToDiceControlCommand { get => _returnToDiceControl; }
        private void ReturnToDiceControl()
        {
            ShowDiceRollControl = true;
        }

        #endregion Commands
    }
}
