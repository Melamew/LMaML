using System.Collections.Generic;
using iLynx.Common.Configuration;

namespace LMaML.Infrastructure
{
    /// <summary>
    /// IConfigSection
    /// </summary>
    public interface IConfigSection
    {
        /// <summary>
        /// Gets the name.
        /// </summary>
        /// <value>
        /// The name.
        /// </value>
        string Name { get; }
        /// <summary>
        /// Gets the values.
        /// </summary>
        /// <value>
        /// The values.
        /// </value>
        IEnumerable<IConfigurableValue> Values { get; }
    }
}
