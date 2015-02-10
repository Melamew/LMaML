using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows.Media;
using iLynx.Common;
using iLynx.Common.Pixels;
using iLynx.Common.WPF;
using iLynx.Common.WPF.Imaging;
using iLynx.Configuration;
using iLynx.Threading;
using LMaML.Infrastructure.Services.Interfaces;
using LMaML.Infrastructure.Visualization;

namespace LMaML.Visualizations.FFT
{
    /// <summary>
    /// 
    /// </summary>
    public unsafe class SpectralFFTVisualizationViewModel : VisualizationViewModelBase
    {
        private int* fftBackBuffer;
        private readonly IPalette<double> palette = new LinearGradientPalette();
        private readonly Timer fftTimer;
        private TimeSpan fftRate = TimeSpan.FromMilliseconds(8d);
        private volatile bool isRunning;
        private readonly IConfigurableValue<int> targetHeight;

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
            : base(threadManager, playerService, publicTransport, dispatcher)
        {
            targetHeight = configurationManager.GetValue("FFT Count", 512, "Spectral FFT");
            palette.MapValue(0d, Colors.Transparent);
            palette.MapValue(0.005, Color.FromArgb(255, 32, 128, 32)/*Colors.Lime*/);
            palette.MapValue(0.025, Colors.Yellow);
            palette.MapValue(0.5d, Color.FromArgb(255, 255, 128, 0));
            palette.MapValue(1d, Color.FromArgb(255, 255, 0, 0));
            targetHeight.ValueChanged += TargetHeightOnValueChanged;
            TargetRenderHeight = targetHeight.Value;
            TargetRenderWidth = 1024;
            fftTimer = new Timer(GetFFT);
            fftBackBuffer = (int*)Marshal.AllocHGlobal((int)TargetRenderHeight * (1024 * 4));
        }

        private void TargetHeightOnValueChanged(object sender, ValueChangedEventArgs<int> valueChangedEventArgs)
        {
            var val = valueChangedEventArgs.NewValue;
            if (0 >= val) return;
            isRunning = false;
            fftTimer.Change(Timeout.Infinite, Timeout.Infinite);
            TargetRenderHeight = val;
            Marshal.FreeHGlobal((IntPtr)fftBackBuffer);
            fftBackBuffer = (int*)Marshal.AllocHGlobal((int)TargetRenderHeight * (1024 * 4));
            RenderHeight = TargetRenderHeight;
            isRunning = true;
            fftTimer.Change(fftRate, Timeout.InfiniteTimeSpan);
        }

        private void GetFFT(object state)
        {
            fftTimer.Change(Timeout.InfiniteTimeSpan, Timeout.InfiniteTimeSpan);
            var sw = Stopwatch.StartNew();
            float sampleRate;
            var fft = PlayerService.FFT(out sampleRate, 1024);
            if (null == fft || fft.Length < 1) return;
            fft = fft.Normalize(0.75f);
            NativeMethods.MemCpy((byte*)fftBackBuffer, 4096, (byte*)fftBackBuffer, 0, (int)((4096 * TargetRenderHeight) - 4096));
            fixed (int* res = fft.Transform(x => palette.GetColour(x)))
                NativeMethods.MemCpy((byte*)res, 0, (byte*)fftBackBuffer, (int)(4096 * TargetRenderHeight - 4096), 4096);
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

        private double renderHeight;
        private double renderWidth;

        public override double RenderHeight
        {
            set
            {
                if (Math.Abs(value - renderHeight) <= double.Epsilon) return;
                renderHeight = value;
                ResizeInit();
            }
        }

        public override double RenderWidth
        {
            set
            {
                if (Math.Abs(value - renderWidth) <= double.Epsilon) return;
                renderWidth = value;
                ResizeInit();
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
