using System;
using System.Collections.Generic;
using System.Linq;
using LMaML.Infrastructure;
using LMaML.Infrastructure.Events;
using LMaML.Infrastructure.Services.Interfaces;
using iLynx.Common;
using iLynx.Common.Configuration;
using iLynx.Common.WPF;

namespace LMaML.Settings.ViewModels
{
    /// <summary>
    /// SettingsViewModel
    /// </summary>
    public class SettingsViewModel : NotificationBase, IClosableItem
    {
        private readonly ISectionViewFactory viewFactory;
        private readonly IConfigurationManager configurationManager;
        private readonly IDispatcher dispatcher;

        /// <summary>
        /// Initializes a new instance of the <see cref="SettingsViewModel" /> class.
        /// </summary>
        /// <param name="viewFactory">The view factory.</param>
        /// <param name="configurationManager">The configuration manager.</param>
        /// <param name="publicTransport">The public transport.</param>
        /// <param name="dispatcher">The dispatcher.</param>
        public SettingsViewModel(ISectionViewFactory viewFactory, IConfigurationManager configurationManager, IPublicTransport publicTransport, IDispatcher dispatcher)
        {
            viewFactory.Guard("viewFactory");
            configurationManager.Guard("configurationManager");
            publicTransport.Guard("publicTransport");
            dispatcher.Guard("dispatcher");
            this.viewFactory = viewFactory;
            this.configurationManager = configurationManager;
            this.dispatcher = dispatcher;
            publicTransport.ApplicationEventBus.Subscribe<ConfigSectionsChangedEvent>(OnConfigSectionsChanged);
        }

        /// <summary>
        /// Called when [config sections changed].
        /// </summary>
        /// <param name="configSectionsChangedEvent">The config sections changed event.</param>
        private void OnConfigSectionsChanged(ConfigSectionsChangedEvent configSectionsChangedEvent)
        {
            dispatcher.Invoke(() => RaisePropertyChanged(() => Sections));
        }

        /// <summary>
        /// Gets the sections.
        /// </summary>
        /// <value>
        /// The sections.
        /// </value>
        public IEnumerable<ISectionView> Sections
        {
            get
            {
                return configurationManager.GetCategories().Select(cat => viewFactory.Build(cat, configurationManager.GetValues(cat)));
            }
        }

        /// <summary>
        /// Occurs when [request close].
        /// </summary>
        public event Action<IClosableItem> RequestClose;
    }

    public class ValueWrapper
    {
        private readonly IConfigurableValue value;

        public ValueWrapper(IConfigurableValue value)
        {
            value.Guard("value");
            this.value = value;
        }

        /// <summary>
        /// Gets the value.
        /// </summary>
        /// <value>
        /// The value.
        /// </value>
        public object Value
        {
            get { return value.Value; }
            set
            {
                if (value.GetType() != this.value.Value.GetType())
                    throw new InvalidCastException();
                this.value.Value = value;
                this.value.Store();
            }
        }

        /// <summary>
        /// Gets the name.
        /// </summary>
        /// <value>
        /// The name.
        /// </value>
        public string Name
        {
            get { return value.Key; }
        }
    }

    /// <summary>
    /// ValueWrapper
    /// </summary>
    public class ValueWrapper<T> : ValueWrapper
    {
        public ValueWrapper(IConfigurableValue value)
            : base(value)
        {
        }

        /// <summary>
        /// Gets the value.
        /// </summary>
        /// <value>
        /// The value.
        /// </value>
        public new T Value
        {
            get { return (T) base.Value; }
            set { base.Value = value; }
        }
    }
}
