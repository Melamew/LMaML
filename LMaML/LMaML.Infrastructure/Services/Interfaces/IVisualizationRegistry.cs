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

        /// <summary>
        /// Gets the name.
        /// </summary>
        /// <value>
        /// The name.
        /// </value>
        string Name { get; }
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
        void Register(IVisualization visualization);

        /// <summary>
        /// Unregisters the specified visualization.
        /// </summary>
        /// <param name="visualization">The visualization.</param>
        void Unregister(IVisualization visualization);

        /// <summary>
        /// Gets the visualizations.
        /// </summary>
        /// <value>
        /// The visualizations.
        /// </value>
        IEnumerable<IVisualization> Visualizations { get; }
    }
}
