using LMaML.Infrastructure;
using LMaML.Tests.Helpers;
using NUnit.Framework;

namespace LMaML.Tests.LMaML.Infrastructure
{
    [TestFixture]
    public class EnvelopeFixture
    {
        [Test]
        public void NullParameterTest()
        {
            TestHelper.NullParameterTest<Envelope<object>>();
        }

        [Test]
        public void WhenInitializedValuesSet()
        {
            // Arrange / Act
            var target = new Envelope<EnvelopeFixture>(this, this);

            // Assert
            Assert.AreSame(this, target.Item);
            Assert.AreSame(this, target.Sender);
        }
    }
}
