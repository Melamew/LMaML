using System;

namespace LMaML.Infrastructure.Util
{
    /// <summary>
    /// IColumnSetup
    /// </summary>
    public interface IColumnSetup
    {
        /// <summary>
        /// Gets the name.
        /// </summary>
        /// <value>
        /// The name.
        /// </value>
        string Name { get; }

        /// <summary>
        /// Gets the id.
        /// </summary>
        /// <value>
        /// The id.
        /// </value>
        Guid Id { get; }
    }
}