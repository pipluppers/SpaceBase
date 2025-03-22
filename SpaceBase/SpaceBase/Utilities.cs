using System.Runtime.CompilerServices;

namespace SpaceBase
{
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

    public interface ISerializable
    {
        string Serialize();
        object? Deserialize(string str);
    }

    #region Enumerations

    /// <summary>
    /// Represents what kind of card this is.
    /// </summary>
    public enum CardType
    {
        /// <summary>
        /// A standard ship card belonging to sectors 1-12 and having a level.
        /// </summary>
        Standard = 0,

        /// <summary>
        /// Similar to a standard card but possessing a charge cube effect.
        /// </summary>
        Charge = 1,

        /// <summary>
        /// A card that has no activation effect.
        /// </summary>
        Colony = 2
    }

    /// <summary>
    /// Represents when the charge cube effect can be activated.
    /// </summary>
    public enum ChargeCardType
    {
        /// <summary>
        /// Can only be activated on the player's turn. In the physical game, the color is blue.
        /// </summary>
        Turn = 0,

        /// <summary>
        /// Can only be activated on an opponent's turn. In the physical game, the color is red.
        /// </summary>
        OpponentTurn = 1,

        /// <summary>
        /// Can only be activated on any player's turn. In the physical game, the color is green.
        /// </summary>
        Anytime = 2
    }

    #endregion Enumerations

    internal static class Utilities
    {
        /// <summary>
        /// A card representing an empty space for the UI.
        /// </summary>
        public static Card NullLevelCard => new(Constants.MinCardLevel, 1, Constants.NullCardCost, ActionType.AddCredits, 1, null, ActionType.AddCredits, 1, null);
    }

}
