using System;
using iLynx.Common.Threading.Unmanaged;

namespace iLynx.Common.Threading
{
    /// <summary>
    /// ParameterizedThreadedResultWorker
    /// </summary>
    /// <typeparam name="TArgs">The type of the args.</typeparam>
    /// <typeparam name="TResult">The type of the result.</typeparam>
    public class ParameterizedThreadedResultWorker<TArgs,TResult> : ThreadedWorkerBase, IParameterizedResultWorker<TArgs,TResult>
    {
        private TResult result;
        private readonly Func<TArgs, TResult> target;

        /// <summary>
        /// Initializes a new instance of the <see cref="ParameterizedThreadedResultWorker{TArgs,TResult}" /> class.
        /// </summary>
        /// <param name="target">The target.</param>
        /// <param name="logger">The logger.</param>
        public ParameterizedThreadedResultWorker(Func<TArgs, TResult> target, ILogger logger) : base(logger)
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
            if (!(args is TArgs)) return;
            result = target((TArgs)args);
        }

        /// <summary>
        /// Executes the specified args.
        /// </summary>
        /// <param name="args">The args.</param>
        public void Execute(TArgs args)
        {
            base.Execute(args);
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
