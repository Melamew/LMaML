using LMaML.Infrastructure;
using LMaML.Infrastructure.Audio;
using Microsoft.Practices.Unity;

namespace LMaML.FMOD
{
    /// <summary>
    /// FMODModule
    /// </summary>
    public class FMODModule : ModuleBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="FMODModule" /> class.
        /// </summary>
        /// <param name="container">The container.</param>
        public FMODModule(IUnityContainer container) : base(container)
        {

        }

        /// <summary>
        /// Registers the types.
        /// <para>
        /// This is the second method called in the initialization process (Called AFTER AddResources)
        /// </para>
        /// </summary>
        protected override void RegisterTypes()
        {
            Container.RegisterType<IAudioPlayer, AudioPlayer>(new PerResolveLifetimeManager());
        }
    }
}
