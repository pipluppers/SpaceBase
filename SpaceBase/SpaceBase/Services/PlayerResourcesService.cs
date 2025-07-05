namespace SpaceBase.Services
{
    public static class PlayerResourcesService
    {
        /// <summary>
        /// Adds the specified amount of credits to the player's credits pool.
        /// </summary>
        /// <param name="player">The player to get the credits.</param>
        /// <param name="credits">The amount of credits to add.</param>
        /// <exception cref="NotSupportedException"><paramref name="credits"/> cannot be negative.</exception>
        public static void AddCredits(Player player, int credits)
        {
            if (credits < 0) throw new NotSupportedException("This API cannot be used to subtract or reset credits.");

            player.Credits += credits;
        }

        /// <summary>
        /// Adds the specified amount of income to the player's income pool.
        /// </summary>
        /// <param name="player">The player to get the income.</param>
        /// <param name="income">The amount of income to add.</param>
        /// <exception cref="NotSupportedException"><paramref name="income"/> cannot be negative.</exception>
        public static void AddIncome(Player player, int income)
        {
            if (income < 0) throw new NotSupportedException("Income cannot be removed");

            player.Income += income;
        }

        /// <summary>
        /// Adds the specified amount of victory points to the player's income pool.
        /// </summary>
        /// <param name="player">The player to get the victory points.</param>
        /// <param name="victoryPoints">The amount of victory points to add.</param>
        public static void AddVictoryPoints(Player player, int victoryPoints)
        {
            // Note: There is a card that removes victory points.

            player.VictoryPoints += victoryPoints;
        }

        /// <summary>
        /// Resets the player's credits to their income if the number of credits is less than the income.
        /// </summary>
        public static void ResetCredits(Player player)
        {
            if (player.Credits < player.Income)
                player.Credits = player.Income;
        }
    }
}
