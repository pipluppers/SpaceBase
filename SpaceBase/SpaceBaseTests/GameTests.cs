using NUnit.Framework;
using SpaceBase.Models;

namespace SpaceBaseTests
{
    [TestFixture]
    internal class GameTests
    {
        [SetUp]
        public void SetUp()
        {

        }

        [TearDown]
        public void TearDown()
        {
        }

        [Test]
        public void CanCreateGame()
        {
            Assert.Multiple(() =>
            {
                var game1 = new Game();
                Assert.That(game1.Players.Count, Is.EqualTo(2));

                int numPlayers = 5;
                var game2 = new Game(numPlayers);
                Assert.That(game2.Players.Count, Is.EqualTo(numPlayers));
            });
        }

        [Test]
        public void CreatingGamesWithInvalidNumberOfPlayersShouldFail()
        {
            Assert.Multiple(() =>
            {
                int numPlayers = 1;
                Assert.Throws<ArgumentException>(() => new Game(numPlayers));

                numPlayers = 6;
                Assert.Throws<ArgumentException>(() => new Game(numPlayers));
            });
        }

        [Test]
        public async Task GameRaisesRoundOverEvent()
        {
            var game = new Game();

            bool round1Over = false;
            bool round5Over = false;

            game.RoundOverEvent += (sender, args) =>
            {
                if (args.EndingRoundNumber == 1)
                    round1Over = true;
                else if (args.EndingRoundNumber == 5)
                    round5Over = true;

                if (round1Over && round5Over)
                    Assert.Pass();
            };

            await game.StartGame();

            Assert.Fail("The round over event did not get raised with round 1 ending.");
        }

        [Test]
        public async Task DiceRollsAreValidEachTime()
        {
            // Just let the game play to 30 rounds

            var game = new Game();

            game.DiceRollEventHandler += (sender, args) =>
            {
                Assert.Multiple(() =>
                {
                    Assert.That(args.Dice1 >= 1 && args.Dice1 <= 6, $"Dice 1 roll invalid: {args.Dice1}");
                    Assert.That(args.Dice2 >= 1 && args.Dice2 <= 6, $"Dice 2 roll invalid: {args.Dice2}");
                });
            };

            game.RoundOverEvent += (sender, args) =>
            {
                if (args.EndingRoundNumber == 30)
                    Assert.Pass();
            };

            await game.StartGame();

            Assert.Fail("The round over event did not get raised with round 30 ending.");
        }

        [Test]
        public async Task GameEndsAt50Rounds()
        {
            var game = new Game();

            game.RoundOverEvent += (sender, args) =>
            {
                if (args.EndingRoundNumber > 50)
                    Assert.Fail("The game did not end at 50 turns");
            };

            game.GameOverEvent += (sender, args) =>
            {
                Assert.Multiple(() =>
                {
                    Assert.That(game.RoundNumber, Is.EqualTo(51), "The round number is 51 because that would be the next round number.");
                    Assert.That(args.WinnerPlayerIDs.Count, Is.EqualTo(2), "All players have 0 victory points");
                });

                Assert.Pass();
            };

            await game.StartGame();

            Assert.Fail("The game over event did not get raised with round 50 ending.");
        }

    }
}
