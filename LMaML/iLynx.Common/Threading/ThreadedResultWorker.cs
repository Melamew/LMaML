using System;
using iLynx.Common.Threading.Unmanaged;

namespace iLynx.Common.Threading
{
    /// <summary>
    /// ThreadedResultWorker{TResult}
    /// </summary>
    /// <typeparam name="TResult">The type of the result.</typeparam>
    public class ThreadedResultWorker<TResult> : ThreadedWorkerBase, IResultWorker<TResult>
    {
        private readonly Func<TResult> target;
        private TResult result;

        /// <summary>
        /// Initializes a new instance of the <see cref="ThreadedResultWorker{TResult}" /> class.
        /// </summary>
        /// <param name="target">The target.</param>
        /// <param name="logger">The logger.</param>
        public ThreadedResultWorker(Func<TResult> target, ILogger logger) : base(logger)
        {
            target.Guard("target");
            this.target = target;
        }

        /// <summary>
        /// Executes the internal.
        /// </summary>
        /// <param name="args">The args.</param>
        protected override void ExecuteInternal(object args)
        {
            result = target();
        }

        /// <summary>
        /// Waits this instance.
        /// </summary>
        /// <returns></returns>
        public new TResult Wait()
        {
            base.Wait();
            return result;
        }

        /// <summary>
        /// Waits the specified timeout.
        /// </summary>
        /// <param name="timeout">The timeout.</param>
        /// <returns></returns>
        public new TResult Wait(TimeSpan timeout)
        {
            base.Wait(timeout);
            return result;
        }
    }
}
