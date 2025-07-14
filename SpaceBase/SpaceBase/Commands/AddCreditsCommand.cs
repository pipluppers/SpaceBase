namespace SpaceBase.Commands
{
    internal class AddCreditsCommand : ICardCommand
    {
        private readonly int _credits;

        internal AddCreditsCommand(int credits)
        {
            _credits = credits;
        }

        /// <summary>
        /// Executes the action according to the given player.
        /// </summary>
        /// <param name="player">The player to receive the effects of the action.</param>
        public void Execute(Player player)
        {
            PlayerResourcesService.AddCredits(player, _credits);
        }
    }
}
