namespace SpaceBase
{
    public class DiceRollEventArgs(int dice1, int dice2, int currentPlayerID) : EventArgs
    {
        public int Dice1 { get; } = dice1;
        public int Dice2 { get; } = dice2;
        public int CurrentPlayerID { get; } = currentPlayerID;
    }
    public delegate void DiceRollEventHandler<DiceRollEventArgs>(object sender, DiceRollEventArgs e);
    public delegate void HelpDiceRollEventHandler<DiceRollEventArgs>(object sender, DiceRollEventArgs e);

    public class AddCardToSectorEventArgs(Card addedCard)
    {
        public Card AddedCard { get; } = addedCard;
    }
    public delegate void AddCardToSectorEvent<AddCardToSectorEventArgs>(object sender, AddCardToSectorEventArgs e);

    public class PlayerReachedVictoryThresholdEventArgs(int playerID)
    {
        public int PlayerID { get; } = playerID;
    }
    public delegate void PlayerReachedVictoryThresholdEvent<PlayerReachedVictoryThresholdEventArgs>(object sender, PlayerReachedVictoryThresholdEventArgs e);

    public class TurnOverEventArgs() { }
    public delegate void TurnOverEvent<TurnOverEventArgs>(object sender, TurnOverEventArgs e);

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

    internal class Events
    {
    }
}
