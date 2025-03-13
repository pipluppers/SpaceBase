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
                Assert.That(player.Credits, Is.EqualTo(0));
                Assert.That(player.Income, Is.EqualTo(0));
                Assert.That(player.VictoryPoints, Is.EqualTo(0));
                Assert.That(player.ChargeCubes, Is.EqualTo(0));
            });
        }

        [TestCase(0, 0, TestName = "ResetWillSetPlayerCreditsToIncomeIfLowerCredits_0Credits_0Income")]
        [TestCase(1, 2, TestName = "ResetWillSetPlayerCreditsToIncomeIfLowerCredits_1Credits_2Income")]
        public void ResetWillSetPlayerCreditsToIncomeIfLowerCredits(int credits, int income)
        {
            var player = new HumanPlayer(1);
            player.AddCredits(credits);
            player.AddIncome(income);

            Assert.Multiple(() =>
            {
                Assert.That(player.Credits, Is.EqualTo(credits));
                Assert.That(player.Income, Is.EqualTo(income));
            });

            player.ResetCredits();
            Assert.That(player.Credits, Is.EqualTo(income));
        }

        [TestCase(5, 5, TestName = "ResetWillNotSetPlayerCreditsToIncomeIfHigherOrEqualCredits_5Credits_5Income")]
        [TestCase(6, 5, TestName = "ResetWillNotSetPlayerCreditsToIncomeIfHigherOrEqualCredits_6Credits_5Income")]
        public void ResetWillNotSetPlayerCreditsToIncomeIfHigherOrEqualCredits(int credits, int income)
        {
            var player = new HumanPlayer(1);
            player.AddCredits(credits);
            player.AddIncome(income);

            Assert.Multiple(() =>
            {
                Assert.That(player.Credits, Is.EqualTo(credits));
                Assert.That(player.Income, Is.EqualTo(income));
            });

            player.ResetCredits();
            Assert.That(player.Credits, Is.EqualTo(credits));
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
        public void BuyingACardWithHigherCostThanCreditsShouldFail()
        {
            var player = new HumanPlayer(1);
            var card = new Card(1, 10, 3, ActionType.AddCredits, 5, null, ActionType.AddCredits, 5, null);

            Assert.Throws<InvalidOperationException>(() => player.BuyCard(card));
            Assert.Throws<InvalidOperationException>(() => player.BuyCard(card, false));
        }

        [Test]
        public void SafeBuyShouldNotReduceCreditsToZero()
        {
            int startingCredits = 10;
            int cardCost = 3;

            var player = new HumanPlayer(1);
            var card = new Card(1, 10, cardCost, ActionType.AddCredits, 5, null, ActionType.AddCredits, 5, null);

            player.AddCredits(startingCredits);
            player.BuyCard(card, false);
            Assert.That(player.Credits, Is.EqualTo(startingCredits - cardCost));
        }

        [Test]
        public void PlayerAddCardToSectorEventFires()
        {
            var player = new HumanPlayer(1);
            player.AddCardToSectorEvent += (sender, e) =>
            {
                Assert.Pass();
            };

            var card = new Card(1, 10, 3, ActionType.AddCredits, 5, null, ActionType.AddCredits, 5, null);

            player.AddCard(card);

            Assert.Fail("The AddCardToSectorEvent should have fired.");
        }

        [Test]
        public void PlayerBuyCardTriggersAddCardToSectorEvent()
        {
            var player = new HumanPlayer(1);
            player.AddCardToSectorEvent += (sender, e) =>
            {
                Assert.Pass();
            };

            var card = new Card(1, 12, 3, ActionType.AddCredits, 5, null, ActionType.AddCredits, 5, null);

            // Make sure the player has enough credits
            player.AddCredits(5);

            player.BuyCard(card);

            Assert.Fail("The AddCardToSectorEvent should have fired.");
        }

        [Test]
        public void PlayerSafeBuyCardTriggersAddCardToSectorEvent()
        {
            var player = new HumanPlayer(1);
            player.AddCardToSectorEvent += (sender, e) =>
            {
                Assert.Pass();
            };

            var card = new Card(1, 12, 3, ActionType.AddCredits, 5, null, ActionType.AddCredits, 5, null);

            // Make sure the player has enough credits
            player.AddCredits(5);

            player.BuyCard(card, false);

            Assert.Fail("The AddCardToSectorEvent should have fired.");
        }

        [Test]
        public void PlayerCanGetAllDeployedCardEffects()
        {
            int sectorID = 2;
            int deployedNumCreditsToAdd = 11;
            int deployedNumIncomeToAdd = 5;
            int deployedNumVictoryPointsToAdd = 3;

            var addCreditsCard = new Card(1, sectorID, 2, ActionType.AddCredits, 1, null, ActionType.AddCredits, deployedNumCreditsToAdd, null);
            var addIncomeCard = new Card(1, sectorID, 2, ActionType.AddIncome, 1, null, ActionType.AddIncome, deployedNumIncomeToAdd, null);
            var addVictoryPointsCard = new Card(1, sectorID, 2, ActionType.AddVictoryPoints, 1, null, ActionType.AddVictoryPoints, deployedNumVictoryPointsToAdd, null);

            var player = new HumanPlayer(1);
            player.AddCard(addCreditsCard);
            player.AddCard(addIncomeCard);
            player.AddCard(addVictoryPointsCard);
            player.AddCard(addCreditsCard);

            Assert.That(player.GetSector(sectorID).DeployedCards.Count, Is.EqualTo(3), "There should be 3 deployed cards.");

            player.ActivateDeployedCardsEffect(sectorID);

            Assert.Multiple(() =>
            {
                Assert.That(player.Credits, Is.EqualTo(deployedNumCreditsToAdd), $"The player should have {deployedNumCreditsToAdd} from the deployed effect.");
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
            var card = new Card(1, sectorID, 3, ActionType.AddCredits, numCreditsToAdd, null, ActionType.AddCredits, deployedNumCreditsToAdd, null);

            player.AddCard(card);
            Assert.That(player.GetSector(sectorID).StationedCard, Is.Not.Null);

            player.ActivateCardEffect(sectorID);
            Assert.That(player.Credits, Is.EqualTo(numCreditsToAdd), $"The player should have {numCreditsToAdd} credits.");

            // Deploy the card and try activating the deployed effect

            player.AddCard(card);
            player.ActivateDeployedCardsEffect(sectorID);

            int expectedNumCredits = numCreditsToAdd + deployedNumCreditsToAdd;
            Assert.That(player.Credits, Is.EqualTo(expectedNumCredits), $"The player should have {expectedNumCredits} credits.");
        }

        [Test]
        public void PlayerCanAddIncomeFromCardEffect()
        {
            int sectorID = 2;
            int numIncomeToAdd = 6;
            int deployedNumIncomeToAdd = 4;

            var player = new HumanPlayer(1);
            var card = new Card(1, sectorID, 3, ActionType.AddIncome, numIncomeToAdd, null, ActionType.AddIncome, deployedNumIncomeToAdd, null);

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
            var card = new Card(1, sectorID, 3, ActionType.AddVictoryPoints, numVictoryPointsToAdd, null, ActionType.AddVictoryPoints, deployedNumVictoryPointsToAdd, null);

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
