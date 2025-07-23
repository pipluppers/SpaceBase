namespace SpaceBase.Services
{
    internal struct DiceRollResult(int dice1, int dice2)
    {
        internal readonly int Dice1 { get => dice1; }
        internal readonly int Dice2 { get => dice2; }
    }

    internal class DiceRollService
    {
        private readonly Random _randomNumberGenerator;

        internal DiceRollService()
        {
            _randomNumberGenerator = new Random(1); // TODO Remove the 1 before playing for real
        }

        internal DiceRollResult RollDice()
        {
            return new DiceRollResult((_randomNumberGenerator.Next() % 6) + 1, (_randomNumberGenerator.Next() % 6) + 1);
        }
    }
}
