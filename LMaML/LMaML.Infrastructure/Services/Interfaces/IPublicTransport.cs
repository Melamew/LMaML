using System;
using iLynx.PubSub;

namespace LMaML.Infrastructure.Services.Interfaces
{
    public interface IPublicTransport
    {
        /// <summary>
        /// Gets the application event bus.
        /// </summary>
        /// <value>
        /// The application event bus.
        /// </value>
        IBus<IApplicationEvent> ApplicationEventBus { get; }

        /// <summary>
        /// Gets the command bus.
        /// </summary>
        /// <value>
        /// The command bus.
        /// </value>
        IBus<IBusMessage> CommandBus { get; }
    }

    public interface IBusMessage
    {
        bool IsHandled { get; set; }
        void Wait(TimeSpan? timeout = null);
        event Action Handled;
    }

    public interface IApplicationEvent : IBusMessage
    {
        
    }

    public interface IBusMessage<TResult> : IBusMessage
    {
        TResult Result { get; set; }
    }
}
