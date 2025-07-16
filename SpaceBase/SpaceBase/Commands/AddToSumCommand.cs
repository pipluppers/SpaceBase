namespace SpaceBase.Commands
{
    internal class AddToSumCommand : ICardCommand
    {
        private readonly int _maxAmount;

        internal AddToSumCommand(int maxAmount)
        {
            _maxAmount = maxAmount;
        }

        public void Execute(Player player)
        {
            // TODO
            // Raise event and pass the max amount to the user to decide
            throw new NotImplementedException();
        }
    }
}
