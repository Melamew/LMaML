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

        /// <summary>
        /// Loads the plugins.
        /// </summary>
        /// <param name="dir">The dir.</param>
        void LoadPlugins(string dir);
    }
}