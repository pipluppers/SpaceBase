﻿namespace SpaceBaseTests
{
    [TestFixture]
    internal class BoardTests
    {
        [Test]
        public void CanCreateBoard()
        {
            Board board = new();

            ObservableCollection<Sector> sectors = board.Sectors;
            Assert.That(sectors, Is.Not.Null);
            Assert.That(sectors.Count, Is.EqualTo(12));
            
            for (int i = 0; i < 12; ++i)
            {
                Assert.Multiple(() =>
                {
                    Assert.That(sectors[i].ID, Is.EqualTo(i + 1));
                    Assert.That(sectors[i].StationedCard, Is.Null);
                    Assert.That(sectors[i].DeployedCards.Count, Is.EqualTo(0));
                });
            }
        }
    }
}
