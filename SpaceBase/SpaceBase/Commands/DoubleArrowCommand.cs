
namespace SpaceBase.Commands
{
    internal class DoubleArrowCommand : ICardCommand
    {
        public DoubleArrowCommand()
        {

        }

        /// <summary>
        /// Executes the action according to the given player.
        /// </summary>
        /// <param name="player">The player to receive the effects of the action.</param>
        public void Execute(Player player)
        {
            // TODO
            // Trigger event if Player is human player
            // Pick either at random or some heuristic if Player is computer player
            throw new NotImplementedException();
        }
    }
}
