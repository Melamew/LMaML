﻿using iLynx.Configuration;
using LMaML.Infrastructure;
using LMaML.Infrastructure.Services.Interfaces;
using Microsoft.Practices.Unity;

namespace LMaML.Visualizations.FFT
{
    public class VisualizationsModule : ModuleBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="VisualizationsModule" /> class.
        /// </summary>
        /// <param name="container">The container.</param>
        public VisualizationsModule(IUnityContainer container) : base(container)
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
            var registry = Container.Resolve<IVisualizationRegistry>();
            registry.Register(() => Container.Resolve<SimpleFFTVisualizationViewModel>(), "Simple FFT");
            registry.Register(() => Container.Resolve<SpectralFFTVisualizationViewModel>(), "Spectral FFT");
            RegisterConfiguration(Container.Resolve<IConfigurationManager>());
        }

        private static void RegisterConfiguration(IConfigurationManager configManager)
        {
            configManager.GetValue("Normalize", true, "FFT Visualization");
            configManager.GetValue("FFT Size", 1024, "FFT Visualization");
            configManager.GetValue("FFT Count", 512, "Spectral FFT");
        }
    }
}
