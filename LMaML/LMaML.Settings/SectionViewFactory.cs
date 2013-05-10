using System;
using System.Collections.Generic;
using LMaML.Infrastructure;
using LMaML.Infrastructure.Services.Interfaces;
using iLynx.Common;
using iLynx.Common.Configuration;

namespace LMaML.Settings.ViewModels
{
    public class SectionViewFactory : ISectionViewFactory
    {
        private readonly Dictionary<string, Func<string, IEnumerable<IConfigurableValue>, ISectionView>> builders = new Dictionary<string, Func<string, IEnumerable<IConfigurableValue>, ISectionView>>();

        /// <summary>
        /// Builds the default.
        /// </summary>
        /// <param name="sectionName">Name of the section.</param>
        /// <param name="section">The section.</param>
        /// <returns></returns>
        private static ISectionView BuildDefault(string sectionName, IEnumerable<IConfigurableValue> section)
        {
            return new DefaultConfigSectionViewModel(sectionName, section);
        }

        /// <summary>
        /// Builds the specified section.
        /// </summary>
        /// <param name="sectionName">Name of the section.</param>
        /// <param name="section">The section.</param>
        /// <returns></returns>
        public ISectionView Build(string sectionName, IEnumerable<IConfigurableValue> section)
        {
            section.Guard("section");
            Func<string, IEnumerable<IConfigurableValue>, ISectionView> builder;
            return !builders.TryGetValue(sectionName, out builder) ? BuildDefault(sectionName, section) : builder(sectionName, section);
        }

        /// <summary>
        /// Adds the builder.
        /// </summary>
        /// <param name="sectionName">Name of the section.</param>
        /// <param name="builder">The builder.</param>
        public void AddBuilder(string sectionName, Func<string, IEnumerable<IConfigurableValue>, ISectionView> builder)
        {
            if (builders.ContainsKey(sectionName)) return;
            builders.Add(sectionName, builder);
        }
    }
}
