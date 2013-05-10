using iLynx.Common;

namespace LMaML.Library.ViewModels
{
    /// <summary>
    /// Alias{T}
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class Alias<T>
    {
        /// <summary>
        /// Gets or sets the original.
        /// </summary>
        /// <value>
        /// The original.
        /// </value>
        public T Original { get; private set; }
        /// <summary>
        /// Gets or sets the value.
        /// </summary>
        /// <value>
        /// The value.
        /// </value>
        public T Value { get; private set; }

        public Alias(T original, T value)
        {
            original.Guard( "original");
            value.Guard("value");
            Original = original;
            Value = value;
        } 

        /// <summary>
        /// Returns a <see cref="System.String" /> that represents this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="System.String" /> that represents this instance.
        /// </returns>
        public override string ToString()
        {
            return Equals(null, Value) ? base.ToString() : Value.ToString();
        }

        /// <summary>
        /// Returns a hash code for this instance.
        /// </summary>
        /// <returns>
        /// A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table. 
        /// </returns>
        public override int GetHashCode()
        {
            return Equals(null, Original) ? base.GetHashCode() : Original.GetHashCode();
        }
    }
}