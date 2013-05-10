using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace iLynx.Common.Serialization.Old
{
    /// <summary>
    ///     A simple implementation of <see cref="IDataSerializer{T}" />
    ///     <para />
    ///     Please note that this serializer will only serialize Types that have properties marked with
    ///     <see
    ///         cref="DataPropertyAttribute" />
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class DataSerializer<T> : IDataSerializer<T> where T : new()
    {
        private readonly Type handledType = typeof (T);
        private readonly Dictionary<string, PropertyInfo> serializedFields = new Dictionary<string, PropertyInfo>();
        private readonly Dictionary<string, IDataProperty<T>> template = new Dictionary<string, IDataProperty<T>>();

        /// <summary>
        ///     Initializes a new instance of <see cref="DataSerializer{T}" /> and builds the internal format table
        /// </summary>
        public DataSerializer()
        {
            BuildFormat();
        }

        /// <summary>
        ///     Serializes the specified value using the internal format table
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public IEnumerable<IDataProperty<T>> Serialize(T value)
        {
            return (from kvp in serializedFields let name = kvp.Key let pInfo = kvp.Value select new DataProperty<T>(template[name]) {Value = pInfo.GetValue(value, null)}).Cast<IDataProperty<T>>().ToList();
        }

        /// <summary>
        ///     Deserializes the speicifed collection of properties in to a new instance of {T} with it's properties set to the specified values
        /// </summary>
        /// <param name="fields"></param>
        /// <returns></returns>
        public T Deserialize(IEnumerable<IDataProperty<T>> fields)
        {
            var instance = new T();
            foreach (var df in fields)
                serializedFields[df.PropertyName].SetValue(instance, df.Value, null);

            return instance;
        }

        /// <summary>
        ///     When implemented in a derrived class, returns a collection of <see cref="DataProperty{T}" /> objects that can be used to search a
        ///     <see
        ///         cref="IDataAdapter{T}" />
        ///     <para />
        ///     - Any properties who's name is contained in <paramref name="excludeProperties" /> will not be returned in the filter array
        /// </summary>
        /// <param name="val"></param>
        /// <param name="excludeProperties"></param>
        /// <returns></returns>
        public IDataProperty<T>[] GetUsableFilter(T val, params string[] excludeProperties)
        {
            if (excludeProperties == null)
                excludeProperties = new string[0];

            return (from kvp in serializedFields where !excludeProperties.Contains(kvp.Key) let propertyValue = kvp.Value.GetValue(val, null) where propertyValue != GetDefaultValue(kvp.Value.PropertyType) select new DataProperty<T>(kvp.Key, propertyValue, kvp.Value.PropertyType)).Cast<IDataProperty<T>>().ToArray();
        }

        /// <summary>
        ///     Gets the template used when serializing and deserializing data using this <see cref="DataSerializer{T}" />
        /// </summary>
        /// <returns></returns>
        public IDictionary<string, IDataProperty<T>> GetDataTemplate()
        {
            return new Dictionary<string, IDataProperty<T>>(template);
        }

        /// <summary>
        ///     ...
        /// </summary>
        private void BuildFormat()
        {
            var properties = handledType.GetProperties();
            foreach (var pInfo in properties.Where(pInfo => Attribute.IsDefined(pInfo, typeof (DataPropertyAttribute)) && pInfo.CanRead && pInfo.CanWrite))
            {
                serializedFields.Add(pInfo.Name, pInfo);
                template.Add(pInfo.Name, new DataProperty<T>(pInfo.Name, null, pInfo.PropertyType));
            }
        }

        /// <summary>
        ///     Gets the default value for the specified type <paramref name="t" />
        /// </summary>
        /// <param name="t"></param>
        /// <returns></returns>
        public static object GetDefaultValue(Type t)
        {
            return t.IsValueType ? Activator.CreateInstance(t) : null;
        }
    }
}