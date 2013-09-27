using System;
using System.Collections.Generic;

namespace LMaML.Infrastructure.Services.Interfaces
{
    /// <summary>
    /// IVisualization
    /// </summary>
    public interface IVisualization
    {
        /// <summary>
        /// Starts this instance.
        /// </summary>
        void Start();

        /// <summary>
        /// Stops this instance.
        /// </summary>
        void Stop();
    }

    /// <summary>
    /// IVisualizationRegistry
    /// </summary>
    public interface IVisualizationRegistry
    {
        /// <summary>
        /// Registers the specified visualization.
        /// </summary>
        /// <param name="visualization">The visualization.</param>
        /// <param name="name">The name.</param>
        void Register(Func<IVisualization> visualization, string name);

        /// <summary>
        /// Unregisters the specified visualization.
        /// </summary>
        /// <param name="name">The name.</param>
        void Unregister(string name);

        /// <summary>
        /// Gets the visualizations.
        /// </summary>
        /// <value>
        /// The visualizations.
        /// </value>
        IEnumerable<KeyValuePair<string, Func<IVisualization>>> Visualizations { get; }
    }
}
