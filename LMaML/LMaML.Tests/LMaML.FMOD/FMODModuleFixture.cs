using System;
using LMaML.FMOD;
using LMaML.Infrastructure.Audio;
using Microsoft.Practices.Unity;
using NUnit.Framework;
using Telerik.JustMock;
using iLynx.Common.Configuration;

namespace LMaML.Tests.LMaML.FMOD
{
    [TestFixture]
    public class FMODModuleFixture
    {
        [Test]
        public void WhenInitializedAudioplayerRegistered()
        {
            // Arrange
            var container = Mock.Create<IUnityContainer>();
            var configurationManager = Mock.Create<IConfigurationManager>();
            var configValue = Mock.Create<IConfigurableValue<string>>();
            Mock.Arrange(() => configValue.Value).Returns(" ");
            Mock.Arrange(() => configurationManager.GetValue(Arg.IsAny<string>(), Arg.IsAny<string>(), Arg.IsAny<string>()))
                .Returns(configValue);
            Mock.Arrange(() => container.Resolve(Arg.Matches<Type>(x => x == typeof(IConfigurationManager)), Arg.IsAny<string>(), Arg.IsAny<ResolverOverride[]>()))
                .Returns(configurationManager);
            var target = new Builder<FMODModule>().With(container).Build();

            // Act
            target.Initialize();

            // Assert
            Mock.Assert(() => container.RegisterType(typeof(IAudioPlayer), typeof(AudioPlayer), Arg.IsAny<string>(), Arg.IsAny<LifetimeManager>(), Arg.IsAny<InjectionMember[]>()));
        }
    }
}
