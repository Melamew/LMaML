using System;
using iLynx.Common;
using iLynx.Configuration;

namespace LMaML.Settings.ViewModels
{
    public class ValueWrapper : NotificationBase
    {
        private readonly IConfigurableValue value;

        public ValueWrapper(IConfigurableValue value)
        {
            this.value = Guard.IsNull(() => value);
            this.value.ValueChanged += ValueOnValueChanged;
        }

        private void ValueOnValueChanged(object sender, ValueChangedEventArgs<object> valueChangedEventArgs)
        {
            RaisePropertyChanged(() => Value);
        }

        /// <summary>
        /// Gets the value.
        /// </summary>
        /// <value>
        /// The value.
        /// </value>
        public object Value
        {
            get { return value.Value; }
            set
            {
                if (value.GetType() != this.value.Value.GetType())
                    throw new InvalidCastException();
                this.value.Value = value;
                this.value.Store();
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Gets the name.
        /// </summary>
        /// <value>
        /// The name.
        /// </value>
        public string Name
        {
            get { return value.Key; }
        }
    }

    /// <summary>
    /// ValueWrapper
    /// </summary>
    public class ValueWrapper<T> : ValueWrapper
    {
        public ValueWrapper(IConfigurableValue value)
            : base(value)
        {
        }

        /// <summary>
        /// Gets the value.
        /// </summary>
        /// <value>
        /// The value.
        /// </value>
        public new T Value
        {
            get { return (T)base.Value; }
            set { base.Value = value; }
        }
    }
}
