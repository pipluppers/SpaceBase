using NUnit.Framework;
using SpaceBase;
using SpaceBase.Models;

namespace SpaceBaseTests
{
    [TestFixture]
    internal class CardTests
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
        public void CardWithInvalidSectorShouldThrow()
        {
            int invalidSectorID1 = 0;
            int invalidSectorID2 = 13;

            Assert.Multiple(() =>
            {
                Assert.Throws<ArgumentOutOfRangeException>(() => new Card(1, invalidSectorID1, 3, ActionType.AddCredits, 1, null, ActionType.AddCredits, 1, null));
                Assert.Throws<ArgumentOutOfRangeException>(() => new Card(1, invalidSectorID2, 3, ActionType.AddCredits, 1, null, ActionType.AddCredits, 1, null));
            });
        }

        [Test]
        public void CanCreateCardWithValidSector()
        {
            for (int i = 1; i < 13; ++i)
            {
                var card = new Card(0, i, 1, ActionType.AddCredits, 1, null, ActionType.AddCredits, 1, null);
                Assert.That(i, Is.EqualTo(card.SectorID));
            }
        }

        [Test]
        public void CardWithInvalidLevelShouldThrow()
        {
            int invalidLevel1 = -1;
            int invalidLevel2 = 4;

            Assert.Multiple(() =>
            {
                Assert.Throws<ArgumentOutOfRangeException>(() => new Card(invalidLevel1, 2, 3, ActionType.AddCredits, 1, null, ActionType.AddCredits, 1, null));
                Assert.Throws<ArgumentOutOfRangeException>(() => new Card(invalidLevel2, 2, 3, ActionType.AddCredits, 1, null, ActionType.AddCredits, 1, null));
            });
        }

        [Test]
        public void CardWithInvalidLevelCostShouldThrow()
        {
            Assert.Multiple(() =>
            {
                // Level 1
                Assert.Throws<ArgumentOutOfRangeException>(() => new Card(1, 2, 1, ActionType.AddCredits, 1, null, ActionType.AddCredits, 1, null));
                Assert.Throws<ArgumentOutOfRangeException>(() => new Card(1, 2, 6, ActionType.AddCredits, 1, null, ActionType.AddCredits, 1, null));

                // Level 2
                Assert.Throws<ArgumentOutOfRangeException>(() => new Card(2, 2, 6, ActionType.AddCredits, 1, null, ActionType.AddCredits, 1, null));
                Assert.Throws<ArgumentOutOfRangeException>(() => new Card(2, 2, 10, ActionType.AddCredits, 1, null, ActionType.AddCredits, 1, null));

                // Level 3
                Assert.Throws<ArgumentOutOfRangeException>(() => new Card(3, 2, 11, ActionType.AddCredits, 1, null, ActionType.AddCredits, 1, null));
                Assert.Throws<ArgumentOutOfRangeException>(() => new Card(3, 2, 15, ActionType.AddCredits, 1, null, ActionType.AddCredits, 1, null));
            });
        }

        [Test]
        public void CanCreateCardWithValidLevelCostCombo()
        {
            Assert.DoesNotThrow(() => new Card(1, 2, 2, ActionType.AddCredits, 1, null, ActionType.AddCredits, 1, null));
            Assert.DoesNotThrow(() => new Card(1, 2, 3, ActionType.AddCredits, 1, null, ActionType.AddCredits, 1, null));
            Assert.DoesNotThrow(() => new Card(1, 2, 4, ActionType.AddCredits, 1, null, ActionType.AddCredits, 1, null));
            Assert.DoesNotThrow(() => new Card(1, 2, 5, ActionType.AddCredits, 1, null, ActionType.AddCredits, 1, null));

            Assert.DoesNotThrow(() => new Card(2, 2, 7, ActionType.AddCredits, 1, null, ActionType.AddCredits, 1, null));
            Assert.DoesNotThrow(() => new Card(2, 2, 8, ActionType.AddCredits, 1, null, ActionType.AddCredits, 1, null));
            Assert.DoesNotThrow(() => new Card(2, 2, 9, ActionType.AddCredits, 1, null, ActionType.AddCredits, 1, null));

            Assert.DoesNotThrow(() =>  new Card(3, 2, 12, ActionType.AddCredits, 1, null, ActionType.AddCredits, 1, null));
            Assert.DoesNotThrow(() =>  new Card(3, 2, 13, ActionType.AddCredits, 1, null, ActionType.AddCredits, 1, null));
            Assert.DoesNotThrow(() =>  new Card(3, 2, 14, ActionType.AddCredits, 1, null, ActionType.AddCredits, 1, null));
        }

        [Test]
        public void CanCreateChargeCard()
        {
            int requiredChargeCubes = 1;
            int chargeCubeLimit = 2;
            int deployedRequiredChargeCubes = 2;
            int deployedChargeCubeLimit = 2;

            // This is the add to sum card
            ChargeCard chargeCard = new(1, 1, 4, ActionType.AddChargeCube, 1, null, ActionType.AddChargeCube, 1, null,
                ChargeActionType.AddToSum1, requiredChargeCubes, chargeCubeLimit, ChargeCardType.Anytime,
                ChargeActionType.AddToSum1, deployedRequiredChargeCubes, deployedChargeCubeLimit, ChargeCardType.Anytime);

            Assert.That(chargeCard.RequiredChargeCubes, Is.EqualTo(requiredChargeCubes));
            Assert.That(chargeCard.ChargeCubeLimit, Is.EqualTo(chargeCubeLimit));
            Assert.That(chargeCard.DeployedRequiredChargeCubes, Is.EqualTo(deployedChargeCubeLimit));
            Assert.That(chargeCard.DeployedChargeCubeLimit, Is.EqualTo(deployedChargeCubeLimit));
        }

        [Test]
        public void CreatingAChargeCardThatRequiresMoreCubesThanLimitShouldFail()
        {
            Assert.Throws<ArgumentException>(() => new ChargeCard(0, 1, 1, ActionType.AddChargeCube, 1, null, ActionType.AddChargeCube, 1, null,
                ChargeActionType.AddToSum1, 3, 2, ChargeCardType.Anytime,
                ChargeActionType.AddToSum1, 2, 2, ChargeCardType.Anytime));

            Assert.Throws<ArgumentException>(() => new ChargeCard(0, 1, 1, ActionType.AddChargeCube, 1, null, ActionType.AddChargeCube, 1, null,
                ChargeActionType.AddToSum1, 2, 2, ChargeCardType.Anytime,
                ChargeActionType.AddToSum1, 3, 2, ChargeCardType.Anytime));

            Assert.Throws<ArgumentException>(() => new ChargeCard(0, 1, 1, ActionType.AddChargeCube, 1, null, ActionType.AddChargeCube, 1, null,
                ChargeActionType.AddToSum1, 3, 2, ChargeCardType.Anytime,
                ChargeActionType.AddToSum1, 3, 2, ChargeCardType.Anytime));
        }

    }
}
