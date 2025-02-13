using NUnit.Framework;
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

            var card = new Card(cardSectorID, 5);
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
                var card = new Card(i, cost);
                var sector = new Sector(i, card);

                Assert.Multiple(() =>
                {
                    Assert.That(sector.ID, Is.EqualTo(i));
                    Assert.That(sector.ActiveCard?.SectorID, Is.EqualTo(i));
                    Assert.That(sector.ActiveCard?.Cost, Is.EqualTo(cost));
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

            var card1 = new Card(sectorID, cost1);
            var card2 = new Card(sectorID, cost2);

            var sector = new Sector(sectorID, card1);
            sector.AddCard(card2);

            Assert.Multiple(() =>
            {
                Assert.That(sector.DeployedCards.Count, Is.EqualTo(1));
                Assert.That(sector.DeployedCards[0].Cost, Is.EqualTo(cost1));
                Assert.That(sector.ActiveCard?.Cost, Is.EqualTo(cost2), "The second card should now be the active card.");
            });
        }
    }
}
