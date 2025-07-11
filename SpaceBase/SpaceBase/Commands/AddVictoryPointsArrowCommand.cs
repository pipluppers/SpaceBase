namespace SpaceBase.Commands
{
    internal class AddVictoryPointsArrowCommand : ICardCommand
    {
        private readonly int _victoryPoints;

        internal AddVictoryPointsArrowCommand(int victoryPoints)
        {
            _victoryPoints = victoryPoints;
        }
        public void Execute(Player player)
        {
            PlayerResourcesService.AddVictoryPoints(player, _victoryPoints);
            throw new NotImplementedException();
        }
    }
}
