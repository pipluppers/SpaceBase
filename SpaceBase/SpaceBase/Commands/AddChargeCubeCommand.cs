namespace SpaceBase.Commands
{
    internal class AddChargeCubeCommand : ICardCommand
    {
        private readonly ChargeCard _chargeCard;

        internal AddChargeCubeCommand(ChargeCard chargeCard)
        {
            _chargeCard = chargeCard;
        }

        public void Execute(Player player)
        {
            _chargeCard.AddChargeCube();
        }
    }
}
