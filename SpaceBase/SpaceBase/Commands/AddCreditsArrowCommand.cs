namespace SpaceBase.Commands
{
    internal class AddCreditsArrowCommand : ICardCommand
    {
        private readonly int _credits;

        internal AddCreditsArrowCommand(int credits)
        {
            _credits = credits;
        }

        public void Execute(Player player)
        {
            PlayerResourcesService.AddCredits(player, _credits);
            throw new NotImplementedException();
        }
    }
}
