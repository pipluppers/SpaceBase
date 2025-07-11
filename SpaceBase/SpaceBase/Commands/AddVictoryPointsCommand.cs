namespace SpaceBase.Commands
{
    internal class AddVictoryPointsCommand : ICardCommand
    {
        private readonly int _victoryPoints;

        internal AddVictoryPointsCommand(int victoryPoints)
        {
            _victoryPoints = victoryPoints;
        }

        public void Execute(Player player)
        {
            PlayerResourcesService.AddVictoryPoints(player, _victoryPoints);
        }
    }
}
