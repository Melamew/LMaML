namespace LMaML.Infrastructure.Util
{
    /// <summary>
    /// Defines the MP3 (or ID3 file format)
    /// </summary>
    public class MP3Format : AudioFormatBase
    {
        /// <summary>
        /// 0x49, 0x44, 0x33
        /// </summary>
        public override byte[] MagicNumber
        {
            get { return new byte[] { 0x49, 0x44, 0x33 }; }
        }

        /// <summary>
        /// MP3
        /// </summary>
        public override string FileType
        {
            get { return "MP3"; }
        }

        ///// <summary>
        ///// MPEG Layer 3
        ///// </summary>
        //public override string Description
        //{
        //    get { return "MPEG Layer 3"; }
        //}

        ///// <summary>
        ///// .mp3
        ///// </summary>
        //public override string DefaultExtension
        //{
        //    get { return ".mp3"; }
        //}

        /// <summary>
        /// Due to the nature of MP3 files this method only compares the extension of the file to "mp3"
        /// </summary>
        /// <param name="file"></param>
        /// <returns></returns>
        public override bool CheckFile(string file)
        {
            if (string.IsNullOrEmpty(file))
                return false;
            return file.ToLower().EndsWith("mp3");
        }

        /// <summary>
        /// <see cref="ID3Format.Mp3"/>
        /// </summary>
        public override ID3Format TagFormat
        {
            get { return ID3Format.Mp3; }
        }
    }

    /// <summary>
    /// Defines the MP4 Format
    /// </summary>
    public class MP4Format : AudioFormatBase
    {
        /// <summary>
        /// 0x00, 0x00, 0x00, 0x20, 0x66, 0x74, 0x79, 0x70, 0x4D, 0x34, 0x41, 0x20, 0x00, 0x00, 0x00, 0x00
        /// </summary>
        public override byte[] MagicNumber
        {
            get { return new byte[] { 0x00, 0x00, 0x00, 0x20, 0x66, 0x74, 0x79, 0x70, 0x4D, 0x34, 0x41, 0x20, 0x00, 0x00, 0x00, 0x00 }; }
        }

        /// <summary>
        /// MP4 / M4A
        /// </summary>
        public override string FileType
        {
            get { return "MP4 / M4A"; }
        }

        /// <summary>
        /// <see cref="ID3Format.M4A"/>
        /// </summary>
        public override ID3Format TagFormat
        {
            get { return ID3Format.M4A; }
        }
    }


    /// <summary>
    /// Defines the Wave "RIFF" format
    /// </summary>
    public class WAVFormat : AudioFormatBase
    {
        /// <summary>
        /// 0x52, 0x49, 0x46, 0x46
        /// </summary>
        public override byte[] MagicNumber
        {
            get { return new byte[] { 0x52, 0x49, 0x46, 0x46 }; }
        }

        /// <summary>
        /// Wav
        /// </summary>
        public override string FileType
        {
            get { return "Wav"; }
        }

        ///// <summary>
        ///// Standard RIFF Wave File
        ///// </summary>
        //public override string Description
        //{
        //    get { return "Standard RIFF Wave File"; }
        //}

        ///// <summary>
        ///// .wav
        ///// </summary>
        //public override string DefaultExtension
        //{
        //    get { return ".wav"; }
        //}

        /// <summary>
        /// <see cref="ID3Format.Wav"/>
        /// </summary>
        public override ID3Format TagFormat
        {
            get { return ID3Format.Wav; }
        }
    }

    /// <summary>
    /// Defines the Ogg / Vorbis format
    /// </summary>
    public class OGGFormat : AudioFormatBase
    {
        /// <summary>
        /// 0x4f, 0x67, 0x67, 0x53, 0x02, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00
        /// </summary>
        public override byte[] MagicNumber
        {
            get { return new byte[] { 0x4f, 0x67, 0x67, 0x53, 0x02, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00}; }
        }

        /// <summary>
        /// Ogg
        /// </summary>
        public override string FileType
        {
            get { return "Ogg"; }
        }

        ///// <summary>
        ///// Ogg Vorbis Codec compressed Multimedia file
        ///// </summary>
        //public override string Description
        //{
        //    get { return "Ogg Vorbis Codec compressed Multimedia file"; }
        //}

        ///// <summary>
        ///// .ogg
        ///// </summary>
        //public override string DefaultExtension
        //{
        //    get { return ".ogg"; }
        //}

        /// <summary>
        /// <see cref="ID3Format.Ogg"/>
        /// </summary>
        public override ID3Format TagFormat
        {
            get { return ID3Format.Ogg; }
        }
    }

    /// <summary>
    /// Defines the FLAC Format (Free Lossless audio codec)
    /// </summary>
    public class FlacFormat : AudioFormatBase
    {
        /// <summary>
        /// 0x66, 0x4C, 0x61, 0x43, 0x00, 0x00, 0x00, 0x22
        /// </summary>
        public override byte[] MagicNumber
        {
            get { return new byte[] { 0x66, 0x4C, 0x61, 0x43, 0x00, 0x00, 0x00, 0x22 }; }
        }

        /// <summary>
        /// flac
        /// </summary>
        public override string FileType
        {
            get { return "flac"; }
        }

        ///// <summary>
        ///// Free Lossless Audio Codec file
        ///// </summary>
        //public override string Description
        //{
        //    get { return "Free Lossless Audio Codec file"; }
        //}

        ///// <summary>
        ///// .flac
        ///// </summary>
        //public override string DefaultExtension
        //{
        //    get { return ".flac"; }
        //}

        /// <summary>
        /// <see cref="ID3Format.Flac"/>
        /// </summary>
        public override ID3Format TagFormat
        {
            get { return ID3Format.Flac; }
        }
    }

    /// <summary>
    /// Defines the ASF Format
    /// </summary>
    public class ASFFormat : AudioFormatBase
    {
        /// <summary>
        /// 0x30, 0x26, 0xB2, 0x75, 0x8E, 0x66, 0xCF, 0x11, 0xA6, 0xD9, 0x00, 0xAA, 0x00, 0x62, 0xCE, 0x6C
        /// </summary>
        public override byte[] MagicNumber
        {
            get { return new byte[] { 0x30, 0x26, 0xB2, 0x75, 0x8E, 0x66, 0xCF, 0x11, 0xA6, 0xD9, 0x00, 0xAA, 0x00, 0x62, 0xCE, 0x6C }; }
        }

        /// <summary>
        /// asf
        /// </summary>
        public override string FileType
        {
            get { return "asf"; }
        }

        ///// <summary>
        ///// Microsoft Windows Media Audio/Video File (Advanced Streaming Format)
        ///// </summary>
        //public override string Description
        //{
        //    get { return "Microsoft Windows Media Audio/Video File (Advanced Streaming Format)"; }
        //}

        ///// <summary>
        ///// .asf
        ///// </summary>
        //public override string DefaultExtension
        //{
        //    get { return ".asf"; }
        //}

        /// <summary>
        /// <see cref="ID3Format.Asf"/>
        /// </summary>
        public override ID3Format TagFormat
        {
            get { return ID3Format.Asf; }
        }
    }

    /// <summary>
    /// Defines the Musepack (SV7) format
    /// </summary>
    public class MusepackSV7Format : AudioFormatBase
    {
        /// <summary>
        /// 0x4D, 0x50, 0x2B
        /// </summary>
        public override byte[] MagicNumber
        {
            get { return new byte[] { 0x4D, 0x50, 0x2B }; }
        }

        /// <summary>
        /// mpc
        /// </summary>
        public override string FileType
        {
            get { return "mpc"; }
        }

        /// <summary>
        /// <see cref="ID3Format.Mpc"/>
        /// </summary>
        public override ID3Format TagFormat
        {
            get { return ID3Format.Mpc; }
        }
    }

    /// <summary>
    /// Defines the Musepack (SV8) format
    /// </summary>
    public class MuspackSV8Format : MusepackSV7Format
    {
        /// <summary>
        /// 0x4D, 0x50, 0x43, 0x4B
        /// </summary>
        public override byte[] MagicNumber
        {
            get { return new byte[] { 0x4D, 0x50, 0x43, 0x4B }; }
        }
    }
}