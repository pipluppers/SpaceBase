namespace SpaceBaseApplication
{
    internal class RelayCommand(Action execute, Func<bool> canExecute) : ICommand
    {
        private readonly Action _execute = execute ?? throw new ArgumentNullException(nameof(execute));

        public RelayCommand(Action execute) : this(execute, () => true) { }

        public event EventHandler? CanExecuteChanged
        {
            add { if (value != null) CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }

        /// <summary>
        /// Gets whether or not the command can be executed.
        /// </summary>
        /// <param name="parameter">Any parameters needed to execute.</param>
        /// <returns>True if the command can be executed. Otherwise, false.</returns>
        public bool CanExecute(object? parameter)
        {
            return canExecute == null || canExecute();
        }

        /// <summary>
        /// Execute the command.
        /// </summary>
        /// <param name="parameter">Any parameters needed to execute.</param>
        public void Execute(object? parameter)
        {
            _execute();
        }

        /// <summary>
        /// Update any listeners to command to refresh whether or not the command can be executed.
        /// </summary>
        public void RaiseCanExecuteChanged() => CommandManager.InvalidateRequerySuggested();
    }

    internal class RelayCommand<T>(Action<T> execute, Func<T, bool> canExecute) : ICommand where T : notnull
    {
        private readonly Action<T> _execute = execute ?? throw new ArgumentNullException(nameof(execute));

        public RelayCommand(Action<T> execute) : this(execute, (arg) => true) { }

        public event EventHandler? CanExecuteChanged
        {
            add { if (value != null) CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }

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

        /// <summary>
        /// Update any listeners to command to refresh whether or not the command can be executed.
        /// </summary>
        public void RaiseCanExecuteChanged() => CommandManager.InvalidateRequerySuggested();
    }

    internal class RelayCommandAsync(Func<Task> execute, Func<bool> canExecute) : ICommand
    {
        private readonly Func<Task> _execute = execute ?? throw new ArgumentNullException(nameof(execute));

        public RelayCommandAsync(Func<Task> execute) : this(execute, () => true) { }

        public event EventHandler? CanExecuteChanged
        {
            add { if (value != null) CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }

        /// <summary>
        /// Gets whether or not the command can be executed.
        /// </summary>
        /// <param name="parameter">Any parameters needed to execute.</param>
        /// <returns>True if the command can be executed. Otherwise, false.</returns>
        public bool CanExecute(object? parameter)
        {
            return canExecute == null || canExecute();
        }

        /// <summary>
        /// Execute the command.
        /// </summary>
        /// <param name="parameter">Any parameters needed to execute.</param>
        public void Execute(object? parameter)
        {
            _execute();
        }

        /// <summary>
        /// Update any listeners to command to refresh whether or not the command can be executed.
        /// </summary>
        public void RaiseCanExecuteChanged() => CommandManager.InvalidateRequerySuggested();
    }
}
