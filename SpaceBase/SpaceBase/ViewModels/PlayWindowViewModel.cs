using System.Windows.Input;

namespace SpaceBase
{
    public class PlayWindowViewModel : ViewModelBase
    {
        public PlayWindowViewModel()
        {
            _quitGameCommand = new(QuitGame);
        }

        #region Commands

        private readonly RelayCommand _quitGameCommand;
        public ICommand QuitGameCommand { get => _quitGameCommand; }
        private void QuitGame()
        {
            Application.Current.Shutdown();
        }

        #endregion Commands
    }
}
