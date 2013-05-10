using System;
using System.ComponentModel;
using System.Linq.Expressions;

namespace iLynx.Common
{
    /// <summary>
    /// NotificationBase
    /// </summary>
    public abstract class NotificationBase : ComponentBase, INotifyPropertyChanged
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="NotificationBase" /> class.
        /// </summary>
        /// <param name="logger">The logger.</param>
        protected NotificationBase(ILogger logger)
            : base(logger)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="NotificationBase" /> class.
        /// </summary>
        protected NotificationBase() : base(RuntimeCommon.DefaultLogger) { }

        /// <summary>
        /// Occurs when [property changed].
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Raises the property changed.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="propertyExpression">The property expression.</param>
        protected virtual void RaisePropertyChanged<T>(Expression<Func<T>> propertyExpression)
        {
            OnPropertyChanged(RuntimeHelper.GetPropertyName(propertyExpression));
        }

        /// <summary>
        /// Called when [property changed].
        /// </summary>
        /// <param name="propertyName">Name of the property.</param>
        protected virtual void OnPropertyChanged(string propertyName)
        {
            if (null != PropertyChanged)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
