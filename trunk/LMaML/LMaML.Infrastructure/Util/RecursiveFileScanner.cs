using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using LMaML.Infrastructure.Events;
using LMaML.Infrastructure.Services.Interfaces;
using iLynx.Common.Configuration;
using iLynx.Common;
using iLynx.Common.Threading;
using iLynx.Common.Threading.Unmanaged;

namespace LMaML.Infrastructure.Util
{
    /// <summary>
    ///     A <see cref="ThreadedResultWorker{TArgs,TCompletedArgs}" /> implementation that can be used to recursively scan for files of a specific type
    /// </summary>
    // TODO: Make this prettier
    public class RecursiveAsyncFileScanner<TInfo> : ProgressResultWorker<FileScannerArgs, ScanCompletedEventArgs>, IAsyncFileScanner<TInfo>
    {
        private readonly IInfoBuilder<TInfo> infoBuilder;
        private readonly IDataPersister<TInfo> storageAdapter;
        private readonly IThreadManager threadManager;
        private readonly IPublicTransport publicTransport;
        private readonly IConfigurableValue<int> pageSize;
        private readonly IConfigurableValue<bool> scanPaged;
        private readonly AutoResetEvent blockade = new AutoResetEvent(false);
        private readonly ConcurrentQueue<TInfo> infos = new ConcurrentQueue<TInfo>();
        private volatile int totalFiles;

        /// <summary>
        /// calls <see cref="ThreadedResultWorker{TArgs,TCompletedArgs}" />
        /// </summary>
        /// <param name="infoBuilder">The info builder.</param>
        /// <param name="storageAdapter">The storage adapter.</param>
        /// <param name="logger">The logger.</param>
        /// <param name="configurationManager">The configuration manager.</param>
        /// <param name="threadManager">The thread manager service.</param>
        /// <param name="publicTransport">The public transport.</param>
        public RecursiveAsyncFileScanner(IInfoBuilder<TInfo> infoBuilder,
                                         IDataPersister<TInfo> storageAdapter,
                                         IConfigurationManager configurationManager,
                                         IThreadManager threadManager,
                                         IPublicTransport publicTransport,
                                         ILogger logger)
            : base(logger)
        {
            infoBuilder.Guard("infoBuilder");
            storageAdapter.Guard("storageAdapter");
            configurationManager.Guard("configurationManager");
            threadManager.Guard("threadManagerService");
            publicTransport.Guard("publicTransport");
            this.infoBuilder = infoBuilder;
            this.storageAdapter = storageAdapter;
            this.threadManager = threadManager;
            this.publicTransport = publicTransport;
            pageSize = configurationManager.GetValue("PageSize", 2000, "File Scanner");
            scanPaged = configurationManager.GetValue("ScanPaged", true, "File Scanner");
        }

        private bool canceled;

        /// <summary>
        /// Sets a value indicating whether this <see cref="RecursiveAsyncFileScanner{TInfo}" /> is canceled.
        /// </summary>
        /// <value>
        ///   <c>true</c> if canceled; otherwise, <c>false</c>.
        /// </value>
        protected override bool Canceled
        {
            set { canceled = value; }
        }

        protected override void OnProgress(double progress)
        {
            publicTransport.ApplicationEventBus.Send(new ProgressEvent(Id, progress));
        }

        private int lastDots;

        protected override void OnProgress(string text,
                                           double progress)
        {
            publicTransport.ApplicationEventBus.Send(new ProgressEvent(Id, progress, text));
        }

        protected override ScanCompletedEventArgs DoWork(FileScannerArgs args)
        {
            var di = new DirectoryInfo(args.Root);
            dirsScanned = false;
            totalFiles = 0;
            var fsWorker = threadManager.StartNew(AddFilesRecursive, di);
            var queueWorker = threadManager.StartNew(ProcessQueue);
            fsWorker.Wait();
            dirsScanned = true;
            blockade.Set();
            queueWorker.Wait();
            OnProgress("Done...", 0d);
            return new ScanCompletedEventArgs(Id);
        }

        private bool dirsScanned;

        private void ProcessQueue()
        {
            var added = 0;
            storageAdapter.Transact(() =>
                                        {
                                            while (!dirsScanned && !canceled)
                                            {
                                                blockade.WaitOne();
                                                if (dirsScanned) break;
                                                Flush(ref added);
                                            }
                                            TInfo item;
                                            while (infos.Count > 0 && infos.TryDequeue(out item))
                                            {
                                                Flush(ref added);
                                            }
                                        }, true);
        }

        private int page = 1;

        private void Flush(ref int totalCount)
        {
            var firstItem = infos.LastOrDefault();
            if (Equals(null, firstItem)) return;
            if (!scanPaged.Value)
            {
                TInfo item;
                if (!infos.TryDequeue(out item)) return;
                ++totalCount;
                Store(item);
            }
            else if (totalFiles > pageSize.Value*page || dirsScanned)
            {
                FlushPage(pageSize.Value, ref totalCount);
                ++page;
            }
            ReportProgress(firstItem, "Discovering", totalCount);
        }

        private void ReportProgress(TInfo item,
                                    string action,
                                    int totalCount)
        {
            if (DateTime.Now - lastProgress < TimeSpan.FromMilliseconds(100)) return;
            var progress = 100d/totalFiles*totalCount;
            string text = item.ToString();
            OnProgress(dirsScanned
                           ? text
                           : action + // Who said oneliners were boring?
                             ".".RepeatString(++lastDots > 3
                                                  ? lastDots = 0
                                                  : lastDots) + " - " + text, progress);
            lastProgress = DateTime.Now;
        }

        private void Store(TInfo info)
        {
            storageAdapter.Save(info);
        }

        private void FlushPage(int size,
                               ref int outerCount)
        {
            var count = 0;
            var stuff = new List<TInfo>();
            while (count++ < size && infos.Count > 0)
            {
                TInfo item;
                if (!infos.TryDequeue(out item)) continue;
                stuff.Add(item);
            }
            outerCount = StorePage(stuff, outerCount);
        }

        private DateTime lastProgress;

        /// <summary>
        /// Stores the page.
        /// </summary>
        /// <param name="stuff">The page.</param>
        /// <param name="outerCount">The outer count.</param>
        /// <returns></returns>
        private int StorePage(ICollection<TInfo> stuff,
                              int outerCount)
        {
            foreach (var pageInfo in stuff)
            {
                ++outerCount;
                Store(pageInfo);
                ReportProgress(pageInfo, "Storing", outerCount);
            }
            stuff.Clear();
            return outerCount;
        }

        /// <summary>
        /// Gets the files recursive async.
        /// </summary>
        /// <param name="root">The root.</param>
        /// <param name="builder">The builder.</param>
        /// <returns></returns>
        public static async Task<IEnumerable<TInfo>> GetFilesRecursiveAsync(DirectoryInfo root,
                                                                            IInfoBuilder<TInfo> builder)
        {
            return await Task.Factory.StartNew(() => GetFilesRecursive(root, builder));
        }

        /// <summary>
        /// Gets the files recursive.
        /// </summary>
        /// <param name="root">The root.</param>
        /// <param name="builder">The builder.</param>
        /// <returns></returns>
        public static IEnumerable<TInfo> GetFilesRecursive(DirectoryInfo root,
                                                           IInfoBuilder<TInfo> builder)
        {
            var results = new List<TInfo>();
            foreach (var file in root.EnumerateFiles("*.*", SearchOption.TopDirectoryOnly))
            {
                bool valid;
                var result = builder.Build(file, out valid);
                if (!valid) continue;
                results.Add(result);
            }
            foreach (var dir in root.EnumerateDirectories())
                results.AddRange(GetFilesRecursive(dir, builder));
            return results;
        }

        private void AddFilesRecursive(DirectoryInfo root)
        {
            if (!root.Exists || canceled) return;
            foreach (var file in root.EnumerateFiles("*.*", SearchOption.TopDirectoryOnly))
            {
                bool valid;
                var result = infoBuilder.Build(file, out valid);
                if (!valid) continue;
                ++totalFiles;
                infos.Enqueue(result);
            }
            blockade.Set();
            foreach (var dir in root.EnumerateDirectories())
                AddFilesRecursive(dir);
        }

        #region Implementation of IAsyncFileScanner<TInfo>

        public new event GenericEventHandler<IAsyncFileScanner<TInfo>, double> Progress;
        public new event GenericEventHandler<IAsyncFileScanner<TInfo>, string> Status;
        public new event GenericEventHandler<IAsyncFileScanner<TInfo>, ScanCompletedEventArgs> WorkCompleted;
        public new event GenericEventHandler<IAsyncFileScanner<TInfo>> WorkStarted;
        public new event GenericEventHandler<IAsyncFileScanner<TInfo>, Exception> WorkFailed;
        public new event GenericEventHandler<IAsyncFileScanner<TInfo>> WorkAborted;

        #endregion
    }
}