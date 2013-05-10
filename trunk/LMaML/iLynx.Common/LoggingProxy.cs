namespace iLynx.Common
{
    /// <summary>
    /// Can act as a "proxy" <see cref="ILogger"/>
    /// </summary>
    public class LoggingProxy : ILogger
    {
        private ILogger logger;

        /// <summary>
        /// Initializes a new instance of <see cref="LoggingProxy"/> and sets the internal <see cref="ILogger"/> to the specified value
        /// </summary>
        /// <param name="logger">The logger to use</param>
        public LoggingProxy(ILogger logger)
        {
            this.logger = logger;
        }

        /// <summary>
        /// Gets or Sets the logger that is currently in use
        /// </summary>
        public ILogger Logger
        {
            get { return logger; }
            set { logger = value; }
        }

        /// <summary>
        /// Writes the specified message to the contained <see cref="ILogger"/>
        /// </summary>
        /// <param name="type">The <see cref="LoggingType"/> of the message</param>
        /// <param name="sender">The component that sent the message</param>
        /// <param name="message">The actual message</param>
        public void Log(LoggingType type, object sender, string message)
        {
            if (logger != null)
                logger.Log(type, sender, message);
        }
    }
}