using System;
using LMaML.Infrastructure.Domain.Concrete;

namespace LMaML.Infrastructure.Events
{
    /// <summary>
    /// TrackChangedEvent
    /// </summary>
    public class TrackChangedEvent : IApplicationEvent
    {
        /// <summary>
        /// Gets or sets the file.
        /// </summary>
        /// <value>
        /// The file.
        /// </value>
        public StorableTaggedFile File { get; private set; }

        /// <summary>
        /// Gets the length of the song.
        /// </summary>
        /// <value>
        /// The length of the song.
        /// </value>
        public TimeSpan SongLength { get; private set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="TrackChangedEvent" /> class.
        /// </summary>
        /// <param name="file">The file.</param>
        /// <param name="songLength">Length of the song.</param>
        public TrackChangedEvent(StorableTaggedFile file, TimeSpan songLength)
        {
            File = file;
            SongLength = songLength;
        }
    }
}
