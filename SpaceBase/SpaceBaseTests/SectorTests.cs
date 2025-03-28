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

            Mock<ICard> mockCard = new();
            mockCard.Setup(card => card.SectorID).Returns(cardSectorID);

            Sector sector = new(sectorID, null);

            Assert.Multiple(() =>
            {
                Assert.Throws<ArgumentException>(() => new Sector(sectorID, mockCard.Object));
                Assert.Throws<ArgumentException>(() => sector.AddCard(mockCard.Object));
            });
        }

        [Test]
        public void CanCreateSectorWithCard()
        {
            int cost = 4;

            for (int i = 1; i < 13; ++i)
            {
                Mock<ICard> mockCard = new();
                mockCard.Setup(card => card.Cost).Returns(cost);
                mockCard.Setup(card => card.SectorID).Returns(i);

                Sector sector = new(i, mockCard.Object);

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

            Mock<IStandardCard> mockCard1 = new();
            mockCard1.Setup(card => card.SectorID).Returns(sectorID);
            mockCard1.Setup(card => card.Cost).Returns(cost1);

            Mock<IStandardCard> mockCard2 = new();
            mockCard2.Setup(card => card.SectorID).Returns(sectorID);
            mockCard2.Setup(card => card.Cost).Returns(cost2);

            Sector sector = new(sectorID, mockCard1.Object);
            sector.AddCard(mockCard2.Object);

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

            Mock<IColonyCard> mockColonyCard = new();
            mockColonyCard.Setup(colonyCard => colonyCard.SectorID).Returns(sectorID);

            Sector sector = new(sectorID, mockColonyCard.Object);

            Mock<IStandardCard> mockStandardCard = new();
            mockStandardCard.Setup(standardCard => standardCard.SectorID).Returns(sectorID);

            Assert.Throws<InvalidOperationException>(() => sector.AddCard(mockStandardCard.Object));
        }
    }
}
