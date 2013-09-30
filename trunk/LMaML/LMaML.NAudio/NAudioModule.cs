using LMaML.Infrastructure;
using LMaML.Infrastructure.Audio;
using Microsoft.Practices.Unity;

namespace LMaML.NAudio
{
    public class NAudioModule : ModuleBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ModuleBase" /> class.
        /// </summary>
        /// <param name="container">The container.</param>
        public NAudioModule(IUnityContainer container) : base(container)
        {
        }

        protected override void RegisterTypes()
        {
            Container.RegisterType<IAudioPlayer, NAudioPlayer>(new ContainerControlledLifetimeManager());
        }
    }
}
