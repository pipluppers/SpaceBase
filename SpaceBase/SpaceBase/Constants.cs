﻿namespace SpaceBase
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
        internal const int NullCardCost = 900;

        #region Database constants

        internal const string ServerKey = "SERVER";
        internal const string DatabaseKey = "DATABASE";
        internal const string ServerEnvironmentVariable = "SpaceBaseServer";
        internal const string DatabaseEnvironmentVariable = "SpaceBaseDatabase";
        internal const string CardsTableEnvironmentVariable = "SpaceBaseCardsTable";

        #endregion Database constants

        #region UI constants

        public const string GameTitle = "Space Base";
        public const string BackgroundSplashScreenPath = @"C:\Users\nguye\Pictures\Space_Base.png";
        public const string IconPath = @"C:\Users\nguye\Pictures\Star.png";

        public const double CardHeight = 160;
        public const double CardWidth = 60;
        public const double CostSectorBubbleWidthHeight = 20;

        public const double Left1 = 21;
        public const double Left2 = 103;
        public const double Left3 = 185;
        public const double Left4 = 267;
        public const double Left5 = 349;
        public const double Left6 = 431;
        public const double Left7 = 513;
        public const double Left8 = 595;
        public const double Left9 = 677;
        public const double Left10 = 759;
        public const double Left11 = 841;
        public const double Left12 = 923;

        public const double CardPoolTop1 = 20;
        public const double CardPoolTop2 = 220;
        public const double CardPoolTop3 = 420;
        public const double CardPoolDeckLeft = 71;
        public const double CardPoolLeft = 301;
        public const double CardPoolLeft1 = 331;
        public const double CardPoolLeft2 = 421;
        public const double CardPoolLeft3 = 511;
        public const double CardPoolLeft4 = 601;
        public const double CardPoolLeft5 = 691;
        public const double CardPoolLeft6 = 781;

        #endregion UI constants
    }
}
