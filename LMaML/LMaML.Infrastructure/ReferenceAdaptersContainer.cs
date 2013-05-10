using LMaML.Infrastructure.Domain.Concrete;
using iLynx.Common;

namespace LMaML.Infrastructure
{
    /// <summary>
    /// ReferenceAdaptersContainer
    /// </summary>
    public class ReferenceAdaptersContainer : IReferenceAdapters
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ReferenceAdaptersContainer" /> class.
        /// </summary>
        /// <param name="artistAdapter">The artist adapter.</param>
        /// <param name="albumAdapter">The album adapter.</param>
        /// <param name="genreAdapter">The genre adapter.</param>
        /// <param name="titleAdapter">The title adapter.</param>
        /// <param name="yearAdapter">The year adapter.</param>
        public ReferenceAdaptersContainer(IDataAdapter<Artist> artistAdapter, IDataAdapter<Album> albumAdapter, IDataAdapter<Genre> genreAdapter, IDataAdapter<Title> titleAdapter, IDataAdapter<Year> yearAdapter)
        {
            artistAdapter.Guard("artistAdapter");
            albumAdapter.Guard("albumAdapter");
            genreAdapter.Guard("genreAdapter");
            titleAdapter.Guard("titleAdapter");
            yearAdapter.Guard("yearAdapter");
            YearAdapter = yearAdapter;
            TitleAdapter = titleAdapter;
            GenreAdapter = genreAdapter;
            AlbumAdapter = albumAdapter;
            ArtistAdapter = artistAdapter;
        }

        /// <summary>
        /// Gets the artist adapter.
        /// </summary>
        /// <value>
        /// The artist adapter.
        /// </value>
        public IDataAdapter<Artist> ArtistAdapter { get; private set; }
        /// <summary>
        /// Gets the album adapter.
        /// </summary>
        /// <value>
        /// The album adapter.
        /// </value>
        public IDataAdapter<Album> AlbumAdapter { get; private set; }
        /// <summary>
        /// Gets the genre adapter.
        /// </summary>
        /// <value>
        /// The genre adapter.
        /// </value>
        public IDataAdapter<Genre> GenreAdapter { get; private set; }
        /// <summary>
        /// Gets the title adapter.
        /// </summary>
        /// <value>
        /// The title adapter.
        /// </value>
        public IDataAdapter<Title> TitleAdapter { get; private set; }
        /// <summary>
        /// Gets the year adapter.
        /// </summary>
        /// <value>
        /// The year adapter.
        /// </value>
        public IDataAdapter<Year> YearAdapter { get; private set; }
    }
}
