namespace SpaceBase.Commands
{
    internal class AddChargeCubeCommand : ICardCommand
    {
        private readonly ChargeCard _chargeCard;

        internal AddChargeCubeCommand(ChargeCard chargeCard)
        {
            _chargeCard = chargeCard;
        }

        /// <summary>
        /// Executes the action according to the given player.
        /// </summary>
        /// <param name="player">The player to receive the effects of the action.</param>
        public void Execute(Player player)
        {
            _chargeCard.AddChargeCube();
        }
    }
}
