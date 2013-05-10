using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.Linq;
using iLynx.Common.Serialization;

namespace iLynx.Common.Configuration
{
    public class BinaryConfigSection
    {
        public const string DefaultCategory = "Default";

        private readonly Dictionary<string, Dictionary<string, ConfigurableValue>> categories = new Dictionary<string, Dictionary<string, ConfigurableValue>>();

        public Dictionary<string, Dictionary<string, ConfigurableValue>> Categories
        {
            get { return categories; }
        }

        /// <summary>
        /// Gets all as.
        /// </summary>
        /// <returns></returns>
        public IEnumerable<IConfigurableValue> GetAll(string category = null)
        {
            return Categories.Where(x => null == category || x.Key == category).SelectMany(c => c.Value.Select(x => x.Value));
        }

        /// <summary>
        /// Reads XML from the configuration file.
        /// </summary>
        /// <param name="stream">The stream.</param>
        public void ReadFrom(Stream stream)
        {
            var serializer = new NaiveSerializer<ValuesContainer>(RuntimeCommon.DefaultLogger);
            while (stream.Position != stream.Length)
            {
                var container = serializer.Deserialize(stream);
                var category = container.Category;
                Dictionary<string, ConfigurableValue> existing;
                if (!categories.TryGetValue(category, out existing))
                    categories.Add(category, (existing = new Dictionary<string, ConfigurableValue>()));
                foreach (var val in container.Values)
                {
                    if (existing.ContainsKey(val.Key))
                        existing[val.Key] = val;
                    else
                        existing.Add(val.Key, val);
                }
            }
        }

        /// <summary>
        /// ValuesContainer
        /// </summary>
        public class ValuesContainer
        {
            /// <summary>
            /// Gets or sets the category.
            /// </summary>
            /// <value>
            /// The category.
            /// </value>
            public string Category { get; set; }

            private ConfigurableValue[] values = new ConfigurableValue[0];

            /// <summary>
            /// Gets or sets the values.
            /// </summary>
            /// <value>
            /// The values.
            /// </value>
            public ConfigurableValue[] Values { get { return values; } set { values = value; } }
        }

        /// <summary>
        /// Writes the outer tags of this configuration element to the configuration file when implemented in a derived class.
        /// </summary>
        /// <returns>
        /// true if writing was successful; otherwise, false.
        /// </returns>
        public void WriteTo(Stream target)
        {
            try
            {
                var serializer = new NaiveSerializer<ValuesContainer>(RuntimeCommon.DefaultLogger);
                foreach (var container in Categories.Select(category => new ValuesContainer
                                                            {
                                                                Category = category.Key,
                                                                Values = category.Value.Values.ToArray()
                                                            }))
                    serializer.Serialize(container, target);
            }
            catch (Exception e)
            {
                Trace.WriteLine(e);
            }
        }

        private Dictionary<string, ConfigurableValue> GetCategory(string cat)
        {
            Dictionary<string, ConfigurableValue> result;
            return Categories.TryGetValue(cat, out result) ? result : null;
        }

        /// <summary>
        /// Gets or sets a property, attribute, or child element of this configuration element.
        /// </summary>
        /// <param name="category">The category.</param>
        /// <param name="key">The key.</param>
        /// <returns></returns>
        public ConfigurableValue this[string category, string key]
        {
            get
            {
                if (null == key) return null;
                if (null == category)
                    category = DefaultCategory;
                var cat = GetCategory(category);
                if (null == cat) return null;
                ConfigurableValue value;
                return cat.TryGetValue(key, out value) ? value : null;
            }
            set
            {
                if (null == key) return;
                if (null == category)
                {
                    category = DefaultCategory;
                    value.Category = category;
                }
                var cat = GetCategory(category);
                if (null == cat)
                {
                    cat = new Dictionary<string, ConfigurableValue>();
                    Categories.Add(category, cat);
                }
                if (cat.ContainsKey(key))
                    cat[key] = value;
                else
                    cat.Add(key, value);
            }
        }

        /// <summary>
        /// Determines whether [contains] [the specified category].
        /// </summary>
        /// <param name="category">The category.</param>
        /// <param name="key">The key.</param>
        /// <returns>
        ///   <c>true</c> if [contains] [the specified category]; otherwise, <c>false</c>.
        /// </returns>
        public bool Contains(string category, string key)
        {
            key.GuardString("key");
            if (null == category) category = DefaultCategory;
            Dictionary<string, ConfigurableValue> sub;
            return Categories.TryGetValue(category, out sub) && sub.ContainsKey(key);
        }

        /// <summary>
        /// Gets the cagerories.
        /// </summary>
        /// <returns></returns>
        public IEnumerable<string> GetCagerories()
        {
            return Categories.Keys;
        }
    }

    /// <summary>
    /// ConfigurableValue
    /// </summary>
    public class ConfigurableValue : IConfigurableValue
    {
        private object value;
        private string category;

        public ConfigurableValue() { }

        /// <summary>
        /// Initializes a new instance of <see cref="ExeConfigValue{T}" /> and attempts to load it's value from the
        /// <para />
        /// configuration manager (using the specified <see cref="IValueConverter{TValue}" /> to convert the retrieved string to a
        /// type
        /// </summary>
        /// <param name="key">The key of the value to look for</param>
        /// <param name="defaultValue">The default value to use if the value could not be retrieved from the configuration file</param>
        /// <param name="category">The category.</param>
        public ConfigurableValue(string key, object defaultValue, string category = null)
        {
            Key = key;
            this.category = category;
            try
            {
                Load(category, key, defaultValue);
            }
            catch
            {
                value = defaultValue;
                Store();
            }
        }

        /// <summary>
        /// Loads the specified category.
        /// </summary>
        /// <param name="cat">The category.</param>
        /// <param name="key">The key.</param>
        /// <param name="defaultValue">The default value.</param>
        private void Load(string cat, string key, object defaultValue)
        {
            if (!ExeConfig.ConfigurableValuesSection.Contains(cat, key))
            {
                value = defaultValue;
                Store();
            }
            else
            {
                var val = ExeConfig.ConfigurableValuesSection[cat, key];
                if (null == val)
                {
                    Store();
                    return;
                }
                value = val.Value;
                category = val.category;
            }
        }

        /// <summary>
        /// Gets the key of this <see cref="IConfigurableValue" />
        /// </summary>
        public string Key { get; set; }

        /// <summary>
        ///     Gets or Sets the value of this <see cref="ExeConfigValue{T}" />
        /// </summary>
        public object Value
        {
            get { return value; }
            set
            {
                this.value = value;
                OnValueChanged();
            }
        }

        /// <summary>
        ///     Gets a value indicating whether or not the value of this <see cref="ExeConfigValue{T}" /> has been saved to disk
        /// </summary>
        public bool IsStored
        {
            get
            {
                var exists = ExeConfig.ConfigurableValuesSection.Contains(category, Key);
                if (!exists)
                    return false;
                var stored = ExeConfig.ConfigurableValuesSection[category, Key];
                return stored != null && stored.Value == Value;
            }
        }

        /// <summary>
        ///     Attempts to store this value in the configuration file
        /// </summary>
        public void Store()
        {
            try
            {
                ExeConfig.ConfigurableValuesSection[category, Key] = this;
                ExeConfig.Save();
            }
            catch (Exception e)
            {
                Trace.WriteLine(string.Format("Unable to store current value: {0}", e));
            }
        }

        /// <summary>
        /// Gets or sets the category.
        /// </summary>
        /// <value>
        /// The category.
        /// </value>
        public string Category
        {
            get { return category; }
            set { category = value; }
        }

        /// <summary>
        /// Returns a <see cref="System.String" /> that represents this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="System.String" /> that represents this instance.
        /// </returns>
        public override string ToString()
        {
            try { return value.ToString(); }
            catch
            {
                return base.ToString();
            }
        }

        /// <summary>
        /// Called when [value changed].
        /// </summary>
        private void OnValueChanged()
        {
            if (ValueChanged != null)
                ValueChanged(this, new EventArgs());
        }

        /// <summary>
        ///     Fired whenever the value part of this <see cref="ExeConfigValue{T}" /> has changed
        /// </summary>
        public event EventHandler ValueChanged;
    }

    /// <summary>
    ///     A simple configurable value (Uses the builtin ConfigurationManager (
    ///     <see
    ///         cref="System.Configuration.ConfigurationManager" />
    ///     ))
    /// </summary>
    /// <typeparam name="T">The type of value to store</typeparam>
    public class ExeConfigValue<T> : ConfigurableValue, IConfigurableValue<T>
    {
        public ExeConfigValue()
        {
        }

        public ExeConfigValue(string key, T defaultValue, string category = null)
            : base(key, defaultValue, category)
        {
        }

        /// <summary>
        ///     Simply retrieves the value of the specified <see cref="ExeConfigValue{T}" />
        /// </summary>
        /// <param name="val"></param>
        /// <returns></returns>
        public static implicit operator T(ExeConfigValue<T> val)
        {
            return val.Value;
        }

        /// <summary>
        ///     Simply returns <see cref="ExeConfigValue{T}.ToString()" />
        /// </summary>
        /// <param name="val"></param>
        /// <returns></returns>
        public static explicit operator string(ExeConfigValue<T> val)
        {
            return val.ToString();
        }

        /// <summary>
        /// Gets or Sets the value of this <see cref="ExeConfigValue{T}" />
        /// </summary>
        public new T Value
        {
            get { return (T)base.Value; }
            set
            {
                if (Equals(base.Value, value)) return;
                var old = Value;
                base.Value = value;
                OnValueChanged(old, value);
            }
        }

        /// <summary>
        /// Called when [value changed].
        /// </summary>
        /// <param name="oldValue">The old value.</param>
        /// <param name="newValue">The new value.</param>
        private void OnValueChanged(T oldValue, T newValue)
        {
            if (null == ValueChanged) return;
            ValueChanged(this, new ValueChangedEventArgs<T>(oldValue, newValue));
        }

        /// <summary>
        /// Occurs when [value changed].
        /// </summary>
        public new event EventHandler<ValueChangedEventArgs<T>> ValueChanged;
    }
}