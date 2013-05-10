using LMaML.Infrastructure.Domain.Concrete;
using iLynx.Common;

namespace LMaML.Infrastructure
{
    /// <summary>
    /// IReferenceAdapters
    /// </summary>
    public interface IReferenceAdapters
    {
        /// <summary>
        /// Gets the artist adapter.
        /// </summary>
        /// <value>
        /// The artist adapter.
        /// </value>
        IDataAdapter<Artist> ArtistAdapter { get; }
        /// <summary>
        /// Gets the album adapter.
        /// </summary>
        /// <value>
        /// The album adapter.
        /// </value>
        IDataAdapter<Album>  AlbumAdapter { get; }
        /// <summary>
        /// Gets the genre adapter.
        /// </summary>
        /// <value>
        /// The genre adapter.
        /// </value>
        IDataAdapter<Genre> GenreAdapter { get; }
        /// <summary>
        /// Gets the title adapter.
        /// </summary>
        /// <value>
        /// The title adapter.
        /// </value>
        IDataAdapter<Title> TitleAdapter { get; }
        /// <summary>
        /// Gets the year adapter.
        /// </summary>
        /// <value>
        /// The year adapter.
        /// </value>
        IDataAdapter<Year> YearAdapter { get; }
    }
}
