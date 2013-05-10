using System.Collections.Generic;
using LMaML.Infrastructure;
using iLynx.Common.Configuration;

namespace LMaML.Settings
{
    /// <summary>
    /// DefaultConfigSection
    /// </summary>
    public class DefaultConfigSection : IConfigSection
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DefaultConfigSection" /> class.
        /// </summary>
        /// <param name="values">The values.</param>
        /// <param name="name">The name.</param>
        public DefaultConfigSection(IEnumerable<IConfigurableValue> values, string name)
        {
            Values = values;
            Name = name;
        }

        /// <summary>
        /// Gets the name.
        /// </summary>
        /// <value>
        /// The name.
        /// </value>
        public string Name { get; private set; }

        /// <summary>
        /// Gets the values.
        /// </summary>
        /// <value>
        /// The values.
        /// </value>
        public IEnumerable<IConfigurableValue> Values { get; private set; }
    }
}
