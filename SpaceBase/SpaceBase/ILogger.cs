namespace SpaceBase
{
    public interface ILogger
    {
        void LogMessage(string message);
        void LogRoundMessage(int round);
    }
}
