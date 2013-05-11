using LMaML.Infrastructure;
using LMaML.Infrastructure.Services.Interfaces;
using LMaML.PlayerControls.Views;
using Microsoft.Practices.Prism.Modularity;
using Microsoft.Practices.Prism.Regions;
using Microsoft.Practices.Unity;

namespace LMaML.PlayerControls
{
    [Module(ModuleName = ModuleNames.PlayerControlsModule)]
    public class PlayerControlsModule : ModuleBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PlayerControlsModule" /> class.
        /// </summary>
        /// <param name="container">The container.</param>
        public PlayerControlsModule(IUnityContainer container) : base(container)
        {
        }

        /// <summary>
        /// Registers the views.
        /// <para>
        /// This is the third method called in the initialization process (Called AFTER RegisterTypes)
        /// </para>
        /// </summary>
        /// <param name="regionManager">The region manager.</param>
        protected override void RegisterViews(IRegionManagerService regionManager)
        {
            regionManager.RegisterViewWithRegion(RegionNames.ControlsRegion, () => Container.Resolve<PlayerControlsView>());
            regionManager.RegisterViewWithRegion(RegionNames.CollapsedPlayerControlsRegion, () => Container.Resolve<CollapsedPlayerControls>());
        }
    }
}
