namespace SpaceBase.Commands
{
    internal class AddVictoryPointsArrowCommand : ICardCommand
    {
        private readonly int _victoryPoints;

        internal AddVictoryPointsArrowCommand(int victoryPoints)
        {
            _victoryPoints = victoryPoints;
        }

        /// <summary>
        /// Executes the action according to the given player.
        /// </summary>
        /// <param name="player">The player to receive the effects of the action.</param>
        public void Execute(Player player)
        {
            PlayerResourcesService.AddVictoryPoints(player, _victoryPoints);
            throw new NotImplementedException();
        }
    }
}
