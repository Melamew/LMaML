using System;
using System.Reflection;
using System.Diagnostics;

namespace iLynx.Common
{
	/// <summary>
	/// Defines a few methods commonly used throughout ToyChat
	/// </summary>
	public abstract class ComponentBase
	{
        /// <summary>
        /// Gets the logger.
        /// </summary>
        /// <value>
        /// The logger.
        /// </value>
        protected ILogger Logger { get; private set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="ComponentBase" /> class.
        /// </summary>
        /// <param name="logger">The logger.</param>
        protected ComponentBase(ILogger logger)
        {
            logger.Guard("logger");
            Logger = logger;
        }

        /// <summary>
        /// Gets or Sets the name of this component
        /// </summary>
        protected virtual string ComponentName
        {
            get;
            set;
        }

        /// <summary>
        /// Writes a log entry to the default logger (<see cref="RuntimeCommon.DefaultLogger"/>)
        /// </summary>
        /// <param name="type">The type of the log entry</param>
        /// <param name="sender">The sender</param>
        /// <param name="format">The string that should be formatted</param>
        /// <param name="args">The arguments for the string.Format call</param>
        public static void Log(LoggingType type, object sender, string format, params object[] args)
        {
            RuntimeCommon.DefaultLogger.Log(type, sender, string.Format(format, args));
        }

        #region Logging Methods

		/// <summary>
		/// Writes an error entry to the log
		/// </summary>
		/// <param name="msg">The message of the error</param>
		/// <param name="args">Optional arguments used for <see cref="string.Format(string, object)"/></param>
		protected void LogError(string msg, params object[] args)
		{
			Log(LoggingType.Error, string.Format(msg, args));
		}

		/// <summary>
		/// Writes a warning entry to the log
		/// </summary>
		/// <param name="msg">The message of the error</param>
        /// <param name="args">Optional arguments used for <see cref="string.Format(string, object)"/></param>
        protected void LogWarning(string msg, params object[] args)
		{
			Log(LoggingType.Warning, string.Format(msg, args));
		}

        /// <summary>
        /// Writes a critical message to the log
        /// </summary>
        /// <param name="msg">The message to write</param>
        /// <param name="args">Optional arguments used for <see cref="string.Format(string, object)"/></param>
        protected void LogCritical(string msg, params object[] args)
        {
            Log(LoggingType.Critical, string.Format(msg, args));
        }

		/// <summary>
		/// Writes a debug entry to the log
		/// </summary>
		/// <param name="msg">The message of the error</param>
        /// <param name="args">Optional arguments used for <see cref="string.Format(string, object)"/></param>
        protected void LogDebug(string msg, params object[] args)
		{
			Log(LoggingType.Debug, string.Format(msg, args));
		}

		/// <summary>
		/// Writes an information entry to the log
		/// </summary>
		/// <param name="msg">The message of the error</param>
        /// <param name="args">Optional arguments used for <see cref="string.Format(string, object)"/></param>
        protected void LogInformation(string msg, params object[] args)
		{
			Log(LoggingType.Information, string.Format(msg, args));
		}

		/// <summary>
		/// Writes the specified message with the specified type to the log
		/// </summary>
		/// <param name="type">The type of logging message</param>
		/// <param name="msg">The message to write</param>
        protected void Log(LoggingType type, string msg)
		{
            if (Logger != null)
                Logger.Log(type, this, string.Format("{0}{1}", !string.IsNullOrEmpty(ComponentName) ? ComponentName + ": " : string.Empty, msg));
            else
            {
                var str = string.Format("{0}: {1}", type, msg);
                try { Trace.WriteLine(str); }
                catch { Console.WriteLine(str); }
            }
		}

		/// <summary>
		/// Writes a formatted exception message to the log (includes stacktrace and so forth)
		/// </summary>
		/// <param name="er">The exception to log</param>
		/// <param name="method">The method the exception occured in</param>
        protected void LogException(Exception er, MethodBase method)
		{
			LogError("{4} Caught Exception: {1}{0}Message: {2}{0}StackTrace: {3}", Environment.NewLine, er.ToString(), er.Message, er.StackTrace, method.Name);
		}

		#endregion Logging Methods
	}
}
