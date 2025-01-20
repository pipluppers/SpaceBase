using NUnit.Framework;
using SpaceBase.Models;

namespace SpaceBaseTests
{
    [TestFixture]
    internal class BoardTests
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
        public void CanCreateBoard()
        {
            var board = new Board();

            var sectors = board.Sectors;
            Assert.That(sectors, Is.Not.Null);
            Assert.That(sectors.Count, Is.EqualTo(12));
            
            for (int i = 0; i < 12; ++i)
            {
                Assert.Multiple(() =>
                {
                    Assert.That(sectors[i].ID, Is.EqualTo(i + 1));
                    Assert.That(sectors[i].ActiveCard.SectorID, Is.EqualTo(i + 1));
                    Assert.That(sectors[i].ActiveCard.Cost, Is.EqualTo(0), "The beginning cards should not have any cost.");
                    Assert.That(sectors[i].DeployedCards.Count, Is.EqualTo(0));
                });
            }
        }
    }
}
