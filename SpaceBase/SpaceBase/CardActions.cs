namespace SpaceBase
{
    public enum ActionType
    {
        AddCredits = 1,
        AddIncome = 2,
        AddVictoryPoints = 3,
        AddCreditsIncome = 4,
        AddCreditsVictoryPoints = 5,
        ClaimCardsAtLevel = 6
    }

    public static class CardActions
    {
        public static Action<Player, int, int> GetAction(ActionType actionType)
        {
            return actionType switch
            {
                ActionType.AddCredits => CardActions.AddCredits,
                ActionType.AddIncome => CardActions.AddIncome,
                ActionType.AddVictoryPoints => CardActions.AddVictoryPoints,
                ActionType.AddCreditsIncome => CardActions.AddCreditsIncome,
                ActionType.AddCreditsVictoryPoints => CardActions.AddCreditsVictoryPoints,
                ActionType.ClaimCardsAtLevel => CardActions.ClaimCardsAtLevel,
                _ => throw new ArgumentOutOfRangeException(nameof(actionType), $"ActionType {actionType} is not valid"),
            };
        }

        /// <summary>
        /// Adds the specified amount of credits to the player.
        /// </summary>
        /// <param name="player">The player to get the credits.</param>
        /// <param name="amount">The amount of credits to add.</param>
        internal static void AddCredits(Player player, int amount, int _)
        {
            player.AddCredits(amount);
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
        /// Adds the specified amount of credits and income to the player.
        /// </summary>
        /// <param name="player">The player to get the credits and income.</param>
        /// <param name="amount">The amount of credits to add.</param>
        /// <param name="secondaryAmount">The amount of income to add.</param>
        internal static void AddCreditsIncome(Player player, int amount, int secondaryAmount)
        {
            player.AddCredits(amount);
            player.AddIncome(secondaryAmount);
        }

        /// <summary>
        /// Adds the specified amount of credits and victory points to the player.
        /// </summary>
        /// <param name="player">The player to get the credits and victory points.</param>
        /// <param name="amount">The amount of credits to add.</param>
        /// <param name="secondaryAmount">The amount of victory points to add.</param>
        internal static void AddCreditsVictoryPoints(Player player, int amount, int secondaryAmount)
        {
            player.AddCredits(amount);
            player.AddVictoryPoints(secondaryAmount);
        }

        /// <summary>
        /// Claims the specified amount of cards at the specified level.
        /// </summary>
        /// <param name="player">The player to claim the cards.</param>
        /// <param name="numCardsToClaim">The number of cards to claim.</param>
        /// <param name="level">The level at which to claim the cards.</param>
        internal static void ClaimCardsAtLevel(Player player, int numCardsToClaim, int level)
        {
            // TODO
            // Wait for user input
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="player">The player whose dice roll is affected.</param>
        /// <param name="amount">The amount to add to the sum of the dice roll.</param>
        internal static void AddToSum(Player player, int amount)
        {
            // TODO
            // Wait for user input
        }

    }
}
