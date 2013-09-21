using System;
using System.Threading;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using LMaML.Infrastructure.Behaviours;
using LMaML.Infrastructure.Events;
using LMaML.Infrastructure.Services.Interfaces;
using iLynx.Common;
using iLynx.Common.Threading;
using iLynx.Common.WPF;
using iLynx.Common.WPF.Imaging;

namespace LMaML.Visualizations.FFT.ViewModels
{
    public class SimpleFFTVisualizationViewModel : NotificationBase, IVisualization
    {
        private readonly IThreadManager threadManager;
        private readonly IPlayerService playerService;
        private readonly IDispatcher dispatcher;
        private UnmanagedBitmapRenderer renderer;
        private double renderHeight;
        private double renderWidth;
        private readonly object syncRoot = new object();
        private readonly Timer changeTimer;

        /// <summary>
        /// Initializes a new instance of the <see cref="SimpleFFTVisualizationViewModel" /> class.
        /// </summary>
        /// <param name="logger">The logger.</param>
        /// <param name="threadManager">The thread manager.</param>
        /// <param name="playerService">The player service.</param>
        /// <param name="publicTransport">The public transport.</param>
        /// <param name="dispatcher">The dispatcher.</param>
        public SimpleFFTVisualizationViewModel(ILogger logger, IThreadManager threadManager, IPlayerService playerService, IPublicTransport publicTransport, IDispatcher dispatcher)
            : base(logger)
        {
            this.threadManager = threadManager;
            this.playerService = playerService;
            this.dispatcher = dispatcher;
            publicTransport.ApplicationEventBus.Subscribe<PlayingStateChangedEvent>(OnPlayingStateChanged);
            publicTransport.ApplicationEventBus.Subscribe<ShellCollapsedEvent>(OnShellCollapsed);
            publicTransport.ApplicationEventBus.Subscribe<ShellExpandedEvent>(OnShellExpanded);
            changeTimer = new Timer(SizeChanged);
        }

        private bool lastRunning;

        private void OnShellExpanded(ShellExpandedEvent shellExpandedEvent)
        {
            if (lastRunning)
                renderer.Start();
        }

        private void OnShellCollapsed(ShellCollapsedEvent shellCollapsedEvent)
        {
            lastRunning = renderer.IsRunning;
            renderer.Stop();
        }

        private void OnPlayingStateChanged(PlayingStateChangedEvent playingStateChangedEvent)
        {
            if (null == renderer)
                return;
            
            if (playingStateChangedEvent.NewState != PlayingState.Playing)
                renderer.Stop();
            else if (!renderer.IsRunning)
                renderer.Start();
        }

        /// <summary>
        /// Sizes the changed.
        /// </summary>
        /// <param name="state">The state.</param>
        private void SizeChanged(object state)
        {
            changeTimer.Change(Timeout.Infinite, Timeout.Infinite);
            Recreate();
        }

        private ICommand sizeChangedCommand;
        public ICommand SizeChangedCommand
        {
            get { return sizeChangedCommand ?? (sizeChangedCommand = new Microsoft.Practices.Prism.Commands.DelegateCommand<SizeContainer>(OnSizeChanged)); }
        }

        /// <summary>
        /// Called when [size changed].
        /// </summary>
        /// <param name="size">The size.</param>
        private void OnSizeChanged(SizeContainer size)
        {
            RenderHeight = size.Height;
            RenderWidth = size.Width;
        }

        /// <summary>
        /// Gets or sets the height of the render.
        /// </summary>
        /// <value>
        /// The height of the render.
        /// </value>
        public double RenderHeight
        {
            set
            {
                if (Math.Abs(value - renderHeight) <= double.Epsilon) return;
                renderHeight = value;
                if (Math.Abs(renderWidth) <= double.Epsilon) return;
                changeTimer.Change(250, Timeout.Infinite);
            }
        }

        /// <summary>
        /// Recreates this instance.
        /// </summary>
        private void Recreate()
        {
            var width = (int)renderWidth;
            var height = (int)renderHeight;
            if (null != renderer)
                renderer.Stop();
            renderer = Create(width, height);
            renderer.SourceCreated += RendererOnSourceCreated;
            if (playerService.State != PlayingState.Playing) return;
            renderer.Start();
        }

        private UnmanagedBitmapRenderer Create(int width, int height)
        {
            var r = new UnmanagedBitmapRenderer(threadManager, width, height, ((width * 32) + 7) / 8) { ClearEachPass = true, DesiredFramerate = 60 };
            r.RegisterRenderCallback(RenderCallback, 0);
            return r;
        }

        /// <summary>
        /// Renderers the on source created.
        /// </summary>
        /// <param name="bitmapSource">The bitmap source.</param>
        private void RendererOnSourceCreated(BitmapSource bitmapSource)
        {
            dispatcher.Invoke(src => Image = src, bitmapSource);
        }

        ///// <summary>
        ///// The colour
        ///// </summary>
        //private const int Colour = unchecked((int)0xFF565656);

        /// <summary>
        /// Renders the callback.
        /// </summary>
        /// <param name="backBuffer">The back buffer.</param>
        /// <param name="width">The width.</param>
        /// <param name="height">The height.</param>
        /// <param name="stride">The stride.</param>
        private void RenderCallback(IntPtr backBuffer, int width, int height, int stride)
        {
            lock (syncRoot)
            {
                unsafe
                {
                    if (playerService.State != PlayingState.Playing) return;
                    float sampleRate;
                    var fft = playerService.FFT(out sampleRate, 128);
                    if (null == fft) return;
                    var freqPerChannel = ((sampleRate/2)/fft.Length);
                    var lastIndex = 21000f/freqPerChannel;
                    fft.Normalize().Transform(x => x * 1f);
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

        /// <summary>
        /// Gets the stretch.
        /// </summary>
        /// <value>
        /// The stretch.
        /// </value>
        public Stretch Stretch { get { return Stretch.Fill; } }

        /// <summary>
        /// Gets or sets the width of the render.
        /// </summary>
        /// <value>
        /// The width of the render.
        /// </value>
        public double RenderWidth
        {
            set
            {
                if (Math.Abs(value - renderWidth) <= double.Epsilon) return;
                renderWidth = value;
                if (Math.Abs(renderHeight) <= double.Epsilon) return;
                //Recreate();
                changeTimer.Change(250, Timeout.Infinite);
            }
        }

        /// <summary>
        /// Starts this instance.
        /// </summary>
        public void Start()
        {
            if (null == renderer)
            {
                return;
            }
            renderer.Start();
        }

        /// <summary>
        /// Stops this instance.
        /// </summary>
        public void Stop()
        {
            renderer.Stop();
        }

        private BitmapSource source;
        /// <summary>
        /// Gets the image.
        /// </summary>
        /// <value>
        /// The image.
        /// </value>
        public BitmapSource Image
        {
            get { return source; }
            set
            {
                if (ReferenceEquals(value, source)) return;
                source = value;
                RaisePropertyChanged(() => Image);
            }
        }

        /// <summary>
        /// Gets the name.
        /// </summary>
        /// <value>
        /// The name.
        /// </value>
        public string Name { get { return "Simple FFT"; } }
    }
}
