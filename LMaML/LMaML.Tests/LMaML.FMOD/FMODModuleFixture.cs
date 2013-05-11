using LMaML.FMOD;
using LMaML.Infrastructure.Audio;
using Microsoft.Practices.Unity;
using NUnit.Framework;
using Telerik.JustMock;

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
            var target = new Builder<FMODModule>().With(container).Build();

            // Act
            target.Initialize();

            // Assert
            Mock.Assert(() => container.RegisterType(typeof(IAudioPlayer), typeof(AudioPlayer), Arg.IsAny<string>(), Arg.IsAny<LifetimeManager>(), Arg.IsAny<InjectionMember[]>()));
        }
    }
}
