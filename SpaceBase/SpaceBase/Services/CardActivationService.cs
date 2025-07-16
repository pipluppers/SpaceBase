namespace SpaceBase.Services
{
    internal static class CardActivationService
    {
        /// <summary>
        /// Activates the stationed effect of the provided card and updates the given player's resources.
        /// </summary>
        /// <param name="card">The card whose effect to activate.</param>
        /// <param name="player">The player to receive the resources.</param>
        internal static void ActivateStationedEffect(IStandardCard standardCard, Player player)
        {
            standardCard.StationedCommand.Execute(player);
        }

        /// <summary>
        /// Activates the deployed effect of the provided card and updates the given player's resources.
        /// </summary>
        /// <param name="card">The card whose deployed effect to activate.</param>
        /// <param name="player">The player to receive the resources.</param>
        internal static void ActivateDeployedEffect(IStandardCard standardCard, Player player)
        {
            standardCard.DeployedCommand.Execute(player);
        }

        /// <summary>
        /// Activates the charge effect of the provided card.
        /// </summary>
        /// <param name="chargeCard">The charge card.</param>
        /// <param name="player">The player to receive the effects.</param>
        internal static void ActivateChargeEffect(IChargeCard chargeCard, Player player)
        {
            chargeCard.ChargeCommand.Execute(player);
        }
    }
}
