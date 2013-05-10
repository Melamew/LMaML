namespace iLynx.Common
{
	/// <summary>
	/// an Interface that should be used for logging purposes
	/// </summary>
	public interface ILogger
	{
		/// <summary>
		/// Writes a line to the log (Whichever that would be)
		/// </summary>
		/// <param name="type">The Type of logging</param>
		/// <param name="sender">Should contain the sending object</param>
		/// <param name="message">The message to post to the log</param>
		void Log(LoggingType type, object sender, string message);
	}
}
