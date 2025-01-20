namespace SpaceBase
{
    public class DiceRollEventArgs(int dice1, int dice2) : EventArgs
    {
        public int Dice1 { get; } = dice1;
        public int Dice2 { get; } = dice2;
    }

    public delegate void DiceRollEventHandler<DiceRollEventArgs>(object sender, DiceRollEventArgs e);

    public class GameOverEventArgs(int playerID)
    {
        public int PlayerID { get; } = playerID;
    }

    public delegate void GameOverEventHandler<GameOverEventArgs>(object sender, GameOverEventArgs e);

    internal class Events
    {
    }
}
