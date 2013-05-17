using System;

namespace LMaML.Infrastructure.Util
{
    /// <summary>
    ///     Simple event args that contains a list of files that have been scanned and found to be a match
    /// </summary>
    public class ScanCompletedEventArgs : EventArgs
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ScanCompletedEventArgs" /> class.
        /// </summary>
        /// <param name="id">The id.</param>
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