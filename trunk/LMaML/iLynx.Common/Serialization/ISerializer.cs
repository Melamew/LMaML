using System.IO;

namespace iLynx.Common.Serialization
{
    /// <summary>
    /// ISerializer
    /// </summary>
    public interface ISerializer<T>
    {
        /// <summary>
        /// Serializes the specified item.
        /// </summary>
        /// <param name="item">The item.</param>
        /// <param name="target">The target.</param>
        void Serialize(T item, Stream target);

        /// <summary>
        /// Deserializes the specified source.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <returns></returns>
        T Deserialize(Stream source);
    }
}
