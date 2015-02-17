using LMaML.Infrastructure.Commands;

namespace LMaML.Infrastructure.Events
{
    public enum PlayingState
    {
        Playing,
        Paused,
        Stopped,
    }

    /// <summary>
    /// PlayingStateChangedEvent
    /// </summary>
    public class PlayingStateChangedEvent : ApplicationEvent
    {
        public PlayingState NewState { get; private set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="PlayingStateChangedEvent" /> class.
        /// </summary>
        /// <param name="state">The state.</param>
        public PlayingStateChangedEvent(PlayingState state)
        {
            NewState = state;
        }
    }
}
