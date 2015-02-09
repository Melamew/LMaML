using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using iLynx.Common.DataAccess;
using iLynx.Configuration;
using iLynx.Threading;
using LMaML.Infrastructure;
using LMaML.Infrastructure.Domain.Concrete;
using LMaML.Infrastructure.Events;
using LMaML.Infrastructure.Services.Interfaces;
using iLynx.Common;

namespace LMaML.Services
{
    public class PlaylistService : IPlaylistService
    {
        private readonly IPublicTransport publicTransport;
        private readonly IReferenceAdapters referenceAdapters;
        private readonly IThreadManager threadManager;
        private readonly IDataAdapter<StorableTaggedFile> fileAdapter;
        private List<StorableTaggedFile> files = new List<StorableTaggedFile>();
        private int currentIndex;
        private volatile bool canLoad = true;
        private readonly IConfigurableValue<Guid[]> playList;
        private readonly IConfigurableValue<bool> shuffleValue;

        /// <summary>
        /// Initializes a new instance of the <see cref="PlaylistService" /> class.
        /// </summary>
        /// <param name="publicTransport">The public transport.</param>
        /// <param name="referenceAdapters">The reference adapters.</param>
        /// <param name="threadManager">The thread manager.</param>
        /// <param name="configurationManager">The configuration manager.</param>
        /// <param name="fileAdapter"></param>
        public PlaylistService(IPublicTransport publicTransport,
                               IReferenceAdapters referenceAdapters,
                               IThreadManager threadManager,
                               IConfigurationManager configurationManager,
                               IDataAdapter<StorableTaggedFile> fileAdapter)
        {
            publicTransport.Guard("publicTransport");
            referenceAdapters.Guard("referenceAdapters");
            threadManager.Guard("threadManager");
            this.publicTransport = publicTransport;
            this.referenceAdapters = referenceAdapters;
            this.threadManager = threadManager;
            this.fileAdapter = fileAdapter;
            playList = configurationManager.GetValue("Playlist.LastPlaylist", new Guid[0], KnownConfigSections.Hidden);
            LoadPlaylist(playList.Value);
            shuffleValue = configurationManager.GetValue("Playlist.Shuffle", false, KnownConfigSections.Hidden);
            shuffleValue.ValueChanged += ShuffleValueOnValueChanged;
            publicTransport.ApplicationEventBus.Subscribe<ShutdownEvent>(OnShutdown);
        }

        private void LoadPlaylist(IEnumerable<Guid> ids)
        {
            AddFiles(ids.AsQueryable().Join(fileAdapter.Query(), guid => guid, file => file.Id, (guid, file) => file).Select(x => x.LazyLoadReferences(referenceAdapters)));
        }

        private void ShuffleValueOnValueChanged(object sender, ValueChangedEventArgs<bool> valueChangedEventArgs)
        {
            publicTransport.ApplicationEventBus.Send(new ShuffleChangedEvent(valueChangedEventArgs.NewValue));
        }

        private void OnShutdown(ShutdownEvent shutdownEvent)
        {
            canLoad = false;
            playList.Value = Files.Select(x => x.Id).ToArray();
        }

        private void Load(IEnumerable<StorableTaggedFile> fs)
        {
            this.LogInformation("Starting file load");
            var cnt = 0;
            var sw = Stopwatch.StartNew();
            foreach (var x in fs.TakeWhile(x => canLoad))
            {
                x.LoadReferences(referenceAdapters);
                ++cnt;
            }
            sw.Stop();
            if (!canLoad) return;
            this.LogInformation("Finished loading {0} files in {1} seconds", cnt, sw.Elapsed.TotalSeconds);
        }

        /// <summary>
        /// Adds the file.
        /// </summary>
        /// <param name="file">The file.</param>
        public void AddFile(StorableTaggedFile file)
        {
            file.LoadReferences(referenceAdapters);
            lock (files)
                files.Add(file);
            publicTransport.ApplicationEventBus.Send(new PlaylistUpdatedEvent());
        }

        /// <summary>
        /// Adds the files.
        /// </summary>
        /// <param name="newFiles">The files.</param>
        public void AddFiles(IEnumerable<StorableTaggedFile> newFiles)
        {
            var fs = newFiles.ToArray();
            lock (files)
                files.AddRange(fs);
            threadManager.StartNew(Load, fs);
            publicTransport.ApplicationEventBus.Send(new PlaylistUpdatedEvent());
        }

        /// <summary>
        /// Removes the file.
        /// </summary>
        /// <param name="file">The file.</param>
        public void RemoveFile(StorableTaggedFile file)
        {
            lock (files)
                files.Remove(file);
            publicTransport.ApplicationEventBus.Send(new PlaylistUpdatedEvent());
        }

        /// <summary>
        /// Removes the files.
        /// </summary>
        /// <param name="oldFiles">The files.</param>
        public void RemoveFiles(IEnumerable<StorableTaggedFile> oldFiles)
        {
            lock (files)
                files.RemoveRange(oldFiles);
            publicTransport.ApplicationEventBus.Send(new PlaylistUpdatedEvent());
        }

        private static readonly Random Rnd = new Random();

        /// <summary>
        /// Gets the random.
        /// </summary>
        /// <returns></returns>
        public StorableTaggedFile GetRandom()
        {
            lock (files)
            {
                var index = Rnd.Next(0, files.Count);
                if (index < 0 || index >= files.Count)
                    return null;
                currentIndex = index;
                return files[currentIndex++];
            }
        }

        /// <summary>
        /// Clears this instance.
        /// </summary>
        public void Clear()
        {
            lock (files)
                files.Clear();
        }

        /// <summary>
        /// Gets the files.
        /// </summary>
        /// <value>
        /// The files.
        /// </value>
        public List<StorableTaggedFile> Files
        {
            set
            {
                lock (files)
                    files = value;
                publicTransport.ApplicationEventBus.Send(new PlaylistUpdatedEvent());
            }
            get { return files; }
        }

        /// <summary>
        /// Orders the by async.
        /// </summary>
        /// <typeparam name="TKey">The type of the key.</typeparam>
        /// <param name="predicate">The predicate.</param>
        /// <returns></returns>
        public async Task OrderByAsync<TKey>(Func<StorableTaggedFile, TKey> predicate) where TKey : IComparable
        {
            await Task.Run(() =>
                               {
                                   lock (files)
                                       Files = new List<StorableTaggedFile>(Files.OrderBy(predicate));
                               });
        }

        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="IPlaylistService" /> is shuffle.
        /// </summary>
        /// <value>
        ///   <c>true</c> if shuffle; otherwise, <c>false</c>.
        /// </value>
        public bool Shuffle
        {
            get { return shuffleValue.Value; }
            set { shuffleValue.Value = value; }
        }

        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="IPlaylistService" /> is repeat.
        /// </summary>
        /// <value>
        ///   <c>true</c> if repeat; otherwise, <c>false</c>.
        /// </value>
        public bool Repeat { get; set; }

        /// <summary>
        /// Gets the next.
        /// </summary>
        /// <returns></returns>
        private StorableTaggedFile GetNext()
        {
            lock (files)
            {
                if (currentIndex >= files.Count)
                    currentIndex = 0;
                return files.Count <= 0 ? null : files[currentIndex++];
            }
        }

        /// <summary>
        /// Nexts this instance.
        /// </summary>
        /// <returns></returns>
        public StorableTaggedFile Next()
        {
            StorableTaggedFile file;
            lock (files)
                file = Shuffle ? GetRandom() : GetNext();
            return file;
        }

        /// <summary>
        /// Sets the index of the playlist.
        /// </summary>
        /// <param name="from">From.</param>
        public void SetPlaylistIndex(StorableTaggedFile from)
        {
            lock (files)
            {
                if (null == from) currentIndex = -1;
                else currentIndex = files.IndexOf(from) + 1;
            }
        }
    }
}