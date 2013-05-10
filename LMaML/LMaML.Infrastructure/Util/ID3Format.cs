namespace LMaML.Infrastructure.Util
{

    /// <summary>
    /// An enum that can be used to specify a specific audio / id3 file format
    /// </summary>
    public enum ID3Format
    {
        /// <summary>
        /// If this value is set, the format is either unknown or invalid
        /// </summary>
        Unknown,

        /// <summary>
        /// MP3
        /// </summary>
        Mp3,

        /// <summary>
        /// Wav
        /// </summary>
        Wav,

        /// <summary>
        /// Flac
        /// </summary>
        Flac,

        /// <summary>
        /// Asf
        /// </summary>
        Asf,

        /// <summary>
        /// Musepack
        /// </summary>
        Mpc,

        /// <summary>
        /// M4A (Or mp4)
        /// </summary>
        M4A,

        /// <summary>
        /// Ogg
        /// </summary>
        Ogg,
    }
}