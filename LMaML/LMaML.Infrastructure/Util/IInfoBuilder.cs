using System.IO;

namespace LMaML.Infrastructure.Util
{
    /// <summary>
    /// IInfoBuilder
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface IInfoBuilder<out T>
    {
        /// <summary>
        /// Builds the specified info.
        /// </summary>
        /// <param name="info">The info.</param>
        /// <param name="valid">if set to <c>true</c> [valid].</param>
        /// <returns></returns>
        T Build(FileInfo info, out bool valid);
    }
}