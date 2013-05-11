using System.Windows;
using LMaML.Infrastructure;
using LMaML.Infrastructure.Domain;
using LMaML.Infrastructure.Domain.Concrete;
using LMaML.Infrastructure.Services.Implementations;
using LMaML.Infrastructure.Services.Interfaces;
using LMaML.Infrastructure.Util;
using Microsoft.Practices.Prism.Logging;
using Microsoft.Practices.Prism.Modularity;
using Microsoft.Practices.Prism.Regions;
using Microsoft.Practices.Prism.UnityExtensions;
using Microsoft.Practices.Unity;
using iLynx.Common;
using iLynx.Common.Configuration;
using iLynx.Common.WPF;
using iLynx.Common.WPF.Themes;

namespace LMaML
{
    /// <summary>
    /// Bootstrapper
    /// </summary>
    public class Bootstrapper : UnityBootstrapper
    {
        private Shell shell;
        private ILogger logger;
        private ILoggerFacade loggerFacade;

        /// <summary>
        /// Creates the shell or main window of the application.
        /// </summary>
        /// <returns>
        /// The shell of the application.
        /// </returns>
        /// <remarks>
        /// If the returned instance is a <see cref="T:System.Windows.DependencyObject" />, the
        /// <see cref="T:Microsoft.Practices.Prism.Bootstrapper" /> will attach the default <seealso cref="T:Microsoft.Practices.Prism.Regions.IRegionManager" /> of
        /// the application in its <see cref="F:Microsoft.Practices.Prism.Regions.RegionManager.RegionManagerProperty" /> attached property
        /// in order to be able to add regions by using the <seealso cref="F:Microsoft.Practices.Prism.Regions.RegionManager.RegionNameProperty" />
        /// attached property from XAML.
        /// </remarks>
        protected override DependencyObject CreateShell()
        {
            return (shell = new Shell());
        }

        /// <summary>
        /// Configures the <see cref="T:Microsoft.Practices.Unity.IUnityContainer" />. May be overwritten in a derived class to add specific
        /// type mappings required by the application.
        /// </summary>
        protected override void ConfigureContainer()
        {
            base.ConfigureContainer();
            Container.RegisterInstance(Container);
            Container.RegisterType<IGlobalHotkeyService, GlobalHotkeyService>(new ContainerControlledLifetimeManager());
            Container.RegisterType<IConfigurationManager, SingletonConfigurationManager>(new ContainerControlledLifetimeManager());
            Container.RegisterInstance(loggerFacade, new ContainerControlledLifetimeManager());
            Container.RegisterInstance(logger, new ContainerControlledLifetimeManager());
            Container.RegisterType<IDispatcher, WPFApplicationDispatcher>(new ContainerControlledLifetimeManager());
            Container.RegisterType(typeof(IEventBus<>), typeof(EventBus<>), new ContainerControlledLifetimeManager());
            Container.RegisterType(typeof(IAsyncFileScanner<>), typeof(RecursiveAsyncFileScanner<>));
            Container.RegisterType<IThemeManager, ThemeManager>(new ContainerControlledLifetimeManager());
            if (null != Application.Current)
            {
                var themeManager = Container.Resolve<IThemeManager>();
                themeManager.ApplyTheme(Application.Current.Resources, new FlatTheme());
            }
            Container.RegisterType<IMenuService, MenuService>(new ContainerControlledLifetimeManager());
            Container.RegisterType<ICommandBus, CommandBus>(new ContainerControlledLifetimeManager());
            Container.RegisterType<IPublicTransport, PublicTransport>(new ContainerControlledLifetimeManager());
            Container.RegisterType<IMergeDictionaryService, MergeDictionaryService>(new ContainerControlledLifetimeManager());
            Container.RegisterType<IRegionManager, RegionManager>(new ContainerControlledLifetimeManager());
            Container.RegisterType<IRegionManagerService, RegionManagerService>(new ContainerControlledLifetimeManager());
            Container.RegisterType<IDataPersister<StorableTaggedFile>, StorableTaggedFilePersister>(new ContainerControlledLifetimeManager());
            Container.RegisterType<IDataPersister<StorableTaggedFile>, StorableTaggedFilePersister>(new ContainerControlledLifetimeManager());
            Container.RegisterType<IInfoBuilder<StorableTaggedFile>, StorableTaggedFileBuilder>(new PerResolveLifetimeManager());
            Container.RegisterType(typeof (IDirectoryScannerService<>), typeof (DirectoryScannerService<>));
            Container.RegisterType<IReferenceAdapters, ReferenceAdaptersContainer>(new TransientLifetimeManager());
        }

        /// <summary>
        /// Create the <see cref="T:Microsoft.Practices.Prism.Logging.ILoggerFacade" /> used by the bootstrapper.
        /// </summary>
        /// <returns></returns>
        /// <remarks>
        /// The base implementation returns a new TextLogger.
        /// </remarks>
        protected override ILoggerFacade CreateLogger()
        {
            logger = RuntimeCommon.DefaultLogger;
            return (loggerFacade = new LoggerFacade(logger));
        }

        public override void Run(bool runWithDefaultConfiguration)
        {
            base.Run(runWithDefaultConfiguration);
            var manager = new ModuleManager(new ModuleInitializer(new UnityServiceLocator(Container), loggerFacade),
                                            new DirectoryModuleCatalog {ModulePath = @".\Plugins"}, loggerFacade);
            manager.Run();
        }

        /// <summary>
        /// Initializes the shell.
        /// </summary>
        protected override void InitializeShell()
        {
            base.InitializeShell();
            shell.PublicTransport = Container.Resolve<IPublicTransport>();
            shell.Logger = Container.Resolve<ILogger>();
            Application.Current.MainWindow = shell;
            Application.Current.MainWindow.Show();
        }

        /// <summary>
        /// Creates the <see cref="T:Microsoft.Practices.Prism.Modularity.IModuleCatalog" /> used by Prism.
        /// </summary>
        /// <returns></returns>
        /// <remarks>
        /// The base implementation returns a new ModuleCatalog.
        /// </remarks>
        protected override IModuleCatalog CreateModuleCatalog()
        {
            return new ConfigurationModuleCatalog();
        }
    }
}
