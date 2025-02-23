using NUnit.Framework;
using SpaceBase;
using SpaceBase.Models;

namespace SpaceBaseTests
{
    [TestFixture]
    internal class SectorTests
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
        public void AddingCardWithDifferentSectorIDShouldFail()
        {
            int sectorID = 5;
            int cardSectorID = 1;

            var card = new Card(0, cardSectorID, 5, ActionType.AddCredits, 1, null, ActionType.AddCredits, 1, null);
            var sector = new Sector(sectorID, null);

            Assert.Multiple(() =>
            {
                Assert.Throws<ArgumentException>(() => new Sector(sectorID, card));
                Assert.Throws<ArgumentException>(() => sector.AddCard(card));
            });
        }

        [Test]
        public void CanCreateSectorWithCard()
        {
            int cost = 4;

            for (int i = 1; i < 13; ++i)
            {
                var card = new Card(1, i, cost, ActionType.AddCredits, cost, null, ActionType.AddCredits, cost, null);
                var sector = new Sector(i, card);

                Assert.Multiple(() =>
                {
                    Assert.That(sector.ID, Is.EqualTo(i));
                    Assert.That(sector.StationedCard?.SectorID, Is.EqualTo(i));
                    Assert.That(sector.StationedCard?.Cost, Is.EqualTo(cost));
                    Assert.That(sector.DeployedCards.Count, Is.EqualTo(0));
                });
            }
        }

        [Test]
        public void CanDeployCard()
        {
            int sectorID = 4;
            int cost1 = 0;
            int cost2 = 8;

            var card1 = new Card(0, sectorID, cost1, ActionType.AddCredits, cost1, null, ActionType.AddCredits, cost1, null);
            var card2 = new Card(2, sectorID, cost2, ActionType.AddCredits, cost2, null, ActionType.AddCredits, cost2, null);

            var sector = new Sector(sectorID, card1);
            sector.AddCard(card2);

            Assert.Multiple(() =>
            {
                Assert.That(sector.DeployedCards.Count, Is.EqualTo(1));
                Assert.That(sector.DeployedCards[0].Cost, Is.EqualTo(cost1));
                Assert.That(sector.StationedCard?.Cost, Is.EqualTo(cost2), "The second card should now be the stationed card.");
            });
        }
    }
}
