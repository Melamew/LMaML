using LMaML.Infrastructure.Domain.Concrete;
using LMaML.Infrastructure.Services.Implementations;
using LMaML.Infrastructure.Util;
using iLynx.Common;

namespace LMaML.Infrastructure.Domain
{
    /// <summary>
    /// TaggedFileScanner
    /// </summary>
    public class TaggedFileScannerProxy : DirectoryScannerService<StorableTaggedFile>, IDirectoryScannerService<StorableTaggedFile>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TaggedFileScannerProxy" /> class.
        /// </summary>
        /// <param name="scanner">The scanner.</param>
        /// <param name="logger">The logger.</param>
        public TaggedFileScannerProxy(IAsyncFileScanner<StorableTaggedFile> scanner, ILogger logger)
            : base(scanner, logger)
        {
        }
    }
}
