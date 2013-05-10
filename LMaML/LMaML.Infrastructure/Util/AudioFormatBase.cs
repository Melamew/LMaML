using System.IO;

namespace LMaML.Infrastructure.Util
{
    /// <summary>
    /// <see cref="IAudioFormat"/>
    /// </summary>
    public abstract class AudioFormatBase : IAudioFormat
    {
        /// <summary>
        /// <see cref="IFileFormat"/>
        /// </summary>
        public abstract byte[] MagicNumber
        {
            get;
        }

        /// <summary>
        /// <see cref="IFileFormat"/>
        /// </summary>
        public abstract string FileType
        {
            get;
        }

        /// <summary>
        /// <see cref="IAudioFormat"/>
        /// </summary>
        public abstract ID3Format TagFormat
        {
            get;
        }

        /// <summary>
        /// Opens the specified file (if it exists) and reads a few bytes from the file to determine wether or not it matches the <see cref="IFileFormat.MagicNumber"/> of this format
        /// </summary>
        public virtual bool CheckFile(string file)
        {
            if (MagicNumber == null || MagicNumber.Length < 1)
                return false;
            if (!File.Exists(file))
                return false;
            FileStream stream;
            try { stream = File.OpenRead(file); }
            catch { return false; }
            bool retVal = true;
            try
            {
                byte[] chunk = new byte[MagicNumber.Length];
                int read = stream.Read(chunk, 0, chunk.Length);
                if (read != chunk.Length)
                    retVal = false;
                else
                {
                    for (int i = 0; i < chunk.Length; i++)
                        retVal &= chunk[i] == MagicNumber[i];
                }
            }
            catch { }
            finally { stream.Close(); }
            return retVal;
        }
    }
}