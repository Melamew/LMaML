using System;
using System.Windows.Media;
using iLynx.Common.Pixels;
using iLynx.Threading;
using LMaML.Infrastructure.Events;
using LMaML.Infrastructure.Services.Interfaces;
using LMaML.Infrastructure.Visualization;
using iLynx.Common.WPF;
using iLynx.Common.WPF.Imaging;

namespace LMaML.Visualizations.FFT.ViewModels
{
    public class SimpleFFTVisualizationViewModel : VisualizationViewModelBase
    {
        private readonly IPalette<double> palette = new LinearGradientPalette();

        /// <summary>
        /// Initializes a new instance of the <see cref="SimpleFFTVisualizationViewModel" /> class.
        /// </summary>
        /// <param name="threadManager">The thread manager.</param>
        /// <param name="playerService">The player service.</param>
        /// <param name="publicTransport">The public transport.</param>
        /// <param name="dispatcher">The dispatcher.</param>
        public SimpleFFTVisualizationViewModel(IThreadManager threadManager, IPlayerService playerService, IPublicTransport publicTransport, IDispatcher dispatcher)
            : base(threadManager, playerService, publicTransport, dispatcher)
        {
            palette.MapValue(0d, Colors.Green);
            palette.MapValue(0.01, Color.FromArgb(255, 64, 0, 192));
            palette.MapValue(0.25, Colors.Lime);
            palette.MapValue(0.5, Colors.Yellow);
            palette.MapValue(1d, Color.FromArgb(255, 255, 0, 0));
        }

        protected override void Render(RenderContext context)
        {
            lock (SyncRoot)
            {
                var backBuffer = context.BackBuffer;
                var width = context.Width;
                var height = context.Height;
                var h = (double) height;
                var stride = context.Stride / 4;
                unsafe
                {
                    if (PlayerService.State != PlayingState.Playing) return;
                    float sampleRate;
                    var fft = PlayerService.FFT(out sampleRate, 2048);
                    if (null == fft) return;
                    var freqPerChannel = ((sampleRate/2)/fft.Length);
                    var lastIndex = 21000f/freqPerChannel;
                    fft.Normalize();
                    lastIndex = lastIndex >= fft.Length ? fft.Length - 1 : lastIndex < 0 ? 0 : lastIndex;
                    var step = width / lastIndex;
                    var buf = (int*)backBuffer;
                    for (var i = 0; i < lastIndex; ++i)
                    {
                        var x1 = (int)Math.Floor(i * step);
                        var x2 = (int)Math.Ceiling((i + 1) * step);
                        var y1 = height - 1;
                        var y2 = height - (height * fft[i]);

                        y2 = y2 < 0 ? 0 : y2;
                        y2 = y2 > height ? height : y2;
                        x2 = x2 > width ? width : x2;
                        x2 = x2 < 0 ? 0 : x2;
                        for (var x = x1; x < x2 && x < stride; ++x)
                        {
                            for (var y = y1; y > y2; --y)
                            {
                                var target = (y * stride) + x;
                                var val = palette.GetColour(1d - (y/h));
                                buf[target] = val;
                                //buf[target] = val[0];
                                //buf[target + 1] = val[1];
                                //buf[target + 2] = val[2];
                                //buf[target + 3] = val[3];
                                //buf[target] = 0x66;
                                //buf[target + 1] = 0x66;
                                //buf[target + 2] = 0x66;
                                //buf[target + 3] = 0xFF;
                            }
                        }
                    }
                }
            }
        }
    }
}
