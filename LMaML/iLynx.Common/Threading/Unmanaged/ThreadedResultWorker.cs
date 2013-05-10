using System;
using System.Reflection;
using System.Threading;

namespace iLynx.Common.Threading.Unmanaged
{
    /// <summary>
    ///     Helper class for executing work asynchronously (Read, in a seperate thread)
    /// </summary>
    /// <typeparam name="TArgs">The type of arguments that are passed to the worker thread</typeparam>
    /// <typeparam name="TCompletedArgs">The expected output type</typeparam>
    public abstract class ThreadedResultWorker<TArgs, TCompletedArgs> : ComponentBase
    {
        private bool completed;
        private TCompletedArgs result;
        private Action<TCompletedArgs> workCompletedCallback;

        /// <summary>
        /// Initializes a new instance of the <see cref="ThreadedResultWorker{TArgs,TCompletedArgs}" /> class.
        /// </summary>
        /// <param name="logger">The logger.</param>
        protected ThreadedResultWorker(ILogger logger) : base(logger)
        {
            Id = Guid.NewGuid();
        }

        /// <summary>
        ///     Set internally by <see cref="Execute(TArgs)" />
        /// </summary>
        private Thread worker;

        /// <summary>
        ///     This property will contain the result of the ThreadedWorker once it has completed execution
        /// </summary>
        public TCompletedArgs Result
        {
            get
            {
                if (!completed)
                {
                    LogCritical("Attempted to retrieve result before worker had finished");
                    throw new NotSupportedException(
                        "The results cannot be retrieved before the worker has completed executing");
                }
                return result;
            }
        }

        /// <summary>
        /// Sets a value indicating whether this <see cref="ThreadedResultWorker{TArgs,TCompletedArgs}" /> is canceled.
        /// </summary>
        /// <value>
        ///   <c>true</c> if canceled; otherwise, <c>false</c>.
        /// </value>
        protected abstract bool Canceled { set; }

        /// <summary>
        ///     Gets or Sets the name of this worker
        /// </summary>
        public virtual string Name
        {
            get { return base.ComponentName; }
            protected set { base.ComponentName = value; }
        }

        /// <summary>
        ///     This event is fired when the worker has completed it's work
        /// </summary>
        public event GenericEventHandler<ThreadedResultWorker<TArgs, TCompletedArgs>, TCompletedArgs> WorkCompleted;

        /// <summary>
        ///     This event is fired when the worker has started working
        /// </summary>
        public event GenericEventHandler<ThreadedResultWorker<TArgs, TCompletedArgs>> WorkStarted;

        /// <summary>
        ///     This event is fired if any exceptions are caught during execution
        /// </summary>
        public event GenericEventHandler<ThreadedResultWorker<TArgs, TCompletedArgs>, Exception> WorkFailed;

        /// <summary>
        ///     This event is fired when the worker is aborted
        /// </summary>
        public event GenericEventHandler<ThreadedResultWorker<TArgs, TCompletedArgs>> WorkAborted;

        /// <summary>
        ///     Executes this worker
        /// </summary>
        /// <param name="args">The arguments that are passed on to the thread</param>
        public void Execute(TArgs args)
        {
            if (worker != null && !completed)
            {
                LogCritical("Attempted to start a running worker");
                throw new NotSupportedException(
                    "This worker has already been started and cannot be started again until it has completed, failed or has been aborted");
            }
            if (worker != null && completed)
            {
                LogDebug("Ensuring worker thread is dead");
                Abort();
                result = default(TCompletedArgs);
            }
            worker = new Thread(DoWork) { Name = ToString() };
            LogInformation("Starting worker at {0}", DateTime.Now);
            worker.Start(args);
        }

        public void Execute(object args = null)
        {
            args.Guard("args");
            if (!(args is TArgs)) throw new InvalidCastException("The specified arguments are not of the expected type");
            Execute((TArgs)args);
        }

        /// <summary>
        /// Cancels this instance.
        /// </summary>
        public void Cancel()
        {
            Canceled = true;
        }

        /// <summary>
        ///     Executes this worker and calls the <paramref name="completedCallback" /> method when it completes
        /// </summary>
        /// <param name="args">The argument to pass to the worker</param>
        /// <param name="completedCallback">
        ///     The <see cref="Action{TCompletedArgs}" /> to call when work is complete
        /// </param>
        public void Execute(TArgs args, Action<TCompletedArgs> completedCallback)
        {
            workCompletedCallback = completedCallback;
            Execute(args);
        }

        /// <summary>
        ///     Used to invoke the <see cref="WorkStarted" /> event
        /// </summary>
        private void OnStarted()
        {
            LogInformation("Worker started at {0}", DateTime.Now);
            if (WorkStarted != null)
                WorkStarted.BeginInvoke(this, iar => WorkStarted.EndInvoke(iar), null);
        }

        /// <summary>
        ///     Used to invoke the <see cref="WorkCompleted" /> event
        /// </summary>
        /// <param name="args">The TCompletedArgs to send in the event</param>
        private void OnCompleted(TCompletedArgs args)
        {
            LogInformation("Worker completed at {0}", DateTime.Now);
            if (WorkCompleted != null)
                WorkCompleted.BeginInvoke(this, args, iar => WorkCompleted.EndInvoke(iar), null);
        }

        /// <summary>
        ///     Used to invoke the <see cref="WorkAborted" /> event
        /// </summary>
        private void OnAborted()
        {
            if (WorkAborted != null)
                WorkAborted.BeginInvoke(this, iar => WorkAborted.EndInvoke(iar), null);
        }

        /// <summary>
        ///     Used to invoke the <see cref="WorkFailed" /> event
        /// </summary>
        private void OnFailed(Exception e)
        {
            //LogError("Worker failed at {0}", DateTime.Now);
            LogException(e, MethodBase.GetCurrentMethod());
            if (WorkFailed != null)
                WorkFailed.BeginInvoke(this, e, iar => WorkFailed.EndInvoke(iar), null);
        }

        private void DoWork(object args)
        {
            try
            {
                OnStarted();
                result = DoWork((TArgs)args);
                completed = true;
                OnCompleted(result);
                if (workCompletedCallback != null)
                    workCompletedCallback(result);
            }
            catch (ThreadAbortException)
            {
                OnAborted();
                completed = true;
            }
            catch (Exception e)
            {
                OnFailed(e);
                completed = true;
            }
        }

        /// <summary>
        ///     Aborts the current worker thread
        /// </summary>
        public void Abort()
        {
            if (worker == null) return;
            try
            {
                worker.Abort();
                worker.Join();
            }
            catch (Exception e)
            {
                LogException(e, MethodBase.GetCurrentMethod());
            }
        }

        /// <summary>
        /// Gets the id.
        /// </summary>
        /// <value>
        /// The id.
        /// </value>
        public Guid Id { get; private set; }

        /// <summary>
        /// Waits this instance.
        /// </summary>
        public void Wait()
        {
            if (null == worker) return;
            worker.Join();
        }

        /// <summary>
        /// Waits the specified timeout.
        /// </summary>
        /// <param name="timeout">The timeout.</param>
        /// <exception cref="System.TimeoutException">Thread shutdown timed out</exception>
        public void Wait(TimeSpan timeout)
        {
            if (!worker.Join(timeout)) throw new TimeoutException("Thread shutdown timed out");
        }

        /// <summary>
        ///     This method is the entry point of the worker thread
        /// </summary>
        /// <param name="args">
        ///     Will contain a <typeparamref name="TArgs" /> object as an object
        /// </param>
        protected abstract TCompletedArgs DoWork(TArgs args);

        /// <summary>
        /// Creates a "generic" implementation of a <see cref="ThreadedResultWorker{TArgs,TCompletedArgs}" />
        /// <para />
        /// that executes the specified <see cref="Func{TArgs, TCompletedArgs}" /> in the worker thread
        /// </summary>
        /// <param name="executeCallback">The <see cref="Func{TArgs, TCompletedArgs}" /> to execute</param>
        /// <param name="logger">The logger.</param>
        /// <returns></returns>
        public static ThreadedResultWorker<TArgs, TCompletedArgs> Create(Func<TArgs, TCompletedArgs> executeCallback, ILogger logger)
        {
            return new GenericImplmenetation<TArgs, TCompletedArgs>(executeCallback, logger);
        }

        /// <summary>
        /// GenericImplmenetation
        /// </summary>
        /// <typeparam name="T1">The type of the 1.</typeparam>
        /// <typeparam name="T2">The type of the 2.</typeparam>
        private class GenericImplmenetation<T1, T2> : ThreadedResultWorker<T1, T2>
        {
            private readonly Func<T1, T2> executeCallback;

            /// <summary>
            /// Initializes a new instance of the <see cref="ThreadedResultWorker{TArgs,TCompletedArgs}.GenericImplmenetation{T1,T2}" /> class.
            /// </summary>
            /// <param name="executeCallback">The execute callback.</param>
            /// <param name="logger">The logger.</param>
            public GenericImplmenetation(Func<T1, T2> executeCallback, ILogger logger)
                : base(logger)
            {
                this.executeCallback = executeCallback;
            }

            /// <summary>
            /// Sets a value indicating whether this <see cref="ThreadedResultWorker{TArgs,TCompletedArgs}.GenericImplmenetation{T1,T2}" /> is canceled.
            /// </summary>
            /// <value>
            ///   <c>true</c> if canceled; otherwise, <c>false</c>.
            /// </value>
            protected override bool Canceled
            {
                set { }
            }

            /// <summary>
            /// Does the work.
            /// </summary>
            /// <param name="args">The args.</param>
            /// <returns></returns>
            protected override T2 DoWork(T1 args)
            {
                return executeCallback(args);
            }
        }
    }
}