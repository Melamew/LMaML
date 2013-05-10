using System;

namespace iLynx.Common
{
    /// <summary>
    /// This class can be used contain value changed properties
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public sealed class ValueChangedEventArgs<T> : EventArgs
    {
        /// <summary>
        /// Initializes a new instance of <see cref="ValueChangedEventArgs{T}"/> and sets it's properties to the specified values
        /// </summary>
        /// <param name="oldValue">The old value (<see cref="OldValue"/>)</param>
        /// <param name="newValue">The new value (<see cref="NewValue"/>)</param>
        public ValueChangedEventArgs(T oldValue, T newValue)
        {
            OldValue = oldValue;
            NewValue = newValue;
        }

        /// <summary>
        /// Gets the old value before the change occured
        /// </summary>
        public T OldValue { get; private set; }

        /// <summary>
        /// Gets the new value (after the change)
        /// </summary>
        public T NewValue { get; private set; }
    }
}