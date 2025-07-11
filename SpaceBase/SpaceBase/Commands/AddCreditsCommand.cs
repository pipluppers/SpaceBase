namespace SpaceBase.Commands
{
    internal class AddCreditsCommand : ICardCommand
    {
        private readonly int _credits;

        internal AddCreditsCommand(int credits)
        {
            _credits = credits;
        }

        public void Execute(Player player)
        {
            PlayerResourcesService.AddCredits(player, _credits);
        }
    }
}
