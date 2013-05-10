using System;
using System.Collections.Generic;
using System.IO;

namespace LMaML.Infrastructure.Util
{
    /// <summary>
    ///     Simple event args that contains a list of files that have been scanned and found to be a match
    /// </summary>
    public class ScanCompletedEventArgs<TInfo> : EventArgs
    {
        /// <summary>
        ///     Default constructor, the <see cref="Files" /> property is created from the specified
        ///     <see
        ///         cref="IEnumerable{FileInfo}" />
        /// </summary>
        /// <param name="infos"></param>
        /// <param name="id"></param>
        public ScanCompletedEventArgs(Guid id)
        {
            Id = id;
        }

        /// <summary>
        ///     Gets a value indicating the ID of the worker that has completed
        /// </summary>
        public Guid Id { get; private set; }
    }
}