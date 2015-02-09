using System;
using iLynx.Common;
using iLynx.Configuration;
using LMaML.Infrastructure;

namespace LMaML.Settings.ViewModels
{
    public class SettingsValueDisplayViewModel : NotificationBase
    {
        private readonly IConfigurableValue value;
        private readonly SettingsValueViewModelBase settingsValueView;

        public SettingsValueDisplayViewModel(IConfigurableValue value, Func<IConfigurableValue, SettingsValueViewModelBase> viewBuilder)
        {
            this.value = Guard.IsNull(() => value);
            settingsValueView = viewBuilder(value);
        }

        public SettingsValueViewModelBase SettingsValueView
        {
            get { return settingsValueView; }
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
}
