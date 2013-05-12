using System;
using LMaML.Infrastructure.Events;
using LMaML.Infrastructure.Services.Implementations;
using LMaML.Tests.Helpers;
using NUnit.Framework;
using Telerik.JustMock;

namespace LMaML.Tests.LMaML.Infrastructure
{
    [TestFixture]
    public class EventBusFixture
    {
        [Test]
        public void NullParameterTest()
        {
            TestHelper.NullParameterTest<EventBus<IEvent>>();
        }

        [Test]
        public void WhenSendEventSent()
        {
            // Arrange
            var target = new Builder<EventBus<IEvent>>().Build();
            var sent = false;
            target.Subscribe<IEvent>(ev => sent = true);

            // Act
            target.Send(Mock.Create<IEvent>());

            // Assert
            Assert.IsTrue(sent);
        }

        private class DummyEvent : IEvent { }

        [Test]
        public void WhenNoSubscribersNothingHappens()
        {
            // Arrange
            var target = new Builder<EventBus<IEvent>>().Build();
            var sent = false;
            target.Subscribe<IEvent>(ev => sent = true);

            // Act
            target.Send(new DummyEvent());

            // Assert
            Assert.IsFalse(sent);
        }

        [Test]
        public void WhenUnsubscribeNoEventsReceived()
        {
            // Arrange
            var target = new Builder<EventBus<IEvent>>().Build();
            var sent = false;
            var action = new Action<IEvent>(e => sent = true);
            target.Subscribe(action);
            
            // Act
            target.Unsubscribe(action);
            target.Send(Mock.Create<IEvent>());

            // Assert
            Assert.IsFalse(sent);
        }
    }
}
