using System.Linq;

namespace LMaML.Infrastructure.Util
{
    /// <summary>
    /// A class containing a few predefined and well-known file formats (<see cref="IFileFormat"/>)
    /// </summary>
    public static class KnownFormats
    {
        /// <summary>
        /// Matches audio files See File: PredefinedFormats.cs
        /// </summary>
        public static readonly IAudioFormat[] AudioFiles = new IAudioFormat[]
        {
            new MP3Format(),
            new WAVFormat(),
            new MP4Format(),
            new FlacFormat(),
            new OGGFormat(),
            new MusepackSV7Format(),
            new MuspackSV8Format(),
            new ASFFormat()
        };

        /// <summary>
        /// Gets an <see cref="IAudioFormat"/> object that the specified file matches
        /// <para/>
        /// If none are found, an instance of <see cref="UnknownAudioFormat"/> is returned
        /// </summary>
        /// <param name="src"></param>
        /// <param name="filename"></param>
        /// <returns></returns>
        public static IAudioFormat Match(this IAudioFormat[] src, string filename)
        {
            return src.FirstOrDefault(format => format.CheckFile(filename)) ?? new UnknownAudioFormat();
        }
    }

    /// <summary>
    /// Defines the unknown
    /// </summary>
    public class UnknownAudioFormat : IAudioFormat
    {
        #region IAudioFormat Members

        /// <summary>
        /// <see cref="ID3Format.Unknown"/>
        /// </summary>
        public ID3Format TagFormat
        {
            get { return ID3Format.Unknown; }
        }

        #endregion

        #region IFileFormat Members

        /// <summary>
        /// 0-length byte array
        /// </summary>
        public byte[] MagicNumber
        {
            get { return new byte[0]; }
        }

        /// <summary>
        /// UNKNOWN
        /// </summary>
        public string FileType
        {
            get { return "UNKNOWN"; }
        }

        /// <summary>
        /// Always returns false
        /// </summary>
        /// <param name="file"></param>
        /// <returns></returns>
        public bool CheckFile(string file)
        {
            return false;
        }

        #endregion
    }

}