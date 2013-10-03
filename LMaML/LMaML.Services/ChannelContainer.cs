using System;
#if DEBUG
using System.Diagnostics;
#endif
using LMaML.Infrastructure.Audio;
using LMaML.Infrastructure.Domain.Concrete;
using iLynx.Common;

namespace LMaML.Services
{
    /// <summary>
    ///     ChannelContainer
    /// </summary>
    public class TrackContainer : ITrack
    {
        private readonly IAudioPlayer player;

        /// <summary>
        ///     Gets or sets the channel.
        /// </summary>
        /// <value>
        ///     The channel.
        /// </value>
        private ITrack track;

        /// <summary>
        ///     Initializes a new instance of the <see cref="TrackContainer" /> class.
        /// </summary>
        /// <param name="player">The player.</param>
        /// <param name="file">The file.</param>
        public TrackContainer(IAudioPlayer player, StorableTaggedFile file)
        {
            player.Guard("player");
            file.Guard("file");

            this.player = player;
            File = file;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TrackContainer" /> class.
        /// </summary>
        /// <param name="track">The track.</param>
        public TrackContainer(ITrack track)
        {
            track.Guard("track");
            this.track = track;
            File = new EmptyFile(track.Name);
        }

        private class EmptyFile : StorableTaggedFile
        {
            public EmptyFile(string name)
            {
                base.Album = new Album { Name = name };
                base.Artist = new Artist { Name = name };
                base.Comment = name;
                base.Filename = name;
                base.Genre = new Genre { Name = name };
                base.Title = new Title { Name = name };
                base.Year = new Year { Name = name, Value = (uint)DateTime.Now.Year };
            }
        }

        /// <summary>
        ///     Gets the file.
        /// </summary>
        /// <value>
        ///     The file.
        /// </value>
        public StorableTaggedFile File { get; private set; }

#if DEBUG
        private static int channelCount;
        private static int nextId;
        private static int NextId { get { return nextId++; } }
        private int id;
#endif

        /// <summary>
        ///     Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            if (null == track) return;
#if DEBUG
            Trace.WriteLine(string.Format("Dispose Channel: {0}, Active Channels: {1}", id, --channelCount));
#endif
            track.Dispose();
            track = null;
        }

        /// <summary>
        ///     Gets the current progress.
        /// </summary>
        /// <value>
        ///     The current progress.
        /// </value>
        public double CurrentProgress
        {
            get { return track == null ? 0d : track.CurrentProgress; }
        }

        /// <summary>
        /// FFTs the stereo.
        /// </summary>
        /// <param name="fftSize">Size of the FFT.</param>
        /// <returns></returns>
        public float[] FFTStereo(int fftSize = 64)
        {
            return track.FFTStereo(fftSize);
        }

        public double CurrentPositionMillisecond
        {
            get { return track == null ? 0d : track.CurrentPositionMillisecond; }
        }

        /// <summary>
        /// Gets the name of this track.
        /// </summary>
        /// <value>
        /// The name.
        /// </value>
        public string Name
        {
            get { return null == File ? null == track ? string.Empty : track.Name : File.Filename; }
        }

        /// <summary>
        ///     Stops this instance.
        /// </summary>
        public void Stop()
        {
            if (null == track) return;
            track.Stop();
        }

        /// <summary>
        ///     Pauses this instance.
        /// </summary>
        public void Pause()
        {
            if (null == track) return;
            track.Pause();
        }

        /// <summary>
        ///     Resumes this instance.
        /// </summary>
        public void Play(float volume)
        {
            if (null == track)
            {
                track = player.CreateChannel(File.Filename);
#if DEBUG
                Trace.WriteLine(string.Format("Create Channel: {0}, Active Channels: {1}", id = NextId, ++channelCount));
#endif
            }
            track.Play(volume);
        }

        /// <summary>
        ///     Seeks the specified offset.
        /// </summary>
        /// <param name="offset">The offset.</param>
        public void Seek(TimeSpan offset)
        {
            if (null == track) return;
            if (offset >= Length) return;
            track.Seek(offset);
        }

        /// <summary>
        /// Seeks to the specified offset (In milliseconds).
        /// </summary>
        /// <param name="offset">The offset.</param>
        public void Seek(double offset)
        {
            if (null == track) return;
            Seek(TimeSpan.FromMilliseconds(offset));
        }

        /// <summary>
        ///     Gets a value indicating whether this instance is paused.
        /// </summary>
        /// <value>
        ///     <c>true</c> if this instance is paused; otherwise, <c>false</c>.
        /// </value>
        public bool IsPaused
        {
            get { return null != track && track.IsPaused; }
            set
            {
                if (null == track) return;
                track.IsPaused = value;
            }
        }

        /// <summary>
        ///     Gets or sets the volume.
        /// </summary>
        /// <value>
        ///     The volume.
        /// </value>
        public float Volume
        {
            get { return track == null ? 0f : track.Volume; }
            set
            {
                if (null == track) return;
                track.Volume = value;
            }
        }

        /// <summary>
        ///     Gets a value indicating whether this instance is playing.
        /// </summary>
        /// <value>
        ///     <c>true</c> if this instance is playing; otherwise, <c>false</c>.
        /// </value>
        public bool IsPlaying
        {
            get { return null != track && track.IsPlaying; }
        }

        /// <summary>
        ///     Gets the length.
        /// </summary>
        /// <value>
        ///     The length.
        /// </value>
        public TimeSpan Length
        {
            get { return null == track ? TimeSpan.FromMilliseconds(0) : track.Length; }
        }

        /// <summary>
        /// FFTs the specified channel offset.
        /// </summary>
        /// <param name="channelOffset">The channel offset.</param>
        /// <param name="fftSize">Size of the FFT.</param>
        /// <returns></returns>
        public float[] FFT(int channelOffset = -1, int fftSize = 64)
        {
            return track.FFT(channelOffset, fftSize);
        }

        /// <summary>
        /// Gets the sample rate.
        /// </summary>
        /// <value>
        /// The sample rate.
        /// </value>
        public float SampleRate { get { return track.SampleRate; } }

        /// <summary>
        ///     Preloads this instance.
        /// </summary>
        public void Preload()
        {
            if (null != track) return; // Already preloaded
            track = player.CreateChannel(File.Filename);
#if DEBUG
            Trace.WriteLine(string.Format("Create Channel: {0}, Active Channels: {1}", id = NextId, ++channelCount));
#endif
        }

        private ReadonlyTrack readOnly;
        
        /// <summary>
        /// Gets as readonly.
        /// </summary>
        /// <value>
        /// As readonly.
        /// </value>
        public ITrack AsReadonly { get { return readOnly ?? (readOnly = new ReadonlyTrack(track)); } }

        /// <summary>
        /// ReadonlyChannel - READ ONLY!
        /// </summary>
        private class ReadonlyTrack : ITrack
        {
            private readonly ITrack source;

            public ReadonlyTrack(ITrack source)
            {
                this.source = source;
            }

            public void Dispose()
            {

            }

            public double CurrentProgress { get { return source.CurrentProgress; } }

            public void Stop()
            {

            }

            public void Pause()
            {

            }

            public void Play(float volume)
            {

            }

            public void Seek(TimeSpan offset)
            {

            }

            public void Seek(double offset)
            {

            }

            public bool IsPaused { get { return source.IsPaused; } set { } }

            public bool IsPlaying { get { return source.IsPlaying; } }

            public float Volume { get { return source.Volume; } set { } }

            public TimeSpan Length { get { return source.Length; } }

            public float[] FFT(int channelOffset = -1, int fftSize = 64)
            {
                return source.FFT(channelOffset, fftSize);
            }

            public float SampleRate { get { return source.SampleRate; } }

            public float[] FFTStereo(int fftSize = 64)
            {
                return source.FFTStereo(fftSize);
            }

            public double CurrentPositionMillisecond { get { return source.CurrentPositionMillisecond; } }

            /// <summary>
            /// Gets the name of this track.
            /// </summary>
            /// <value>
            /// The name.
            /// </value>
            public string Name
            {
                get { return source.Name; }
            }
        }
    }
}