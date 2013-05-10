using System;

namespace iLynx.Common.Threading.Unmanaged
{
    /// <summary>
    /// IResultWorker
    /// </summary>
    /// <typeparam name="TResult">The type of the result.</typeparam>
    public interface IResultWorker<out TResult> : IWorker
    {
        /// <summary>
        /// Waits this instance.
        /// </summary>
        /// <returns></returns>
        new TResult Wait();

        /// <summary>
        /// Waits the specified timeout.
        /// </summary>
        /// <param name="timeout">The timeout.</param>
        /// <returns></returns>
        new TResult Wait(TimeSpan timeout);
    }
}