using System.Windows.Input;

namespace SpaceBase
{
    public class PlayWindowViewModel : ViewModelBase
    {
        private readonly PlayWindow _playwindow;
        private readonly MainWindowViewModel _mainWindowViewModel;

        private readonly RelayCommand _playGameCommand;
        private readonly RelayCommand _quitGameCommand;

        public PlayWindowViewModel()
        {
            _playwindow = new PlayWindow() { DataContext = this };
            _mainWindowViewModel = new MainWindowViewModel();

            _playGameCommand = new(PlayGame);
            _quitGameCommand = new(QuitGame);

        }

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

        #endregion Commands
    }
}
