using System;
using LMaML.Infrastructure.Events;
using LMaML.Infrastructure.Services.Implementations;
using LMaML.Infrastructure.Services.Interfaces;
using LMaML.Tests.Helpers;
using LMaML.ViewModels;
using NUnit.Framework;
using Telerik.JustMock;

namespace LMaML.Tests.LMaML
{
    [TestFixture, RequiresSTA]
    public class StatusViewModelFixture
    {
        /// <summary>
        /// Nulls the parameter test.
        /// </summary>
        [Test]
        public void NullParameterTest()
        {
            TestHelper.NullParameterTest<StatusViewModel>();
        }

        /// <summary>
        /// Whens the progress event raised status and progress set.
        /// </summary>
        [Test]
        public void WhenProgressEventRaisedStatusAndProgressSet()
        {
            // Arrange
            var publicTransportMock = Mock.Create<IPublicTransport>();
            var eventBus = new EventBus<IApplicationEvent>();
            Mock.Arrange(() => publicTransportMock.ApplicationEventBus).Returns(eventBus);
            var target = new Builder<StatusViewModel>().With(publicTransportMock).Build();
            const double expectedProgress = 50d;
            const string expectedStatus = "Some string";

            // Act
            eventBus.Send(new ProgressEvent(Guid.Empty, expectedProgress, expectedStatus));

            // Assert
            Assert.That(target.CurrentStatus == expectedStatus);
            Assert.That(Math.Abs(target.CurrentProgress - expectedProgress) <= double.Epsilon);
        }

        [Test]
        public void Todo()
        {
            Assert.Inconclusive("Add support for multiple workers in StatusViewModel");
        }
    }
}
