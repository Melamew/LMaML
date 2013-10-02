using System;
using System.Diagnostics;
using LMaML.Infrastructure.Audio;
using Un4seen.Bass;
using Un4seen.Bass.AddOn.Mix;
using iLynx.Common;
using Bassh = Un4seen.Bass.Bass;

namespace LMaML.Bass
{
    public class BassTrack : ITrack
    {
        private readonly TimeSpan length;
        private readonly float sampleRate;
        private readonly int channelHandle;
        private readonly int mixerHandle;

        internal BassTrack(int channelHandle, int mixerHandle)
        {
            this.channelHandle = channelHandle;
            this.mixerHandle = mixerHandle;
            var trackLength = Bassh.BASS_ChannelGetLength(channelHandle, BASSMode.BASS_POS_BYTES);
            length = TimeSpan.FromSeconds(Bassh.BASS_ChannelBytes2Seconds(channelHandle, trackLength));
            var channelInfo = Bassh.BASS_ChannelGetInfo(channelHandle);
            sampleRate = channelInfo.freq;
        }

        #region Implementation of IDisposable

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            BassMix.BASS_Mixer_ChannelRemove(channelHandle);
            Bassh.BASS_StreamFree(channelHandle);
        }

        #endregion

        #region Implementation of ITrack

        /// <summary>
        /// Gets the current progress.
        /// </summary>
        /// <value>
        /// The current progress.
        /// </value>
        public double CurrentProgress
        {
            get
            {
                var totalLengthMillis = length.TotalMilliseconds;
                var currentPosition = CurrentPositionMillisecond;
                return (100d / totalLengthMillis) * currentPosition;
            }
        }

        /// <summary>
        /// Stops this instance.
        /// </summary>
        public void Stop()
        {
            Pause();
            BassMix.BASS_Mixer_ChannelSetPosition(channelHandle, 1, BASSMode.BASS_POS_BYTES);
        }

        /// <summary>
        /// Pauses this instance.
        /// </summary>
        public void Pause()
        {
            BassMix.BASS_Mixer_ChannelPause(channelHandle);
        }

        /// <summary>
        /// Resumes this instance.
        /// </summary>
        public void Play(float volume)
        {
            Volume = volume;
            BassMix.BASS_Mixer_ChannelPlay(channelHandle);
        }

        /// <summary>
        /// Seeks the specified offset.
        /// </summary>
        /// <param name="offset">The offset.</param>
        public void Seek(TimeSpan offset)
        {
            Seek(offset.TotalMilliseconds);
        }

        /// <summary>
        /// Seeks to the specified offset (In milliseconds).
        /// </summary>
        /// <param name="offset">The offset.</param>
        public void Seek(double offset)
        {
            var byteOffset = Bassh.BASS_ChannelSeconds2Bytes(channelHandle, offset/1000d);
            BassMix.BASS_Mixer_ChannelSetPosition(channelHandle, byteOffset, BASSMode.BASS_POS_BYTES);
        }

        /// <summary>
        /// Gets a value indicating whether this instance is paused.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this instance is paused; otherwise, <c>false</c>.
        /// </value>
        public bool IsPaused
        {
            get
            {
                var state = BassMix.BASS_Mixer_ChannelIsActive(channelHandle);
                return state == BASSActive.BASS_ACTIVE_PAUSED || state == BASSActive.BASS_ACTIVE_STOPPED;
            }
            set
            {
                if (value)
                    BassMix.BASS_Mixer_ChannelPause(channelHandle);
                else
                    BassMix.BASS_Mixer_ChannelPlay(channelHandle);
            }
        }

        /// <summary>
        /// Gets a value indicating whether this instance is playing.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance is playing; otherwise, <c>false</c>.
        /// </value>
        public bool IsPlaying
        {
            get
            {
                var state = BassMix.BASS_Mixer_ChannelIsActive(channelHandle);
                return state == BASSActive.BASS_ACTIVE_PLAYING || state == BASSActive.BASS_ACTIVE_STALLED;
            }
        }

        /// <summary>
        /// Gets or sets the volume.
        /// </summary>
        /// <value>
        /// The volume.
        /// </value>
        public float Volume
        {
            get
            {
                var volume = 0f;
                if (!Bassh.BASS_ChannelGetAttribute(channelHandle, BASSAttribute.BASS_ATTRIB_VOL, ref volume))
                    throw new InvalidOperationException("Unable to get channel volume");
                return volume;
            }
            set
            {
                Bassh.BASS_ChannelSetAttribute(channelHandle, BASSAttribute.BASS_ATTRIB_VOL, value);
            }
        }

        /// <summary>
        /// Gets the length.
        /// </summary>
        /// <value>
        /// The length.
        /// </value>
        public TimeSpan Length
        {
            get { return length; }
        }

        private int GetFFTSize(int size)
        {
            switch (size)
            {
                case 256:
                    return (int) BASSData.BASS_DATA_FFT256;
                case 512:
                    return (int) BASSData.BASS_DATA_FFT512;
                case 1024:
                    return (int) BASSData.BASS_DATA_FFT1024;
                case 2048:
                    return (int) BASSData.BASS_DATA_FFT2048;
                case 4096:
                    return (int) BASSData.BASS_DATA_FFT4096;
                case 8192:
                    return (int) BASSData.BASS_DATA_FFT8192;
                case 16384:
                    return (int) BASSData.BASS_DATA_FFT16384;
                default:
                    return size;
            }
        }

        /// <summary>
        /// FFTs the specified channel offset.
        /// </summary>
        /// <param name="channelOffset">The channel offset.</param>
        /// <param name="fftSize">Size of the FFT.</param>
        /// <returns></returns>
        public float[] FFT(int channelOffset = -1,
                           int fftSize = 256)
        {
            if (!fftSize.IsPowerOfTwo())
                throw new ArgumentOutOfRangeException("fftSize");
            var result = new float[fftSize];
            BassMix.BASS_Mixer_ChannelGetData(channelHandle, result, GetFFTSize(fftSize * 2));
            return result;
        }

        /// <summary>
        /// Gets the sample rate.
        /// </summary>
        /// <value>
        /// The sample rate.
        /// </value>
        public float SampleRate
        {
            get { return sampleRate; }
        }

        /// <summary>
        /// FFTs the stereo.
        /// </summary>
        /// <param name="fftSize">Size of the FFT.</param>
        /// <returns></returns>
        public float[] FFTStereo(int fftSize = 256)
        {
            return FFT(-1, fftSize);
        }

        /// <summary>
        /// Gets the current position.
        /// </summary>
        /// <value>
        /// The current position.
        /// </value>
        public double CurrentPositionMillisecond
        {
            get
            {
                var posBytes = BassMix.BASS_Mixer_ChannelGetPosition(channelHandle, BASSMode.BASS_POS_BYTES);
                return Bassh.BASS_ChannelBytes2Seconds(channelHandle, posBytes) * 1000d;
            }
        }

        #endregion
    }
}
