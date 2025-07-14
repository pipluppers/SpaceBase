
namespace SpaceBase.Commands
{
    internal class AddCreditsIncomeCommand : ICardCommand
    {
        private readonly int _credits;
        private readonly int _income;

        internal AddCreditsIncomeCommand(int credits, int income)
        {
            _credits = credits;
            _income = income;
        }

        /// <summary>
        /// Executes the action according to the given player.
        /// </summary>
        /// <param name="player">The player to receive the effects of the action.</param>
        public void Execute(Player player)
        {
            PlayerResourcesService.AddCredits(player, _credits);
            PlayerResourcesService.AddIncome(player, _income);
        }
    }
}
