using System;
using iLynx.Common.Threading;
using iLynx.Common.Threading.Unmanaged;

namespace iLynx.Common.WPF.Imaging
{
    /// <summary>
    /// ManagedWriteableBitmapRendererBase
    /// </summary>
    public abstract class RendererBase
    {
        private readonly IThreadManager threadManager;
        private IWorker renderWorker;
        protected volatile bool Render = false;
        protected TimeSpan RenderInterval = TimeSpan.FromSeconds(1d / 60d);
        private bool clearEachPass = true;
        private double desiredFramerate = 60d;

        /// <summary>
        /// Initializes a new instance of the <see cref="RendererBase" /> class.
        /// </summary>
        /// <param name="threadManager">The thread manager.</param>
        protected RendererBase(IThreadManager threadManager)
        {
            threadManager.Guard("threadManager");
            this.threadManager = threadManager;
            RenderInterval = TimeSpan.FromSeconds(1d/60d);
        }

        /// <summary>
        /// Gets or sets a value indicating whether [clear each pass].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [clear each pass]; otherwise, <c>false</c>.
        /// </value>
        public bool ClearEachPass
        {
            get { return clearEachPass; }
            set { clearEachPass = value; }
        }

        /// <summary>
        /// Gets or sets the desired framerate.
        /// </summary>
        /// <value>
        /// The desired framerate.
        /// </value>
        public double DesiredFramerate
        {
            get { return desiredFramerate; }
            set
            {
                desiredFramerate = value;
                RenderInterval = TimeSpan.FromSeconds(1d / desiredFramerate);
            }
        }

        /// <summary>
        /// Renders the loop.
        /// </summary>
        protected abstract void RenderLoop();

        /// <summary>
        /// Starts this instance.
        /// </summary>
        public void Start()
        {
            Stop();
            Render = true;
            renderWorker = threadManager.StartNew(RenderLoop);
        }

        /// <summary>
        /// Gets a value indicating whether this instance is running.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance is running; otherwise, <c>false</c>.
        /// </value>
        public bool IsRunning
        {
            get { return Render; }
        }

        /// <summary>
        /// Stops this instance.
        /// </summary>
        public void Stop()
        {
            Render = false;
            if (null == renderWorker) return;
            try
            {
                renderWorker.Wait(RenderInterval + TimeSpan.FromMilliseconds(RenderInterval.TotalMilliseconds*5)); // This is a completely arbitrary number
            }
            catch (TimeoutException)
            {
                renderWorker.Abort();
            }
        }
    }
}