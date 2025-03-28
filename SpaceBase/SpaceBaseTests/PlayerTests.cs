﻿namespace SpaceBaseTests
{
    [TestFixture]
    internal class PlayerTests
    {
        [Test]
        public void CanCreatePlayer()
        {
            int id = 1;
            HumanPlayer player = new(id);

            Assert.Multiple(() =>
            {
                Assert.That(player.ID, Is.EqualTo(id));
                Assert.That(player.Board, Is.Not.Null);
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
            HumanPlayer player = new(1);
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
            HumanPlayer player = new(1);
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
        public void BuyingACardWithHigherCostThanCreditsShouldFail()
        {
            int cardCost = 1;

            HumanPlayer player = new(1);
            Assert.That(player.Credits, Is.EqualTo(0));

            Mock<ICard> mockCard = new();
            mockCard.Setup(card => card.SectorID).Returns(2);
            mockCard.Setup(card => card.Cost).Returns(cardCost);

            Assert.Throws<InvalidOperationException>(() => player.BuyCard(mockCard.Object));
            Assert.Throws<InvalidOperationException>(() => player.BuyCard(mockCard.Object, false));
        }

        [Test]
        public void SafeBuyShouldNotReduceCreditsToZero()
        {
            int startingCredits = 10;
            int cardCost = 3;

            HumanPlayer player = new(1);

            Mock<ICard> mockCard = new();
            mockCard.Setup(card => card.SectorID).Returns(2);
            mockCard.Setup(card => card.Cost).Returns(cardCost);

            player.AddCredits(startingCredits);
            player.BuyCard(mockCard.Object, false);
            Assert.That(player.Credits, Is.EqualTo(startingCredits - cardCost));
        }

        [Test]
        public void PlayerAddCardToSectorEventFires()
        {
            HumanPlayer player = new(1);
            player.AddCardToSectorEvent += (sender, e) =>
            {
                Assert.Pass();
            };

            Mock<ICard> mockCard = new();
            mockCard.Setup(card => card.SectorID).Returns(2);
            mockCard.Setup(card => card.Cost).Returns(3);

            player.AddCard(mockCard.Object);

            Assert.Fail("The AddCardToSectorEvent should have fired.");
        }

        [Test]
        public void PlayerBuyCardTriggersAddCardToSectorEvent()
        {
            HumanPlayer player = new(1);
            player.AddCardToSectorEvent += (sender, e) =>
            {
                Assert.Pass();
            };

            Mock<ICard> mockCard = new();
            mockCard.Setup(card => card.SectorID).Returns(2);
            mockCard.Setup(card => card.Cost).Returns(3);

            // Make sure the player has enough credits
            player.AddCredits(5);

            player.BuyCard(mockCard.Object);

            Assert.Fail("The AddCardToSectorEvent should have fired.");
        }

        [Test]
        public void PlayerSafeBuyCardTriggersAddCardToSectorEvent()
        {
            HumanPlayer player = new(1);
            player.AddCardToSectorEvent += (sender, e) =>
            {
                Assert.Pass();
            };

            Mock<ICard> mockCard = new();
            mockCard.Setup(card => card.SectorID).Returns(2);
            mockCard.Setup(card => card.Cost).Returns(3);

            // Make sure the player has enough credits
            player.AddCredits(5);

            player.BuyCard(mockCard.Object, false);

            Assert.Fail("The AddCardToSectorEvent should have fired.");
        }

        [Test]
        public void PlayerCanGetAllDeployedCardEffects()
        {
            int sectorID = 2;
            int deployedNumCreditsToAdd = 11;
            int deployedNumIncomeToAdd = 5;
            int deployedNumVictoryPointsToAdd = 3;

            // TODO
            //Mock<IStandardCard> mockAddCreditsCard = new();
            //mockAddCreditsCard.Setup(addCreditsCard => addCreditsCard.SectorID).Returns(sectorID);
            //mockAddCreditsCard.Setup(addCreditsCard => addCreditsCard.Cost).Returns(2);
            //mockAddCreditsCard.Setup(addCreditsCard => addCreditsCard.EffectType).Returns(ActionType.AddCredits);
            //mockAddCreditsCard.Setup(addCreditsCard => addCreditsCard.Amount).Returns(1);
            //mockAddCreditsCard.Setup(addCreditsCard => addCreditsCard.DeployedEffectType).Returns(ActionType.AddCredits);
            //mockAddCreditsCard.Setup(addCreditsCard => addCreditsCard.DeployedAmount).Returns(deployedNumCreditsToAdd);

            //Mock<IStandardCard> mockAddIncomeCard = new();
            //mockAddIncomeCard.Setup(addIncomeCard => addIncomeCard.SectorID).Returns(sectorID);
            //mockAddIncomeCard.Setup(addIncomeCard => addIncomeCard.Cost).Returns(2);
            //mockAddIncomeCard.Setup(addIncomeCard => addIncomeCard.EffectType).Returns(ActionType.AddIncome);
            //mockAddIncomeCard.Setup(addIncomeCard => addIncomeCard.Amount).Returns(1);
            //mockAddIncomeCard.Setup(addIncomeCard => addIncomeCard.DeployedEffectType).Returns(ActionType.AddIncome);
            //mockAddIncomeCard.Setup(addIncomeCard => addIncomeCard.DeployedAmount).Returns(deployedNumIncomeToAdd);

            //Mock<IStandardCard> mockAddVictoryPointsCard = new();
            //mockAddVictoryPointsCard.Setup(card => card.SectorID).Returns(sectorID);
            //mockAddVictoryPointsCard.Setup(card => card.Cost).Returns(2);
            //mockAddVictoryPointsCard.Setup(card => card.EffectType).Returns(ActionType.AddVictoryPoints);
            //mockAddVictoryPointsCard.Setup(card => card.Amount).Returns(1);
            //mockAddVictoryPointsCard.Setup(card => card.DeployedEffectType).Returns(ActionType.AddVictoryPoints);
            //mockAddVictoryPointsCard.Setup(card => card.DeployedAmount).Returns(deployedNumVictoryPointsToAdd);

            Card addCreditsCard = new(1, sectorID, 2, ActionType.AddCredits, 1, null, ActionType.AddCredits, deployedNumCreditsToAdd, null);
            Card addIncomeCard = new(1, sectorID, 2, ActionType.AddIncome, 1, null, ActionType.AddIncome, deployedNumIncomeToAdd, null);
            Card addVictoryPointsCard = new(1, sectorID, 2, ActionType.AddVictoryPoints, 1, null, ActionType.AddVictoryPoints, deployedNumVictoryPointsToAdd, null);

            HumanPlayer player = new(1);
            player.AddCard(addCreditsCard);
            player.AddCard(addIncomeCard);
            player.AddCard(addVictoryPointsCard);
            player.AddCard(addCreditsCard);
            //player.AddCard(mockAddCreditsCard.Object);
            //player.AddCard(mockAddIncomeCard.Object);
            //player.AddCard(mockAddVictoryPointsCard.Object);
            //player.AddCard(mockAddCreditsCard.Object);

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

            HumanPlayer player = new(1);
            Card card = new(1, sectorID, 3, ActionType.AddCredits, numCreditsToAdd, null, ActionType.AddCredits, deployedNumCreditsToAdd, null);

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

            HumanPlayer player = new(1);
            Card card = new(1, sectorID, 3, ActionType.AddIncome, numIncomeToAdd, null, ActionType.AddIncome, deployedNumIncomeToAdd, null);

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

            HumanPlayer player = new(1);
            Card card = new(1, sectorID, 3, ActionType.AddVictoryPoints, numVictoryPointsToAdd, null, ActionType.AddVictoryPoints, deployedNumVictoryPointsToAdd, null);

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

        [Test]
        public void PlayerCanUseAddChargeCubesToCard()
        {
            int sectorID = 1;

            HumanPlayer player = new(1);
            ChargeCard chargeCard = new(1, sectorID, 4, ActionType.AddChargeCube, 1, null, ActionType.AddChargeCube, 1, null,
                ChargeActionType.AddToSum1, 1, 2, ChargeCardType.Anytime,
                ChargeActionType.AddToSum1, 2, 2, ChargeCardType.Anytime);

            player.AddCard(chargeCard);

            if (player.GetSector(sectorID).StationedCard is not ChargeCard playerChargeCard)
            {
                Assert.Fail("Charge card should not be null.");
                return;
            }

            Assert.That(playerChargeCard.NumChargeCubes, Is.EqualTo(0));

            player.ActivateCardEffect(sectorID);
            Assert.That(playerChargeCard.NumChargeCubes, Is.EqualTo(1));
        }
    }
}
