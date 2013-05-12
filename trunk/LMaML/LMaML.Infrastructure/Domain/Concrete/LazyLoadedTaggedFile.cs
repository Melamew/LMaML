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
            AlbumId = original.AlbumId;
            ArtistId = original.ArtistId;
            Comment = original.Comment;
            Filename = original.Filename;
            GenreId = original.GenreId;
            Id = original.Id;
            TitleId = original.TitleId;
            YearId = original.YearId;
        }

        public override Album Album
        {
            get { return base.Album ?? (base.Album = adapters.AlbumAdapter.GetFirstById(AlbumId)); }
            set
            {
                base.Album = value;
            }
        }

        public override Artist Artist
        {
            get { return base.Artist ?? (base.Artist =  adapters.ArtistAdapter.GetFirstById(ArtistId)); }
            set
            {
                base.Artist = value;
            }
        }

        public override Genre Genre
        {
            get { return base.Genre ?? (base.Genre = adapters.GenreAdapter.GetFirstById(GenreId)); }
            set
            {
                base.Genre = value;
            }
        }

        public override Title Title
        {
            get { return base.Title ?? (base.Title = adapters.TitleAdapter.GetFirstById(TitleId)); }
            set
            {
                base.Title = value;
            }
        }

        public override Year Year
        {
            get { return base.Year ?? (base.Year = adapters.YearAdapter.GetFirstById(YearId)); }
            set
            {
                base.Year = value;
            }
        }
    }
}