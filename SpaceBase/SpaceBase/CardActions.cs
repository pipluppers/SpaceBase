namespace SpaceBase
{
    public enum ActionType
    {
        AddCredits = 1,
        AddIncome = 2,
        AddVictoryPoints = 3,
        AddCreditsIncome = 4,
        AddCreditsVictoryPoints = 5,
        ClaimCardsAtLevel = 6,
        AddChargeCube = 20
    }

    public enum ChargeActionType
    {
        AddToSum1 = 1,
        AddToSum2 = 2,
    }

    public static class CardActions
    {
        public static Action<Player, Card, int, int> GetAction(ActionType actionType)
        {
            return actionType switch
            {
                ActionType.AddCredits => CardActions.AddCredits,
                ActionType.AddIncome => CardActions.AddIncome,
                ActionType.AddVictoryPoints => CardActions.AddVictoryPoints,
                ActionType.AddCreditsIncome => CardActions.AddCreditsIncome,
                ActionType.AddCreditsVictoryPoints => CardActions.AddCreditsVictoryPoints,
                ActionType.ClaimCardsAtLevel => CardActions.ClaimCardsAtLevel,
                ActionType.AddChargeCube => CardActions.AddChargeCube,
                _ => throw new ArgumentOutOfRangeException(nameof(actionType), $"ActionType {actionType} is not valid"),
            };
        }

        public static Action<Player, int, int> GetChargeAction(ChargeActionType chargeActionType)
        {
            return chargeActionType switch
            {
                ChargeActionType.AddToSum1 => CardActions.AddToSum,
                ChargeActionType.AddToSum2 => CardActions.AddToSum,
                _ => throw new ArgumentOutOfRangeException(nameof(chargeActionType), $"ChargeActionType {chargeActionType} is not valid")
            };
        }

        /// <summary>
        /// Adds the specified amount of credits to the player.
        /// </summary>
        /// <param name="player">The player to get the credits.</param>
        /// <param name="amount">The amount of credits to add.</param>
        internal static void AddCredits(Player player, Card _, int amount, int __)
        {
            player.AddCredits(amount);
        }

        /// <summary>
        /// Adds the specified amount of income to the player.
        /// </summary>
        /// <param name="player">The player to get the income.</param>
        /// <param name="amount">The amount of income to add.</param>
        internal static void AddIncome(Player player, Card _, int amount, int __)
        {
            player.AddIncome(amount);
        }

        /// <summary>
        /// Adds the specified amount of victory points to the player.
        /// </summary>
        /// <param name="player">The player to get the victory points.</param>
        /// <param name="amount">The amount of victory points to add.</param>
        internal static void AddVictoryPoints(Player player, Card _, int amount, int __)
        {
            player.AddVictoryPoints(amount);
        }

        /// <summary>
        /// Adds the specified amount of credits and income to the player.
        /// </summary>
        /// <param name="player">The player to get the credits and income.</param>
        /// <param name="amount">The amount of credits to add.</param>
        /// <param name="secondaryAmount">The amount of income to add.</param>
        internal static void AddCreditsIncome(Player player, Card _, int amount, int secondaryAmount)
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
        internal static void AddCreditsVictoryPoints(Player player, Card _, int amount, int secondaryAmount)
        {
            player.AddCredits(amount);
            player.AddVictoryPoints(secondaryAmount);
        }

        /// <summary>
        /// Adds a charge cube to the input card.
        /// </summary>
        /// <param name="card">The card to gain the charge cube.</param>
        internal static void AddChargeCube(Player _, Card card, int __, int ___)
        {
            if (card is ChargeCard chargeCard)
                chargeCard.AddChargeCube();
        }

        /// <summary>
        /// Claims the specified amount of cards at the specified level.
        /// </summary>
        /// <param name="player">The player to claim the cards.</param>
        /// <param name="numCardsToClaim">The number of cards to claim.</param>
        /// <param name="level">The level at which to claim the cards.</param>
        internal static void ClaimCardsAtLevel(Player player, Card _, int numCardsToClaim, int level)
        {
            // TODO
            // Wait for user input
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="player">The player whose dice roll is affected.</param>
        /// <param name="amount">The amount to add to the sum of the dice roll.</param>
        internal static void AddToSum(Player player, int amount, int _)
        {
            // TODO
            // Wait for user input
        }

    }
}
