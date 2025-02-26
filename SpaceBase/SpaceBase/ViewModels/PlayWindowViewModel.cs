using System.Windows.Input;

namespace SpaceBase
{
    public class PlayWindowViewModel : ViewModelBase
    {
        private readonly MainWindowViewModel _mainWindowViewModel;

        public PlayWindowViewModel()
        {
            _playGameCommand = new(PlayGame);
            _quitGameCommand = new(QuitGame);
            _mainWindowViewModel = new MainWindowViewModel();
        }

        #region Commands

        private readonly RelayCommand<PlayWindow> _playGameCommand;
        public ICommand PlayGameCommand { get => _playGameCommand; }

        /// <summary>
        /// Launches the Game UI and closes the PlayWindow.
        /// </summary>
        private async void PlayGame(PlayWindow playWindow)
        {
            Debug.Assert(playWindow != null);

            playWindow.Close();
            _mainWindowViewModel.Show();
            await _mainWindowViewModel.StartGame();
        }

        private readonly RelayCommand _quitGameCommand;
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
