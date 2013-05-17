using System;

namespace LMaML.Infrastructure.Services.Interfaces
{
    /// <summary>
    /// IMergeDictionaryService
    /// </summary>
    public interface IMergeDictionaryService
    {
        /// <summary>
        /// Adds the resource.
        /// </summary>
        /// <param name="uri">The URI.</param>
        void AddResource(Uri uri);
    }
}
