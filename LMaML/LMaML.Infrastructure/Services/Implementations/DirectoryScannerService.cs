using System;
using LMaML.Infrastructure.Util;
using iLynx.Common;

namespace LMaML.Infrastructure.Services.Implementations
{
    /// <summary>
    ///     A Class that can be used to scan a directory structure for files of a specific type
    /// </summary>
    public class DirectoryScannerService<TInfo> : ComponentBase, IDirectoryScannerService<TInfo>
    {
        private readonly IAsyncFileScanner<TInfo> scanner;

        /// <summary>
        ///     This event is fired when a scan has completed
        ///     <para />
        ///     Please note that this event may be fired from a different thread than the UI or whatever thread a scan was started on
        /// </summary>
        public event EventHandler ScanCompleted;

        /// <summary>
        ///     This event is fired when a scan's progress changes
        ///     <para />
        ///     Please note that this event may be fired from a different thread than the UI or whatever thread a scan was started on
        /// </summary>
        public event Action<double> ScanProgress;

        /// <summary>
        /// Initializes a new instance of the <see cref="DirectoryScannerService{TInfo}" /> class.
        /// </summary>
        /// <param name="scanner">The scanner.</param>
        /// <param name="logger">The logger.</param>
        public DirectoryScannerService(IAsyncFileScanner<TInfo> scanner, ILogger logger)
            : base(logger)
        {
            scanner.Guard("scanner");
            this.scanner = scanner;
            scanner.Progress += OnProgress;
        }

        /// <summary>
        /// Cancels all.
        /// </summary>
        public void CancelAll()
        {
            scanner.Cancel();
        }

        /// <summary>
        ///     Attempts to scan the specified directory and all subdirectories for files of a given type <see cref="IFileFormat" />
        ///     <para />
        ///     This method will return a GUI that is the ID of the started scan
        /// </summary>
        /// <param name="root">The root directory</param>
        /// <returns></returns>
        /// <exception cref="System.InvalidOperationException">The path is null or empty, or there are no formats defined to check for</exception>
        public virtual Guid Scan(string root)
        {
            //TODO: Make this capable of multiple scans at a time.
            root.GuardString("root");
            var scanID = Guid.NewGuid();
            var args = new FileScannerArgs { Root = root };
            scanner.Execute(args, OnScanCompleted);
            return scanID;
        }

        /// <summary>
        ///     Called from a ScannerWrapper when a scan is completed (Called from a seperate thread!)
        /// </summary>
        /// <param name="args">
        ///     The <see cref="ScanCompletedEventArgs" /> that were generated
        /// </param>
        protected virtual void OnScanCompleted(ScanCompletedEventArgs args)
        {
            if (ScanCompleted != null)
                ScanCompleted(this, args);
        }

        private void RaiseProgress(double progress)
        {
            if (null == ScanProgress) return;
            ScanProgress(progress);
        }

        /// <summary>
        /// Called from a ScannerWrapper when a progress update is "available"... (Called from a seperate thread)
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="progress">The progress.</param>
        protected virtual void OnProgress(IAsyncFileScanner<TInfo> sender,
                                          double progress)
        {
            RaiseProgress(progress);
        }
    }
}