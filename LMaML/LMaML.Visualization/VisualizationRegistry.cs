using System.Collections.Generic;
using LMaML.Infrastructure.Events;
using LMaML.Infrastructure.Services.Interfaces;
using iLynx.Common;

namespace LMaML.Visualization
{
    /// <summary>
    /// VisualizationRegistry
    /// </summary>
    public class VisualizationRegistry : IVisualizationRegistry
    {
        private readonly List<IVisualization> visualizations = new List<IVisualization>();
        private readonly IEventBus<IApplicationEvent> eventBus;

        /// <summary>
        /// Initializes a new instance of the <see cref="VisualizationRegistry" /> class.
        /// </summary>
        /// <param name="publicTransport">The public transport.</param>
        public VisualizationRegistry(IPublicTransport publicTransport)
        {
            publicTransport.Guard("publicTransport");
            eventBus = publicTransport.ApplicationEventBus;
        }
        
        /// <summary>
        /// Registers the specified visualization.
        /// </summary>
        /// <param name="visualization">The visualization.</param>
        public void Register(IVisualization visualization)
        {
            visualization.Guard("visualization");
            Unregister(visualization);
            visualizations.Add(visualization);
            eventBus.Send(new VisualizationsChangedEvent());
        }

        /// <summary>
        /// Unregisters the specified visualization.
        /// </summary>
        /// <param name="visualization">The visualization.</param>
        public void Unregister(IVisualization visualization)
        {
            visualization.Guard("visualization");
            visualizations.Remove(visualization);
            eventBus.Send(new VisualizationsChangedEvent());
        }

        /// <summary>
        /// Gets the visualizations.
        /// </summary>
        /// <value>
        /// The visualizations.
        /// </value>
        public IEnumerable<IVisualization> Visualizations { get { return visualizations.AsReadOnly(); } }
    }
}
