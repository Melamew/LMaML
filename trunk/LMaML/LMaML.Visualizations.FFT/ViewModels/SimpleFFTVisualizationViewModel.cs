using System;
using LMaML.Infrastructure.Events;
using LMaML.Infrastructure.Services.Interfaces;
using LMaML.Infrastructure.Visualization;
using iLynx.Common;
using iLynx.Common.Threading;
using iLynx.Common.WPF;
using iLynx.Common.WPF.Imaging;

namespace LMaML.Visualizations.FFT.ViewModels
{
    public class SimpleFFTVisualizationViewModel : VisualizationViewModelBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SimpleFFTVisualizationViewModel" /> class.
        /// </summary>
        /// <param name="logger">The logger.</param>
        /// <param name="threadManager">The thread manager.</param>
        /// <param name="playerService">The player service.</param>
        /// <param name="publicTransport">The public transport.</param>
        /// <param name="dispatcher">The dispatcher.</param>
        public SimpleFFTVisualizationViewModel(ILogger logger, IThreadManager threadManager, IPlayerService playerService, IPublicTransport publicTransport, IDispatcher dispatcher)
            : base(logger, threadManager, playerService, publicTransport, dispatcher)
        {
        }

        protected override void Render(RenderContext context)
        {
            lock (SyncRoot)
            {
                var backBuffer = context.BackBuffer;
                var width = context.Width;
                var height = context.Height;
                var stride = context.Stride;
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
                    var buf = (byte*)backBuffer;
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
                                var target = (y * stride) + (x * 4);
                                buf[target] = 0x66;
                                buf[target + 1] = 0x66;
                                buf[target + 2] = 0x66;
                                buf[target + 3] = 0xFF;
                            }
                        }
                    }
                }
            }
        }
    }
}
