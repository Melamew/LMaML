using System;
using System.IO;

namespace iLynx.Common
{
    /// <summary>
    /// Simple Console Logger - will dump anything to console
    /// </summary>
    public class ConsoleLogger : ILogger, IDisposable
    {
        private Stream os;
        private StreamWriter writer;

        /// <summary>
        /// Empty Constructor
        /// </summary>
        public ConsoleLogger()
        {
            if (Console.LargestWindowHeight > 50 && Console.LargestWindowWidth > 50)
                Console.SetWindowSize((int)(Console.LargestWindowWidth / 1.5), (int)(Console.LargestWindowHeight / 1.5));
        }

        /// <summary>
        /// Initializes a new instance of <see cref="ConsoleLogger"/> and optionally logs to a file
        /// </summary>
        /// <param name="dumpFile"></param>
        public ConsoleLogger(string dumpFile)
        {
            var ext = Path.GetExtension(dumpFile);
            if (string.IsNullOrEmpty(ext))
                ext = ".log";

            var p2 = 0;
            //while (File.Exists(Path.Combine(Environment.CurrentDirectory, string.Format("{0}{1}{2}", dumpFile, p2, ext))))
            //    p2++;
            //os = File.Create(Path.Combine(Environment.CurrentDirectory, string.Format("{0}{1}{2}", dumpFile, p2, ext)));
            os = File.Open(Path.Combine(Environment.CurrentDirectory, string.Format("{0}{1}", dumpFile, ext)),
                           FileMode.Append);
            writer = new StreamWriter(os);
        }

        /// <summary>
        /// Writes the specified message to the console
        /// </summary>
        /// <param name="type">The type of message</param>
        /// <param name="sender">The sender of the message</param>
        /// <param name="message">The message itself</param>
        public void Log(LoggingType type, object sender, string message)
        {
            var line = string.Format("[{0}:{1}]: {2}", type.ToString()[0], null == sender ? "NOWHERE" : sender.GetType().FullName, message);
            Console.WriteLine(line);
            if (writer == null) return;
            writer.WriteLine(line);
            writer.Flush();
        }

        /// <summary>
        /// Destructor
        /// </summary>
        ~ConsoleLogger()
        {
            try
            {
                if (writer != null)
                    writer.Close();
            }
// ReSharper disable EmptyGeneralCatchClause
            catch (Exception)
// ReSharper restore EmptyGeneralCatchClause
            { }
            finally
            {
                os = null;
                writer = null;
            }
        }

        #region IDisposable Members

        /// <summary>
        /// Disposes the logger
        /// </summary>
        public void Dispose()
        {
            if (writer != null)
                writer.Dispose();
            if (os != null)
                os.Dispose();
        }

        #endregion
    }
}
