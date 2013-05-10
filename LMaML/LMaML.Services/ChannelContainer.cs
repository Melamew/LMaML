using System;
using LMaML.Infrastructure.Audio;
using LMaML.Infrastructure.Domain.Concrete;
using iLynx.Common;

namespace LMaML.Services
{
    /// <summary>
    ///     ChannelContainer
    /// </summary>
    public class ChannelContainer : IChannel
    {
        private readonly IAudioPlayer player;

        /// <summary>
        ///     Gets or sets the channel.
        /// </summary>
        /// <value>
        ///     The channel.
        /// </value>
        private IChannel channel;

        /// <summary>
        ///     Initializes a new instance of the <see cref="ChannelContainer" /> class.
        /// </summary>
        /// <param name="player">The player.</param>
        /// <param name="file">The file.</param>
        public ChannelContainer(IAudioPlayer player, StorableTaggedFile file)
        {
            player.Guard("player");
            file.Guard("file");

            this.player = player;
            File = file;
        }

        /// <summary>
        ///     Gets the file.
        /// </summary>
        /// <value>
        ///     The file.
        /// </value>
        public StorableTaggedFile File { get; private set; }

        //private static int disposeCount;
        //private static int channelCount;

        /// <summary>
        ///     Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            if (null == channel) return;
            //Trace.WriteLine(string.Format("Dispose Channel: {0}, Active Channels: {1}", ++disposeCount, channelCount - disposeCount));
            channel.Dispose();
            channel = null;
        }

        /// <summary>
        ///     Gets the current progress.
        /// </summary>
        /// <value>
        ///     The current progress.
        /// </value>
        public double CurrentProgress
        {
            get { return channel == null ? 0d : channel.CurrentProgress; }
        }

        /// <summary>
        /// FFTs the stereo.
        /// </summary>
        /// <param name="fftSize">Size of the FFT.</param>
        /// <returns></returns>
        public float[] FFTStereo(int fftSize = 64)
        {
            return channel.FFTStereo(fftSize);
        }

        public double CurrentPosition
        {
            get { return channel == null ? 0d : channel.CurrentPosition; }
        }

        /// <summary>
        ///     Stops this instance.
        /// </summary>
        public void Stop()
        {
            if (null == channel) return;
            channel.Stop();
        }

        /// <summary>
        ///     Pauses this instance.
        /// </summary>
        public void Pause()
        {
            if (null == channel) return;
            channel.Pause();
        }

        /// <summary>
        ///     Resumes this instance.
        /// </summary>
        public void Play(float volume)
        {
            if (null == channel)
            {
                channel = player.CreateChannel(File.Filename);
                //Trace.WriteLine(string.Format("Create Channel: {0}, Active Channels: {1}", ++channelCount, channelCount - disposeCount));
            }
            channel.Play(volume);
        }

        /// <summary>
        ///     Seeks the specified offset.
        /// </summary>
        /// <param name="offset">The offset.</param>
        public void Seek(TimeSpan offset)
        {
            if (null == channel) return;
            if (offset >= Length) return;
            channel.Seek(offset);
        }

        /// <summary>
        /// Seeks to the specified offset (In milliseconds).
        /// </summary>
        /// <param name="offset">The offset.</param>
        public void Seek(double offset)
        {
            if (null == channel) return;
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
            get { return null != channel && channel.IsPaused; }
            set
            {
                if (null == channel) return;
                channel.IsPaused = value;
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
            get { return channel == null ? 0f : channel.Volume; }
            set
            {
                if (null == channel) return;
                channel.Volume = value;
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
            get { return null != channel && channel.IsPlaying; }
        }

        /// <summary>
        ///     Gets the length.
        /// </summary>
        /// <value>
        ///     The length.
        /// </value>
        public TimeSpan Length
        {
            get { return null == channel ? TimeSpan.FromMilliseconds(0) : channel.Length; }
        }

        /// <summary>
        /// FFTs the specified channel offset.
        /// </summary>
        /// <param name="channelOffset">The channel offset.</param>
        /// <param name="fftSize">Size of the FFT.</param>
        /// <returns></returns>
        public float[] FFT(int channelOffset = -1, int fftSize = 64)
        {
            return channel.FFT(channelOffset, fftSize);
        }

        /// <summary>
        /// Gets the sample rate.
        /// </summary>
        /// <value>
        /// The sample rate.
        /// </value>
        public float SampleRate { get { return channel.SampleRate; } }

        /// <summary>
        ///     Fades the out.
        /// </summary>
        /// <param name="over">The over.</param>
        public void FadeOut(TimeSpan over)
        {
            if (null == channel) return;
            channel.FadeOut(over);
        }

        /// <summary>
        ///     Fades the in.
        /// </summary>
        /// <param name="over">The over.</param>
        public void FadeIn(TimeSpan over)
        {
            if (null == channel) return;
            channel.FadeIn(over);
        }

        /// <summary>
        ///     Preloads this instance.
        /// </summary>
        public void Preload()
        {
            if (null != channel) return; // Already preloaded
            //Trace.WriteLine(string.Format("Create Channel: {0}, Active Channels: {1}", ++channelCount, channelCount - disposeCount));
            channel = player.CreateChannel(File.Filename);
        }

        private ReadonlyChannel readOnly;

        /// <summary>
        /// Gets as readonly.
        /// </summary>
        /// <value>
        /// As readonly.
        /// </value>
        public IChannel AsReadonly { get { return readOnly ?? (readOnly = new ReadonlyChannel(channel)); } }

        /// <summary>
        /// ReadonlyChannel - READ ONLY!
        /// </summary>
        private class ReadonlyChannel : IChannel
        {
            private readonly IChannel source;

            public ReadonlyChannel(IChannel source)
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

            public double CurrentPosition { get { return source.CurrentPosition; } }

            public void FadeOut(TimeSpan over)
            {
            }

            public void FadeIn(TimeSpan over)
            {
            }
        }
    }
}