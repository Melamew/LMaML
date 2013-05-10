using LMaML.Infrastructure.Events;

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
        IEventBus<IApplicationEvent> ApplicationEventBus { get; }

        /// <summary>
        /// Gets the command bus.
        /// </summary>
        /// <value>
        /// The command bus.
        /// </value>
        ICommandBus CommandBus { get; }
    }
}
