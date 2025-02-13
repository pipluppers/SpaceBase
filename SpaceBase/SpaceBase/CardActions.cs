namespace SpaceBase
{
    public enum ActionType
    {
        AddGold = 1,
        AddIncome = 2,
        AddVictoryPoints = 3,
        AddGoldIncome = 4
    }

    public static class CardActions
    {
        public static Action<Player, int, int> GetAction(ActionType actionType)
        {
            return actionType switch
            {
                ActionType.AddGold => CardActions.AddGold,
                ActionType.AddIncome => CardActions.AddIncome,
                ActionType.AddVictoryPoints => CardActions.AddVictoryPoints,
                ActionType.AddGoldIncome => CardActions.AddGoldIncome,
                _ => throw new ArgumentOutOfRangeException(nameof(actionType), $"ActionType {actionType} is not valid"),
            };
        }

        /// <summary>
        /// Adds the specified amount of gold to the player.
        /// </summary>
        /// <param name="player">The player to get the gold.</param>
        /// <param name="amount">The amount of gold to add.</param>
        internal static void AddGold(Player player, int amount, int _)
        {
            player.AddGold(amount);
        }

        /// <summary>
        /// Adds the specified amount of income to the player.
        /// </summary>
        /// <param name="player">The player to get the income.</param>
        /// <param name="amount">The amount of income to add.</param>
        internal static void AddIncome(Player player, int amount, int _)
        {
            player.AddIncome(amount);
        }

        /// <summary>
        /// Adds the specified amount of victory points to the player.
        /// </summary>
        /// <param name="player">The player to get the victory points.</param>
        /// <param name="amount">The amount of victory points to add.</param>
        internal static void AddVictoryPoints(Player player, int amount, int _)
        {
            player.AddVictoryPoints(amount);
        }

        /// <summary>
        /// Adds the specified amount of gold and income to the player.
        /// </summary>
        /// <param name="player">The player to get the gold and income.</param>
        /// <param name="amount">The amount of gold to add.</param>
        /// <param name="secondaryAmount">The amount of income to add.</param>
        internal static void AddGoldIncome(Player player, int amount, int secondaryAmount)
        {
            player.AddGold(amount);
            player.AddIncome(secondaryAmount);
        }
    }
}
