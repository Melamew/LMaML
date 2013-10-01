using LMaML.Infrastructure;
using LMaML.Infrastructure.Audio;
using Microsoft.Practices.Unity;

namespace LMaML.Bass
{
    public class BassModule : ModuleBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ModuleBase" /> class.
        /// </summary>
        /// <param name="container">The container.</param>
        public BassModule(IUnityContainer container) : base(container)
        {
        }

        protected override void RegisterTypes()
        {
            Container.RegisterType<IAudioPlayer, BassPlayer>(new ContainerControlledLifetimeManager());
        }
    }
}
