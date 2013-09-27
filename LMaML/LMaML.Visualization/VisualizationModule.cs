using LMaML.Infrastructure;
using LMaML.Infrastructure.Services.Interfaces;
using LMaML.Visualization.Views;
using Microsoft.Practices.Prism.Modularity;
using Microsoft.Practices.Prism.Regions;
using Microsoft.Practices.Unity;

namespace LMaML.Visualization
{
    [Module(ModuleName = "VisualizationModule")]
    public class VisualizationModule : ModuleBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="VisualizationModule" /> class.
        /// </summary>
        /// <param name="container">The container.</param>
        public VisualizationModule(IUnityContainer container) : base(container)
        {

        }

        /// <summary>
        /// Adds the resources.
        /// <para>
        /// This is the first method called in the initialization process
        /// </para>
        /// </summary>
        protected override void AddResources()
        {
            AddResources("DataTemplates.xaml");
        }

        /// <summary>
        /// Registers the types.
        /// <para>
        /// This is the second method called in the initialization process (Called AFTER AddResources)
        /// </para>
        /// </summary>
        protected override void RegisterTypes()
        {
            Container.RegisterType<IVisualizationRegistry, VisualizationRegistry>(new ContainerControlledLifetimeManager());
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
            regionManager.RegisterViewWithRegion(RegionNames.VisualizationRegion, () => Container.Resolve<VisualizationView>());
        }
    }
}
