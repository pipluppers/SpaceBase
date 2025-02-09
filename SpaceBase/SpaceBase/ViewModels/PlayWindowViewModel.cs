using System.Windows.Input;

namespace SpaceBase
{
    public class PlayWindowViewModel : ViewModelBase
    {
        public PlayWindowViewModel()
        {
            _playGameCommand = new(PlayGame);
            _quitGameCommand = new(QuitGame);
        }

        #region Commands

        private readonly RelayCommand<PlayWindow> _playGameCommand;
        public ICommand PlayGameCommand { get => _playGameCommand; }

        /// <summary>
        /// Launches the Game UI and closes the PlayWindow.
        /// </summary>
        private void PlayGame(PlayWindow playWindow)
        {
            //PlayWindow playWindow = parameter as PlayWindow;
            Debug.Assert(playWindow != null);

            var mainWindow = new MainWindow();
            mainWindow.Show();

            playWindow.Close();
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
