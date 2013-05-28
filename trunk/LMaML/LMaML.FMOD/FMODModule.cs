using System;
using System.IO;
using LMaML.Infrastructure;
using LMaML.Infrastructure.Audio;
using Microsoft.Practices.Unity;
using iLynx.Common.Configuration;

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
            var configurationManager = Container.Resolve<IConfigurationManager>();
            var pluginDir = configurationManager.GetValue("FMOD Plugin Directory", "Plugins\\Codecs");
            Container.RegisterType<IAudioPlayer, AudioPlayer>(new PerResolveLifetimeManager());
            var player = Container.Resolve<IAudioPlayer>();
            var path = Path.Combine(Environment.CurrentDirectory, pluginDir.Value);
            player.LoadPlugins(path);
        }

    }
}
