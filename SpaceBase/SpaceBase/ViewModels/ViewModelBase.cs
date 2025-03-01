using System.Runtime.CompilerServices;

namespace SpaceBase
{
    public abstract class ViewModelBase : PropertyChangedBase
    {
    }

    public abstract class PropertyChangedBase : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        protected bool SetProperty<T>(ref T o, T value, [CallerMemberName] string propertyName = "")
        {
            if (Equals(o, value))
                return false;

            o = value;
            OnPropertyChanged(propertyName);

            return true;
        }

        protected void NotifyPropertyChanged(string propertyName)
        {
            OnPropertyChanged(propertyName);
        }
    }
}
