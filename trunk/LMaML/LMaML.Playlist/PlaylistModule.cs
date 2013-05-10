using LMaML.Infrastructure;
using LMaML.Playlist.Views;
using Microsoft.Practices.Prism.Modularity;
using Microsoft.Practices.Prism.Regions;
using Microsoft.Practices.Unity;

namespace LMaML.Playlist
{
    /// <summary>
    /// PlaylistModule
    /// </summary>
    [Module(ModuleName = ModuleNames.PlaylistModule)]
    public class PlaylistModule : ModuleBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PlaylistModule" /> class.
        /// </summary>
        /// <param name="container">The container.</param>
        public PlaylistModule(IUnityContainer container)
            : base(container)
        {
        }

        /// <summary>
        /// Registers the views.
        /// <para>
        /// This is the third method called in the initialization process (Called AFTER RegisterTypes)
        /// </para>
        /// </summary>
        protected override void RegisterViews()
        {
            var regionManager = Container.Resolve<IRegionManager>();
            regionManager.RegisterViewWithRegion(RegionNames.PlaylistRegion, () => Container.Resolve<PlaylistView>());
        }
    }
}
