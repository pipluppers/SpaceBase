using NUnit.Framework;
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
            Assert.Throws<ArgumentOutOfRangeException>(() => new Card(0, 3));
            Assert.Throws<ArgumentOutOfRangeException>(() => new Card(13, 3));
        }

        [Test]
        public void CanCreateCardWithValidSector()
        {
            for (int i = 1; i < 13; ++i)
            {
                var card = new Card(i, 1);
                Assert.That(i, Is.EqualTo(card.SectorID));
            }
        }
    }
}
