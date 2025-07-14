namespace SpaceBase.Commands
{
    internal class AddCreditsVictoryPointsCommand : ICardCommand
    {
        private readonly int _credits;
        private readonly int _victoryPoints;

        internal AddCreditsVictoryPointsCommand(int credits, int victoryPoints)
        {
            _credits = credits;
            _victoryPoints = victoryPoints;
        }

        /// <summary>
        /// Executes the action according to the given player.
        /// </summary>
        /// <param name="player">The player to receive the effects of the action.</param>
        public void Execute(Player player)
        {
            PlayerResourcesService.AddCredits(player, _credits);
            PlayerResourcesService.AddVictoryPoints(player, _victoryPoints);
        }
    }
}
