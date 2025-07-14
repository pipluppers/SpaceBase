namespace SpaceBase.Commands
{
    internal class ClaimCardsCommand : ICardCommand
    {
        private int _levelID;
        private int _numCards;

        internal ClaimCardsCommand(int levelID, int numCards)
        {
            _levelID = levelID;
            _numCards = numCards;
        }

        /// <summary>
        /// Executes the action according to the given player.
        /// </summary>
        /// <param name="player">The player to receive the effects of the action.</param>
        public void Execute(Player player)
        {
            // TODO Trigger an event for the player to claim the cards
            throw new NotImplementedException();
        }
    }
}
