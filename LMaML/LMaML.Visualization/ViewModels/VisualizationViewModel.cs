using System;
using System.Collections.Generic;
using System.Linq;
using iLynx.Configuration;
using LMaML.Infrastructure;
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
        private readonly IConfigurableValue<string> lastVisualization;
        private bool haveSelectedLast = false;

        /// <summary>
        /// Initializes a new instance of the <see cref="VisualizationViewModel" /> class.
        /// </summary>
        /// <param name="visualizationRegistry">The visualization registry.</param>
        /// <param name="publicTransport">The public transport.</param>
        /// <param name="dispatcher">The dispatcher.</param>
        /// <param name="configurationManager">...</param>
        public VisualizationViewModel(IVisualizationRegistry visualizationRegistry, IPublicTransport publicTransport, IDispatcher dispatcher, IConfigurationManager configurationManager)
        {
            this.visualizationRegistry = visualizationRegistry;
            this.dispatcher = dispatcher;
            publicTransport.ApplicationEventBus.Subscribe<VisualizationsChangedEvent>(OnVisualizationsChanged);
            lastVisualization = configurationManager.GetValue("Visualizations.LastVisualization", string.Empty,
                KnownConfigSections.Hidden);
            publicTransport.ApplicationEventBus.Subscribe<ShutdownEvent>(OnShutdown);
            ResetAvailable();
        }

        private void OnShutdown(ShutdownEvent shutdownEvent)
        {
            lastVisualization.Value = SelectedVisualization;
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
            dispatcher.Invoke(() =>
            {
                AvailableVisualizations = visualizationRegistry.Visualizations.Select(x => x.Key);
                if (null == selectedVisualization || !availableVisualizations.Contains(selectedVisualization))
                    SelectedVisualization = availableVisualizations.FirstOrDefault();
                if (haveSelectedLast) return;
                var last = lastVisualization.Value;
                if (string.IsNullOrEmpty(last) || !availableVisualizations.Contains(last)) return;
                if (selectedVisualization == last)
                {
                    haveSelectedLast = true;
                    return;
                }
                SelectedVisualization = lastVisualization.Value;
                haveSelectedLast = true;
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
            var builder = visualizationRegistry.Visualizations.FirstOrDefault(x => x.Key == selection).Value;
            if (null == builder) return;
            if (null != Visualization)
            {
                Visualization.IsActive = false;
                Visualization.Stop();
            }
            Visualization = builder();
            if (null == Visualization) return;
            Visualization.IsActive = true;
            Visualization.Start();
        }
    }
}
