using System.IO;

namespace LMaML.Infrastructure.Util
{
    /// <summary>
    /// An abstract class that is simply a wrapper for IFileFormat with the <see cref="IFileFormat.CheckFile(string)"/> method implemented
    /// </summary>
    public abstract class FileFormatBase : IFileFormat
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
        /// <see cref="IFileFormat"/>
        /// </summary>
        public abstract string Description
        {
            get;
        }

        /// <summary>
        /// <see cref="IFileFormat"/>
        /// </summary>
        public abstract string DefaultExtension
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
            var retVal = true;
            try
            {
                var chunk = new byte[MagicNumber.Length];
                var read = stream.Read(chunk, 0, chunk.Length);
                if (read != chunk.Length)
                    retVal = false;
                else
                {
                    for (var i = 0; i < chunk.Length; i++)
                        retVal &= chunk[i] == MagicNumber[i];
                }
            }
            finally { stream.Close(); }
            return retVal;
        }
    }
}