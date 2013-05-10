

namespace iLynx.Common.Configuration
{
    /// <summary>
    /// An interface that can be used to implement a simple string to {TValue} converter
    /// </summary>
    /// <typeparam name="TValue">The type of value we're dealing with</typeparam>
    public interface IValueConverter<TValue>
    {
        /// <summary>
        /// Converts the specified string value to a <typeparamref name="TValue"/> type
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        TValue FromXmlSerializable(object value);

        /// <summary>
        /// Converts the specified <typeparamref name="TValue"/> type to a string
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        object ToXmlSerializable(TValue value);
    }
}