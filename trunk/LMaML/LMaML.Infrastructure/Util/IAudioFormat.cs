namespace LMaML.Infrastructure.Util
{
    /// <summary>
    /// An interface that extends <see cref="IFileFormat"/> and currently simply describes which <see cref="TagFormat"/> this format belongs to
    /// </summary>
    public interface IAudioFormat : IFileFormat
    {
        /// <summary>
        /// The <see cref="TagFormat"/> of this format
        /// </summary>
        ID3Format TagFormat { get; }
    }
}