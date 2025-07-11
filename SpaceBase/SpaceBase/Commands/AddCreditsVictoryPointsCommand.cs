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

        public void Execute(Player player)
        {
            PlayerResourcesService.AddCredits(player, _credits);
            PlayerResourcesService.AddVictoryPoints(player, _victoryPoints);
        }
    }
}
