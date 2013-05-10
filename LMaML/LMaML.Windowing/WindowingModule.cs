using LMaML.Infrastructure;
using LMaML.Infrastructure.Services.Interfaces;
using LMaML.Services;
using Microsoft.Practices.Prism.Modularity;
using Microsoft.Practices.Unity;

namespace LMaML.Windowing
{
    [Module(ModuleName = "WindowingModule")]
    public class WindowingModule : ModuleBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="WindowingModule" /> class.
        /// </summary>
        /// <param name="container">The container.</param>
        public WindowingModule(IUnityContainer container) : base(container)
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
            Container.RegisterType<IWindowManager, WindowManager>(new ContainerControlledLifetimeManager());
            Container.RegisterType<IWindowFactoryService, WindowFactory>(new ContainerControlledLifetimeManager());
        }
    }
}
