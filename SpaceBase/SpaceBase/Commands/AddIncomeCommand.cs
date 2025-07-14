namespace SpaceBase.Commands
{
    internal class AddIncomeCommand : ICardCommand
    {
        private readonly int _income;

        internal AddIncomeCommand(int income)
        {
            _income = income;
        }

        /// <summary>
        /// Executes the action according to the given player.
        /// </summary>
        /// <param name="player">The player to receive the effects of the action.</param>
        public void Execute(Player player)
        {
            PlayerResourcesService.AddIncome(player, _income);
        }
    }
}
