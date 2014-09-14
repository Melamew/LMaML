using System;
using System.Collections.Generic;
using System.Linq;
using iLynx.Common.Configuration;

namespace LMaML.Infrastructure.Services.Implementations
{
    /// <summary>
    /// SingletoneConfigurationManager
    /// </summary>
    public class SingletonConfigurationManager : IConfigurationManager
    {
        private static readonly Dictionary<string, ValueContainer> OpenValues = new Dictionary<string, ValueContainer>();

        /// <summary>
        /// ValueContainer
        /// </summary>
        private class ValueContainer
        {
            /// <summary>
            /// Gets or sets the value.
            /// </summary>
            /// <value>
            /// The value.
            /// </value>
            public IConfigurableValue Value { get; set; }
            
            /// <summary>
            /// Gets or sets the type of the value.
            /// </summary>
            /// <value>
            /// The type of the value.
            /// </value>
            public Type ValueType { get; set; }
        }

        /// <summary>
        /// Gets the value.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key">The key.</param>
        /// <param name="defaultValue">The default value.</param>
        /// <param name="category">The category.</param>
        /// <returns></returns>
        /// <exception cref="System.InvalidCastException">A value with the specified key does exist, it is however not of the correct type</exception>
        public IConfigurableValue<T> GetValue<T>(string key, T defaultValue, string category = null)
        {
            ValueContainer container;
            if (!OpenValues.TryGetValue(GetKey(key, category), out container))
            {
                container = new ValueContainer
                                {
                                    Value = new ExeConfigValue<T>(key, defaultValue, category),
                                    ValueType = typeof (T)
                                };
                OpenValues.Add(GetKey(key, category), container);
            }
            if (container.ValueType != typeof(T)) throw new InvalidCastException("A value with the specified key does exist, it is however not of the correct type");
            return (IConfigurableValue<T>)container.Value;
        }

        private string GetKey(string value,
                              string category)
        {
            return (category ?? "") + value;
        }

        /// <summary>
        /// Gets all the (Loaded!) configurable values in the specified category, category is null or empty, will return ALL values.
        /// </summary>
        /// <param name="category">The category.</param>
        /// <returns></returns>
        public IEnumerable<IConfigurableValue> GetLoadedValues(string category = null)
        {
            return OpenValues.Values.Where(x => null == category || x.Value.Category == category).Select(x => x.Value);
        }

        /// <summary>
        /// Gets the categories.
        /// </summary>
        /// <returns></returns>
        public IEnumerable<string> GetCategories()
        {
            return ExeConfig.ConfigurableValuesSection.GetCagerories();
        } 
    }
}