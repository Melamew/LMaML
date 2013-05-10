using System;
using System.Threading;
using iLynx.Common.Threading.Unmanaged;

namespace iLynx.Common.Threading
{
    /// <summary>
    /// ThreadedWorkerBase
    /// </summary>
    public abstract class ThreadedWorkerBase : IWorker
    {
        private readonly ILogger logger;
        private readonly Thread thread;
        private bool started;

        /// <summary>
        /// Initializes a new instance of the <see cref="ThreadedWorkerBase" /> class.
        /// </summary>
        /// <param name="logger">The logger.</param>
        protected ThreadedWorkerBase(ILogger logger)
        {
            this.logger = logger;
            Id = Guid.NewGuid();
            thread = new Thread(DoExecute);
        }

        /// <summary>
        /// Called when [thread exit].
        /// </summary>
        protected virtual void OnThreadExit()
        {
            if (null != ThreadExit)
                ThreadExit(Id);
        }

        /// <summary>
        /// Executes the specified args.
        /// </summary>
        /// <param name="args">The args.</param>
        public virtual void Execute(object args = null)
        {
            started = true;
            thread.Start(args);
        }

        /// <summary>
        /// Does the execute.
        /// </summary>
        /// <param name="args">The args.</param>
        private void DoExecute(object args)
        {
            try { ExecuteInternal(args); }
            catch (ThreadAbortException) { }
            catch (Exception e)
            {
                logger.Log(LoggingType.Error, this, string.Format("Thread has crashed: {0}", e));
                exception = e;
            }
            OnThreadExit();
        }

        /// <summary>
        /// Throws if not started.
        /// </summary>
        /// <exception cref="System.InvalidOperationException">This worker has not been started yet</exception>
        private void ThrowIfNotStarted()
        {
            if (!started)
                throw new InvalidOperationException("This worker has not been started yet");
        }

        /// <summary>
        /// Throws if failed.
        /// </summary>
        private void ThrowIfFailed()
        {
            if (null != exception)
                throw new Exception("This worker has crashed, See InnerException for details", exception);
        }

        /// <summary>
        /// Gets the exception.
        /// </summary>
        /// <value>
        /// The exception.
        /// </value>
        private Exception exception;

        /// <summary>
        /// Executes the internal.
        /// </summary>
        /// <param name="args">The args.</param>
        protected abstract void ExecuteInternal(object args);

        /// <summary>
        /// Cancels this instance.
        /// </summary>
        public void Abort()
        {
            ThrowIfNotStarted();
            thread.Abort();
            ThrowIfFailed();
        }

        /// <summary>
        /// Waits this instance.
        /// </summary>
        public void Wait()
        {
            ThrowIfNotStarted();
            thread.Join();
            ThrowIfFailed();
        }

        /// <summary>
        /// Waits the specified timeout.
        /// </summary>
        /// <param name="timeout">The timeout.</param>
        public void Wait(TimeSpan timeout)
        {
            ThrowIfNotStarted();
            if (!thread.Join(timeout)) throw new TimeoutException("Thread did not terminate within the allotted time");
            ThrowIfFailed();
        }

        /// <summary>
        /// Gets the id.
        /// </summary>
        /// <value>
        /// The id.
        /// </value>
        public Guid Id { get; private set; }

        /// <summary>
        /// Occurs when [thread exit].
        /// </summary>
        public event Action<Guid> ThreadExit;
    }
}