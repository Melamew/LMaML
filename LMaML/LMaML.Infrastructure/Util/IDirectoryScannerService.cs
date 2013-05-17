using System;

namespace LMaML.Infrastructure.Util
{
    /// <summary>
    /// IDirectoryScannerService
    /// </summary>
    /// <typeparam name="TInfo">The type of the info.</typeparam>
    public interface IDirectoryScannerService<TInfo>
    {
        /// <summary>
        ///     This event is fired when a scan has completed
        ///     <para />
        ///     Please note that this event may be fired from a different thread than the UI or whatever thread a scan was started on
        /// </summary>
        event EventHandler ScanCompleted;

        /// <summary>
        ///     This event is fired when a scan's progress changes
        ///     <para />
        ///     Please note that this event may be fired from a different thread than the UI or whatever thread a scan was started on
        /// </summary>
        event Action<double> ScanProgress;

        /// <summary>
        ///     Attempts to scan the specified directory and all subdirectories for files of a given type <see cref="IFileFormat" />
        ///     <para />
        ///     This method will return a GUI that is the ID of the started scan
        /// </summary>
        /// <param name="root">The root directory</param>
        /// <returns></returns>
        /// <exception cref="System.InvalidOperationException">The path is null or empty, or there are no formats defined to check for</exception>
        Guid Scan(string root);

        /// <summary>
        /// Cancels all.
        /// </summary>
        void CancelAll();
    }
}