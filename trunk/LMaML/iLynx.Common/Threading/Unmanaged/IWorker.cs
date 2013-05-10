using System;

namespace iLynx.Common.Threading.Unmanaged
{
    /// <summary>
    /// IWorker
    /// </summary>
    public interface IWorker
    {
        /// <summary>
        /// Executes the specified args.
        /// </summary>
        /// <param name="args">The args.</param>
        void Execute(object args = null);

        /// <summary>
        /// Cancels this instance.
        /// </summary>
        void Abort();

        /// <summary>
        /// Waits this instance.
        /// </summary>
        void Wait();

        /// <summary>
        /// Waits the specified timeout.
        /// </summary>
        /// <param name="timeout">The timeout.</param>
        void Wait(TimeSpan timeout);

        /// <summary>
        /// Gets the id.
        /// </summary>
        /// <value>
        /// The id.
        /// </value>
        Guid Id { get; }

        /// <summary>
        /// Occurs when [thread exit].
        /// </summary>
        event Action<Guid> ThreadExit;
    }
}
