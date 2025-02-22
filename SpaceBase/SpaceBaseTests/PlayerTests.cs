using NUnit.Framework;
using SpaceBase;
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
            var player = new HumanPlayer(1);

            Assert.Multiple(() =>
            {
                Assert.That(player.Credits, Is.EqualTo(5), "Each player should start with 5 credits.");
                Assert.That(player.Income, Is.EqualTo(0));
                Assert.That(player.VictoryPoints, Is.EqualTo(0));
                Assert.That(player.ChargeCubes, Is.EqualTo(0));
            });
        }

        [TestCase(0, 0, TestName = "ResetWillSetPlayerCreditsToIncome_0Credits_0Income")]
        [TestCase(5, 2, TestName = "ResetWillSetPlayerCreditsToIncome_5Credits_2Income")]
        [TestCase(12, 3, TestName = "ResetWillSetPlayerCreditsToIncome_12Credits_3Income")]
        public void ResetWillSetPlayerCreditsToIncome(int credits, int income)
        {
            var player = new HumanPlayer(1);
            player.AddCredits(credits);
            player.AddIncome(income);

            Assert.Multiple(() =>
            {
                Assert.That(player.Credits, Is.EqualTo(5 + credits), "The 5 is from the player's starting amount.");
                Assert.That(player.Income, Is.EqualTo(income));
            });

            player.ResetCredits();
            Assert.That(player.Credits, Is.EqualTo(income));
        }

        [Test]
        public void PlayerReachedVictoryThresholdEventFiresAt40Points()
        {
            int playerID = 42;
            var player = new HumanPlayer(playerID);
            player.PlayerReachedVictoryThresholdEvent += (sender, e) =>
            {
                Assert.That(e.PlayerID, Is.EqualTo(playerID));
                Assert.Pass("PlayerReachedVictoryThreshold event fired.");
            };

            player.AddVictoryPoints(39);
            Assert.That(player.VictoryPoints, Is.EqualTo(39));

            player.AddVictoryPoints(1);

            Assert.Fail("The game over event did not fire before this assertion.");
        }

        [Test]
        public void PlayerCanGetAllDeployedCardEffects()
        {
            int sectorID = 2;
            int deployedNumCreditsToAdd = 11;
            int deployedNumIncomeToAdd = 5;
            int deployedNumVictoryPointsToAdd = 3;

            var addCreditsCard = new Card(sectorID, 2, ActionType.AddCredits, 1, null, ActionType.AddCredits, deployedNumCreditsToAdd, null);
            var addIncomeCard = new Card(sectorID, 2, ActionType.AddIncome, 1, null, ActionType.AddIncome, deployedNumIncomeToAdd, null);
            var addVictoryPointsCard = new Card(sectorID, 2, ActionType.AddVictoryPoints, 1, null, ActionType.AddVictoryPoints, deployedNumVictoryPointsToAdd, null);

            var player = new HumanPlayer(1);
            player.AddCard(addCreditsCard);
            player.AddCard(addIncomeCard);
            player.AddCard(addVictoryPointsCard);
            player.AddCard(addCreditsCard);

            Assert.That(player.GetSector(sectorID).DeployedCards.Count, Is.EqualTo(3), "There should be 3 deployed cards.");

            player.ActivateDeployedCardsEffect(sectorID);

            int expectedCredits = 5 + deployedNumCreditsToAdd;
            Assert.Multiple(() =>
            {
                Assert.That(player.Credits, Is.EqualTo(expectedCredits), $"The player starts off with 5 credits and gets {deployedNumCreditsToAdd} from the deployed effect.");
                Assert.That(player.Income, Is.EqualTo(deployedNumIncomeToAdd), $"The player should have gotten {deployedNumIncomeToAdd} from the deployed effect.");
                Assert.That(player.VictoryPoints, Is.EqualTo(deployedNumVictoryPointsToAdd), $"The player should have gotten {deployedNumVictoryPointsToAdd} from the deployed effect.");
            });
        }

        [Test]
        public void PlayerCanAddCreditsFromCardEffect()
        {
            int sectorID = 2;
            int numCreditsToAdd = 6;
            int deployedNumCreditsToAdd = 4;

            var player = new HumanPlayer(1);
            var card = new Card(sectorID, 0, ActionType.AddCredits, numCreditsToAdd, null, ActionType.AddCredits, deployedNumCreditsToAdd, null);

            Assert.That(player.Credits, Is.EqualTo(5), "Pre-test: Player should start with 5 credits.");

            player.AddCard(card);
            Assert.That(player.GetSector(sectorID).StationedCard, Is.Not.Null);

            player.ActivateCardEffect(sectorID);
            int expectedCredits = 5 + numCreditsToAdd;
            Assert.That(player.Credits, Is.EqualTo(expectedCredits), $"The player should have 5 + {numCreditsToAdd} credits.");

            // Deploy the card and try activating the deployed effect

            player.AddCard(card);
            player.ActivateDeployedCardsEffect(sectorID);

            expectedCredits += deployedNumCreditsToAdd;
            Assert.That(player.Credits, Is.EqualTo(expectedCredits), $"The player should have 5 + {numCreditsToAdd} + {deployedNumCreditsToAdd} credits.");
        }

        [Test]
        public void PlayerCanAddIncomeFromCardEffect()
        {
            int sectorID = 2;
            int numIncomeToAdd = 6;
            int deployedNumIncomeToAdd = 4;

            var player = new HumanPlayer(1);
            var card = new Card(sectorID, 0, ActionType.AddIncome, numIncomeToAdd, null, ActionType.AddIncome, deployedNumIncomeToAdd, null);

            Assert.That(player.Income, Is.EqualTo(0), "Pre-test: Player should start with 0 income.");

            player.AddCard(card);
            Assert.That(player.GetSector(sectorID).StationedCard, Is.Not.Null);

            player.ActivateCardEffect(sectorID);
            int expectedIncome = numIncomeToAdd;
            Assert.That(player.Income, Is.EqualTo(expectedIncome), $"The player should have {numIncomeToAdd} credits.");

            // Deploy the card and try activating the deployed effect

            player.AddCard(card);
            player.ActivateDeployedCardsEffect(sectorID);

            expectedIncome += deployedNumIncomeToAdd;
            Assert.That(player.Income, Is.EqualTo(expectedIncome), $"The player should have {numIncomeToAdd} + {deployedNumIncomeToAdd} credits.");
        }

        [Test]
        public void PlayerCanAddVictoryPointsFromCardEffect()
        {
            int sectorID = 2;
            int numVictoryPointsToAdd = 6;
            int deployedNumVictoryPointsToAdd = 4;

            var player = new HumanPlayer(1);
            var card = new Card(sectorID, 0, ActionType.AddVictoryPoints, numVictoryPointsToAdd, null, ActionType.AddVictoryPoints, deployedNumVictoryPointsToAdd, null);

            Assert.That(player.Income, Is.EqualTo(0), "Pre-test: Player should start with 0 victory points.");

            player.AddCard(card);
            Assert.That(player.GetSector(sectorID).StationedCard, Is.Not.Null);

            player.ActivateCardEffect(sectorID);
            int expectedVictoryPoints = numVictoryPointsToAdd;
            Assert.That(player.VictoryPoints, Is.EqualTo(expectedVictoryPoints), $"The player should have {numVictoryPointsToAdd} victory points.");

            // Deploy the card and try activating the deployed effect

            player.AddCard(card);
            player.ActivateDeployedCardsEffect(sectorID);

            expectedVictoryPoints += deployedNumVictoryPointsToAdd;
            Assert.That(player.VictoryPoints, Is.EqualTo(expectedVictoryPoints), $"The player should have {numVictoryPointsToAdd} + {deployedNumVictoryPointsToAdd} victory points.");
        }
    }
}
