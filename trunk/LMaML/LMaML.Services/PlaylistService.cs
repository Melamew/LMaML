using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using LMaML.Infrastructure;
using LMaML.Infrastructure.Domain.Concrete;
using LMaML.Infrastructure.Events;
using LMaML.Infrastructure.Services.Interfaces;
using iLynx.Common;
using iLynx.Common.Configuration;
using iLynx.Common.Serialization;
using iLynx.Common.Threading;
using iLynx.Common.Threading.Unmanaged;

namespace LMaML.Services
{
    public class PlaylistService : ComponentBase, IPlaylistService
    {
        private readonly IPublicTransport publicTransport;
        private readonly IReferenceAdapters referenceAdapters;
        private readonly IThreadManager threadManager;
        private readonly ILogger logger;
        private List<StorableTaggedFile> files = new List<StorableTaggedFile>();
        private int currentIndex;
        private readonly List<IWorker> loadWorkers = new List<IWorker>();
        private volatile bool canLoad = true;
        private readonly IConfigurableValue<string> playlistRelPath;
        private readonly ISerializerService serializerService;

        /// <summary>
        /// Initializes a new instance of the <see cref="PlaylistService" /> class.
        /// </summary>
        /// <param name="publicTransport">The public transport.</param>
        /// <param name="referenceAdapters">The reference adapters.</param>
        /// <param name="threadManager">The thread manager.</param>
        /// <param name="configurationManager">The configuration manager.</param>
        /// <param name="serializerService">The serializer service.</param>
        /// <param name="logger">The logger.</param>
        public PlaylistService(IPublicTransport publicTransport,
            IReferenceAdapters referenceAdapters,
            IThreadManager threadManager,
            IConfigurationManager configurationManager,
            ISerializerService serializerService,
            ILogger logger)
            : base (logger)
        {
            publicTransport.Guard("publicTransport");
            referenceAdapters.Guard("referenceAdapters");
            threadManager.Guard("threadManager");
            serializerService.Guard("serializerService");
            logger.Guard("logger");
            this.publicTransport = publicTransport;
            this.referenceAdapters = referenceAdapters;
            this.threadManager = threadManager;
            this.logger = logger;
            this.serializerService = serializerService;
            playlistRelPath = configurationManager.GetValue("Playlist Relative Path", ".\\Playlist.bin");
            LoadPlaylist(playlistRelPath.Value);
            publicTransport.ApplicationEventBus.Subscribe<ShutdownEvent>(OnShutdown);
        }

        /// <summary>
        /// PlaylistContainer
        /// </summary>
        private class PlaylistContainer
        {
            /// <summary>
            /// Gets or sets a value indicating whether this <see cref="PlaylistContainer" /> is shuffle.
            /// </summary>
            /// <value>
            ///   <c>true</c> if shuffle; otherwise, <c>false</c>.
            /// </value>
            public bool Shuffle { get; set; }

            /// <summary>
            /// Gets or sets the files.
            /// </summary>
            /// <value>
            /// The files.
            /// </value>
            public StorableTaggedFile[] Files { get; set; }
        }

        private void LoadPlaylist(string file)
        {
            if (!File.Exists(file)) return;
            PlaylistContainer container;
            try
            {
                using (var stream = File.OpenRead(file))
                {
                    container = serializerService.Deserialize<PlaylistContainer>(stream);
                }
            }
            catch (Exception e)
            {
                LogException(e, MethodBase.GetCurrentMethod());
                return;
            }
            Shuffle = container.Shuffle;
            if (null == container.Files) return;
            files = new List<StorableTaggedFile>(container.Files);
            publicTransport.ApplicationEventBus.Send(new PlaylistUpdatedEvent());
        }

        private void SavePlaylist(string file)
        {
            var container = new PlaylistContainer { Files = Files.ToArray(), Shuffle = Shuffle };
            try
            {
                using (var stream = File.Open(file, FileMode.Create, FileAccess.ReadWrite))
                {
                    serializerService.Serialize(container, stream);
                    stream.Flush(true);
                }
            }
            catch (Exception e) { LogException(e, MethodBase.GetCurrentMethod()); }
        }

        private void OnShutdown(ShutdownEvent shutdownEvent)
        {
            canLoad = false;
            foreach (var worker in loadWorkers)
            {
                try { worker.Wait(TimeSpan.FromMilliseconds(250)); }
                catch { worker.Abort(); }
            }
            SavePlaylist(playlistRelPath.Value);
        }

        private void Load(IEnumerable<StorableTaggedFile> fs)
        {
            logger.Log(LoggingType.Information, this, "Starting file load");
            var cnt = 0;
            var sw = Stopwatch.StartNew();
            foreach (var x in fs.TakeWhile(x => canLoad))
            {
                x.LoadReferences(referenceAdapters);
                ++cnt;
            }
            sw.Stop();
            if (!canLoad) return;
            logger.Log(LoggingType.Information, this, string.Format("Finished loading {0} files in {1} seconds", cnt, sw.Elapsed.TotalSeconds));
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
            files.Clear();
        }

        /// <summary>
        /// Gets the files.
        /// </summary>
        /// <value>
        /// The files.
        /// </value>
        public IEnumerable<StorableTaggedFile> Files { get { return files.AsReadOnly(); } }

        private bool shuffle;
        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="IPlaylistService" /> is shuffle.
        /// </summary>
        /// <value>
        ///   <c>true</c> if shuffle; otherwise, <c>false</c>.
        /// </value>
        public bool Shuffle
        {
            get { return shuffle; }
            set
            {
                if (value == shuffle) return;
                shuffle = value;
                publicTransport.ApplicationEventBus.Send(new ShuffleChangedEvent(value));
            }
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
            if (currentIndex >= files.Count)
                currentIndex = 0;
            return files.Count <= 0 ? null : files[currentIndex++];
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
                currentIndex = files.IndexOf(from) + 1;
        }
    }
}
