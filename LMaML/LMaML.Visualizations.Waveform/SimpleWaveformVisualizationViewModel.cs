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
        private readonly IConfigurableValue<Color> color1;
        private readonly IConfigurableValue<Color> color2;
        private readonly IConfigurableValue<Color> color3;
        private readonly Timer offsetTimer;
        private readonly IPalette<double> palette; 

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
            color1 = configurationManager.GetValue("Colour 1", Color.FromArgb(255, 192, 0, 0), "Waveform");
            color2 = configurationManager.GetValue("Colour 2", Color.FromArgb(255, 192, 192, 0), "Waveform");
            color3 = configurationManager.GetValue("Colour 3", Color.FromArgb(255, 0, 192, 0), "Waveform");
            palette = GetDefaultPalette();

            color1.ValueChanged += Color1OnValueChanged;
            color2.ValueChanged += Color2OnValueChanged;
            color3.ValueChanged += Color3OnValueChanged;
            offsetTimer = new Timer(OnOffset, null, 10, 10);
        }

        private void Color3OnValueChanged(object sender, ValueChangedEventArgs<Color> valueChangedEventArgs)
        {
            var val = valueChangedEventArgs.NewValue;
            palette.MapValue(.5d, val);
            palette.MapValue(1.5d, val);
        }

        private void Color2OnValueChanged(object sender, ValueChangedEventArgs<Color> valueChangedEventArgs)
        {
            var val = valueChangedEventArgs.NewValue;
            palette.MapValue(.25d, val);
            palette.MapValue(.75d, val);
            palette.MapValue(1.25d, val);
            palette.MapValue(1.75d, val);
        }

        private void Color1OnValueChanged(object sender, ValueChangedEventArgs<Color> valueChangedEventArgs)
        {
            var val = valueChangedEventArgs.NewValue;
            palette.MapValue(0d, val);
            palette.MapValue(1d, val);
            palette.MapValue(2d, val);
        }

        private void OnShutdown(ShutdownEvent shutdownEvent)
        {
            offsetTimer.Change(Timeout.Infinite, Timeout.Infinite);
        }

        private void OnOffset(object state)
        {
            paletteOffset += 0.01d;
            if (paletteOffset >= 1d)
                paletteOffset = 0d;
        }

        private IPalette<double> GetDefaultPalette()
        {
            var pal = new LinearGradientPalette();
            var c1 = color1.Value;
            var c2 = color2.Value;
            var c3 = color3.Value;
            pal.MapValue(0d, c1);
            pal.MapValue(.25, c2);
            pal.MapValue(.5d, c3);
            pal.MapValue(.75, c2);
            pal.MapValue(1d, c1);
            pal.MapValue(1.25, c2);
            pal.MapValue(1.5d, c3);
            pal.MapValue(1.75, c2);
            pal.MapValue(2d, c1);
            return pal;
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
            var pal = palette.AsFrozen();
            for (var i = 1; i < waveForm.Length; ++i)
            {
                var x1 = (i - 1) * xStep;
                var x2 = i * xStep;
                var y1 = (halfHeight) * waveForm[i - 1];
                var y2 = (halfHeight) * waveForm[i];
                var sample = ((double)i / waveForm.Length + paletteOffset);
                backBuffer.DrawLineVector(new VectorD(x1, (height - halfHeight) - y1), new VectorD(x2, (height - halfHeight) - y2),
                    (d, d1, position) => pal.GetColour(sample + position * subLineStep),
                    width, height);
            }
        }

        #endregion
    }
}
