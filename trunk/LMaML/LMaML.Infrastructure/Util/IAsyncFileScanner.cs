using System;
using iLynx.Common;

namespace LMaML.Infrastructure.Util
{
    /// <summary>
    /// IAsyncFileScanner
    /// </summary>
    /// <typeparam name="TInfo">The type of the info.</typeparam>
    public interface IAsyncFileScanner<TInfo>
    {
        /// <summary>
        /// This event is fired when the underlying worker has progress in the form percent to report
        /// </summary>
        event GenericEventHandler<IAsyncFileScanner<TInfo>, double> Progress;

        /// <summary>
        /// This event is fired when the underlying worker has progress in the form of a message to report
        /// </summary>
        event GenericEventHandler<IAsyncFileScanner<TInfo>, string> Status;

        /// <summary>
        ///     This event is fired when the worker has completed it's work
        /// </summary>
        event GenericEventHandler<IAsyncFileScanner<TInfo>, ScanCompletedEventArgs<TInfo>> WorkCompleted;

        /// <summary>
        ///     This event is fired when the worker has started working
        /// </summary>
        event GenericEventHandler<IAsyncFileScanner<TInfo>> WorkStarted;

        /// <summary>
        ///     This event is fired if any exceptions are caught during execution
        /// </summary>
        event GenericEventHandler<IAsyncFileScanner<TInfo>, Exception> WorkFailed;

        /// <summary>
        ///     This event is fired when the worker is aborted
        /// </summary>
        event GenericEventHandler<IAsyncFileScanner<TInfo>> WorkAborted;

        /// <summary>
        ///     Executes this worker
        /// </summary>
        /// <param name="args">The arguments that are passed on to the thread</param>
        void Execute(FileScannerArgs args);

        /// <summary>
        ///     Executes this worker and calls the <paramref name="completedCallback" /> method when it completes
        /// </summary>
        /// <param name="args">The argument to pass to the worker</param>
        /// <param name="completedCallback">
        ///     The <see cref="Action{TCompletedArgs}" /> to call when work is complete
        /// </param>
        void Execute(FileScannerArgs args, Action<ScanCompletedEventArgs<TInfo>> completedCallback);

        /// <summary>
        ///     Aborts the current worker thread
        /// </summary>
        void Abort();

        /// <summary>
        /// Cancels this instance.
        /// </summary>
        void Cancel();
    }
}