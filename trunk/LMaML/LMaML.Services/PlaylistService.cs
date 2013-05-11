using System;
using System.Collections.Generic;
using LMaML.Infrastructure.Domain.Concrete;
using LMaML.Infrastructure.Events;
using LMaML.Infrastructure.Services.Interfaces;
using iLynx.Common;

namespace LMaML.Services
{
    public class PlaylistService : IPlaylistService
    {
        private readonly IPublicTransport publicTransport;
        private readonly List<StorableTaggedFile> files = new List<StorableTaggedFile>();
        private int currentIndex;

        public PlaylistService(IPublicTransport publicTransport)
        {
            publicTransport.Guard("publicTransport");
            this.publicTransport = publicTransport;
        }

        /// <summary>
        /// Adds the file.
        /// </summary>
        /// <param name="file">The file.</param>
        public void AddFile(StorableTaggedFile file)
        {
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
            lock (files)
                files.AddRange(newFiles);
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
    }
}
