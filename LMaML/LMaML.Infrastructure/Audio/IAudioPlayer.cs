using System.IO;

namespace LMaML.Infrastructure.Audio
{
    /// <summary>
    /// IAudioPlayer
    /// </summary>
    public interface IAudioPlayer
    {
        /// <summary>
        /// Plays the file.
        /// </summary>
        /// <param name="file">The file.</param>
        /// <returns></returns>
        /// <exception cref="FileNotFoundException">Can't find file</exception>
        IChannel PlayFile(string file);

        /// <summary>
        /// Creates the channel.
        /// </summary>
        /// <param name="file">The file.</param>
        /// <returns></returns>
        IChannel CreateChannel(string file);
    }
}