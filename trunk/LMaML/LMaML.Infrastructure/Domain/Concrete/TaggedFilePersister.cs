using System;
using System.Collections.Generic;
using System.Diagnostics;
using iLynx.Common;
using iLynx.Common.Configuration;

namespace LMaML.Infrastructure.Domain.Concrete
{
    /// <summary>
    /// StorableID3FileStorer
    /// </summary>
    public class TaggedFilePersister : IDataPersister<StorableTaggedFile>
    {
        private readonly IDataAdapter<StorableTaggedFile> fileAdapter;
        private readonly IReferenceAdapters referenceAdapters;
        private readonly Dictionary<Guid, Artist> artistCache = new Dictionary<Guid, Artist>();
        private readonly Dictionary<Guid, Genre> genreCache = new Dictionary<Guid, Genre>();
        private readonly Dictionary<Guid, Year> yeareCache = new Dictionary<Guid, Year>();
        private readonly Dictionary<Guid, Album> albumCache = new Dictionary<Guid, Album>();
        private readonly Dictionary<Guid, Title> titleCache = new Dictionary<Guid, Title>();
        private readonly IConfigurableValue<int> maxCacheSize;

        /// <summary>
        /// Initializes a new instance of the <see cref="TaggedFilePersister" /> class.
        /// </summary>
        /// <param name="fileAdapter">The file adapter.</param>
        /// <param name="referenceAdapters">The reference adapters.</param>
        /// <param name="configurationManager">The configuration manager.</param>
        public TaggedFilePersister(IDataAdapter<StorableTaggedFile> fileAdapter,
            IReferenceAdapters referenceAdapters,
            IConfigurationManager configurationManager)
        {
            fileAdapter.Guard("fileAdapter");
            referenceAdapters.Guard("referenceAdapters");
            configurationManager.Guard("configurationManager");
            this.fileAdapter = fileAdapter;
            this.referenceAdapters = referenceAdapters;
            maxCacheSize = configurationManager.GetValue("StorableTaggedFilePersister.MaxCacheItems", 200);
            CreateIndices();
        }

        private void CreateIndices()
        {
            var b = new TagReference();
            var idField = RuntimeHelper.GetPropertyName(() => b.Id);
            fileAdapter.CreateIndex(idField);
            referenceAdapters.ArtistAdapter.CreateIndex(idField);
            referenceAdapters.YearAdapter.CreateIndex(idField);
            referenceAdapters.GenreAdapter.CreateIndex(idField);
            referenceAdapters.AlbumAdapter.CreateIndex(idField);
            referenceAdapters.TitleAdapter.CreateIndex(idField);
            var instance = new StorableTaggedFile();
            fileAdapter.CreateIndex(RuntimeHelper.GetPropertyName(() => instance.GenreId),
                                    RuntimeHelper.GetPropertyName(() => instance.AlbumId),
                                    RuntimeHelper.GetPropertyName(() => instance.ArtistId),
                                    RuntimeHelper.GetPropertyName(() => instance.TitleId),
                                    RuntimeHelper.GetPropertyName(() => instance.YearId));
        }

#if DEBUG
        private int persisted;
        private TimeSpan totalTime;
        private DateTime lastTime = DateTime.Now;
#endif

        /// <summary>
        /// Stores the specified value.
        /// </summary>
        /// <param name="value">The value.</param>
        public void Save(StorableTaggedFile value)
        {
#if DEBUG
            var sw = Stopwatch.StartNew();
#endif
            var existingFile = fileAdapter.GetFirstById(value.Id);
            if (null != existingFile)
            {
                existingFile = existingFile.LoadReferences(referenceAdapters);
                UpdateConditional(existingFile, value);
                value = existingFile;
                fileAdapter.Delete(existingFile);
            }
            else
            {
                value.Title = GetTitle(value);
                value.Album = GetAlbum(value);
                value.Genre = GetGenre(value);
                value.Artist = GetArtist(value);
                value.Year = GetYear(value);
            }
            fileAdapter.Save(value);
            CommitToCache(value);
#if DEBUG
            sw.Stop();
            ++persisted;
            totalTime += sw.Elapsed;
            if (DateTime.Now - lastTime < TimeSpan.FromSeconds(2)) return;
            Trace.WriteLine(string.Format("Persister: ~{0} Items/sec", persisted / totalTime.TotalSeconds));
            lastTime = DateTime.Now;
#endif
        }

        private void CommitToCache(StorableTaggedFile file)
        {
            if (null == maxCacheSize) return;
            if (yeareCache.Count >= maxCacheSize.Value) yeareCache.Clear();
            if (albumCache.Count >= maxCacheSize.Value) albumCache.Clear();
            if (genreCache.Count >= maxCacheSize.Value) genreCache.Clear();
            if (artistCache.Count >= maxCacheSize.Value) artistCache.Clear();
            if (titleCache.Count >= maxCacheSize.Value) titleCache.Clear();
            if (!yeareCache.ContainsKey(file.Year.Id))
                yeareCache.Add(file.Year.Id, file.Year);
            if (!albumCache.ContainsKey(file.Album.Id))
                albumCache.Add(file.Album.Id, file.Album);
            if (!genreCache.ContainsKey(file.Genre.Id))
                genreCache.Add(file.Genre.Id, file.Genre);
            if (!artistCache.ContainsKey(file.Artist.Id))
                artistCache.Add(file.Artist.Id, file.Artist);
            if (!titleCache.ContainsKey(file.Title.Id))
                titleCache.Add(file.Title.Id, file.Title);
        }

        /// <summary>
        /// Saves the or update.
        /// </summary>
        /// <param name="value">The value.</param>
        public void SaveOrUpdate(StorableTaggedFile value)
        {
            Save(value);
        }

        private void UpdateConditional(StorableTaggedFile target, StorableTaggedFile file)
        {
            if (null == target.Album || target.Album.Id != file.Album.Id)
                target.Album = GetAlbum(file);
            if (null == target.Genre || target.Genre.Id != file.Genre.Id)
                target.Genre = GetGenre(file);
            if (null == target.Artist || target.Artist.Id != file.Artist.Id)
                target.Artist = GetArtist(file);
            if (null == target.Year || target.Year.Value != file.Year.Value)
                target.Year = GetYear(file);
            if (null == target.Title || target.Title.Id != file.Title.Id)
                target.Title = GetTitle(file);
            target.Comment = file.Comment;
            target.Filename = file.Filename;
            target.Title = file.Title;
            target.TrackNo = file.TrackNo;
        }

        private Title GetTitle(StorableTaggedFile file)
        {
            var id = StorableTaggedFile.GenerateLowerCaseId(file.Title.Name, StorableTaggedFile.TitleNamespace);
            Title result;
            if (!titleCache.TryGetValue(id, out result))
                result = referenceAdapters.TitleAdapter.GetFirstById(id);
            if (null == result)
            {
                result = file.Title;
                referenceAdapters.TitleAdapter.Save(result);
            }
            return result;
        }

        /// <summary>
        /// Transacts the specified a.
        /// </summary>
        /// <param name="a">A.</param>
        /// <param name="flushAfter">if set to <c>true</c> [flush after].</param>
        public void Transact(Action a, bool flushAfter)
        {
            fileAdapter.Transact(a, flushAfter);
        }

        /// <summary>
        /// Creates the index.
        /// </summary>
        /// <param name="fields">The fields.</param>
        public void CreateIndex(params string[] fields)
        {
            fileAdapter.CreateIndex(fields);
        }

        public void BulkInsert(IEnumerable<StorableTaggedFile> data)
        {
            fileAdapter.BulkInsert(data);
        }

        /// <summary>
        /// Gets the album.
        /// </summary>
        /// <param name="file">The file.</param>
        /// <returns></returns>
        private Album GetAlbum(StorableTaggedFile file)
        {
            var id = StorableTaggedFile.GenerateLowerCaseId(file.Album.Name, StorableTaggedFile.AlbumNamespace);
            Album result;
            if (!albumCache.TryGetValue(id, out result))
                result = referenceAdapters.AlbumAdapter.GetFirstById(id);
            if (null == result)
            {
                result = file.Album;
                referenceAdapters.AlbumAdapter.Save(result);
            }
            return result;
        }

        /// <summary>
        /// Gets the year.
        /// </summary>
        /// <param name="file">The file.</param>
        /// <returns></returns>
        private Year GetYear(StorableTaggedFile file)
        {
            var id = StorableTaggedFile.GenerateLowerCaseId(file.Year.Name, StorableTaggedFile.YearNamespace);
            Year result;
            if (!yeareCache.TryGetValue(id, out result))
                result = referenceAdapters.YearAdapter.GetFirstById(id);
            if (null == result)
            {
                result = file.Year;
                referenceAdapters.YearAdapter.Save(result);
            }
            return result;
        }

        /// <summary>
        /// Gets the genre.
        /// </summary>
        /// <param name="file">The file.</param>
        /// <returns></returns>
        private Genre GetGenre(StorableTaggedFile file)
        {
            var id = StorableTaggedFile.GenerateLowerCaseId(file.Genre.Name, StorableTaggedFile.GenreNamespace);
            Genre result;
            if (!genreCache.TryGetValue(id, out result))
                result = referenceAdapters.GenreAdapter.GetFirstById(id);
            if (null == result)
            {
                result = file.Genre;
                referenceAdapters.GenreAdapter.Save(result);
            }
            return result;
        }

        /// <summary>
        /// Gets the artist.
        /// </summary>
        /// <param name="file">The file.</param>
        /// <returns></returns>
        private Artist GetArtist(StorableTaggedFile file)
        {
            var id = StorableTaggedFile.GenerateLowerCaseId(file.Artist.Name, StorableTaggedFile.ArtistNamespace);
            Artist result;
            if (!artistCache.TryGetValue(id, out result))
                result = referenceAdapters.ArtistAdapter.GetFirstById(id);
            if (null == result)
            {
                result = file.Artist;
                referenceAdapters.ArtistAdapter.Save(result);
            }
            return result;
        }
    }
}