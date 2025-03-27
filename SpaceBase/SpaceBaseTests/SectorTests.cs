namespace SpaceBaseTests
{
    [TestFixture]
    internal class SectorTests
    {
        [Test]
        public void AddingCardWithDifferentSectorIDShouldFail()
        {
            int sectorID = 5;
            int cardSectorID = 1;

            Card card = new(0, cardSectorID, 5, ActionType.AddCredits, 1, null, ActionType.AddCredits, 1, null);
            Sector sector = new(sectorID, null);

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
                Card card = new(1, i, cost, ActionType.AddCredits, cost, null, ActionType.AddCredits, cost, null);
                Sector sector = new(i, card);

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

            Card card1 = new(0, sectorID, cost1, ActionType.AddCredits, cost1, null, ActionType.AddCredits, cost1, null);
            Card card2 = new(2, sectorID, cost2, ActionType.AddCredits, cost2, null, ActionType.AddCredits, cost2, null);

            Sector sector = new(sectorID, card1);
            sector.AddCard(card2);

            Assert.Multiple(() =>
            {
                Assert.That(sector.DeployedCards.Count, Is.EqualTo(1));
                Assert.That(sector.DeployedCards[0].Cost, Is.EqualTo(cost1));
                Assert.That(sector.StationedCard?.Cost, Is.EqualTo(cost2), "The second card should now be the stationed card.");
            });
        }

        [Test]
        public void AddingCardToSectorWithColonyCardShouldFail()
        {
            int sectorID = 1;

            ColonyCard colonyCard = new(1, 9);
            Sector sector = new(sectorID, colonyCard);

            Card newCard = new(1, 1, 3, ActionType.AddCredits, 2, null, ActionType.AddCredits, 1, null);

            Assert.Throws<InvalidOperationException>(() => sector.AddCard(newCard));
        }
    }
}
