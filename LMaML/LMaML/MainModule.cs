using LMaML.Infrastructure;
using LMaML.Views;
using Microsoft.Practices.Prism.Modularity;
using Microsoft.Practices.Prism.Regions;
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
        protected override void RegisterViews()
        {
            var regionManager = Container.Resolve<IRegionManager>();
            regionManager.RegisterViewWithRegion(RegionNames.HeaderRegion, () => Container.Resolve<MainMenuView>());
            regionManager.RegisterViewWithRegion(RegionNames.StatusRegion, () => Container.Resolve<StatusView>());
        }
    }
}
