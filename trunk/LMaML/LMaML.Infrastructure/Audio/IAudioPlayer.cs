namespace LMaML.Infrastructure.Audio
{
    /// <summary>
    /// IAudioPlayer
    /// </summary>
    public interface IAudioPlayer
    {
        /// <summary>
        /// Creates the channel.
        /// </summary>
        /// <param name="file">The file.</param>
        /// <returns></returns>
        IChannel CreateChannel(string file);
    }
}