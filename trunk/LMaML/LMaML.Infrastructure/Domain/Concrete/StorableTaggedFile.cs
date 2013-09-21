using System;
using System.Globalization;
using System.IO;
using LMaML.Infrastructure.Audio;
using iLynx.Common;

namespace LMaML.Infrastructure.Domain.Concrete
{
    /// <summary>
    /// StorableTaggedFile
    /// </summary>
    public class StorableTaggedFile : ILibraryEntity
    {
        private const string Unknown = "Unknown";
        public static readonly Guid ArtistNamespace = new Guid("EBD8CA02-9FA2-40CA-884A-8DE5E7CB9AD8");
        public static readonly Guid GenreNamespace = new Guid("6F7E3A3D-70A5-4C5E-B2ED-799EBA4D377A");
        public static readonly Guid YearNamespace = new Guid("40105278-E5F8-41FE-9ACC-E68DF329B7A3");
        public static readonly Guid AlbumNamespace = new Guid("4C6810D2-51CB-4EE9-A4BB-5862CCAC7750");
        public static readonly Guid TitleNamespace = new Guid("649E6BB7-7961-4DB6-A1CA-B15A58581A5A");
        public static readonly Guid FilenameNamespace = new Guid("363CB8DC-C3DB-4122-B96B-75F4F3A128BC");
        protected bool WasFullyLoaded = false;

        private Year year;
        private Genre genre;
        private Album album;
        private Title title;
        private Artist artist;

        /// <summary>
        ///     Copies values from a specified <see cref="ID3File" /> to a new instance of <see cref="StorableTaggedFile" />
        /// </summary>
        /// <param name="file"></param>
        /// <returns></returns>
        public static StorableTaggedFile Copy(ID3File file)
        {
            var alb = file.Album.Trim();
            alb = string.IsNullOrEmpty(alb) ? Unknown : alb;
            var art = file.Artist.Trim();
            art = string.IsNullOrEmpty(art) ? Unknown : art;
            var gen = file.Genre.Trim();
            gen = string.IsNullOrEmpty(gen) ? Unknown : gen;
            var tit = file.Title.Trim();
            tit = string.IsNullOrEmpty(tit) ? Unknown : tit;
            var artistId = GenerateLowerCaseId(art, ArtistNamespace);
            var albumId = GenerateLowerCaseId(alb, AlbumNamespace);
            var genreId = GenerateLowerCaseId(gen, GenreNamespace);
            var titleId = GenerateLowerCaseId(tit, TitleNamespace);
            var yearId = GenerateLowerCaseId(file.Year.ToString(CultureInfo.InvariantCulture), YearNamespace);
            var filename = file.Filename;
            return new StorableTaggedFile
                       {
                           Id = GenerateLowerCaseId(filename, FilenameNamespace),
                           Comment = file.Comment,
                           Filename = filename,
                           TrackNo = file.TrackNo,
                           Duration = file.Duration,
                           Album = new Album
                           {
                               Name = alb,
                               Id = albumId,
                           },
                           Artist = new Artist
                           {
                               Name = art,
                               Id = artistId,
                           },
                           Genre = new Genre
                           {
                               Name = gen,
                               Id = genreId,
                           },
                           Title = new Title
                           {
                               Name = tit,
                               Id = titleId,
                           },
                           Year = new Year
                           {
                               Value = file.Year,
                               Id = yearId,
                           },
                       };
        }

        public virtual TimeSpan Duration { get; set; }

        /// <summary>
        /// Loads the references of this file.
        /// </summary>
        /// <param name="referenceAdapters">The reference adapters.</param>
        /// <returns></returns>
        public virtual StorableTaggedFile LoadReferences(IReferenceAdapters referenceAdapters)
        {
            referenceAdapters.Guard("referenceAdapters");
            Album = referenceAdapters.AlbumAdapter.GetFirst(a => a.Id == AlbumId) ?? Album;
            Genre = referenceAdapters.GenreAdapter.GetFirst(a => a.Id == GenreId) ?? Genre;
            Title = referenceAdapters.TitleAdapter.GetFirst(a => a.Id == TitleId) ?? Title;
            Artist = referenceAdapters.ArtistAdapter.GetFirst(a => a.Id == ArtistId) ?? Artist;
            Year = referenceAdapters.YearAdapter.GetFirst(a => a.Id == YearId) ?? Year;
            WasFullyLoaded = true;
            return this;
        }

        /// <summary>
        /// Lazily loads the references of this file (NOTE: Should not be used with storage backends that require the same instance to be persisted after it has been loaded).
        /// </summary>
        /// <param name="referenceAdapters">The reference adapters.</param>
        /// <returns></returns>
        public virtual StorableTaggedFile LazyLoadReferences(IReferenceAdapters referenceAdapters)
        {
            return new LazyLoadedTaggedFile(referenceAdapters, this);
        }

        /// <summary>
        /// Generates the id.
        /// </summary>
        /// <param name="basedOn">The based on.</param>
        /// <param name="space">The space.</param>
        /// <returns></returns>
        public static Guid GenerateLowerCaseId(string basedOn, Guid space)
        {
            return (basedOn ?? string.Empty).ToLower().CreateGuidV5(space);
        }

        /// <summary>
        /// Gets or sets the id.
        /// </summary>
        /// <value>
        /// The id.
        /// </value>
        public virtual Guid Id { get; set; }

        /// <summary>
        /// Copies the specified from.
        /// </summary>
        /// <param name="source">From.</param>
        /// <param name="directIds">If this value is set to true, take the 'TagReferenceId' fields and copy them directly, if false, try to take the 'TagReference.Id' fields instead.</param>
        /// <returns></returns>
        public static StorableTaggedFile Copy(StorableTaggedFile source, bool directIds = true)
        {
            return new StorableTaggedFile
                       {
                           Comment = source.Comment,
                           Filename = source.Filename,
                           TrackNo = source.TrackNo,
                           Id = source.Id,
                           Title = source.Title,
                           Duration = source.Duration,
                           TitleId = directIds || null == source.Title ? source.TitleId : source.Title.Id,
                           Album = source.Album,
                           AlbumId = directIds || null == source.Album ? source.AlbumId : source.Album.Id,
                           Artist = source.Artist,
                           ArtistId = directIds || null == source.Artist ? source.ArtistId : source.Artist.Id,
                           Genre = source.Genre,
                           GenreId = directIds || null == source.Genre ? source.GenreId : source.Genre.Id,
                           Year = source.Year,
                           YearId = directIds || null == source.Year ? source.YearId : source.Year.Id,
                       };
        }

        /// <summary>
        ///     Gets or Sets the track number of this file
        /// </summary>
        public virtual uint TrackNo { get; set; }

        /// <summary>
        ///     Gets or Sets the ID3 comment of this file
        /// </summary>
        public virtual string Comment { get; set; }

        /// <summary>
        ///     Gets or Sets the release year of this file / album
        /// </summary>
        public virtual Year Year
        {
            get { return year; }
            set
            {
                year = value;
                YearId = null == value ? Guid.Empty : value.Id;
            }
        }

        /// <summary>
        /// Gets or sets the year id.
        /// </summary>
        /// <value>
        /// The year id.
        /// </value>
        public virtual Guid YearId { get; set; }

        /// <summary>
        ///     Gets or Sets the genre of this file
        /// </summary>
        public virtual Genre Genre
        {
            get { return genre; }
            set
            {
                genre = value;
                GenreId = value == null ? Guid.Empty : genre.Id;
            }
        }

        /// <summary>
        /// Gets or sets the genre id.
        /// </summary>
        /// <value>
        /// The genre id.
        /// </value>
        public virtual Guid GenreId { get; set; }

        /// <summary>
        ///     Gets or Sets the album that this file belongs to
        /// </summary>
        public virtual Album Album
        {
            get { return album; }
            set
            {
                album = value;
                AlbumId = null == value ? Guid.Empty : value.Id;
            }
        }

        /// <summary>
        /// Gets or sets the album id.
        /// </summary>
        /// <value>
        /// The album id.
        /// </value>
        public virtual Guid AlbumId { get; set; }

        /// <summary>
        ///     Gets or Sets the title of this file
        /// </summary>
        public virtual Title Title
        {
            get { return title; }
            set
            {
                title = value;
                TitleId = null == value ? Guid.Empty : value.Id;
            }
        }

        /// <summary>
        /// Gets or sets the title id.
        /// </summary>
        /// <value>
        /// The title id.
        /// </value>
        public virtual Guid TitleId { get; set; }

        /// <summary>
        ///     Gets or Sets the artist of this file
        /// </summary>
        public virtual Artist Artist
        {
            get { return artist; }
            set
            {
                artist = value;
                ArtistId = null == value ? Guid.Empty : value.Id;
            }
        }

        /// <summary>
        /// Gets or sets the artist id.
        /// </summary>
        /// <value>
        /// The artist id.
        /// </value>
        public virtual Guid ArtistId { get; set; }

        /// <summary>
        ///     Gets or Sets the physical filename of this file
        /// </summary>
        public virtual string Filename { get; set; }

        /// <summary>
        /// Returns a <see cref="System.String" /> that represents this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="System.String" /> that represents this instance.
        /// </returns>
        public override string ToString()
        {
            var possiblyUnloaded = (null == Title && null == Artist);
            if (possiblyUnloaded)
                return "LOADME!";
            var tit = IsUnknown(null == Title ? Unknown : Title.Name) ? Path.GetFileName(Filename) : Title == null ? Unknown : Title.Name;
            var art = Artist == null ? Unknown : Artist.Name;
            return String.Format("{0} - {1}", art, tit);
        }

        /// <summary>
        /// Determines whether the specified value is unknown.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>
        ///   <c>true</c> if the specified value is unknown; otherwise, <c>false</c>.
        /// </returns>
        private static bool IsUnknown(string value)
        {
            if (null == value) return true;
            return value.Trim().ToLowerInvariant() == Unknown.Trim().ToLowerInvariant();
        }
    }
}