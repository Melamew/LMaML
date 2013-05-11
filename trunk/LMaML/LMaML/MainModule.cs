using LMaML.Infrastructure;
using LMaML.Infrastructure.Services.Interfaces;
using LMaML.Views;
using Microsoft.Practices.Prism.Modularity;
using Microsoft.Practices.Unity;

namespace LMaML
{
    [Module(ModuleName = "MainModule")]
    public class MainModule : ModuleBase
    {
        public MainModule(IUnityContainer container) : base(container)
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
            regionManager.RegisterViewWithRegion(RegionNames.HeaderRegion, () => Container.Resolve<MainMenuView>());
            regionManager.RegisterViewWithRegion(RegionNames.StatusRegion, () => Container.Resolve<StatusView>());
        }
    }
}
