using System.Windows.Input;

namespace SpaceBase
{
    internal class RelayCommand : ICommand
    {
        private readonly Action _action;
        private readonly Func<bool> _canExecute;

        public RelayCommand(Action action) : this(action, () => true) { }

        public RelayCommand(Action action, Func<bool> canExecute)
        {
            ArgumentNullException.ThrowIfNull(action);

            _action = action;
            _canExecute = canExecute;
        }

        public event EventHandler? CanExecuteChanged;

        public bool CanExecute(object? parameter)
        {
            if (_canExecute == null)
                return true;

            return _canExecute();
        }

        public void Execute(object? parameter)
        {
            _action.Invoke();
        }
    }
}
