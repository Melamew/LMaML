using System.IO;

namespace iLynx.Common.Serialization
{
    /// <summary>
    /// ITypeSerializer
    /// </summary>
    public interface ITypeSerializer
    {
        /// <summary>
        /// Serializes the specified item.
        /// </summary>
        /// <param name="item">The item.</param>
        /// <param name="target">The target.</param>
        void WriteTo(object item, Stream target);

        /// <summary>
        /// Deserializes the specified source.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <returns></returns>
        object ReadFrom(Stream source);
    }
}
