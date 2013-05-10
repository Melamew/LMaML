using System.Collections.Generic;
using System.Linq;
using LMaML.Infrastructure.Events;
using LMaML.Infrastructure.Services.Interfaces;
using iLynx.Common;
using iLynx.Common.WPF;

namespace LMaML.Visualization.ViewModels
{
    public class VisualizationViewModel : NotificationBase
    {
        private readonly IVisualizationRegistry visualizationRegistry;
        private readonly IDispatcher dispatcher;
        private IEnumerable<IVisualization> actualVisualizations;

        /// <summary>
        /// Initializes a new instance of the <see cref="VisualizationViewModel" /> class.
        /// </summary>
        /// <param name="visualizationRegistry">The visualization registry.</param>
        /// <param name="publicTransport">The public transport.</param>
        /// <param name="dispatcher">The dispatcher.</param>
        /// <param name="logger">The logger.</param>
        public VisualizationViewModel(IVisualizationRegistry visualizationRegistry, IPublicTransport publicTransport, IDispatcher dispatcher, ILogger logger)
            : base(logger)
        {
            this.visualizationRegistry = visualizationRegistry;
            this.dispatcher = dispatcher;
            publicTransport.ApplicationEventBus.Subscribe<VisualizationsChangedEvent>(OnVisualizationsChanged);
            ResetAvailable();
        }

        /// <summary>
        /// Called when [visualizations changed].
        /// </summary>
        /// <param name="visualizationsChangedEvent">The visualizations changed event.</param>
        private void OnVisualizationsChanged(VisualizationsChangedEvent visualizationsChangedEvent)
        {
            ResetAvailable();
        }

        /// <summary>
        /// Resets the available.
        /// </summary>
        private void ResetAvailable()
        {
            actualVisualizations = visualizationRegistry.Visualizations;
            dispatcher.Invoke(() =>
            {
                AvailableVisualizations = actualVisualizations.Select(x => x.Name);
                if (null == selectedVisualization || !availableVisualizations.Contains(selectedVisualization))
                    SelectedVisualization = availableVisualizations.FirstOrDefault();
            });
        }

        private IEnumerable<string> availableVisualizations;
        /// <summary>
        /// Gets or sets the available visualizations.
        /// </summary>
        /// <value>
        /// The available visualizations.
        /// </value>
        public IEnumerable<string> AvailableVisualizations
        {
            get { return availableVisualizations; }
            private set
            {
                if (ReferenceEquals(value, availableVisualizations)) return;
                availableVisualizations = value;
                RaisePropertyChanged(() => AvailableVisualizations);
            }
        }

        private string selectedVisualization;
        /// <summary>
        /// Gets or sets the selected visualization.
        /// </summary>
        /// <value>
        /// The selected visualization.
        /// </value>
        public string SelectedVisualization
        {
            get { return selectedVisualization; }
            set
            {
                if (value == selectedVisualization) return;
                selectedVisualization = value;
                RaisePropertyChanged(() => SelectedVisualization);
                OnSelectedVisualizationChanged(value);
            }
        }

        private IVisualization visualization;
        /// <summary>
        /// Gets or sets the visualization.
        /// </summary>
        /// <value>
        /// The visualization.
        /// </value>
        public IVisualization Visualization
        {
            get { return visualization; }
            private set
            {
                if (value == visualization)
                    return;
                visualization = value;
                RaisePropertyChanged(() => Visualization);
            }
        }

        /// <summary>
        /// Called when [selected visualization changed].
        /// </summary>
        /// <param name="selection">The selection.</param>
        private void OnSelectedVisualizationChanged(string selection)
        {
            if (null == selection && null != Visualization)
                Visualization.Stop();
            if (null == selection) return;
            Visualization = actualVisualizations.FirstOrDefault(x => x.Name == selection);
            if (null == Visualization) return;
            Visualization.Start();
        }
    }
}
