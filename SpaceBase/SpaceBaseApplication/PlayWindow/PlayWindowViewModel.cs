using SpaceBaseApplication.MainWindow;

namespace SpaceBaseApplication.PlayWindow
{
    public class PlayWindowViewModel : ViewModelBase
    {
        private readonly PlayWindow _playwindow;
        private readonly MainWindowViewModel _mainWindowViewModel;
        private bool _showOptions;
        private int _acceptedVictoryThreshold;
        private string _victoryThreshold;

        private readonly RelayCommand _playGameCommand;
        private readonly RelayCommand _quitGameCommand;
        private readonly RelayCommand _showOptionsCommand;
        private readonly RelayCommand _applyChangesCommand;
        private readonly RelayCommand _cancelChangesCommand;

        public PlayWindowViewModel()
        {
            _playwindow = new PlayWindow() { DataContext = this };
            _mainWindowViewModel = new MainWindowViewModel();
            _showOptions = false;
            _acceptedVictoryThreshold = 40;
            _victoryThreshold = _acceptedVictoryThreshold.ToString();

            _playGameCommand = new(PlayGame);
            _quitGameCommand = new(QuitGame);
            _showOptionsCommand = new(ShowOptionsDisplay);
            _applyChangesCommand = new(ApplyChanges, () => CanApplyChanges);
            _cancelChangesCommand = new(CancelChanges);
        }

        #region Properties

        /// <summary>
        /// If true, show the options display. Otherwise, hide it.
        /// </summary>
        public bool ShowOptions { get => _showOptions; set => SetProperty(ref _showOptions, value); }

        /// <summary>
        /// The amount of points in the textbox. The Apply button must be clicked to persist this value.
        /// </summary>
        public string VictoryThreshold { get => _victoryThreshold; set => SetProperty(ref _victoryThreshold, value); }

        /// <summary>
        /// True if there is a non-zero victory threshold integer.
        /// </summary>
        public bool CanApplyChanges { get => !string.IsNullOrEmpty(_victoryThreshold) && int.TryParse(_victoryThreshold, out int victoryThreshold) && victoryThreshold > 0; }

        #endregion Properties

        /// <summary>
        /// Shows the PlayWindow.
        /// </summary>
        public void Show()
        {
            Debug.Assert(_playwindow != null);
            _playwindow.Show();
        }

        /// <summary>
        /// Clean up resources.
        /// </summary>
        public void Close()
        {
            _mainWindowViewModel.Close();
        }

        #region Commands

        public ICommand PlayGameCommand { get => _playGameCommand; }

        /// <summary>
        /// Launches the Game UI and closes the PlayWindow.
        /// </summary>
        private async void PlayGame()
        {
            _playwindow.CloseAndShowMainWindow();

            _mainWindowViewModel.UpdateVictoryThreshold(_acceptedVictoryThreshold);
            _mainWindowViewModel.Show();
            await _mainWindowViewModel.StartGame();
        }

        public ICommand QuitGameCommand { get => _quitGameCommand; }

        /// <summary>
        /// Closes the application.
        /// </summary>
        private void QuitGame()
        {
            Application.Current.Shutdown();
        }

        public ICommand ShowOptionsCommand { get => _showOptionsCommand; }

        /// <summary>
        /// Shows the options display where players can customize the game.
        /// </summary>
        private void ShowOptionsDisplay()
        {
            VictoryThreshold = _acceptedVictoryThreshold.ToString();
            ShowOptions = true;
        }

        public ICommand ApplyChangesCommand { get => _applyChangesCommand; }

        /// <summary>
        /// Save any customizations made by the user and close the options display.
        /// </summary>
        private void ApplyChanges()
        {
            if (!int.TryParse(VictoryThreshold, out int victoryThreshold))
                return;

            _acceptedVictoryThreshold = victoryThreshold;
            ShowOptions = false;
        }

        public ICommand CancelChangesCommand { get => _cancelChangesCommand; }

        /// <summary>
        /// Ignore any customizations made by the user and close the options display.
        /// </summary>
        private void CancelChanges()
        {
            ShowOptions = false;
        }

        #endregion Commands
    }
}
