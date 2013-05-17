using System.Collections.Generic;

namespace iLynx.Common.Configuration
{
    public interface IConfigurationManager
    {
        /// <summary>
        /// Gets the value.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key">The key.</param>
        /// <param name="defaultValue">The default value.</param>
        /// <param name="category">The category.</param>
        /// <returns></returns>
        /// <exception cref="System.InvalidCastException">A value with the specified key does exist, it is however not of the correct type</exception>
        IConfigurableValue<T> GetValue<T>(string key, T defaultValue, string category = null);

        /// <summary>
        /// Gets all the configurable values in the specified category, category is null or empty, will return ALL values.
        /// </summary>
        /// <param name="category">The category.</param>
        /// <returns></returns>
        IEnumerable<IConfigurableValue> GetLoadedValues(string category = null);

        /// <summary>
        /// Gets the categories.
        /// </summary>
        /// <returns></returns>
        IEnumerable<string> GetCategories();
    }
}