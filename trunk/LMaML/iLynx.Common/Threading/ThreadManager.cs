using System;
using System.Collections.Generic;
using iLynx.Common.Threading.Unmanaged;

namespace iLynx.Common.Threading
{
    /// <summary>
    /// ThreadManagerService
    /// </summary>
    public class ThreadManager : IThreadManager, IDisposable
    {
        private readonly ILogger logger;
        private readonly Dictionary<Guid, IWorker> workers = new Dictionary<Guid, IWorker>();

        /// <summary>
        /// Initializes a new instance of the <see cref="ThreadManager" /> class.
        /// </summary>
        /// <param name="logger">The logger.</param>
        public ThreadManager(ILogger logger)
        {
            this.logger = logger;
        }

        /// <summary>
        /// Starts the new.
        /// </summary>
        /// <param name="target">The target.</param>
        /// <returns></returns>
        public IWorker StartNew(Action target)
        {
            var worker = new ThreadedWorker(target, logger);
            TrackWorker(worker);
            worker.Execute();
            return worker;
        }

        private void TrackWorker(IWorker worker)
        {
            if (workers.ContainsKey(worker.Id)) throw new InvalidOperationException("A worker with the specified Id is already being tracked!");
            workers.Add(worker.Id, worker);
            worker.ThreadExit += WorkerOnThreadExit;
        }

        private void WorkerOnThreadExit(Guid guid)
        {
            workers.Remove(guid);
        }

        /// <summary>
        /// Starts the new.
        /// </summary>
        /// <typeparam name="TArgs">The type of the args.</typeparam>
        /// <param name="target">The target.</param>
        /// <param name="args">The args.</param>
        /// <returns></returns>
        public IParameterizedWorker<TArgs> StartNew<TArgs>(Action<TArgs> target, TArgs args)
        {
            var worker = new ParameterizedThreadedWorker<TArgs>(target, logger);
            TrackWorker(worker);
            worker.Execute(args);
            return worker;
        }

        /// <summary>
        /// Starts the new.
        /// </summary>
        /// <typeparam name="TResult">The type of the result.</typeparam>
        /// <param name="target">The target.</param>
        /// <returns></returns>
        public IResultWorker<TResult> StartNew<TResult>(Func<TResult> target)
        {
            var worker = new ThreadedResultWorker<TResult>(target, logger);
            TrackWorker(worker);
            worker.Execute();
            return worker;
        }

        /// <summary>
        /// Starts the new.
        /// </summary>
        /// <typeparam name="TArgs">The type of the args.</typeparam>
        /// <typeparam name="TResult">The type of the result.</typeparam>
        /// <param name="target">The target.</param>
        /// <param name="args">The args.</param>
        /// <returns></returns>
        public IParameterizedResultWorker<TArgs, TResult> StartNew<TArgs, TResult>(Func<TArgs, TResult> target, TArgs args)
        {
            var worker = new ParameterizedThreadedResultWorker<TArgs, TResult>(target, logger);
            TrackWorker(worker);
            worker.Execute(args);
            return worker;
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            foreach (var worker in workers.Values)
                worker.Abort();
        }
    }
}
