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

    /// <summary>
    /// Represents a type of action for a card effect.
    /// </summary>
    public enum ActionType
    {
        AddCredits = 1,
        AddIncome = 2,
        AddVictoryPoints = 3,
        AddCreditsIncome = 4,
        AddCreditsVictoryPoints = 5,
        DoubleArrow = 6,
        AddCreditsArrow = 7, // SecondaryAmount of 0 means Left and 1 means Right
        AddVictoryPointsArrow = 8, // SecondaryAmount of 0 means Left and 1 means Right
        ClaimCardsAtLevel = 9,
        AddChargeCube = 10
    }

    /// <summary>
    /// Represents a type of action for a card's charge effect.
    /// </summary>
    public enum ChargeActionType
    {
        AddToSum1 = 1,
        AddToSum2 = 2,
        BuyCardAndAdd3Credits = 3,
        BuyCardAndAdd4Credits = 4,
        BuyCardAndAdd4VictoryPoints = 5,
        BuyCardAndPlaceInAnySector7To12 = 6,
        RerollDie = 7,
        Set1DieBeforeRoll = 8,
        Set1DieBeforeRollAndAdd1Credit = 9,
        Set1DieBeforeRollAndAdd4Credits = 10,
        Swap112Sectors = 11,
        Swap211Sectors = 12,
        Swap49Sectors = 13,
        Swap410Sectors = 14,
        Swap58Sectors = 15,
        Swap67Sectors = 16,
        Add3Credits = 17,
        Add20Credits = 18,
        Claim2Level1Cards = 19,
        Claim3Level1Cards = 20,
        Claim1Level2Card = 21,
        Claim2Level2Cards = 22,
        Claim1Level2CardAnd1Level1Card = 23,
        Claim1Level3Card = 24,
        Claim1Level3CardAnd1Level1Card = 25,
        OppLose3VictoryPointsAndBuy1Card = 26,
        OppLose4VictoryPointsAndBuy2Cards = 27,
        Place1ChargeAnywhere = 28,
        Place1ChargeAnywhereAndMove1Charge = 29,
        DoubleSectorRewards = 30,
        ExchangeWithAnyCard = 31,
        InstantVictory = 32
    }

    #endregion Enumerations

    internal static class Utilities
    {
        /// <summary>
        /// A card representing an empty space for the UI.
        /// </summary>
        public static Card NullLevelCard => (Card)CardFactory.CreateStandardCard(Constants.MinCardLevel, 1, Constants.NullCardCost, ActionType.AddCredits, 1, null, ActionType.AddCredits, 1, null); // TODO remove casting

        public static IColonyCard NullColonyCard => CardFactory.CreateColonyCard(1, Constants.NullCardCost, 0);
    }

}
