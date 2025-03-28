namespace SpaceBase
{
    public class DiceRollEventArgs(int dice1, int dice2, int activePlayerID) : EventArgs
    {
        public int Dice1 { get; } = dice1;
        public int Dice2 { get; } = dice2;
        public int ActivePlayerID { get; } = activePlayerID;
    }
    public delegate void DiceRollEventHandler<DiceRollEventArgs>(object sender, DiceRollEventArgs e);
    public delegate void HelpDiceRollEventHandler<DiceRollEventArgs>(object sender, DiceRollEventArgs e);
    public delegate void RemoveHelpDiceRollEffectsEventHandler<DiceRollEventArgs>(object sender, DiceRollEventArgs e);

    public class AddCardToSectorEventArgs(ICard addedCard)
    {
        public ICard AddedCard { get; } = addedCard;
    }
    public delegate void AddCardToSectorEvent<AddCardToSectorEventArgs>(object sender, AddCardToSectorEventArgs e);

    public delegate void TurnOverEvent<EventArgs>(object sender, EventArgs e);

    public class RoundOverEventArgs(int endingRoundNumber) : EventArgs
    {
        public int EndingRoundNumber { get; } = endingRoundNumber;
    }
    public delegate void RoundOverEventHandler<RoundOverEventArgs>(object sender, RoundOverEventArgs e);

    public class GameOverEventArgs(List<int> winnerPlayerIDs)
    {
        public List<int> WinnerPlayerIDs { get; } = winnerPlayerIDs;
    }
    public delegate void GameOverEventHandler<GameOverEventArgs>(object sender, GameOverEventArgs e);
}
