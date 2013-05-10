namespace LMaML.Infrastructure.Events
{
    /// <summary>
    /// TrackProgressEvent
    /// <para/>
    /// This event is fired approximately every 250 ms
    /// </summary>
    public class TrackProgressEvent : IApplicationEvent
    {
        /// <summary>
        /// Gets the position.
        /// </summary>
        /// <value>
        /// The position.
        /// </value>
        public double Position { get; private set; }
        /// <summary>
        /// Gets the position percent.
        /// </summary>
        /// <value>
        /// The position percent.
        /// </value>
        public double PositionPercent { get; private set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="TrackProgressEvent" /> class.
        /// </summary>
        /// <param name="position">The position.</param>
        /// <param name="positionPercent">The position percent.</param>
        public TrackProgressEvent(double position, double positionPercent)
        {
            Position = position;
            PositionPercent = positionPercent;
        }
    }
}
