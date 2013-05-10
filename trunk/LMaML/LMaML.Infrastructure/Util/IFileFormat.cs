namespace LMaML.Infrastructure.Util
{
    /// <summary>
    /// An interface "describing" a file format
    /// </summary>
    public interface IFileFormat
    {
        /// <summary>
        /// Gets an array of bytes that can be found at the beginning of every file of this format (Magic Number)
        /// </summary>
        byte[] MagicNumber { get; }

        /// <summary>
        /// Gets a value indicating the file type's name
        /// </summary>
        string FileType { get; }

        ///// <summary>
        ///// Gets a description of this <see cref="IFileFormat"/>
        ///// </summary>
        //string Description { get; }

        ///// <summary>
        ///// Gets a value indicating what the default file-extension for files of this type is
        ///// </summary>
        //string DefaultExtension { get; }

        /// <summary>
        /// When implemented, checks a given file for the <see cref="IFileFormat.MagicNumber"/>
        /// </summary>
        /// <param name="file"></param>
        /// <returns></returns>
        bool CheckFile(string file);
    }
}