using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Media;
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
    public class SpectralFFTVisualizationViewModel : VisualizationViewModelBase
    {
        private readonly List<float[]> ffts = new List<float[]>();
        private readonly LinearGradientPalette palette = new LinearGradientPalette();

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
            //palette.MapValue(0.01, 255, 64, 64, 0);
            //palette.MapValue(0.025, 255, 128, 0, 32);
            palette.MapValue(0.15, 255, 0, 0, 255);
            //palette.MapValue(0.8, 255, 192, 0, 32);
            palette.MapValue(1d, 255, 255, 0, 0);
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
            float sampleRate;
            var fft = PlayerService.FFT(out sampleRate, 1024);
            if (null == fft) return;
            ffts.Add(fft);
            while (ffts.Count > 200)
                ffts.RemoveAt(0);
            var samplesPerColumn = 1024d / width;
            var samplesPerRow = 200d / height;
            unsafe
            {
                var buffer = (int*)backBuffer;
                var index = 0d;
                var lastIndex = (int)index;
                var currentRow = GetRow(ffts, lastIndex, samplesPerRow, samplesPerColumn);
                for (var y = 0; y < height; ++y)
                {
                    if ((int) index != lastIndex)
                    {
                        lastIndex = (int) index;
                        currentRow = GetRow(ffts, lastIndex, samplesPerRow, samplesPerColumn);
                        Array.Resize(ref currentRow, width); // Just making sure.
                    }
                    fixed (int* res = currentRow)
                    {
                        NativeMethods.MemCpy((byte*) res, 0, (byte*) buffer, y*(width * 4), width * 4);
                    }
                    //for (var x = 0; x < width; ++x)
                    //{
                    //    if (x >= currentRow.Length) break;
                    //    buffer[x + y*width] = GetColour(currentRow[x]); //(int)(currentRow[x] * (unchecked((int)0xFF00FF00)));
                    //}
                    index += samplesPerRow;
                }
            }
        }

        private int[] GetRow(List<float[]> rows,
                               int index,
                               double samplesPerRow,
                               double samplesPerColumn)
        {
            var first = rows.FirstOrDefault();
            if (null == first) return new int[0];
            var length = first.Length;
            var res = new float[(int)(length / samplesPerColumn)];
            for (var row = (double)index; row < index + Math.Ceiling(samplesPerRow) && row < rows.Count; row += samplesPerRow)
            {
                var currentRow = rows[(int) row];
                var source = 0d;
                for (var col = 0; col < res.Length; ++col)
                {
                    for (var i = source; i < source + samplesPerColumn; ++i)
                    {
                        res[col] += currentRow[(int)i];
                    }
                    source += samplesPerColumn;
                }
                //var target = 0;
                //for (var column = 0d; column < length; column += samplesPerColumn)
                //{
                //    for (var i = column; i < column + samplesPerColumn && i < length; ++i)
                //    {
                //        res[target] += currentRow[(int)i];
                //    }
                //    ++target;
                //}
            }
            return res.Normalize().Transform(x => palette.GetColour(x));
        }

        #endregion
    }
}
