using System;
using System.Threading;
using System.Windows;
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

namespace LMaML.Infrastructure.Visualization
{
    public abstract class VisualizationViewModelBase : NotificationBase, IVisualization
    {
        private readonly IThreadManager threadManager;
        protected readonly IPlayerService PlayerService;
        private readonly IDispatcher dispatcher;
        private UnmanagedBitmapRenderer renderer;
        protected double TargetRenderHeight;
        protected double TargetRenderWidth;
        protected readonly object SyncRoot = new object();
        private readonly Timer changeTimer;
        private bool isResizing;
        private ICommand sizeChangedCommand;
        private BitmapSource source;

        /// <summary>
        /// </summary>
        /// <param name="logger">The logger.</param>
        /// <param name="threadManager">The thread manager.</param>
        /// <param name="playerService">The player service.</param>
        /// <param name="publicTransport">The public transport.</param>
        /// <param name="dispatcher">The dispatcher.</param>
        protected VisualizationViewModelBase(ILogger logger, IThreadManager threadManager, IPlayerService playerService, IPublicTransport publicTransport, IDispatcher dispatcher)
            : base(logger)
        {
            this.threadManager = threadManager;
            PlayerService = playerService;
            this.dispatcher = dispatcher;
            publicTransport.ApplicationEventBus.Subscribe<PlayingStateChangedEvent>(OnPlayingStateChanged);
            publicTransport.ApplicationEventBus.Subscribe<ShellCollapsedEvent>(OnShellCollapsed);
            publicTransport.ApplicationEventBus.Subscribe<ShellExpandedEvent>(OnShellExpanded);
            publicTransport.ApplicationEventBus.Subscribe<ShellResizeBeginEvent>(OnShellResizeBegin);
            publicTransport.ApplicationEventBus.Subscribe<ShellResizeEndEvent>(OnShellResizeEnd);
            changeTimer = new Timer(SizeChanged);
            isVisible = true;
        }

        private ICommand loadedCommand;
        public ICommand LoadedCommand
        {
            get { return loadedCommand ?? (loadedCommand = new DelegateCommand<FrameworkElement>(OnLoaded)); }
        }

        private void OnLoaded(FrameworkElement frameworkElement)
        {
            RenderHeight = frameworkElement.ActualHeight;
            RenderWidth = frameworkElement.ActualWidth;
        }

        private ICommand dataContextChangedCommand;
        public ICommand DataContextChangedCommand
        {
            get { return dataContextChangedCommand ?? (dataContextChangedCommand = new DelegateCommand<FrameworkElement>(OnDataContextChanged)); }
        }

        private void OnDataContextChanged(FrameworkElement element)
        {
            LogDebug("DataContext: {0}", element.DataContext);
            LogDebug("Width: {0}", element.ActualWidth);
            LogDebug("Height: {0}", element.ActualHeight);
            var vis = element.DataContext as VisualizationViewModelBase;
            if (null == vis) return;
            vis.RenderHeight = element.ActualHeight;
            vis.RenderWidth = element.ActualWidth;
        }

        private ICommand isVisibleChangedCommand;
        public ICommand IsVisibleChangedCommand
        {
            get { return isVisibleChangedCommand ?? (isVisibleChangedCommand = new DelegateCommand<bool>(OnVisibilityChanged)); }
        }

        private bool isVisible;

        private void OnVisibilityChanged(bool visibility)
        {
            isVisible = visibility;
            if (visibility)
                Start();
            else
                Stop();
        }

        public ICommand SizeChangedCommand
        {
            get { return sizeChangedCommand ?? (sizeChangedCommand = new DelegateCommand<SizeContainer>(OnSizeChanged)); }
        }

        /// <summary>
        /// Gets or sets the height of the render.
        /// </summary>
        /// <value>
        /// The height of the render.
        /// </value>
        public virtual double RenderHeight
        {
            set
            {
                if (Math.Abs(value - TargetRenderHeight) <= double.Epsilon) return;
                TargetRenderHeight = value;
                if (Math.Abs(TargetRenderWidth) <= double.Epsilon) return;
                ResizeInit();
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
        public virtual double RenderWidth
        {
            set
            {
                if (Math.Abs(value - TargetRenderWidth) <= double.Epsilon) return;
                TargetRenderWidth = value;
                if (Math.Abs(TargetRenderHeight) <= double.Epsilon) return;
                ResizeInit();
            }
        }

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

        private void OnShellResizeEnd(ShellResizeEndEvent shellResizeEndEvent)
        {
            isResizing = false;
            ResizeInit();
        }

        private void OnShellResizeBegin(ShellResizeBeginEvent shellResizeBeginEvent)
        {
            changeTimer.Change(Timeout.Infinite, Timeout.Infinite);
            isResizing = true;
        }

        private void OnShellExpanded(ShellExpandedEvent shellExpandedEvent)
        {
            Start();
        }

        private void OnShellCollapsed(ShellCollapsedEvent shellCollapsedEvent)
        {
            Stop();
        }

        private void OnPlayingStateChanged(PlayingStateChangedEvent playingStateChangedEvent)
        {
            if (null == renderer)
                return;

            if (playingStateChangedEvent.NewState != PlayingState.Playing)
                Stop();
            else if (!renderer.IsRunning)
                Start();
        }

        protected virtual void OnStopped()
        {

        }

        protected virtual void OnStarted()
        {

        }

        protected virtual void ResizeInit()
        {
            changeTimer.Change(250, Timeout.Infinite);
        }

        /// <summary>
        /// Sizes the changed.
        /// </summary>
        /// <param name="state">The state.</param>
        private void SizeChanged(object state)
        {
            if (isResizing) return;
            changeTimer.Change(Timeout.Infinite, Timeout.Infinite);
            Recreate();
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
        /// Recreates this instance.
        /// </summary>
        private void Recreate()
        {
            var width = (int)TargetRenderWidth;
            var height = (int)TargetRenderHeight;
            if (null != renderer)
                renderer.Stop();
            if (0 == width || 0 == height) return;
            renderer = Create(width, height);
            renderer.SourceCreated += RendererOnSourceCreated;
            if (PlayerService.State != PlayingState.Playing) return;
            Start();
        }

        protected virtual double DesiredFramerate
        {
            get { return 60d; }
        }

        private UnmanagedBitmapRenderer Create(int width, int height)
        {
            var r = new UnmanagedBitmapRenderer(threadManager, dispatcher, width, height, ((width * 32) + 7) / 8) { ClearEachPass = true, DesiredFramerate = DesiredFramerate };
            r.RegisterRenderCallback(Render, 0);
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

        /// <summary>
        /// Renders the callback.
        /// </summary>
        /// <param name="backBuffer">The back buffer.</param>
        /// <param name="width">The width.</param>
        /// <param name="height">The height.</param>
        /// <param name="stride">The stride.</param>
        protected abstract void Render(IntPtr backBuffer, int width, int height, int stride);

        /// <summary>
        /// Starts this instance.
        /// </summary>
        public void Start()
        {
            if (!isVisible) return;
            if (null == renderer)
            {
                if (0 == (int)TargetRenderHeight || 0 == (int)TargetRenderWidth) return;
                renderer = Create((int)TargetRenderWidth, (int)TargetRenderHeight);
                renderer.SourceCreated += RendererOnSourceCreated;
            }
            renderer.Start();
            OnStarted();
        }

        /// <summary>
        /// Stops this instance.
        /// </summary>
        public void Stop()
        {
            if (null == renderer) return;
            renderer.Stop();
            OnStopped();
        }
    }
}