namespace SpaceBase.Commands
{
    internal class AddIncomeCommand : ICardCommand
    {
        private readonly int _income;

        internal AddIncomeCommand(int income)
        {
            _income = income;
        }

        public void Execute(Player player)
        {
            PlayerResourcesService.AddIncome(player, _income);
        }
    }
}
