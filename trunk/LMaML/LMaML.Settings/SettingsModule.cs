using LMaML.Infrastructure;
using LMaML.Infrastructure.Services.Interfaces;
using LMaML.Infrastructure.Util;
using LMaML.Settings.ViewModels;
using Microsoft.Practices.Unity;

namespace LMaML.Settings
{
    public class SettingsModule : ModuleBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SettingsModule" /> class.
        /// </summary>
        /// <param name="container">The container.</param>
        public SettingsModule(IUnityContainer container) : base(container)
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
            Container.RegisterType<ISectionViewFactory, SectionViewFactory>(new ContainerControlledLifetimeManager());
            Container.Resolve<ISectionViewFactory>().AddBuilder(KnownConfigSections.GlobalHotkeys, (s, values) => new GlobalHotkeySettingsViewModel(s, values));
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
        /// Registers the views.
        /// <para>
        /// This is the third method called in the initialization process (Called AFTER RegisterTypes)
        /// </para>
        /// </summary>
        protected override void RegisterViews()
        {
            var menuService = Container.Resolve<IMenuService>();
            menuService.Register(new CallbackMenuItem(null, "Tools", new CallbackMenuItem(OpenSettingsCallback, "Settings")));
        }

        /// <summary>
        /// Opens the settings callback.
        /// </summary>
        private void OpenSettingsCallback()
        {
            var windowManager = Container.Resolve<IWindowManager>();
            windowManager.OpenNew(Container.Resolve<SettingsViewModel>(), "Settings", 600, 480);
        }
    }
}
