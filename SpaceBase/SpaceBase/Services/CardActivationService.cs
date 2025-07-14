namespace SpaceBase.Services
{
    internal static class CardActivationService
    {
        /// <summary>
        /// Activates the stationed effect of the provided card and updates the given player's resources.
        /// </summary>
        /// <param name="card">The card whose effect to activate.</param>
        /// <param name="player">The player to receive the resources.</param>
        internal static void ActivateStationedEffect(ICard inputCard, Player player)
        {
            // Colony cards do not have any stationed effects.
            if (inputCard is ColonyCard)
                return;

            if (inputCard is not Card card)
                return;

            card.StationedCommand.Execute(player);
        }

        /// <summary>
        /// Activates the deployed effect of the provided card and updates the given player's resources.
        /// </summary>
        /// <param name="card">The card whose deployed effect to activate.</param>
        /// <param name="player">The player to receive the resources.</param>
        internal static void ActivateDeployedEffect(IStandardCard standardCard, Player player)
        {
            if (standardCard is not Card card)
                return;

            card.DeployedCommand.Execute(player);
        }
    }
}
