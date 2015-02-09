using System;
using iLynx.Common;
using iLynx.Configuration;

namespace LMaML.Infrastructure
{
    public abstract class SettingsValueViewModelBase : NotificationBase
    {
        private readonly IConfigurableValue value;

        protected SettingsValueViewModelBase(IConfigurableValue value)
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
    }

    public class SettingsValueViewModelBase<T> : SettingsValueViewModelBase
    {
        public SettingsValueViewModelBase(IConfigurableValue value)
            : base(value)
        {
        }

        public new T Value
        {
            get { return (T)base.Value; }
            set { base.Value = value; }
        }
    }
}