using System;
using iLynx.Common.Threading.Unmanaged;

namespace iLynx.Common.Threading
{
    /// <summary>
    /// IThreadManagerService
    /// </summary>
    public interface IThreadManager
    {
        /// <summary>
        /// Starts the new.
        /// </summary>
        /// <param name="target">The target.</param>
        /// <returns></returns>
        IWorker StartNew(Action target);

        /// <summary>
        /// Starts the new.
        /// </summary>
        /// <typeparam name="TArgs">The type of the args.</typeparam>
        /// <param name="target">The target.</param>
        /// <param name="args">The args.</param>
        /// <returns></returns>
        IParameterizedWorker<TArgs> StartNew<TArgs>(Action<TArgs> target, TArgs args);

        /// <summary>
        /// Starts the new.
        /// </summary>
        /// <typeparam name="TResult">The type of the result.</typeparam>
        /// <param name="target">The target.</param>
        /// <returns></returns>
        IResultWorker<TResult> StartNew<TResult>(Func<TResult> target);

        /// <summary>
        /// Starts the new.
        /// </summary>
        /// <typeparam name="TArgs">The type of the args.</typeparam>
        /// <typeparam name="TResult">The type of the result.</typeparam>
        /// <param name="target">The target.</param>
        /// <param name="args">The args.</param>
        /// <returns></returns>
        IParameterizedResultWorker<TArgs, TResult> StartNew<TArgs, TResult>(Func<TArgs, TResult> target, TArgs args);
    }
}
