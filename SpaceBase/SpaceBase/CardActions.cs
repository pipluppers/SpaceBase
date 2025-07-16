namespace SpaceBase
{
    public static class CardActions
    {
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
        public static void AddCredits(Player player, Card _, int amount, int __)
        {
            PlayerResourcesService.AddCredits(player, amount);
        }

        /// <summary>
        /// Adds the specified amount of income to the player.
        /// </summary>
        /// <param name="player">The player to get the income.</param>
        /// <param name="amount">The amount of income to add.</param>
        public static void AddIncome(Player player, Card _, int amount, int __)
        {
            PlayerResourcesService.AddIncome(player, amount);
        }

        /// <summary>
        /// Adds the specified amount of victory points to the player.
        /// </summary>
        /// <param name="player">The player to get the victory points.</param>
        /// <param name="amount">The amount of victory points to add.</param>
        public static void AddVictoryPoints(Player player, Card _, int amount, int __)
        {
            PlayerResourcesService.AddVictoryPoints(player, amount);
        }

        /// <summary>
        /// Adds the specified amount of credits and income to the player.
        /// </summary>
        /// <param name="player">The player to get the credits and income.</param>
        /// <param name="amount">The amount of credits to add.</param>
        /// <param name="secondaryAmount">The amount of income to add.</param>
        public static void AddCreditsIncome(Player player, Card _, int amount, int secondaryAmount)
        {
            PlayerResourcesService.AddCredits(player, amount);
            PlayerResourcesService.AddIncome(player, secondaryAmount);
        }

        /// <summary>
        /// Adds the specified amount of credits and victory points to the player.
        /// </summary>
        /// <param name="player">The player to get the credits and victory points.</param>
        /// <param name="amount">The amount of credits to add.</param>
        /// <param name="secondaryAmount">The amount of victory points to add.</param>
        public static void AddCreditsVictoryPoints(Player player, Card _, int amount, int secondaryAmount)
        {
            PlayerResourcesService.AddCredits(player, amount);
            PlayerResourcesService.AddVictoryPoints(player, amount);
        }

        public static void AddRewardFromLeftOrRightSector(Player player, Card card, int amount, int secondaryAmount)
        {
            // TODO
        }

        public static void AddCreditsRewardFromAdjacentSector(Player player, Card card, int amount, int secondaryAmount)
        {
            // TODO
            // Amount of 0 means left sector, 1 means right sector
        }

        public static void AddVictoryPointsRewardFromAdjacentSector(Player player, Card card, int amount, int secondaryAmount)
        {
            // TODO
            // Amount of 0 means left sector, 1 means right sector
        }

        /// <summary>
        /// Claims the specified amount of cards at the specified level.
        /// </summary>
        /// <param name="player">The player to claim the cards.</param>
        /// <param name="numCardsToClaim">The number of cards to claim.</param>
        /// <param name="level">The level at which to claim the cards.</param>
        public static void ClaimCardsAtLevel(Player player, Card _, int numCardsToClaim, int level)
        {
            // TODO
            // Wait for user input
        }

        /// <summary>
        /// Adds a charge cube to the input card.
        /// </summary>
        /// <param name="card">The card to gain the charge cube.</param>
        public static void AddChargeCube(Player _, Card card, int __, int ___)
        {
            if (card is ChargeCard chargeCard)
                chargeCard.AddChargeCube();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="player">The player whose dice roll is affected.</param>
        /// <param name="amount">The amount to add to the sum of the dice roll.</param>
        public static async void AddToSum(Player player, int amount, int _)
        {
            // TODO
            // Wait for user input

            await Task.Run(() =>
            {

            });
        }

    }
}
