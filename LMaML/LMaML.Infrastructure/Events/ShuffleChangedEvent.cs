namespace LMaML.Infrastructure.Events
{
    /// <summary>
    /// ShuffleChangedEvent
    /// </summary>
    public class ShuffleChangedEvent : IApplicationEvent
    {
        /// <summary>
        /// Gets a value indicating whether [new value].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [new value]; otherwise, <c>false</c>.
        /// </value>
        public bool NewValue { get; private set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="ShuffleChangedEvent" /> class.
        /// </summary>
        /// <param name="newValue">if set to <c>true</c> [new value].</param>
        public ShuffleChangedEvent(bool newValue)
        {
            NewValue = newValue;
        }
    }
}
