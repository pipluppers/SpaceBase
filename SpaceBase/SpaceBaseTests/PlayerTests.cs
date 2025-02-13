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
            var player = new HumanPlayer(1);
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
            Action<Player, int, int> addGold = CardActions.GetAction(ActionType.AddGold);
            Action<Player, int, int> addIncome = CardActions.GetAction(ActionType.AddIncome);
            Action<Player, int, int> addVictory = CardActions.GetAction(ActionType.AddVictoryPoints);
            int deployedNumGoldToAdd = 11;
            int deployedNumIncomeToAdd = 5;
            int deployedNumVictoryPointsToAdd = 3;

            var addGoldCard = new Card(sectorID, 2, addGold, 1, null, addGold, deployedNumGoldToAdd, null);
            var addIncomeCard = new Card(sectorID, 2, addIncome, 1, null, addIncome, deployedNumIncomeToAdd, null);
            var addVictoryPointsCard = new Card(sectorID, 2, addVictory, 1, null, addVictory, deployedNumVictoryPointsToAdd, null);

            var player = new HumanPlayer(1);
            player.AddCard(addGoldCard);
            player.AddCard(addIncomeCard);
            player.AddCard(addVictoryPointsCard);
            player.AddCard(addGoldCard);

            Assert.That(player.GetSector(sectorID).DeployedCards.Count, Is.EqualTo(3), "There should be 3 deployed cards.");

            player.ActivateDeployedCardsEffect(sectorID);

            int expectedGold = 5 + deployedNumGoldToAdd;
            Assert.Multiple(() =>
            {
                Assert.That(player.Gold, Is.EqualTo(expectedGold), $"The player starts off with 5 gold and gets {deployedNumGoldToAdd} from the deployed effect.");
                Assert.That(player.Income, Is.EqualTo(deployedNumIncomeToAdd), $"The player should have gotten {deployedNumIncomeToAdd} from the deployed effect.");
                Assert.That(player.VictoryPoints, Is.EqualTo(deployedNumVictoryPointsToAdd), $"The player should have gotten {deployedNumVictoryPointsToAdd} from the deployed effect.");
            });
        }

        [Test]
        public void PlayerCanAddGoldFromCardEffect()
        {
            int sectorID = 2;
            Action<Player, int, int> addGold = CardActions.GetAction(ActionType.AddGold);
            int numGoldToAdd = 6;
            int deployedNumGoldToAdd = 4;

            var player = new HumanPlayer(1);
            var card = new Card(sectorID, 0, addGold, numGoldToAdd, null, addGold, deployedNumGoldToAdd, null);

            Assert.That(player.Gold, Is.EqualTo(5), "Pre-test: Player should start with 5 gold.");

            player.AddCard(card);
            Assert.That(player.GetSector(sectorID).ActiveCard, Is.Not.Null);

            player.ActivateCardEffect(sectorID);
            int expectedGold = 5 + numGoldToAdd;
            Assert.That(player.Gold, Is.EqualTo(expectedGold), $"The player should have 5 + {numGoldToAdd} gold.");

            // Deploy the card and try activating the deployed effect

            player.AddCard(card);
            player.ActivateDeployedCardsEffect(sectorID);

            expectedGold += deployedNumGoldToAdd;
            Assert.That(player.Gold, Is.EqualTo(expectedGold), $"The player should have 5 + {numGoldToAdd} + {deployedNumGoldToAdd} gold.");
        }

        [Test]
        public void PlayerCanAddIncomeFromCardEffect()
        {
            int sectorID = 2;
            Action<Player, int, int> addIncome = CardActions.GetAction(ActionType.AddIncome);
            int numIncomeToAdd = 6;
            int deployedNumIncomeToAdd = 4;

            var player = new HumanPlayer(1);
            var card = new Card(sectorID, 0, addIncome, numIncomeToAdd, null, addIncome, deployedNumIncomeToAdd, null);

            Assert.That(player.Income, Is.EqualTo(0), "Pre-test: Player should start with 0 income.");

            player.AddCard(card);
            Assert.That(player.GetSector(sectorID).ActiveCard, Is.Not.Null);

            player.ActivateCardEffect(sectorID);
            int expectedIncome = numIncomeToAdd;
            Assert.That(player.Income, Is.EqualTo(expectedIncome), $"The player should have {numIncomeToAdd} gold.");

            // Deploy the card and try activating the deployed effect

            player.AddCard(card);
            player.ActivateDeployedCardsEffect(sectorID);

            expectedIncome += deployedNumIncomeToAdd;
            Assert.That(player.Income, Is.EqualTo(expectedIncome), $"The player should have {numIncomeToAdd} + {deployedNumIncomeToAdd} gold.");
        }

        [Test]
        public void PlayerCanAddVictoryPointsFromCardEffect()
        {
            int sectorID = 2;
            Action<Player, int, int> addIncome = CardActions.GetAction(ActionType.AddVictoryPoints);
            int numVictoryPointsToAdd = 6;
            int deployedNumVictoryPointsToAdd = 4;

            var player = new HumanPlayer(1);
            var card = new Card(sectorID, 0, addIncome, numVictoryPointsToAdd, null, addIncome, deployedNumVictoryPointsToAdd, null);

            Assert.That(player.Income, Is.EqualTo(0), "Pre-test: Player should start with 0 victory points.");

            player.AddCard(card);
            Assert.That(player.GetSector(sectorID).ActiveCard, Is.Not.Null);

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
