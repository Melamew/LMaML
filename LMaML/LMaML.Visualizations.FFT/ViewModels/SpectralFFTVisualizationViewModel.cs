using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows.Media;
using iLynx.Threading;
using LMaML.Infrastructure.Services.Interfaces;
using LMaML.Infrastructure.Visualization;
using iLynx.Common;
using iLynx.Common.Pixels;
using iLynx.Common.WPF;
using iLynx.Common.WPF.Imaging;

namespace LMaML.Visualizations.FFT.ViewModels
{
    /// <summary>
    /// 
    /// </summary>
    public unsafe class SpectralFFTVisualizationViewModel : VisualizationViewModelBase
    {
        private readonly int* fftBackBuffer;
        private readonly IPalette<double> palette = new LinearGradientPalette();
        private readonly Timer fftTimer;
        private readonly TimeSpan fftRate = TimeSpan.FromMilliseconds(5d);

        /// <summary>
        /// </summary>
        /// <param name="threadManager">The thread manager.</param>
        /// <param name="playerService">The player service.</param>
        /// <param name="publicTransport">The public transport.</param>
        /// <param name="dispatcher">The dispatcher.</param>
        public SpectralFFTVisualizationViewModel(IThreadManager threadManager,
                 IPlayerService playerService,
                 IPublicTransport publicTransport,
                 IDispatcher dispatcher)
            : base(threadManager, playerService, publicTransport, dispatcher)
        {
            palette.MapValue(0d, Colors.Transparent);
            palette.MapValue(0.15, Colors.Lime);
            palette.MapValue(0.5, Colors.Yellow);
            palette.MapValue(0.75, Colors.Blue);
            palette.MapValue(1d, Color.FromArgb(255, 255, 0, 0));
            TargetRenderHeight = 256;
            TargetRenderWidth = 1024;
            fftTimer = new Timer(GetFFT);
            fftBackBuffer = (int*)Marshal.AllocHGlobal((int)TargetRenderHeight * (1024 * 4));
        }

        private void GetFFT(object state)
        {
            fftTimer.Change(Timeout.InfiniteTimeSpan, Timeout.InfiniteTimeSpan);
            var sw = Stopwatch.StartNew();
            float sampleRate;
            var fft = PlayerService.FFT(out sampleRate, 1024);
            if (null == fft || fft.Length < 1) return;
            fft = fft.Normalize();
            NativeMethods.MemCpy((byte*)fftBackBuffer, 4096, (byte*)fftBackBuffer, 0, (int) ((4096 * TargetRenderHeight) - 4096));
            fixed (int* res = fft.Transform(x => palette.GetColour(x)))
                NativeMethods.MemCpy((byte*)res, 0, (byte*)fftBackBuffer, (int) (4096 * TargetRenderHeight - 4096), 4096);
            sw.Stop();
            var remainder = (fftRate - sw.Elapsed).TotalMilliseconds;
            if (0 > remainder)
                remainder = 0;
            fftTimer.Change((long) remainder, Timeout.Infinite);
        }

        protected override void OnStopped()
        {
            fftTimer.Change(Timeout.InfiniteTimeSpan, Timeout.InfiniteTimeSpan);
        }

        protected override void OnStarted()
        {
            fftTimer.Change(fftRate, Timeout.InfiniteTimeSpan);
        }

        protected override void Dispose(bool disposing)
        {
            fftTimer.Change(Timeout.InfiniteTimeSpan, Timeout.InfiniteTimeSpan);
        }

        ~SpectralFFTVisualizationViewModel()
        {
            Marshal.FreeHGlobal((IntPtr)fftBackBuffer);
        }

        private double renderHeight;
        private double renderWidth;

        public override double RenderHeight
        {
            // ReSharper disable ValueParameterNotUsed
            set
            // ReSharper restore ValueParameterNotUsed
            {
                if (Math.Abs(value - renderHeight) <= double.Epsilon) return;
                renderHeight = value;
                ResizeInit();
            }
        }

        public override double RenderWidth
        {
            // ReSharper disable ValueParameterNotUsed
            set
            // ReSharper restore ValueParameterNotUsed
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
