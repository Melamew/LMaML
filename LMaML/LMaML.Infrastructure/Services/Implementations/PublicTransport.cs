using LMaML.Infrastructure.Events;
using LMaML.Infrastructure.Services.Interfaces;

namespace LMaML.Infrastructure.Services.Implementations
{
    /// <summary>
    /// PublicTransport
    /// </summary>
    public class PublicTransport : IPublicTransport
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PublicTransport" /> class.
        /// </summary>
        /// <param name="applicationEventBus">The application event bus.</param>
        /// <param name="commandBus">The command bus.</param>
        public PublicTransport(IEventBus<IApplicationEvent> applicationEventBus, ICommandBus commandBus)
        {
            ApplicationEventBus = applicationEventBus;
            CommandBus = commandBus;
        }

        /// <summary>
        /// Gets the application event bus.
        /// </summary>
        /// <value>
        /// The application event bus.
        /// </value>
        public IEventBus<IApplicationEvent> ApplicationEventBus { get; private set; }

        /// <summary>
        /// Gets the command bus.
        /// </summary>
        /// <value>
        /// The command bus.
        /// </value>
        public ICommandBus CommandBus { get; private set; }
    }
}
