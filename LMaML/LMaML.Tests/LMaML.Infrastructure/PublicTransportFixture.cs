using iLynx.PubSub;
using LMaML.Infrastructure.Events;
using LMaML.Infrastructure.Services.Implementations;
using LMaML.Infrastructure.Services.Interfaces;
using LMaML.Tests.Helpers;
using NUnit.Framework;
using Telerik.JustMock;

namespace LMaML.Tests.LMaML.Infrastructure
{
    [TestFixture]
    public class PublicTransportFixture
    {
        [Test]
        public void NullParameterTest()
        {
            TestHelper.NullParameterTest<PublicTransport>();
        }

        [Test]
        public void WhenInitializedValuesSet()
        {
            // Arrange
            var eventBus = Mock.Create<IBus<IApplicationEvent>>();
            var commandBus = Mock.Create<IBus<IBusMessage>>();

            // Act
            var target = new Builder<PublicTransport>().With(commandBus).With(eventBus).Build();

            // Assert
            Assert.AreSame(eventBus, target.ApplicationEventBus);
            Assert.AreSame(commandBus, target.CommandBus);
        }
    }
}
