namespace SpaceBase.Commands
{
    public interface ICardCommand
    {
        /// <summary>
        /// Executes the action according to the given player.
        /// </summary>
        /// <param name="player">The player to receive the effects of the action.</param>
        public void Execute(Player player);
    }
}
