using NUnit.Framework;
using SpaceBase.Models;

namespace SpaceBaseTests
{
    [TestFixture]
    internal class PlayerTests
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
        public void CanCreatePlayer()
        {
            var player = new Player(1);

            Assert.Multiple(() =>
            {
                Assert.That(player.Gold, Is.EqualTo(5), "Each player should start with 5 gold.");
                Assert.That(player.Income, Is.EqualTo(0));
                Assert.That(player.VictoryPoints, Is.EqualTo(0));
                Assert.That(player.ChargeCubes, Is.EqualTo(0));
            });
        }

        [TestCase(0, 0, TestName = "ResetWillSetPlayerGoldToIncome_0Gold_0Income")]
        [TestCase(5, 2, TestName = "ResetWillSetPlayerGoldToIncome_5Gold_2Income")]
        [TestCase(12, 3, TestName = "ResetWillSetPlayerGoldToIncome_12Gold_3Income")]
        public void ResetWillSetPlayerGoldToIncome(int gold, int income)
        {
            var player = new Player(1);
            player.AddGold(gold);
            player.AddIncome(income);

            Assert.Multiple(() =>
            {
                Assert.That(player.Gold, Is.EqualTo(5 + gold), "The 5 is from the player's starting amount.");
                Assert.That(player.Income, Is.EqualTo(income));
            });

            player.ResetGold();
            Assert.That(player.Gold, Is.EqualTo(income));
        }

        [Test]
        public void GameOverEventFiresAt40Points()
        {
            int playerID = 42;
            var player = new Player(playerID);
            player.GameOverEvent += (sender, e) =>
            {
                Assert.That(e.PlayerID, Is.EqualTo(playerID));
                Assert.Pass("GameOver event fired.");
            };

            player.AddVictoryPoints(39);
            Assert.That(player.VictoryPoints, Is.EqualTo(39));

            player.AddVictoryPoints(1);

            Assert.Fail("The game over event did not fire before this assertion.");
        }
    }
}
