﻿namespace SpaceBaseApplication.MainWindow
{
    public class MainWindowViewModel : ViewModelBase
    {
        private readonly MainWindow _mainWindow;
        private bool _showEscapeMenu;
        private HumanPlayer _humanPlayer;
        private int _dice1;
        private int _dice2;
        private bool _canRollDice;
        private bool _canDragCards;
        private bool _showGameOverScreen;

        private readonly RelayCommand _showEscapeMenuCommand;
        private readonly RelayCommand _continuePlayingCommand;
        private readonly RelayCommand _quitGameCommand;
        private readonly RelayCommand _rollDiceCommand;
        private readonly RelayCommand _dontBuyCommand;
        private readonly RelayCommand<object> _chooseDiceRollKeyCommand;

        #region Events

        public event HelpDiceRollEventHandler<DiceRollEventArgs>? HelpDiceRollEvent;
        public event RemoveHelpDiceRollEffectsEventHandler<DiceRollEventArgs>? RemoveHelpDiceRollEffectsEvent;

        #endregion Events

        public MainWindowViewModel()
        {
            _showEscapeMenu = false;
            Game = new Game();
            _humanPlayer = (HumanPlayer)Game.Players.First(p => p is HumanPlayer);
            _canRollDice = true;
            _canDragCards = false;

            _showEscapeMenuCommand = new RelayCommand(ShowAndHideEscapeMenu, () => true);
            _continuePlayingCommand = new RelayCommand(ContinuePlaying, () => ShowEscapeMenu);
            _quitGameCommand = new RelayCommand(QuitGame, () => ShowEscapeMenu);
            _rollDiceCommand = new RelayCommand(RollDice, () => CanRollDice);
            _dontBuyCommand = new RelayCommand(DontBuy, () => CanDragCards);
            _chooseDiceRollKeyCommand = new RelayCommand<object>(ChooseDiceRollKey, (o) => WaitForPlayerDiceRollSelection);

            _mainWindow = new MainWindow() { DataContext = this };

            WaitForPlayerInput = false;
            WaitForPlayerDiceRollSelection = false;

            _showGameOverScreen = false;
            WinningPlayerIDs = [];

            SubscribeToEvents();
        }

        #region Properties

        /// <summary>
        /// If true, then show the escape menu. Otherwise, hide it.
        /// </summary>
        public bool ShowEscapeMenu
        {
            get => _showEscapeMenu;
            set
            {
                if (SetProperty(ref _showEscapeMenu, value))
                {
                    _continuePlayingCommand.RaiseCanExecuteChanged();
                    _quitGameCommand.RaiseCanExecuteChanged();
                }
            }
        }

        public Game Game { get; }

        public HumanPlayer HumanPlayer { get => _humanPlayer; set => SetProperty(ref _humanPlayer, value); }

        public bool IsHumanPlayerActive { get => HumanPlayer != null && Game != null && HumanPlayer.ID == Game.ActivePlayerID; }

        public bool CanRollDice { get => _canRollDice; set => SetProperty(ref _canRollDice, value); }

        public int Dice1 { get => _dice1; set => SetProperty(ref _dice1, value); }
        public int Dice2 { get => _dice2; set => SetProperty(ref _dice2, value); }

        public bool WaitForPlayerInput { get; set; }

        public bool WaitForPlayerDiceRollSelection { get; set; }

        /// <summary>
        /// True if the user is allowed to drag cards. Otherwise, false.
        /// </summary>
        public bool CanDragCards { get => _canDragCards; set => SetProperty(ref _canDragCards, value); }

        public bool ShowGameOverScreen { get => _showGameOverScreen; set => SetProperty(ref _showGameOverScreen, value); }

        public ObservableCollection<int> WinningPlayerIDs { get; }

        #endregion Properties

        /// <summary>
        /// Updates the game member with the required amount of points to win.
        /// </summary>
        /// <param name="victoryThreshold">The new required amount of points to win.</param>
        public void UpdateVictoryThreshold(int victoryThreshold)
        {
            if (victoryThreshold < 1)
                return;

            Game.UpdateVictoryThreshold(victoryThreshold);
        }

        /// <summary>
        /// Show the main window.
        /// </summary>
        public void Show() => _mainWindow.Show();

        /// <summary>
        /// Load the cards and start the game.
        /// </summary>
        public async Task StartGame()
        {
            await SpaceBase.Services.CardLoadingService.LoadCards(Game);
            WaitForPlayerInput = true;
            await Game.StartGame();
        }

        /// <summary>
        /// Clean up reference to view.
        /// </summary>
        public void Close() => _mainWindow.Close();

        #region Event handlers

        /// <summary>
        /// Subscribe events to the appropriate handlers.
        /// </summary>
        private void SubscribeToEvents()
        {
            HumanPlayer.AddCardToSectorEvent += HumanPlayer_AddCardToSectorEvent;

            Game.PreDiceRollEvent += Game_PreDiceRollEventHandler;
            Game.DiceRollEvent += Game_DiceRollEventHandler;
            Game.BuyEvent += Game_BuyEventHandler;
            Game.TurnOverEvent += Game_TurnOverEventHandler;
            Game.GameOverEvent += Game_GameOverEvent;
        }

        /// <summary>
        /// Transition to the next state and refresh buttons' enable state.
        /// </summary>
        private void HumanPlayer_AddCardToSectorEvent(object _, AddCardToSectorEventArgs __)
        {
            _rollDiceCommand.RaiseCanExecuteChanged();
            _dontBuyCommand.RaiseCanExecuteChanged();
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
            WaitForPlayerDiceRollSelection = true;

            Dice1 = e.Dice1;
            Dice2 = e.Dice2;

            HelpDiceRollEvent?.Invoke(this, e);

            while (WaitForPlayerDiceRollSelection) { }
        }

        /// <summary>
        /// If the active player is the current player, set up the window to allow buying a card.
        /// If the active player is a computer player, select a card based on a heuristic.
        /// </summary>
        private void Game_BuyEventHandler(object? _, EventArgs __)
        {
            if (IsHumanPlayerActive)
            {
                WaitForPlayerInput = true;
                CanDragCards = true;

                while (WaitForPlayerInput) { }

                CanDragCards = false;
            }
            else
            {
                CanDragCards = false;

                ComputerPlayer? computerPlayer = Game.Players[Game.ActivePlayerID - 1] as ComputerPlayer;
                Debug.Assert(computerPlayer != null);

                IStandardCard? cardToBuy = null;
                static IStandardCard? GetCardToBuy(ObservableCollection<IStandardCard> cards, int credits)
                {
                    // TODO Always buy the first card you can
                    foreach (var card in cards)
                    {
                        if (card.Cost <= credits)
                            return card;
                    }

                    return null;
                }

                if (computerPlayer.Credits >= 12)
                    cardToBuy = GetCardToBuy(Game.Level3Cards, computerPlayer.Credits);

                if (cardToBuy == null && computerPlayer.Credits >= 7)
                    cardToBuy = GetCardToBuy(Game.Level2Cards, computerPlayer.Credits);

                cardToBuy ??= GetCardToBuy(Game.Level1Cards, computerPlayer.Credits);

                if (cardToBuy != null)
                {
                    Application.Current.Dispatcher.Invoke(() =>
                    {
                        computerPlayer.BuyCard(cardToBuy);
                    });
                }
            }
        }

        /// <summary>
        /// Set current state back to waiting for player input.
        /// </summary>
        private void Game_TurnOverEventHandler(object _, EventArgs __)
        {
            WaitForPlayerInput = true;
            NotifyPropertyChanged(nameof(IsHumanPlayerActive));
        }

        private void Game_GameOverEvent(object sender, GameOverEventArgs e)
        {
            int maxVictoryPoints = Game.Players.Select(p => p.VictoryPoints).Max();
            foreach (var player in Game.Players)
            {
                if (player.VictoryPoints == maxVictoryPoints)
                    WinningPlayerIDs.Add(player.ID);
            }

            ShowGameOverScreen = true;
        }

        #endregion Event handlers

        #region Commands

        public ICommand ShowEscapeMenuCommand { get => _showEscapeMenuCommand; }
        /// <summary>
        /// Hide the game display and show the escape menu. If the escape menu is currently being displayed, then show the screen before the Esc key was clicked.
        /// </summary>
        private void ShowAndHideEscapeMenu()
        {
            if (!ShowEscapeMenu)
            {
                ShowEscapeMenu = true;
                ShowGameOverScreen = false;
            }
            else
            {
                ContinuePlaying();
            }
        }

        public ICommand ContinuePlayingCommand { get => _continuePlayingCommand; }
        /// <summary>
        /// Hide the escape menu and show the game display again.
        /// </summary>
        private void ContinuePlaying()
        {
            ShowEscapeMenu = false;

            if (WinningPlayerIDs.Count > 0)
                ShowGameOverScreen = true;
        }

        public ICommand QuitGameCommand { get => _quitGameCommand; }
        /// <summary>
        /// Close the application.
        /// </summary>
        private void QuitGame()
        {
            Application.Current.Shutdown();
        }

        public ICommand RollDiceCommand { get => _rollDiceCommand; }
        /// <summary>
        /// Starts the round with the dice roll.
        /// </summary>
        private void RollDice()
        {
            WaitForPlayerInput = false;
        }

        public ICommand DontBuyCommand { get => _dontBuyCommand; }
        /// <summary>
        /// Skip the buy phase and continue the game.
        /// </summary>
        private void DontBuy()
        {
            WaitForPlayerInput = false;
        }

        public ICommand ChooseDiceRollKeyCommand { get => _chooseDiceRollKeyCommand; }
        /// <summary>
        /// Activates the card effects at the given sectors if applicable.
        /// </summary>
        /// <param name="parameter">The ID of the sector that was pressed.</param>
        private void ChooseDiceRollKey(object parameter)
        {
            if (!int.TryParse(parameter?.ToString(), out int chosenValue))
                return;

            if (chosenValue != Dice1 && chosenValue != Dice2 && chosenValue != Dice1 + Dice2)
                return;

            Utilities.ActivateCurrentPlayerCardEffects(HumanPlayer, chosenValue, Dice1, Dice2, Game.ActivePlayerID);
            Utilities.ChooseComputerPlayersSectors(Game.Players, Dice1, Dice2, Game.ActivePlayerID);

            RemoveHelpDiceRollEffectsEvent?.Invoke(this, new DiceRollEventArgs(Dice1, Dice2, Game.ActivePlayerID));

            WaitForPlayerDiceRollSelection = false;
        }

        #endregion Commands
    }
}
