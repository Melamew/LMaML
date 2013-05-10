using System;
using System.Windows;
using LMaML.Infrastructure.Services.Interfaces;
using iLynx.Common;

namespace LMaML.Infrastructure.Services.Implementations
{
    /// <summary>
    /// MergeDictionaryService
    /// </summary>
    public class MergeDictionaryService : ComponentBase, IMergeDictionaryService
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MergeDictionaryService" /> class.
        /// </summary>
        /// <param name="logger">The logger.</param>
        public MergeDictionaryService(ILogger logger)
            : base(logger)
        {
            
        }

        /// <summary>
        /// Adds the resource.
        /// </summary>
        /// <param name="uri">The URI.</param>
        public void AddResource(Uri uri)
        {
            LogInformation("Attempting to add resource: {0}", uri);
            Application.Current.Resources.MergedDictionaries.Add(new ResourceDictionary { Source = uri });
        }

        /// <summary>
        /// Adds the resource.
        /// </summary>
        /// <param name="filename">The filename.</param>
        public void AddResource(string filename)
        {
            AddResource(new Uri(filename, UriKind.RelativeOrAbsolute));
        }
    }
}
