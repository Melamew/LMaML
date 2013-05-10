using System;
using System.Collections.Generic;
using iLynx.Common.Configuration;

namespace LMaML.Infrastructure.Services.Interfaces
{
    /// <summary>
    /// ISectionViewFactory
    /// </summary>
    public interface ISectionViewFactory
    {
        /// <summary>
        /// Builds the specified section.
        /// </summary>
        /// <param name="sectionName">Name of the section.</param>
        /// <param name="section">The section.</param>
        /// <returns></returns>
        ISectionView Build(string sectionName, IEnumerable<IConfigurableValue> section);

        /// <summary>
        /// Adds the builder.
        /// </summary>
        /// <param name="sectionName">Name of the section.</param>
        /// <param name="builder">The builder.</param>
        void AddBuilder(string sectionName, Func<string, IEnumerable<IConfigurableValue>, ISectionView> builder);
    }
}