using System.IO;
using LMaML.Infrastructure;
using LMaML.Infrastructure.Audio;
using LMaML.Infrastructure.Events;
using LMaML.Infrastructure.Services.Interfaces;
using LMaML.Infrastructure.Util;
using Microsoft.Practices.Prism.Modularity;
using Microsoft.Practices.Unity;
using iLynx.Common.Threading;

namespace LMaML.Services
{
    [Module(ModuleName = ModuleNames.ServicesModule)]
    public class ServicesModule : ModuleBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ServicesModule" /> class.
        /// </summary>
        /// <param name="container">The container.</param>
        public ServicesModule(IUnityContainer container) : base(container)
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
            Container.RegisterType<IThreadManager, ThreadManager>(new ContainerControlledLifetimeManager());
            Container.RegisterType<IPlayerService, PlayerService>(new ContainerControlledLifetimeManager());
            //Container.RegisterType<IPlayerService, PlayerService>(new ContainerControlledLifetimeManager());
            Container.RegisterType<IPlaylistService, PlaylistService>(new ContainerControlledLifetimeManager());
            Container.RegisterType<IInfoBuilder<FileInfo>, FileInfoBuilder>(new PerResolveLifetimeManager());
            Container.RegisterType<IInfoBuilder<ID3File>, ID3FileBuilder>(new PerResolveLifetimeManager());
            Container.Resolve<IEventBus<IApplicationEvent>>().Subscribe<ShutdownEvent>(OnShutdown);
        }

        private void OnShutdown(ShutdownEvent shutdownEvent)
        {
            var audioPlayer = Container.Resolve<IPlayerService>();
            audioPlayer.Stop();
            audioPlayer.Dispose();
        }
    }
}
