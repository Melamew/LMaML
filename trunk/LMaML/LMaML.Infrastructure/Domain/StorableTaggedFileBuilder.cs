using System.IO;
using LMaML.Infrastructure.Audio;
using LMaML.Infrastructure.Domain.Concrete;
using LMaML.Infrastructure.Util;

namespace LMaML.Infrastructure.Domain
{
    /// <summary>
    /// StorableID3FileBuilder
    /// </summary>
    public class StorableTaggedFileBuilder : IInfoBuilder<StorableTaggedFile>
    {
        private readonly IInfoBuilder<ID3File> sourceBuilder;

        /// <summary>
        /// Initializes a new instance of the <see cref="StorableTaggedFileBuilder" /> class.
        /// </summary>
        /// <param name="sourceBuilder">The source builder.</param>
        public StorableTaggedFileBuilder(IInfoBuilder<ID3File> sourceBuilder)
        {
            this.sourceBuilder = sourceBuilder;
        }

        /// <summary>
        /// Builds the specified info.
        /// </summary>
        /// <param name="info">The info.</param>
        /// <param name="valid">if set to <c>true</c> [valid].</param>
        /// <returns></returns>
        public StorableTaggedFile Build(FileInfo info, out bool valid)
        {
            var result = sourceBuilder.Build(info, out valid);
            return valid ? StorableTaggedFile.Copy(result) : null;
        }
    }
}
