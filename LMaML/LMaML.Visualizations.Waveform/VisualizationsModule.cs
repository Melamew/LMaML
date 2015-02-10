using LMaML.Infrastructure;
using LMaML.Infrastructure.Services.Interfaces;
using Microsoft.Practices.Unity;

namespace LMaML.Visualizations.Waveform
{
    public class WaveformVisualizationsModule : ModuleBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="WaveformVisualizationsModule" /> class.
        /// </summary>
        /// <param name="container">The container.</param>
        public WaveformVisualizationsModule(IUnityContainer container)
            : base(container)
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
            registry.Register(() => Container.Resolve<SimpleWaveformVisualizationViewModel>(), "Simple Waveform");
        }
    }
}
