using System.Windows.Input;

namespace SpaceBase
{
    public class PlayWindowViewModel : ViewModelBase
    {
        private string _name;
        private RoutedCommand _quitCommand;

        public PlayWindowViewModel()
        {
            _quitCommand = new("Quit", GetType());
            _name = "DA";
            Name = "Hallo";

        }

        public string Name { get => _name; set => SetProperty(ref _name, value); }

        public ICommand QuitCommand { get => _quitCommand; }
    }
}
