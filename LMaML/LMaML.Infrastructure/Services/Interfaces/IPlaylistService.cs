using System.Collections.Generic;
using LMaML.Infrastructure.Audio;
using LMaML.Infrastructure.Domain;
using LMaML.Infrastructure.Domain.Concrete;

namespace LMaML.Infrastructure.Services.Interfaces
{
    /// <summary>
    /// IPlaylistService
    /// </summary>
    public interface IPlaylistService
    {
        /// <summary>
        /// Clears this instance.
        /// </summary>
        void Clear();

        /// <summary>
        /// Adds the file.
        /// </summary>
        /// <param name="file">The file.</param>
        void AddFile(StorableTaggedFile file);

        /// <summary>
        /// Adds the files.
        /// </summary>
        /// <param name="newFiles">The files.</param>
        void AddFiles(IEnumerable<StorableTaggedFile> newFiles);

        /// <summary>
        /// Removes the file.
        /// </summary>
        /// <param name="file">The file.</param>
        void RemoveFile(StorableTaggedFile file);

        /// <summary>
        /// Removes the files.
        /// </summary>
        /// <param name="oldFiles">The files.</param>
        void RemoveFiles(IEnumerable<StorableTaggedFile> oldFiles);

        /// <summary>
        /// Gets the files.
        /// </summary>
        /// <value>
        /// The files.
        /// </value>
        IEnumerable<StorableTaggedFile> Files { get; }

        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="IPlaylistService" /> is shuffle.
        /// </summary>
        /// <value>
        ///   <c>true</c> if shuffle; otherwise, <c>false</c>.
        /// </value>
        bool Shuffle { get; set; }

        ///// <summary>
        ///// Previouses this instance.
        ///// </summary>
        ///// <returns></returns>
        //StorableID3File Previous();

        /// <summary>
        /// Nexts this instance.
        /// </summary>
        /// <returns></returns>
        StorableTaggedFile Next();
    }
}
