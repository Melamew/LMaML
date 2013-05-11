using LMaML.Infrastructure.Events;
using LMaML.Infrastructure.Services.Interfaces;
using LMaML.Tests.Helpers;
using NUnit.Framework;
using Telerik.JustMock;

namespace LMaML.Tests.LMaML
{
    [TestFixture, RequiresSTA]
    public class ShellViewModelFixture
    {
        [Test]
        public void NullParameterTest()
        {
            TestHelper.NullParameterTest<ShellViewModel>();
        }

        [Test]
        public void WhenCollapsedCommandExecutedEventRaised()
        {
            IEventBus<IApplicationEvent> eventBusMock;
            var publicTransportMock = TestHelper.MakePublicTransportMock(out eventBusMock);
            var target = new Builder<ShellViewModel>().With(publicTransportMock).Build();

            target.CollapsedCommand.Execute(null);

            Mock.Assert(() => eventBusMock.Send(Arg.IsAny<ShellCollapsedEvent>()));
        }

        [Test]
        public void WhenExpandedCommandExecutedEventRaised()
        {
            IEventBus<IApplicationEvent> eventBusMock;
            var publicTransportMock = TestHelper.MakePublicTransportMock(out eventBusMock);
            var target = new Builder<ShellViewModel>().With(publicTransportMock).Build();

            target.ExpandedCommand.Execute(null);

            Mock.Assert(() => eventBusMock.Send(Arg.IsAny<ShellExpandedEvent>()));
        }
    }
}
