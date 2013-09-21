using iLynx.Common;

namespace LMaML.Infrastructure.Domain.Concrete
{
    /// <summary>
    /// LazyLoadedTaggedFile
    /// </summary>
    public sealed class LazyLoadedTaggedFile : StorableTaggedFile
    {
        private readonly IReferenceAdapters adapters;

        /// <summary>
        /// Initializes a new instance of the <see cref="LazyLoadedTaggedFile" /> class.
        /// </summary>
        /// <param name="adapters">The adapters.</param>
        /// <param name="original">The original.</param>
        public LazyLoadedTaggedFile(IReferenceAdapters adapters, StorableTaggedFile original)
        {
            adapters.Guard("adapters");
            original.Guard("original");
            this.adapters = adapters;
            Duration = original.Duration;
            AlbumId = original.AlbumId;
            ArtistId = original.ArtistId;
            Comment = original.Comment;
            Filename = original.Filename;
            GenreId = original.GenreId;
            Id = original.Id;
            TitleId = original.TitleId;
            YearId = original.YearId;
        }

        /// <summary>
        /// Gets or Sets the album that this file belongs to
        /// </summary>
        public override Album Album
        {
            get { return WasFullyLoaded ? base.Album : base.Album ?? (base.Album = adapters.AlbumAdapter.GetFirstById(AlbumId)); }
            set
            {
                base.Album = value;
            }
        }

        /// <summary>
        /// Gets or Sets the artist of this file
        /// </summary>
        public override Artist Artist
        {
            get { return WasFullyLoaded ? base.Artist : base.Artist ?? (base.Artist =  adapters.ArtistAdapter.GetFirstById(ArtistId)); }
            set
            {
                base.Artist = value;
            }
        }

        /// <summary>
        /// Gets or Sets the genre of this file
        /// </summary>
        public override Genre Genre
        {
            get { return WasFullyLoaded ? base.Genre : base.Genre ?? (base.Genre = adapters.GenreAdapter.GetFirstById(GenreId)); }
            set
            {
                base.Genre = value;
            }
        }

        /// <summary>
        /// Gets or Sets the title of this file
        /// </summary>
        public override Title Title
        {
            get { return WasFullyLoaded ? base.Title : base.Title ?? (base.Title = adapters.TitleAdapter.GetFirstById(TitleId)); }
            set
            {
                base.Title = value;
            }
        }

        /// <summary>
        /// Gets or Sets the release year of this file / album
        /// </summary>
        public override Year Year
        {
            get { return WasFullyLoaded ? base.Year : base.Year ?? (base.Year = adapters.YearAdapter.GetFirstById(YearId)); }
            set
            {
                base.Year = value;
            }
        }
    }
}