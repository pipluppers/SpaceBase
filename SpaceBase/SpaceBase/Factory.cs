namespace SpaceBase
{
    /// <summary>
    /// Abstract factory to create cards.
    /// </summary>
    public static class CardFactory
    {
        /// <summary>
        /// Factory method to create a standard card.
        /// </summary>
        /// <param name="id">The id of the card.</param>
        /// <param name="level">The level.</param>
        /// <param name="sectorID">The ID of the sector.</param>
        /// <param name="cost">The cost.</param>
        /// <param name="effectType">The type of effect.</param>
        /// <param name="amount">The amount of resources.</param>
        /// <param name="secondaryAmount">The amount of secondary resources.</param>
        /// <param name="deployedEffectType">The type of deployed effect.</param>
        /// <param name="deployedAmount">The amount of resources.</param>
        /// <param name="deployedSecondaryAmount">The amount of secondary resources.</param>
        /// <returns>The card as an IStandardCard.</returns>
        /// <exception cref="ArgumentOutOfRangeException">Either one of multiple of <paramref name="sectorID"/>, <paramref name="level"/>, or <paramref name="cost"/> are invalid.</exception>
        public static IStandardCard CreateStandardCard(int id, int level, int sectorID, int cost, ActionType effectType, int amount, int? secondaryAmount,
            ActionType deployedEffectType, int deployedAmount, int? deployedSecondaryAmount)
        {
            if (sectorID < Constants.MinSectorID || sectorID > Constants.MaxSectorID)
                throw new ArgumentOutOfRangeException(nameof(sectorID), $"The sector must be between {Constants.MinSectorID} and {Constants.MaxSectorID} inclusive.");

            if (level != 0 && level < Constants.MinCardLevel || level > Constants.MaxCardLevel)
                throw new ArgumentOutOfRangeException($"The card level must be between {Constants.MinCardLevel} and {Constants.MaxCardLevel} inclusive.");

            if (level == 1 && (cost < 2 || cost > 5))
                throw new ArgumentOutOfRangeException(nameof(cost), "If the level is 1, then the cost must be between 2 and 5.");
            else if (level == 2 && (cost < 7 || cost > 9))
                throw new ArgumentOutOfRangeException(nameof(cost), "If the level is 2, then the cost must be between 7 and 9.");
            else if (level == 3 && (cost < 12 || cost > 14))
                throw new ArgumentOutOfRangeException(nameof(cost), "If the level is 3, then the cost must be between 12 and 14.");

            return new Card(id, level, sectorID, cost, effectType, amount, secondaryAmount, deployedEffectType, deployedAmount, deployedSecondaryAmount);
        }

        /// <summary>
        /// Factory method to create a colony card.
        /// </summary>
        /// <param name="sectorID">The ID of the sector.</param>
        /// <param name="cost">The cost.</param>
        /// <param name="amount">The amount of resources.</param>
        /// <returns>The colony card as an IColonyCard.</returns>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="sectorID"/> is invalid.</exception>
        public static IColonyCard CreateColonyCard(int id, int sectorID, int cost, int amount)
        {
            if (sectorID < Constants.MinSectorID || sectorID > Constants.MaxSectorID)
                throw new ArgumentOutOfRangeException(nameof(sectorID), $"The sector must be between {Constants.MinSectorID} and {Constants.MaxSectorID} inclusive.");

            return new ColonyCard(id, sectorID, cost, amount);
        }
    }
}
