using System;
using System.Diagnostics;
using System.IO;
using System.Numerics;
using System.Threading;
using LMaML.Infrastructure.Audio;
using MathNet.Numerics.IntegralTransforms;
using NAudio.Wave;
using NAudio.Wave.SampleProviders;
using iLynx.Common;

namespace LMaML.NAudio
{
    public class NAudioTrack : ITrack
    {
        private readonly AudioFileReader inputStream;
        private readonly NAudioPlayer player;
        private readonly MixingSampleProvider outputMixer;
        private readonly SampleWrapper sampleProvider;
        private const int FadeSteps = 50;
        private bool isPaused;
        private bool isStopped = true;

        private class SampleWrapper : ISampleProvider
        {
            private readonly ISampleProvider source;
            private readonly float[] backBuffer;// = new float[8192];
            private readonly float[] tempBuffer;// = new float[8192];
            private int tempOffset = 0;

            public SampleWrapper(ISampleProvider source)
            {
                this.source = source;
                backBuffer = new float[2048];
                tempBuffer = new float[2048];
            }

            public float[] FFT(int length = 1024)
            {
                if (!length.IsPowerOfTwo()) throw new ArgumentOutOfRangeException("length", string.Format("FFT can only be performed on sample lengths that are a power of two"));
                if (length * 2 > backBuffer.Length) throw new ArgumentOutOfRangeException("length", string.Format("length cannot be greater than {0} at this time", backBuffer.Length));
                //var result = backBuffer.Transform(x => new System.Numerics.Complex(x, 0));
                //var window = 
                var result = new Complex[length * 2];
                for (var i = 0; i < result.Length; ++i)
                {
                    result[i] = new Complex(global::NAudio.Dsp.FastFourierTransform.HammingWindow(i, length * 2) * backBuffer[i], 0);
                }
                Transform.FourierInverse(result, FourierOptions.Default);
                return result.Transform(x => (float)Math.Abs(x.Magnitude)).Slice(0, result.Length / 2);
            }

            #region Implementation of ISampleProvider

            /// <summary>
            /// Fill the specified buffer with 32 bit floating point samples
            /// </summary>
            /// <param name="buffer">The buffer to fill with samples.</param><param name="offset">Offset into buffer</param><param name="count">The number of samples to read</param>
            /// <returns>
            /// the number of samples written to the buffer.
            /// </returns>
            public int Read(float[] buffer,
                            int offset,
                            int count)
            {
                var c = source.Read(buffer, offset, count);
                var l = c;
                if (l + tempOffset >= tempBuffer.Length)
                    l = tempBuffer.Length - tempOffset;
                Array.Copy(buffer, 0, tempBuffer, tempOffset, l);
                tempOffset += l;
                if (tempOffset >= tempBuffer.Length)
                {
                    tempOffset = 0;
                    Array.Copy(tempBuffer, backBuffer, tempBuffer.Length);
                }
                return c;
            }

            /// <summary>
            /// Gets the WaveFormat of this Sample Provider.
            /// </summary>
            /// <value>
            /// The wave format.
            /// </value>
            public WaveFormat WaveFormat
            {
                get { return source.WaveFormat; }
            }

            #endregion
        }

        internal NAudioTrack(NAudioPlayer player, MixingSampleProvider outputMixer, AudioFileReader inputStream)
        {
            this.player = player;
            this.outputMixer = outputMixer;
            this.inputStream = inputStream;
            sampleProvider = new SampleWrapper(inputStream.ToSampleProvider());

        }

        private void RemoveInput()
        {
            outputMixer.RemoveMixerInput(sampleProvider);
        }

        private void AddInput()
        {
            outputMixer.AddMixerInput(sampleProvider);
        }

        #region Implementation of IDisposable

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            RemoveInput();
            inputStream.Close();
            try { inputStream.Dispose(); }
            catch (NullReferenceException) { Trace.WriteLine("Caught NullReferenceException in NAudioChannel.Dispose() => inputStream.Dispose() (Is this a bug?)"); }
        }

        #endregion

        #region Implementation of IChannel

        /// <summary>
        /// Gets the current progress.
        /// </summary>
        /// <value>
        /// The current progress.
        /// </value>
        public double CurrentProgress
        {
            get { return (100d / inputStream.TotalTime.TotalMilliseconds) * inputStream.CurrentTime.Milliseconds; }
        }

        /// <summary>
        /// Stops this instance.
        /// </summary>
        public void Stop()
        {
            RemoveInput();
            inputStream.Seek(0, SeekOrigin.Begin);
            isStopped = true;
        }

        /// <summary>
        /// Pauses this instance.
        /// </summary>
        public void Pause()
        {
            RemoveInput();
            isPaused = true;
        }

        /// <summary>
        /// Resumes this instance.
        /// </summary>
        public void Play(float volume)
        {
            if (volume > 1.0f) volume = 1.0f;
            if (volume < 0.0f) volume = 0.0f;
            inputStream.Volume = volume;
            if (isPaused || isStopped)
                AddInput();
            player.DoPlay();
            isStopped = false;
            isPaused = false;
        }

        /// <summary>
        /// Seeks the specified offset.
        /// </summary>
        /// <param name="offset">The offset.</param>
        public void Seek(TimeSpan offset)
        {
            inputStream.CurrentTime = offset;
        }

        /// <summary>
        /// Seeks to the specified offset (In milliseconds).
        /// </summary>
        /// <param name="offset">The offset.</param>
        public void Seek(double offset)
        {
            Seek(TimeSpan.FromMilliseconds(offset));
        }

        /// <summary>
        /// Gets a value indicating whether this instance is paused.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this instance is paused; otherwise, <c>false</c>.
        /// </value>
        public bool IsPaused
        {
            get { return isPaused; }
            set
            {
                if (value)
                    Pause();
                else
                    Play(Volume);
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
            get { return (!isPaused) && (!isStopped); }
        }

        /// <summary>
        /// Gets or sets the volume.
        /// </summary>
        /// <value>
        /// The volume.
        /// </value>
        public float Volume
        {
            get { return inputStream.Volume; }
            set
            {
                if (value < 0.0f) value = 0.0f;
                if (value > 1.0f) value = 1.0f;
                inputStream.Volume = value;
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
            get { return inputStream.TotalTime; }
        }

        /// <summary>
        /// FFTs the specified channel offset.
        /// </summary>
        /// <param name="channelOffset">The channel offset.</param>
        /// <param name="fftSize">Size of the FFT.</param>
        /// <returns></returns>
        public float[] FFT(int channelOffset = -1,
                           int fftSize = 64)
        {
            return sampleProvider.FFT(fftSize);
        }

        /// <summary>
        /// Gets the sample rate.
        /// </summary>
        /// <value>
        /// The sample rate.
        /// </value>
        public float SampleRate
        {
            get { return inputStream.WaveFormat.SampleRate; }
        }

        /// <summary>
        /// FFTs the stereo.
        /// </summary>
        /// <param name="fftSize">Size of the FFT.</param>
        /// <returns></returns>
        public float[] FFTStereo(int fftSize = 64)
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
            get { return inputStream.CurrentTime.TotalMilliseconds; }
        }

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

        #endregion
    }
}
