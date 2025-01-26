﻿namespace SpaceBase
{
    public class DiceRollEventArgs(int dice1, int dice2) : EventArgs
    {
        public int Dice1 { get; } = dice1;
        public int Dice2 { get; } = dice2;
    }
    public delegate void DiceRollEventHandler<DiceRollEventArgs>(object sender, DiceRollEventArgs e);

    public class PlayerReachedVictoryThresholdEventArgs(int playerID)
    {
        public int PlayerID { get; } = playerID;
    }
    public delegate void PlayerReachedVictoryThresholdEvent<PlayerReachedVictoryThresholdEventArgs>(object sender, PlayerReachedVictoryThresholdEventArgs e);

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
