using System;
using System.Security.AccessControl;
using iLynx.Common;
using iLynx.Configuration;

namespace LMaML.Infrastructure
{
    public abstract class SettingsValueViewModelBase : NotificationBase
    {
        private readonly IConfigurableValue value;
        private bool autoSave;

        protected SettingsValueViewModelBase(IConfigurableValue value, bool autoSave = true)
        {
            this.value = Guard.IsNull(() => value);
            this.value.ValueChanged += ValueOnValueChanged;
            this.autoSave = autoSave;
        }

        private void ValueOnValueChanged(object sender, ValueChangedEventArgs<object> valueChangedEventArgs)
        {
            OnValueChanged(valueChangedEventArgs);
        }

        protected virtual void OnValueChanged(ValueChangedEventArgs<object> e)
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
                if (autoSave)
                    this.value.Store();
                OnPropertyChanged();
            }
        }

        protected virtual void Save()
        {
            value.Store();
        }
    }

    public class SettingsValueViewModelBase<T> : SettingsValueViewModelBase
    {
        public SettingsValueViewModelBase(IConfigurableValue value, bool autoSave = true)
            : base(value, autoSave)
        {
            var val = value as IConfigurableValue<T>;
            if (null == val) throw new ArgumentException();
            val.ValueChanged += ValOnValueChanged;
        }

        private void ValOnValueChanged(object sender, ValueChangedEventArgs<T> valueChangedEventArgs)
        {
            OnValueChanged(valueChangedEventArgs);
        }

        protected virtual void OnValueChanged(ValueChangedEventArgs<T> e)
        {
            
        }

        public new T Value
        {
            get { return (T)base.Value; }
            set { base.Value = value; }
        }
    }
}