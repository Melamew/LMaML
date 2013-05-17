using LMaML.Infrastructure;
using LMaML.Tests.Helpers;
using Microsoft.Practices.Prism.Logging;
using NUnit.Framework;
using Telerik.JustMock;
using iLynx.Common;

namespace LMaML.Tests.LMaML.Infrastructure
{
    [TestFixture]
    public class LoggerFacadeFixture
    {
        [Test]
        public void NullParameterTest()
        {
            TestHelper.NullParameterTest<LoggerFacade>();
        }

        [Test]
        public void WhenLogLoggerIsUsed()
        {
            var loggerMock = Mock.Create<ILogger>();
            var target = new Builder<LoggerFacade>().With(loggerMock).Build();

            target.Log("Something", Category.Debug, Priority.None);

            Mock.Assert(() => loggerMock.Log(Arg.IsAny<LoggingType>(), Arg.IsAny<object>(), Arg.IsAny<string>()));
        }
    }
}
