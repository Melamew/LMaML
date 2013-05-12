using LMaML.Infrastructure;
using LMaML.Infrastructure.Services.Implementations;
using LMaML.Infrastructure.Services.Interfaces;
using LMaML.Tests.Helpers;
using Microsoft.Practices.Unity;
using NUnit.Framework;
using Telerik.JustMock;
using iLynx.Common;

namespace LMaML.Tests.LMaML.Infrastructure
{
    [TestFixture]
    public class CommandBusFixture
    {
        [Test]
        public void NullParameterTest()
        {
            TestHelper.NullParameterTest<CommandBus>();
        }

        [Test]
        public void WhenSendLogged()
        {
            // Arrange
            var loggerMock = Mock.Create<ILogger>();
            var target = new Builder<CommandBus>().With(loggerMock).Build();

            // Act
            target.Send(new Envelope<int>(50, this));

            // Assert
            Mock.Assert(() => loggerMock.Log(Arg.IsAny<LoggingType>(), target, Arg.IsAny<string>()));
        }

        [Test]
        public void WhenSendResolveAll()
        {
            // Arrange
            var containerMock = Mock.Create<IUnityContainer>();
            var target = new Builder<CommandBus>().With(containerMock).Build();

            // Act
            target.Send(new Envelope<int>(50, this));

            // Assert
            Mock.Assert(() => containerMock.ResolveAll(typeof(ICommandHandler<int>), Arg.IsAny<ResolverOverride[]>()));
        }

        [Test]
        public void WhenSendHandleCalled()
        {
            // Arrange
            var containerMock = Mock.Create<IUnityContainer>();
            var handlerMock = Mock.Create<ICommandHandler<int>>();
            Mock.Arrange(() => containerMock.ResolveAll(typeof (ICommandHandler<int>), Arg.IsAny<ResolverOverride[]>()))
                .Returns(new[] {handlerMock});
            var target = new Builder<CommandBus>().With(containerMock).Build();
            var expected = new Envelope<int>(50, this);
            
            // Act
            target.Send(expected);

            // Assert
            Mock.Assert(() => handlerMock.Handle(expected));
        }
    }
}
