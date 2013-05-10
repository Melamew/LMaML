using System.IO;

namespace LMaML.Infrastructure.Util
{
    /// <summary>
    /// This is basically a raw builder, to be used when you just want a list of all files in a given directory.
    /// </summary>
    public class FileInfoBuilder : IInfoBuilder<FileInfo>
    {
        /// <summary>
        /// Builds the specified info.
        /// </summary>
        /// <param name="info">The info.</param>
        /// <param name="valid">if set to <c>true</c> [valid].</param>
        /// <returns></returns>
        public FileInfo Build(FileInfo info, out bool valid)
        {
            valid = null != info;
            return info;
        }
    }
}
