using System;
using iLynx.Common.Threading.Unmanaged;

namespace iLynx.Common.Threading
{
    /// <summary>
    /// ParameterizedThreadedWorker
    /// </summary>
    /// <typeparam name="TArgs">The type of the args.</typeparam>
    public class ParameterizedThreadedWorker<TArgs> : ThreadedWorkerBase, IParameterizedWorker<TArgs>
    {
        private readonly Action<TArgs> target;
        private Exception exception;

        /// <summary>
        /// Initializes a new instance of the <see cref="ParameterizedThreadedWorker{TArgs}" /> class.
        /// </summary>
        /// <param name="target">The target.</param>
        /// <param name="logger">The logger.</param>
        public ParameterizedThreadedWorker(Action<TArgs> target, ILogger logger) : base(logger)
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
            target((TArgs)args);
        }

        /// <summary>
        /// Executes the specified args.
        /// </summary>
        /// <param name="args">The args.</param>
        public void Execute(TArgs args)
        {
            Execute((object)args);
        }
    }
}
