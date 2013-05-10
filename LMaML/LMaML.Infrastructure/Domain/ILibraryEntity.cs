using System;

namespace LMaML.Infrastructure.Domain
{
    /// <summary>
    /// IEntityBase
    /// </summary>
    public interface ILibraryEntity
    {
        /// <summary>
        /// Gets or sets the id.
        /// </summary>
        /// <value>
        /// The id.
        /// </value>
        Guid Id { get; set; }
    }
}