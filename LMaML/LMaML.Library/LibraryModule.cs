using LMaML.Infrastructure;
using LMaML.Infrastructure.Services.Interfaces;
using LMaML.Library.ViewModels;
using LMaML.Library.Views;
using Microsoft.Practices.Prism.Modularity;
using Microsoft.Practices.Prism.Regions;
using Microsoft.Practices.Unity;

namespace LMaML.Library
{
    /// <summary>
    /// LibraryModule
    /// </summary>
    [Module(ModuleName = ModuleNames.LibraryModule)]
    public class LibraryModule : ModuleBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="LibraryModule" /> class.
        /// </summary>
        /// <param name="container">The container.</param>
        public LibraryModule(IUnityContainer container) : base(container)
        {
            
        }

        protected override void AddResources()
        {
            AddResources("DataTemplates.xaml");
        }

        protected override void RegisterTypes()
        {
            Container.RegisterType<ILibraryManagerService, LibraryManagerService>(new ContainerControlledLifetimeManager());
            Container.RegisterType<IFilteringService, FilteringService>(new ContainerControlledLifetimeManager());
        }

        /// <summary>
        /// Notifies the module that it has be initialized.
        /// </summary>
        protected override void RegisterViews()
        {
            var regionManager = Container.Resolve<IRegionManager>();
            regionManager.RegisterViewWithRegion(RegionNames.BrowserRegion, () => Container.Resolve<BrowserView>());
        }
    }
}
