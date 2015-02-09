using System;
using System.Collections.Generic;
using iLynx.Common;
using iLynx.Configuration;
using LMaML.Infrastructure;
using LMaML.Infrastructure.Services;
using LMaML.Infrastructure.Services.Interfaces;
using LMaML.Settings.ViewModels;

namespace LMaML.Settings
{
    public class ValueEditorViewFactory : IValueEditorViewFactory
    {
        private readonly Dictionary<Type, Func<IConfigurableValue, object>> typeTable = new Dictionary<Type, Func<IConfigurableValue, object>>();

        public ValueEditorViewFactory()
        {
            //typeTable.Add(typeof(string), value => );
        }

        public object CreateFor(IConfigurableValue value)
        {
            var type = value.Value.GetType();
            Func<IConfigurableValue, object> builder;
            return !typeTable.TryGetValue(type, out builder) ? MakeWrapper(value) : builder(value);
        }

        private object MakeWrapper(IConfigurableValue value)
        {
            var wrapperType = typeof(ValueWrapper<>);
            wrapperType = wrapperType.MakeGenericType(value.Value.GetType());
            return Activator.CreateInstance(wrapperType, value);
        }
    }

    public class SectionViewFactory : ISectionViewFactory
    {
        private readonly IValueEditorViewFactory viewFactory;

        private readonly Dictionary<string, Func<string, IEnumerable<IConfigurableValue>, IValueEditorViewFactory, ISectionView>> builders =
            new Dictionary<string, Func<string, IEnumerable<IConfigurableValue>, IValueEditorViewFactory, ISectionView>>();

        public SectionViewFactory(IValueEditorViewFactory viewFactory)
        {
            this.viewFactory = Guard.IsNull(() => viewFactory);
        }

        /// <summary>
        /// Builds the default.
        /// </summary>
        /// <param name="sectionName">Name of the section.</param>
        /// <param name="section">The section.</param>
        /// <param name="viewFactory"></param>
        /// <returns></returns>
        private static ISectionView BuildDefault(string sectionName,
            IEnumerable<IConfigurableValue> section,
            IValueEditorViewFactory viewFactory)
        {
            return new DefaultConfigSectionViewModel(sectionName, section, viewFactory);
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
            Func<string, IEnumerable<IConfigurableValue>, IValueEditorViewFactory, ISectionView> builder;
            return !builders.TryGetValue(sectionName, out builder) ? BuildDefault(sectionName, section, viewFactory) : builder(sectionName, section, viewFactory);
        }

        /// <summary>
        /// Adds the builder.
        /// </summary>
        /// <param name="sectionName">Name of the section.</param>
        /// <param name="builder">The builder.</param>
        public void AddBuilder(string sectionName, Func<string, IEnumerable<IConfigurableValue>, IValueEditorViewFactory, ISectionView> builder)
        {
            if (builders.ContainsKey(sectionName)) return;
            builders.Add(sectionName, builder);
        }
    }
}
