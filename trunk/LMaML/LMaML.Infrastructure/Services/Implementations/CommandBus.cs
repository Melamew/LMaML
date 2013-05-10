using LMaML.Infrastructure.Services.Interfaces;
using Microsoft.Practices.Unity;
using iLynx.Common;

namespace LMaML.Infrastructure.Services.Implementations
{
    /// <summary>
    /// CommandBus
    /// </summary>
    public class CommandBus : ICommandBus
    {
        private readonly IUnityContainer container;
        private readonly ILogger logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="CommandBus" /> class.
        /// </summary>
        /// <param name="container">The container.</param>
        /// <param name="logger">The logger.</param>
        public CommandBus(IUnityContainer container, ILogger logger)
        {
            container.Guard("container");
            logger.Guard("logger");
            this.container = container;
            this.logger = logger;
        }

        /// <summary>
        /// Sends the specified item.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="item">The item.</param>
        public void Send<T>(Envelope<T> item)
        {
            logger.Log(LoggingType.Information, this, string.Format("Send: {0}", typeof(T)));
            foreach (var handler in container.ResolveAll<ICommandHandler<T>>())
                handler.Handle(item);
        }
    }
}
