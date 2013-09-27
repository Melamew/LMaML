using System;
using System.Collections.Generic;
using System.Linq;
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
        private readonly Dictionary<string, Func<IVisualization>> visualizations = new Dictionary<string, Func<IVisualization>>();
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
        /// <param name="name">The name.</param>
        public void Register(Func<IVisualization> visualization, string name)
        {
            visualization.Guard("visualization");
            name.GuardString("name");
            Unregister(name);
            visualizations.Add(name, visualization);
            eventBus.Send(new VisualizationsChangedEvent());
        }

        /// <summary>
        /// Unregisters the specified visualization.
        /// </summary>
        /// <param name="name">The name.</param>
        public void Unregister(string name)
        {
            name.GuardString("name");
            visualizations.Remove(name);
            eventBus.Send(new VisualizationsChangedEvent());
        }

        /// <summary>
        /// Gets the visualizations.
        /// </summary>
        /// <value>
        /// The visualizations.
        /// </value>
        public IEnumerable<KeyValuePair<string, Func<IVisualization>>> Visualizations { get { return visualizations.ToList(); } }
    }
}
