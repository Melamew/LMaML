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
        private readonly Dictionary<Type, Func<IConfigurableValue, SettingsValueViewModelBase>> typeTable = new Dictionary<Type, Func<IConfigurableValue, SettingsValueViewModelBase>>();

        public void RegisterBuilder(Type valueType, Func<IConfigurableValue, SettingsValueViewModelBase> callback)
        {
            if (typeTable.ContainsKey(valueType))
                typeTable[valueType] = callback;
            else
                typeTable.Add(valueType, callback);
        }

        public object CreateFor(IConfigurableValue value)
        {
            var type = value.Value.GetType();
            Func<IConfigurableValue, SettingsValueViewModelBase> builder;
            return !typeTable.TryGetValue(type, out builder) ? MakeWrapper(value, MakeDefaultView) : MakeWrapper(value, builder);
        }

        private static SettingsValueViewModelBase MakeDefaultView(IConfigurableValue value)
        {
            var displayType = typeof (SettingsValueViewModelBase<>);
            displayType = displayType.MakeGenericType(value.Value.GetType());
            return (SettingsValueViewModelBase) Activator.CreateInstance(displayType, value, true);
        }

        private static object MakeWrapper(IConfigurableValue value, Func<IConfigurableValue, SettingsValueViewModelBase> viewBuilder)
        {
            return new SettingsValueDisplayViewModel(value, viewBuilder);
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
