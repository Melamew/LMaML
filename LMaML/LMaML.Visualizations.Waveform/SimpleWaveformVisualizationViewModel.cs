using System;
using System.Windows.Media;
using iLynx.Common.WPF;
using iLynx.Common.WPF.Imaging;
using iLynx.Configuration;
using iLynx.Threading;
using LMaML.Infrastructure.Services.Interfaces;
using LMaML.Infrastructure.Visualization;

namespace LMaML.Visualizations.Waveform
{
    public class SimpleWaveformVisualizationViewModel : VisualizationViewModelBase
    {
        private readonly IConfigurableValue<int> waveformLength;
        private readonly IConfigurableValue<Color> waveformColour;

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
            waveformLength = configurationManager.GetValue("Length", 512, "Waveform");
            waveformColour = configurationManager.GetValue("Colour", Color.FromArgb(255, 255, 0, 0), "Waveform");
        }

        #region Overrides of VisualizationViewModelBase

        protected override void Render(RenderContext context)
        {
            var backBuffer = context.BackBuffer;
            var height = context.Height;
            var width = context.Width;
            var waveForm = PlayerService.GetWaveform(waveformLength.Value).Transform<float>(x => x > 1 ? 1 : x < -1 ? -1 : x);
            var xStep = width/(double) waveForm.Length;
            var halfHeight = height*0.5d;
            var colour = waveformColour.Value;
            for (var i = 1; i < waveForm.Length; ++i)
            {
                var x1 = (i - 1)*xStep;
                var x2 = i*xStep;
                var y1 = (halfHeight)*waveForm[i - 1];
                var y2 = (halfHeight)*waveForm[i];
                backBuffer.DrawLineVector(new VectorD(x1, (height - halfHeight) - y1), new VectorD(x2, (height - halfHeight) - y2), colour, width, height);
            }
        }

        #endregion
    }
}
