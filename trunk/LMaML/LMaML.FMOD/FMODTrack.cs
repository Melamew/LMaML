using System;
using System.Runtime;
using System.Threading;
using FMOD;
using LMaML.Infrastructure.Audio;
using iLynx.Common;

namespace LMaML.FMOD
{
    /// <summary>
    /// AudioChannel
    /// </summary>
    public class FMODTrack : ComponentBase, ITrack
    {
        /// <summary>
        /// The fmod sound
        /// </summary>
        private readonly Sound fmodSound;

        /// <summary>
        /// The fmod channel
        /// </summary>
        private readonly Channel fmodChannel;

        private const int FadeSteps = 50;

        /// <summary>
        /// Initializes a new instance of the <see cref="FMODTrack" /> class.
        /// </summary>
        /// <param name="fmodSound">The fmod sound.</param>
        /// <param name="system">The system.</param>
        /// <param name="logger">The logger.</param>
        public FMODTrack(Sound fmodSound, global::FMOD.System system, ILogger logger)
            : base(logger)
        {
            fmodSound.Guard("fmodSound");
            system.Guard("system");
            this.fmodSound = fmodSound;
            Length = TimeSpan.FromMilliseconds(GetLengthMs(fmodSound));
            fmodChannel = CreateChannel(system, fmodSound);
            Init();
        }

        private void Init()
        {
            float freq = 0f, volume = 0f, pan = 0f;
            var prio = 0;
            var result = fmodSound.getDefaults(ref freq, ref volume, ref pan, ref prio);
            if (RESULT.OK != result)
                throw FMODPlayer.GetException("Unable to get sound defaults", result);
            SampleRate = freq;
        }

        //private static uint GetLengthSamples(Sound fmodSound)
        //{
        //    return GetLength(fmodSound, TIMEUNIT.PCM);
        //}

        private static uint GetLength(Sound fmodSound, TIMEUNIT unit)
        {
            uint length = 0;
            var result = fmodSound.getLength(ref length, unit);
            if (RESULT.OK != result)
                throw FMODPlayer.GetException("Unable to get length of sound", result);
            return length;
        }

        /// <summary>
        /// Gets the length ms.
        /// </summary>
        /// <param name="fmodSound">The fmod sound.</param>
        /// <returns></returns>
        private static uint GetLengthMs(Sound fmodSound)
        {
            return GetLength(fmodSound, TIMEUNIT.MS);
        }

        /// <summary>
        /// FFTs the specified channel offset.
        /// </summary>
        /// <param name="channelOffset">Channels start at index 0 for stereo: (0 = Left, 1 = Right), use -1 to get for both.</param>
        /// <param name="fftSize">Size of the FFT.</param>
        /// <returns></returns>
        /// <exception cref="NotSupportedException">Only ffts of length equal to a power of two are supported (Min=64, Max=8192)</exception>
        public float[] FFT(int channelOffset = -1, int fftSize = 64)
        {
            if (!(fftSize.IsPowerOfTwo() && fftSize.IsInRange(64, 8192)))
                throw new NotSupportedException("Only ffts of length equal to a power of two are supported (Min=64, Max=8192)");
            if (-1 == channelOffset)
                return FFTStereo(fftSize);
            var final = new float[fftSize];
            var result = fmodChannel.getSpectrum(final, fftSize, channelOffset, DSP_FFT_WINDOW.BLACKMAN);
            FFTErr(result);
            return final;
        }

        /// <summary>
        /// Gets the sample rate.
        /// </summary>
        /// <value>
        /// The sample rate.
        /// </value>
        public float SampleRate { get; private set; }

        [TargetedPatchingOptOut("Pffft")]
        private static void FFTErr(RESULT result)
        {
            if (RESULT.OK != result)
                throw FMODPlayer.GetException("Unable to get fft", result);
        }

        /// <summary>
        /// FFTs the stereo.
        /// </summary>
        /// <param name="fftSize">Size of the FFT.</param>
        /// <returns></returns>
        /// <exception cref="NotSupportedException">Only ffts of length equal to a power of two are supported (Min=64, Max=8192)</exception>
        public float[] FFTStereo(int fftSize = 64)
        {
            if (!(fftSize.IsPowerOfTwo() && fftSize.IsInRange(64, 8192)))
                throw new NotSupportedException("Only ffts of length equal to a power of two are supported (Min=64, Max=8192)");
            var first = new float[fftSize];
            var second = new float[fftSize];
            var result = fmodChannel.getSpectrum(first, fftSize, 0, DSP_FFT_WINDOW.BLACKMAN);
            FFTErr(result);
            result = fmodChannel.getSpectrum(second, fftSize, 0, DSP_FFT_WINDOW.BLACKMAN);
            FFTErr(result);
            var final = new float[fftSize];
            while (fftSize-- > 0)
                final[fftSize] = (first[fftSize] + second[fftSize]) * 0.5f;
            return final;
        }

        /// <summary>
        /// Stops this instance.
        /// </summary>
        public void Stop()
        {
            Pause();
            Seek(TimeSpan.FromMilliseconds(0));
        }

        /// <summary>
        /// Gets the progress.
        /// </summary>
        /// <value>
        /// The progress.
        /// </value>
        public double CurrentProgress
        {
            get
            {
                if (isDisposed) return 0d;
                var position = CurrentPositionMillisecond;
                return (100d / Length.TotalMilliseconds) * position;
            }
        }

        /// <summary>
        /// Gets or sets the current position.
        /// </summary>
        /// <value>
        /// The current position.
        /// </value>
        public double CurrentPositionMillisecond
        {
            get
            {
                if (isDisposed) return 0d;
                var position = 0u;
                var result = fmodChannel.getPosition(ref position, TIMEUNIT.MS);
                if (RESULT.OK != result)
                    throw FMODPlayer.GetException("Unable to get current sound position", result);
                return position;
            }
        }

        /// <summary>
        /// Pauses this instance.
        /// </summary>
        public void Pause()
        {
            IsPaused = true;
        }

        /// <summary>
        /// Resumes this instance.
        /// </summary>
        public void Play(float volume)
        {
            Volume = volume;
            IsPaused = false;
        }

        /// <summary>
        /// Gets the volume.
        /// </summary>
        /// <value>
        /// The volume.
        /// </value>
        public float Volume
        {
            get
            {
                if (isDisposed) return 0f;
                var vol = 0f;
                var result = fmodChannel.getVolume(ref vol);
                if (RESULT.OK != result)
                    throw FMODPlayer.GetException("Unable to get current channel volume", result);
                return vol;
            }
            set
            {
                if (isDisposed) return;
                var result = fmodChannel.setVolume(value);
                if (RESULT.OK != result)
                    throw FMODPlayer.GetException("Unable to set channel volume", result);
            }
        }

        /// <summary>
        /// Seeks the specified offset.
        /// </summary>
        /// <param name="offset">The offset.</param>
        public void Seek(TimeSpan offset)
        {
            if (offset < TimeSpan.FromMilliseconds(0)) return;
            if (offset > Length) return;
            Seek(offset.TotalMilliseconds);
        }

        /// <summary>
        /// Seeks to the specified offset (In milliseconds).
        /// </summary>
        /// <param name="offset">The offset.</param>
        public void Seek(double offset)
        {
            var result = fmodChannel.setPosition((uint)offset, TIMEUNIT.MS);
            if (RESULT.OK != result)
                throw FMODPlayer.GetException("Unable to seek in this channel", result);
        }

        /// <summary>
        /// Creates the channel.
        /// </summary>
        /// <param name="system">The system.</param>
        /// <param name="sound">The sound.</param>
        /// <returns></returns>
        private static Channel CreateChannel(global::FMOD.System system, Sound sound)
        {
            Channel channel = null;
            var result = system.playSound(CHANNELINDEX.FREE, sound, true, ref channel);
            if (result != RESULT.OK)
                throw FMODPlayer.GetException("Unable to play sound", result);
            return channel;
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
                if (isDisposed) return false;
                var val = false;
                var result = fmodChannel.getPaused(ref val);
                if (RESULT.OK != result)
                    throw FMODPlayer.GetException("Unable to get paused value from channel", result);
                return val;
            }
            set
            {
                if (isDisposed) return;
                var current = false;
                var result = fmodChannel.getPaused(ref current);
                if (RESULT.OK != result)
                    throw FMODPlayer.GetException("Unable to get paused state", result);
                if (current == value) return;
                result = fmodChannel.setPaused(value);
                if (RESULT.OK != result)
                    throw FMODPlayer.GetException("Unable to set paused", result);
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
                if (isDisposed) return false;
                var val = false;
                var result = fmodChannel.isPlaying(ref val);
                if (RESULT.OK != result)
                    throw FMODPlayer.GetException("Unable to get playing value from channel", result);
                return val;
            }
        }

        public TimeSpan Length { get; private set; }

        /// <summary>
        /// Fades the out.
        /// </summary>
        /// <param name="over">The over.</param>
        public void FadeOut(TimeSpan over)
        {
            var distance = Volume;
            var stepSize = (distance / FadeSteps);
            var sleepTime = TimeSpan.FromMilliseconds(over.TotalMilliseconds / FadeSteps);
            for (var i = 0; i < FadeSteps; ++i)
            {
                Volume -= stepSize;
                Thread.CurrentThread.Join(sleepTime);
            }
        }

        /// <summary>
        /// Fades the in.
        /// </summary>
        /// <param name="over">The over.</param>
        public void FadeIn(TimeSpan over)
        {
            var distance = 100f - Volume;
            var stepSize = (distance / FadeSteps);
            var sleepTime = TimeSpan.FromMilliseconds(over.TotalMilliseconds / FadeSteps);
            for (var i = 0; i < FadeSteps; ++i)
            {
                Volume += stepSize;
                Thread.CurrentThread.Join(sleepTime);
            }
        }

        private bool isDisposed;

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            if (isDisposed) return;
            isDisposed = true;
            var result = fmodChannel.stop();
            if (RESULT.OK != result && RESULT.ERR_INVALID_HANDLE != result)
                throw FMODPlayer.GetException("Unable to stop FMOD channel", result);
        }
    }
}