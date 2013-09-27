using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime;
using System.Windows.Media;
using LMaML.Infrastructure.Services.Interfaces;
using LMaML.Infrastructure.Visualization;
using iLynx.Common;
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
        private readonly LinearInterpolatingPalette palette = new LinearInterpolatingPalette();

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

        private int GetColour(float level)
        {
            return palette.GetColour(level);
            //var val = (byte) ((level * 2)*255d);
            //var res = (val << 24) | (val << 16);
            //return res;
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

    /// <summary>
    /// InterpolatingPalette
    /// </summary>
    public class LinearInterpolatingPalette : NotificationBase
    {
        private readonly SortedList<double, int> colourMap = new SortedList<double, int>();
        private double[] sortedKeys;
        private readonly bool isFrozen;
        private readonly object writeLock = new object();

        public LinearInterpolatingPalette()
        {
            MaxValue = int.MinValue;
            MinValue = int.MaxValue;
        }

        protected LinearInterpolatingPalette(SortedList<double, int> values, bool isFrozen = true)
        {
            this.isFrozen = isFrozen;
            colourMap = values;
            MinValue = colourMap.Keys.Min();
            MaxValue = colourMap.Keys.Max();
            sortedKeys = colourMap.Keys.ToArray();
        }

        /// <summary>
        /// Removes the value.
        /// </summary>
        /// <param name="sampleValue">The sample value.</param>
        public void RemoveValue(double sampleValue)
        {
            if (isFrozen) throw new InvalidOperationException("This instance is frozen, it cannot be modified");
            lock (writeLock)
            {
                colourMap.Remove(sampleValue);
                if (colourMap.Count != 0)
                {
                    if (sampleValue == MaxValue)
                        MaxValue = colourMap.Keys.Max();
                    if (sampleValue == MinValue)
                        MinValue = colourMap.Keys.Min();
                }
                sortedKeys = colourMap.Keys.ToArray();
            }
        }

        /// <summary>
        /// Remaps the value.
        /// </summary>
        /// <param name="oldValue">The old value.</param>
        /// <param name="newValue">The new value.</param>
        public void RemapValue(double oldValue, double newValue)
        {
            if (isFrozen) throw new InvalidOperationException("This instance is frozen, it cannot be modified");
            lock (writeLock)
            {
                if (!colourMap.ContainsKey(oldValue)) return;
                var colour = colourMap[oldValue];
                colourMap.Remove(oldValue);
                MinValue = Math.Min(newValue, MinValue);
                MaxValue = Math.Max(newValue, MaxValue);

                colourMap.Add(newValue, colour);
                if (oldValue == MaxValue)
                    MaxValue = colourMap.Keys.Max();
                if (oldValue == MinValue)
                    MinValue = colourMap.Keys.Min();
                sortedKeys = colourMap.Keys.ToArray();
            }
        }

        /// <summary>
        /// Maps the value.
        /// </summary>
        /// <param name="sampleValue">The sample value.</param>
        /// <param name="colour">The colour.</param>
        /// <exception cref="System.ArgumentOutOfRangeException">colour;@The specified byte array is not a valid colour</exception>
        public unsafe void MapValue(double sampleValue, byte[] colour)
        {
            fixed (byte* col = colour)
                MapValue(sampleValue, *((int*)col)); // Direct conversion
        }

        /// <summary>
        /// Determines whether the specified value is mapped.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>
        ///   <c>true</c> if the specified value is mapped; otherwise, <c>false</c>.
        /// </returns>
        public bool IsMapped(double value)
        {
            return colourMap.ContainsKey(value);
        }

        private double minValue;
        /// <summary>
        /// Gets the min sample value.
        /// </summary>
        /// <value>
        /// The min value.
        /// </value>
        public double MinValue
        {
            get { return minValue; }
            private set
            {
                if (value == minValue) return;
                minValue = value;
                RaisePropertyChanged(() => MinValue);
            }
        }

        private double maxValue;
        /// <summary>
        /// Gets the max sample value.
        /// </summary>
        /// <value>
        /// The max value.
        /// </value>
        public double MaxValue
        {
            get { return maxValue; }
            private set
            {
                if (value == maxValue) return;
                maxValue = value;
                RaisePropertyChanged(() => MaxValue);
            }
        }

        /// <summary>
        /// Maps the value.
        /// </summary>
        /// <param name="sampleValue">The sample value.</param>
        /// <param name="a">A.</param>
        /// <param name="r">The r.</param>
        /// <param name="g">The g.</param>
        /// <param name="b">The b.</param>
        public void MapValue(double sampleValue, byte a, byte r, byte g, byte b)
        {
            MapValue(sampleValue, new[] { b, g, r, a });
        }

        /// <summary>
        /// Maps the value.
        /// </summary>
        /// <param name="sampleValue">The sample value.</param>
        /// <param name="colour">The colour.</param>
        public void MapValue(double sampleValue, int colour)
        {
            if (isFrozen) throw new InvalidOperationException("This instance is frozen, it cannot be modified");
            lock (writeLock)
            {
                MaxValue = Math.Max(MaxValue, sampleValue);
                MinValue = Math.Min(MinValue, sampleValue);
                if (colourMap.ContainsKey(sampleValue))
                    colourMap[sampleValue] = colour;
                else
                    colourMap.Add(sampleValue, colour);
                sortedKeys = colourMap.Keys.ToArray();
            }
        }

        /// <summary>
        /// Gets the colour.
        /// </summary>
        /// <param name="sampleValue">The sample value.</param>
        /// <returns></returns>
        public unsafe int GetColour(double sampleValue)
        {
            double min, max;

            FindSamples(sampleValue, out min, out max);

            if (min == max)
            {
                var val = colourMap[min];
                return val;
            }
            var f = colourMap[min];
            var s = colourMap[max];
            var first = (byte*)&f;
            var second = (byte*)&s;
            var res = new byte[4];
            res[0] = (byte)InterpolateLinear(sampleValue, min, max, first[0], second[0]);
            res[1] = (byte)InterpolateLinear(sampleValue, min, max, first[1], second[1]);
            res[2] = (byte)InterpolateLinear(sampleValue, min, max, first[2], second[2]);
            res[3] = (byte)InterpolateLinear(sampleValue, min, max, first[3], second[3]);

            fixed (byte* p = res)
            {
                var colour = *((int*)p); // As if by magic.
                return colour;
            }
        }

        [TargetedPatchingOptOut("This is a mathematical fact as far as I'm aware.")]
        public static double InterpolateLinear(double x, double x0, double x1, double y0, double y1)
        {
            return y0 + ((x - x0) * (y1 - y0)) / (x1 - x0);
        }

        /// <summary>
        /// Finds the two samples with values greater than and less than the specified mean value.
        /// </summary>
        /// <param name="mean">The mean.</param>
        /// <param name="min">The min.</param>
        /// <param name="max">The max.</param>
        public void FindSamples(double mean, out double min, out double max)
        {
            min = 0;
            max = min;
            if (null == sortedKeys) return;
            FindSamples(sortedKeys, mean, out min, out max);
        }

        /// <summary>
        /// Finds the two samples with values greater than and less than the specified mean value.
        /// </summary>
        /// <param name="samples">The samples.</param>
        /// <param name="mean">The mean.</param>
        /// <param name="min">The min.</param>
        /// <param name="max">The max.</param>
        private static void FindSamples(double[] samples, double mean, out double min, out double max)
        {
            var index = Array.BinarySearch(samples, mean);
            if (index < 0)
            {
                index = ~index;
                if (index >= samples.Length)
                {
                    min = samples[samples.Length - 1];
                    max = min;
                }
                else
                {
                    max = samples[index];
                    min = index <= 0 ? max : samples[index - 1];
                }
            }
            else
            {
                min = samples[index];
                max = min;
            }
        }

        public string Name { get; set; }

        /// <summary>
        /// Gets the pixel format.
        /// </summary>
        /// <value>
        /// The pixel format.
        /// </value>
        public PixelFormat PixelFormat
        {
            get { return PixelFormats.Bgra32; }
        }

        /// <summary>
        /// Gets the number of values currently mapped in this palette.
        /// </summary>
        /// <value>
        /// The count.
        /// </value>
        public int Count
        {
            get { return colourMap.Count; }
        }
    }
}
