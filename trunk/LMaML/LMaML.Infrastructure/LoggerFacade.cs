using System.Collections.Generic;
using Microsoft.Practices.Prism.Logging;
using iLynx.Common;

namespace LMaML.Infrastructure
{
    /// <summary>
    /// LoggerFacade
    /// </summary>
    public class LoggerFacade : ILoggerFacade
    {
        private readonly ILogger logger;
        private readonly Dictionary<Category, LoggingType> categoryMap = new Dictionary<Category, LoggingType>
                                                                            {
                                                                                {Category.Debug, LoggingType.Debug},
                                                                                {Category.Exception, LoggingType.Error},
                                                                                {Category.Info, LoggingType.Information},
                                                                                {Category.Warn, LoggingType.Warning},
                                                                            };
        /// <summary>
        /// Initializes a new instance of the <see cref="LoggerFacade" /> class.
        /// </summary>
        /// <param name="logger">The logger.</param>
        public LoggerFacade(ILogger logger)
        {
            logger.Guard("logger");
            this.logger = logger;
        }

        /// <summary>
        /// Write a new log entry with the specified category and priority.
        /// </summary>
        /// <param name="message">Message body to log.</param>
        /// <param name="category">Category of the entry.</param>
        /// <param name="priority">The priority of the entry.</param>
        public void Log(string message, Category category, Priority priority)
        {
            logger.Log(categoryMap[category], this, message);
        }
    }
}
