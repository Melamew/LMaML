//using System;
//using System.Windows;
//using iLynx.Common.WPF;
//using iLynx.Common;

//namespace LMaML.Infrastructure.Services.Implementations
//{
//    /// <summary>
//    /// MergeDictionaryService
//    /// </summary>
//    public class MergeDictionaryService : IMergeDictionaryService
//    {
//        /// <summary>
//        /// Adds the resource.
//        /// </summary>
//        /// <param name="uri">The URI.</param>
//        public void AddResource(Uri uri)
//        {
//            this.LogInformation("Attempting to add resource: {0}", uri);
//            Application.Current.Resources.MergedDictionaries.Add(new ResourceDictionary { Source = uri });
//        }

//        /// <summary>
//        /// Adds the resource.
//        /// </summary>
//        /// <param name="filename">The filename.</param>
//        public void AddResource(string filename)
//        {
//            AddResource(new Uri(filename, UriKind.RelativeOrAbsolute));
//        }
//    }
//}
