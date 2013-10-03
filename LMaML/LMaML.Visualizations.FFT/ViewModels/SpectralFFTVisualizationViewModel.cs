using System;
using System.Runtime.InteropServices;
using System.Threading;
using LMaML.Infrastructure.Services.Interfaces;
using LMaML.Infrastructure.Visualization;
using iLynx.Common;
using iLynx.Common.Pixels;
using iLynx.Common.Threading;
using iLynx.Common.WPF;

namespace LMaML.Visualizations.FFT.ViewModels
{
    /// <summary>
    /// 
    /// </summary>
    public unsafe class SpectralFFTVisualizationViewModel : VisualizationViewModelBase
    {
        private readonly int* fftBackBuffer;
        private readonly LinearGradientPalette palette = new LinearGradientPalette();
        private readonly Timer fftTimer;

        /// <summary>
        /// </summary>
        /// <param name="logger">The logger.</param>
        /// <param name="threadManager">The thread manager.</param>
        /// <param name="playerService">The player service.</param>
        /// <param name="publicTransport">The public transport.</param>
        /// <param name="dispatcher">The dispatcher.</param>
        public SpectralFFTVisualizationViewModel(ILogger logger,
                 IThreadManager threadManager,
                 IPlayerService playerService,
                 IPublicTransport publicTransport,
                 IDispatcher dispatcher)
            : base(logger, threadManager, playerService, publicTransport, dispatcher)
        {
            palette.MapValue(0d, 0, 0, 0, 0);
            palette.MapValue(0.001, 255, 0, 255, 0);
            palette.MapValue(0.0015, 255, 0, 0, 255);
            palette.MapValue(0.02, 255, 255, 0, 0);
            palette.MapValue(0.05, 255, 192, 0, 64);
            palette.MapValue(0.06, 255, 64, 0, 192);
            palette.MapValue(1d, 255, 255, 0, 255);
            TargetRenderHeight = 256;
            TargetRenderWidth = 1024;
            fftTimer = new Timer(GetFFT);
            //fftTimer.Change(1, 1);
            fftBackBuffer = (int*)Marshal.AllocHGlobal((int)TargetRenderHeight * (1024 * 4));
        }

        private void GetFFT(object state)
        {
            float sampleRate;
            var fft = PlayerService.FFT(out sampleRate, 1024);
            if (null == fft || fft.Length < 1) return;
            NativeMethods.MemCpy((byte*)fftBackBuffer, 4096, (byte*)fftBackBuffer, 0, (int) ((4096 * TargetRenderHeight) - 4096));
            fixed (int* res = fft.Transform(x => palette.GetColour(x * 1d)))
                NativeMethods.MemCpy((byte*)res, 0, (byte*)fftBackBuffer, (int) (4096 * TargetRenderHeight - 4096), 4096);
        }

        protected override void OnStopped()
        {
            fftTimer.Change(Timeout.Infinite, Timeout.Infinite);
        }

        protected override void OnStarted()
        {
            fftTimer.Change(1, 1);
        }

        public override void Dispose()
        {
            fftTimer.Change(Timeout.Infinite, Timeout.Infinite);
            base.Dispose();
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

        /// <summary>
        /// Renders the callback.
        /// </summary>
        /// <param name="backBuffer">The back buffer.</param>
        /// <param name="width">The width.</param>
        /// <param name="height">The height.</param>
        /// <param name="stride">The stride.</param>
        protected override void Render(IntPtr backBuffer,
                                               int width,
                                               int height,
                                               int stride)
        {
            
            if (width != (int)TargetRenderWidth || height != (int)TargetRenderHeight) return;
            //var sw = Stopwatch.StartNew();
            NativeMethods.MemCpy((byte*)fftBackBuffer, 0, (byte*)backBuffer, 0, width * height * 4);
            //sw.Stop();
            //Debug.WriteLine(sw.Elapsed.TotalMilliseconds.ToString("F2"));
            //ffts.Add(fft.Transform(x => palette.GetColour(x * 1d)));
            //while (ffts.Count > 200)
            //    ffts.RemoveAt(0);
            //unsafe
            //{
            //    var buffer = (int*)backBuffer;
            //    for (var y = 0; y < ffts.Count; ++y)
            //    {
            //        fixed (int* res = ffts[y])
            //        {
            //            NativeMethods.MemCpy((byte*) res, 0, (byte*) buffer, y*(width * 4), width * 4);
            //        }
            //    }
            //}
        }

        //private int[] GetRow(List<float[]> rows,
        //                       int index,
        //                       double samplesPerRow,
        //                       double samplesPerColumn)
        //{
        //    var first = rows.FirstOrDefault();
        //    if (null == first) return new int[0];
        //    var length = first.Length;
        //    var res = new float[(int)(length / samplesPerColumn)];
        //    for (var row = (double)index; row < index + Math.Ceiling(samplesPerRow) && row < rows.Count; row += samplesPerRow)
        //    {
        //        var currentRow = rows[(int) row];
        //        var source = 0d;
        //        for (var col = 0; col < res.Length; ++col)
        //        {
        //            for (var i = source; i < source + samplesPerColumn; ++i)
        //            {
        //                res[col] += currentRow[(int)i];
        //            }
        //            source += samplesPerColumn;
        //        }
        //        //var target = 0;
        //        //for (var column = 0d; column < length; column += samplesPerColumn)
        //        //{
        //        //    for (var i = column; i < column + samplesPerColumn && i < length; ++i)
        //        //    {
        //        //        res[target] += currentRow[(int)i];
        //        //    }
        //        //    ++target;
        //        //}
        //    }
        //    return res.Normalize().Transform(x => palette.GetColour(x));
        //}

        #endregion
    }
}
