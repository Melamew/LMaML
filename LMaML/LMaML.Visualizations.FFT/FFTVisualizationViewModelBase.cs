using System;
using System.Windows.Media;
using iLynx.Common;
using iLynx.Common.Pixels;
using iLynx.Common.WPF;
using iLynx.Configuration;
using iLynx.Threading;
using LMaML.Infrastructure.Services.Interfaces;
using LMaML.Infrastructure.Visualization;

namespace LMaML.Visualizations.FFT
{
    public abstract class FFTVisualizationViewModelBase : VisualizationViewModelBase
    {
        private readonly IConfigurableValue<int> fftSize;
        protected readonly IPalette<double> Palette = new LinearGradientPalette();

        /// <summary>
        /// </summary>
        /// <param name="threadManager">The thread manager.</param>
        /// <param name="playerService">The player service.</param>
        /// <param name="publicTransport">The public transport.</param>
        /// <param name="dispatcher">The dispatcher.</param>
        /// <param name="configurationManager"></param>
        protected FFTVisualizationViewModelBase(IThreadManager threadManager, IPlayerService playerService, IPublicTransport publicTransport, IDispatcher dispatcher,
            IConfigurationManager configurationManager)
            : base(threadManager, playerService, publicTransport, dispatcher)
        {
            Palette = configurationManager.GetValue("Palette", GetDefaultPalette(), "FFT Visualization").Value;
            fftSize = configurationManager.GetValue("FFT Size", 1024, "FFT Visualization");
            if (!fftSize.Value.IsPowerOfTwo())
                fftSize.Value = 1024;
            if (256 > fftSize.Value)
                fftSize.Value = 256;
            fftSize.ValueChanged += FFTSizeOnValueChanged;
            TargetRenderWidth = fftSize.Value;
        }

        private static IPalette<double> GetDefaultPalette()
        {
            var palette = new LinearGradientPalette();
            palette.MapValue(0d, Colors.Transparent);
            palette.MapValue(0.005, Color.FromArgb(255, 32, 128, 32)/*Colors.Lime*/);
            palette.MapValue(0.025, Colors.Yellow);
            palette.MapValue(0.5d, Color.FromArgb(255, 255, 128, 0));
            palette.MapValue(1d, Color.FromArgb(255, 255, 0, 0));
            return palette;
        } 

        protected int FFTSize
        {
            get { return fftSize.Value; }
        }

        private void FFTSizeOnValueChanged(object sender, ValueChangedEventArgs<int> valueChangedEventArgs)
        {
            if (!valueChangedEventArgs.NewValue.IsPowerOfTwo())
            {
                fftSize.Value = 1024;
                return;
            }
            if (256 > valueChangedEventArgs.NewValue)
            {
                fftSize.Value = 256;
                return;
            }
            Reset();
        }

        protected abstract void Reset();
    }
}