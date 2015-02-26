using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows.Media;
using iLynx.Common;
using iLynx.Common.WPF;
using iLynx.Common.WPF.Imaging;
using iLynx.Configuration;
using iLynx.Threading;
using LMaML.Infrastructure.Services.Interfaces;

namespace LMaML.Visualizations.FFT
{
    /// <summary>
    /// 
    /// </summary>
    public unsafe class SpectralFFTVisualizationViewModel : FFTVisualizationViewModelBase
    {
        private int* fftBackBuffer;
        private readonly Timer fftTimer;
        private TimeSpan fftRate = TimeSpan.FromMilliseconds(8d);
        private volatile bool isRunning;
        private readonly IConfigurableValue<int> targetHeight;
        private readonly IConfigurableValue<float> normalizationLevel;

        /// <summary>
        /// </summary>
        /// <param name="threadManager">The thread manager.</param>
        /// <param name="playerService">The player service.</param>
        /// <param name="publicTransport">The public transport.</param>
        /// <param name="configurationManager"></param>
        /// <param name="dispatcher">The dispatcher.</param>
        public SpectralFFTVisualizationViewModel(IThreadManager threadManager,
                 IPlayerService playerService,
                 IPublicTransport publicTransport,
                 IConfigurationManager configurationManager,
                 IDispatcher dispatcher)
            : base(threadManager, playerService, publicTransport, dispatcher,
            configurationManager)
        {
            targetHeight = configurationManager.GetValue("FFT Count", 512, "Spectral FFT");
            normalizationLevel = configurationManager.GetValue("Normalization Leve", .75f, "Spectral FFT");
            targetHeight.ValueChanged += TargetHeightOnValueChanged;
            TargetRenderHeight = targetHeight.Value;
            fftTimer = new Timer(GetFFT);
            fftBackBuffer = (int*)Marshal.AllocHGlobal((int)TargetRenderHeight * (FFTSize * 4));
        }

        private void TargetHeightOnValueChanged(object sender, ValueChangedEventArgs<int> valueChangedEventArgs)
        {
            if (0 >= valueChangedEventArgs.NewValue)
            {
                targetHeight.Value = 1;
                return;
            }
            Reset();
        }

        protected override void Reset()
        {
            isRunning = false;
            fftTimer.Change(Timeout.Infinite, Timeout.Infinite);
            TargetRenderHeight = targetHeight.Value;
            TargetRenderWidth = FFTSize;
            Marshal.FreeHGlobal((IntPtr)fftBackBuffer);
            var bufferSize = (int)TargetRenderHeight * (FFTSize * 4);
            fftBackBuffer = (int*)Marshal.AllocHGlobal(bufferSize);
            NativeMethods.MemSet((IntPtr)fftBackBuffer, 0, bufferSize);
            Recreate();
            isRunning = true;
            fftTimer.Change(fftRate, Timeout.InfiniteTimeSpan);
        }

        private void GetFFT(object state)
        {
            fftTimer.Change(Timeout.InfiniteTimeSpan, Timeout.InfiniteTimeSpan);
            var sw = Stopwatch.StartNew();
            float sampleRate;
            var width = (int)TargetRenderWidth;
            var byteWidth = width * 4;
            var fft = PlayerService.FFT(out sampleRate, width);
            if (null == fft || fft.Length < 1) return;
            fft = fft.Normalize(normalizationLevel.Value);
            NativeMethods.MemCpy((byte*)fftBackBuffer, byteWidth, (byte*)fftBackBuffer, 0, (int)((byteWidth * TargetRenderHeight) - byteWidth));
            fixed (int* res = fft.Transform(x => Palette.GetColour(x)))
                NativeMethods.MemCpy((byte*)res, 0, (byte*)fftBackBuffer, (int)(byteWidth * TargetRenderHeight - byteWidth), byteWidth);
            sw.Stop();
            var remainder = (fftRate - sw.Elapsed).TotalMilliseconds;
            if (0d > remainder)
            {
                ++errors;
                if (errors > 10)
                {
                    this.LogWarning("Spectral FFT interval is too low - Time exceeded by: {0}ms", Math.Abs(remainder));
                    var inc = ((fftRate.TotalMilliseconds) / 100d) * 10d;
                    this.LogInformation("Increasing Interval by: {0}ms", inc);
                    fftRate = fftRate.Add(TimeSpan.FromMilliseconds(inc));
                    errors = 0;
                }
                remainder = 0;
            }
            else
                errors = 0;
            if (!isRunning) return;
            fftTimer.Change((long)remainder, Timeout.Infinite);
        }

        private volatile int errors;

        protected override void OnStopped()
        {
            isRunning = false;
            fftTimer.Change(Timeout.InfiniteTimeSpan, Timeout.InfiniteTimeSpan);
        }

        protected override void OnStarted()
        {
            isRunning = true;
            fftTimer.Change(fftRate, Timeout.InfiniteTimeSpan);
        }

        protected override void Dispose(bool disposing)
        {
            isRunning = false;
            fftTimer.Change(Timeout.InfiniteTimeSpan, Timeout.InfiniteTimeSpan);
            targetHeight.ValueChanged -= TargetHeightOnValueChanged;
        }

        ~SpectralFFTVisualizationViewModel()
        {
            Marshal.FreeHGlobal((IntPtr)fftBackBuffer);
        }

        public override double RenderHeight
        {
            set
            {

            }
        }

        public override double RenderWidth
        {
            set
            {

            }
        }

        #region Overrides of VisualizationViewModelBase

        protected override void Render(RenderContext context)
        {
            if (context.Width != (int)TargetRenderWidth || context.Height != (int)TargetRenderHeight) return;
            NativeMethods.MemCpy((byte*)fftBackBuffer, 0, (byte*)context.BackBuffer, 0, context.Height * context.Stride);
        }

        #endregion
    }
}
