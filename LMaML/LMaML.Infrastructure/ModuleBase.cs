using System;
using System.Reflection;
using LMaML.Infrastructure.Services.Interfaces;
using Microsoft.Practices.Prism.Modularity;
using Microsoft.Practices.Unity;
using iLynx.Common;

namespace LMaML.Infrastructure
{
    /// <summary>
    /// ModuleBase
    /// </summary>
    public abstract class ModuleBase : IModule
    {
        private readonly IUnityContainer container;
        private readonly IMergeDictionaryService mergeDictionaryService;

        /// <summary>
        /// Initializes a new instance of the <see cref="ModuleBase" /> class.
        /// </summary>
        /// <param name="container">The container.</param>
        protected ModuleBase(IUnityContainer container)
        {
            container.Guard("container");
            this.container = container;
            mergeDictionaryService = container.Resolve<IMergeDictionaryService>();
        }

        /// <summary>
        /// Adds the resources.
        /// <para>
        /// This is the first method called in the initialization process
        /// </para>
        /// </summary>
        protected virtual void AddResources() { }

        /// <summary>
        /// Registers the types.
        /// <para>
        /// This is the second method called in the initialization process (Called AFTER AddResources)
        /// </para>
        /// </summary>
        protected virtual void RegisterTypes() { }

        /// <summary>
        /// Registers the views.
        /// <para>
        /// This is the third method called in the initialization process (Called AFTER RegisterTypes)
        /// </para>
        /// </summary>
        /// <param name="regionManagerService">The region manager service.</param>
        protected virtual void RegisterViews(IRegionManagerService regionManagerService) { }

        /// <summary>
        /// Gets the container.
        /// </summary>
        /// <value>
        /// The container.
        /// </value>
        protected virtual IUnityContainer Container { get { return container; } }

        /// <summary>
        /// Adds the resources.
        /// </summary>
        /// <param name="file">The file.</param>
        protected virtual void AddResources(Uri file)
        {
            mergeDictionaryService.AddResource(file);
        }

        /// <summary>
        /// Adds the resources.
        /// </summary>
        /// <param name="file">The file.</param>
        protected virtual void AddResources(string file)
        {
            mergeDictionaryService.AddResource(RuntimeHelper.MakePackUri(Assembly.GetCallingAssembly(), file));
        }

        /// <summary>
        /// Notifies the module that it has be initialized.
        /// </summary>
        public void Initialize()
        {
            AddResources();
            RegisterTypes();
            RegisterViews(Container.Resolve<IRegionManagerService>());
        }
    }
}
