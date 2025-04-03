namespace SpaceBase
{
    internal class Constants
    {
        internal const int MinNumPlayers = 2;
        internal const int MaxNumPlayers = 5;
        internal const int MaxNumRounds = 50;
        internal const int VictoryThreshold = 40;
        internal const int MinSectorID = 1;
        internal const int MaxSectorID = 12;
        internal const int MinCardLevel = 0;
        internal const int MaxCardLevel = 3;
        internal const int ColonyCardLevel = 4;
        internal const int NullCardCost = 900;

        #region Database constants

        internal const string ServerKey = "SERVER";
        internal const string DatabaseKey = "DATABASE";
        internal const string ServerEnvironmentVariable = "SpaceBaseServer";
        internal const string DatabaseEnvironmentVariable = "SpaceBaseDatabase";
        internal const string CardsTableEnvironmentVariable = "SpaceBaseCardsTable";

        #endregion Database constants
    }
}
