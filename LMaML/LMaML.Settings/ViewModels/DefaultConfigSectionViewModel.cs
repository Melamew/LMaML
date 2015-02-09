using System;
using System.Collections.Generic;
using System.Linq;
using iLynx.Configuration;
using LMaML.Infrastructure;
using iLynx.Common;
using LMaML.Infrastructure.Services;

namespace LMaML.Settings.ViewModels
{
    /// <summary>
    /// AppSettingsSectionViewModel
    /// </summary>
    public class DefaultConfigSectionViewModel : ISectionView
    {
        private readonly IEnumerable<IConfigurableValue> values;
        private readonly IValueEditorViewFactory editorFactory;

        /// <summary>
        /// Initializes a new instance of the <see cref="SettingsViewModel" /> class.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="values">The values.</param>
        /// <param name="editorFactory"></param>
        public DefaultConfigSectionViewModel(string name, IEnumerable<IConfigurableValue> values, IValueEditorViewFactory editorFactory)
        {
            this.values = values;
            this.editorFactory = editorFactory;
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
                return values.Select(x => editorFactory.CreateFor(x));
            }
        }
    }
}
