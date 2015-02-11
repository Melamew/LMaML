using System;
using System.Globalization;
using System.Threading;
using System.Windows.Media;
using System.Xml;
using iLynx.Common;
using iLynx.Common.Pixels;
using iLynx.Common.WPF;
using iLynx.Common.WPF.Imaging;
using iLynx.Configuration;
using iLynx.Serialization;
using iLynx.Serialization.Xml;
using iLynx.Threading;
using LMaML.Infrastructure.Events;
using LMaML.Infrastructure.Services.Interfaces;
using LMaML.Infrastructure.Visualization;

namespace LMaML.Visualizations.Waveform
{
    public class SimpleWaveformVisualizationViewModel : VisualizationViewModelBase
    {
        private readonly IConfigurableValue<int> waveformLength;
        private readonly IConfigurableValue<IPalette<double>> paletteValue;
        private readonly Timer offsetTimer;

        /// <summary>
        /// </summary>
        /// <param name="threadManager">The thread manager.</param>
        /// <param name="playerService">The player service.</param>
        /// <param name="publicTransport">The public transport.</param>
        /// <param name="dispatcher">The dispatcher.</param>
        /// <param name="configurationManager"></param>
        public SimpleWaveformVisualizationViewModel(IThreadManager threadManager, IPlayerService playerService, IPublicTransport publicTransport, IDispatcher dispatcher,
            IConfigurationManager configurationManager)
            : base(threadManager, playerService, publicTransport, dispatcher)
        {
            publicTransport.ApplicationEventBus.Subscribe<ShutdownEvent>(OnShutdown);
            waveformLength = configurationManager.GetValue("Length", 512, "Waveform");
            paletteValue = configurationManager.GetValue("Palette", GetDefaultPalette(), "Waveform");
            paletteValue.Store();
            offsetTimer = new Timer(OnOffset, null, 10, 10);
        }

        private void OnShutdown(ShutdownEvent shutdownEvent)
        {
            offsetTimer.Change(Timeout.Infinite, Timeout.Infinite);
        }

        private void OnOffset(object state)
        {
            paletteOffset += 0.01d;
            if (paletteOffset >= .75d)
                paletteOffset = 0d;
        }

        private static IPalette<double> GetDefaultPalette()
        {
            var palette = new LinearGradientPalette();
            palette.MapValue(0d, Color.FromArgb(255, 192, 0, 0));
            palette.MapValue(.125, Color.FromArgb(255, 192, 192, 0));
            palette.MapValue(.25d, Color.FromArgb(255, 0, 192, 0));
            palette.MapValue(.375, Color.FromArgb(255, 192, 192, 0));
            palette.MapValue(.5d, Color.FromArgb(255, 0, 0, 255));
            palette.MapValue(.625, Color.FromArgb(255, 192, 192, 0));
            palette.MapValue(.75d, Color.FromArgb(255, 0, 192, 0));
            palette.MapValue(.875, Color.FromArgb(255, 192, 192, 0));
            palette.MapValue(1d, Color.FromArgb(255, 192, 0, 0));
            palette.MapValue(1.125, Color.FromArgb(255, 192, 192, 0));
            palette.MapValue(1.25d, Color.FromArgb(255, 0, 192, 0));
            palette.MapValue(1.375, Color.FromArgb(255, 192, 192, 0));
            palette.MapValue(1.5d, Color.FromArgb(255, 0, 0, 255));
            palette.MapValue(1.625, Color.FromArgb(255, 192, 192, 0));
            palette.MapValue(1.75d, Color.FromArgb(255, 0, 192, 0));
            palette.MapValue(1.875, Color.FromArgb(255, 192, 192, 0));
            palette.MapValue(2d, Color.FromArgb(255, 192, 0, 0));
            return palette;
        }

        private double paletteOffset;

        #region Overrides of VisualizationViewModelBase

        protected override void Render(RenderContext context)
        {
            var backBuffer = context.BackBuffer;
            var height = context.Height;
            var width = context.Width;
            var waveForm = PlayerService.GetWaveform(waveformLength.Value).Transform<float>(x => x > 1 ? 1 : x < -1 ? -1 : x);
            var xStep = width / (double)waveForm.Length;
            var halfHeight = height * 0.5d;
            var subLineStep = 1d / waveForm.Length;
            var palette = paletteValue.Value.AsFrozen();
            for (var i = 1; i < waveForm.Length; ++i)
            {
                var x1 = (i - 1) * xStep;
                var x2 = i * xStep;
                var y1 = (halfHeight) * waveForm[i - 1];
                var y2 = (halfHeight) * waveForm[i];
                var sample = ((double)i / waveForm.Length + paletteOffset);
                backBuffer.DrawLineVector(new VectorD(x1, (height - halfHeight) - y1), new VectorD(x2, (height - halfHeight) - y2),
                    (d, d1, position) => palette.GetColour(sample + position * subLineStep),
                    width, height);
            }
        }

        #endregion
    }
}
