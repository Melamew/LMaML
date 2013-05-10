using System;

namespace LMaML.Infrastructure.Events
{
    /// <summary>
    /// ProgressEvent
    /// </summary>
    public class ProgressEvent : IApplicationEvent
    {
        /// <summary>
        /// Gets the unique id.
        /// </summary>
        /// <value>
        /// The unique id.
        /// </value>
        public Guid UniqueId { get; private set; }

        /// <summary>
        /// Gets the progress.
        /// </summary>
        /// <value>
        /// The progress.
        /// </value>
        public double Progress { get; private set; }

        /// <summary>
        /// Gets the progress string.
        /// </summary>
        /// <value>
        /// The progress string.
        /// </value>
        public string ProgressString { get; private set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="ProgressEvent" /> class.
        /// </summary>
        /// <param name="uniqueId">The unique id.</param>
        /// <param name="progress">The progress.</param>
        /// <param name="progressString">The progress string.</param>
        public ProgressEvent(Guid uniqueId, double progress, string progressString = "")
        {
            if (progress > 100)
                progress = 100d;
            UniqueId = uniqueId;
            Progress = progress;
            ProgressString = progressString;
        }
    }

    /// <summary>
    /// WorkCompletedEvent
    /// </summary>
    public class WorkCompletedEvent : IApplicationEvent
    {
        public Guid UniqueId { get; private set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="WorkCompletedEvent" /> class.
        /// </summary>
        /// <param name="uniqueId">The unique id.</param>
        public WorkCompletedEvent(Guid uniqueId)
        {
            UniqueId = uniqueId;
        }
    }

    /// <summary>
    /// WorkStartedEvent
    /// </summary>
    public class WorkStartedEvent : IApplicationEvent
    {
        public Guid UniqueId { get; private set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="WorkStartedEvent" /> class.
        /// </summary>
        /// <param name="uniqueId">The unique id.</param>
        public WorkStartedEvent(Guid uniqueId)
        {
            UniqueId = uniqueId;
        }
    }
}
