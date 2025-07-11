
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

        public void Execute(Player player)
        {
            PlayerResourcesService.AddCredits(player, _credits);
            PlayerResourcesService.AddIncome(player, _income);
        }
    }
}
