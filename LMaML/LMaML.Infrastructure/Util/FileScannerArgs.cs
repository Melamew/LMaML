using System;

namespace LMaML.Infrastructure.Util
{
    /// <summary>
    ///     Simply a container for arguments passed to a <see cref="RecursiveAsyncFileScanner{TInfo}" />s Execute Method
    /// </summary>
    public struct FileScannerArgs
    {
        /// <summary>
        ///     The root directory at which to start scanning
        /// </summary>
        public string Root;

        /// <summary>
        /// Gets or sets the id.
        /// </summary>
        /// <value>
        /// The id.
        /// </value>
        public Guid Id { get; set; }
    }
}