using System;
using System.Collections.Generic;
using System.Linq;
using LMaML.Infrastructure;
using iLynx.Common;
using iLynx.Common.Configuration;

namespace LMaML.Settings.ViewModels
{
    /// <summary>
    /// AppSettingsSectionViewModel
    /// </summary>
    public class DefaultConfigSectionViewModel : ISectionView
    {
        private readonly IEnumerable<IConfigurableValue> values;

        /// <summary>
        /// Initializes a new instance of the <see cref="SettingsViewModel" /> class.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="values">The values.</param>
        public DefaultConfigSectionViewModel(string name, IEnumerable<IConfigurableValue> values)
        {
            this.values = values;
            name.Guard("section");
            values.Guard("values");
            Title = name;
        }

        /// <summary>
        /// Gets the title.
        /// </summary>
        /// <value>
        /// The title.
        /// </value>
        public string Title { get; private set; }

        /// <summary>
        /// Gets the values.
        /// </summary>
        /// <value>
        /// The values.
        /// </value>
        public IEnumerable<object> Values
        {
            get
            {
                return values.Select(x =>
                {
                    var wrapperType = typeof(ValueWrapper<>);
                    wrapperType = wrapperType.MakeGenericType(x.Value.GetType());
                    return Activator.CreateInstance(wrapperType, x);
                });
            }
        }
    }
}
