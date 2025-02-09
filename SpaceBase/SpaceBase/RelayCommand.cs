using System.Windows.Input;

namespace SpaceBase
{
    internal class RelayCommand(Action execute, Func<bool> canExecute) : ICommand
    {
        private readonly Action _execute = execute ?? throw new ArgumentNullException(nameof(execute));

        public RelayCommand(Action execute) : this(execute, () => true) { }

        public event EventHandler? CanExecuteChanged;

        public bool CanExecute(object? parameter)
        {
            return canExecute == null || canExecute();
        }

        public void Execute(object? parameter)
        {
            _execute();
        }

        public void RaiseCanExecuteChanged()
        {
            CanExecuteChanged?.Invoke(this, EventArgs.Empty);
        }
    }

    internal class RelayCommand<T>(Action<T> execute, Func<T, bool> canExecute) : ICommand where T : notnull
    {
        private readonly Action<T> _execute = execute ?? throw new ArgumentNullException(nameof(execute));

        public RelayCommand(Action<T> execute) : this(execute, (arg) => true) { }

        public event EventHandler? CanExecuteChanged;

#pragma warning disable CS8600
#pragma warning disable CS8604
        public bool CanExecute(object? parameter)
        {
            return canExecute == null || canExecute((T)parameter);
        }

        public void Execute(object? parameter)
        {
            _execute((T)parameter);
        }
#pragma warning restore CS8600
#pragma warning restore CS8604

        public void RaiseCanExecuteChanged()
        {
            CanExecuteChanged?.Invoke(this, EventArgs.Empty);
        }
    }
}
