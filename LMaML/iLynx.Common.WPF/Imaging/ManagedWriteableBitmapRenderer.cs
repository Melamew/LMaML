using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows;
using iLynx.Common.Threading;

namespace iLynx.Common.WPF.Imaging
{
    /// <summary>
    /// ManagedWriteableBitmapRenderer
    /// </summary>
    public class ManagedWriteableBitmapRenderer : RendererBase
    {
        /// <summary>
        /// Used for rendering, the backBuffer parameter will be a pointer to the writeablebitmap's backbuffer.
        /// </summary>
        /// <param name="backBuffer">The back buffer.</param>
        /// <param name="width">The width.</param>
        /// <param name="height">The height.</param>
        /// <param name="stride">The stride.</param>
        public delegate void RenderCallback(byte[] backBuffer, int width, int height, int stride);

        private IWriteableBitmapWrapper target;
        private readonly SortedList<int, RenderCallback> renderCallbacks = new SortedList<int, RenderCallback>();

        /// <summary>
        /// Initializes a new instance of the <see cref="UnmanagedWriteableBitmapRenderer" /> class.
        /// </summary>
        /// <param name="threadManager">The thread manager.</param>
        /// <param name="target">The target.</param>
        public ManagedWriteableBitmapRenderer(IThreadManager threadManager, IWriteableBitmapWrapper target) : base(threadManager)
        {
            target.Guard("target");
            this.target = target;
        }

        /// <summary>
        /// Renders the loop.
        /// </summary>
        protected override void RenderLoop()
        {
            var buffer = new byte[target.PixelHeight*target.BackBufferStride];
            while (Render)
            {
                Thread.CurrentThread.Join(RenderInterval);
                var cnt = renderCallbacks.Count - 1;
                if (!target.TryLock(new Duration(RenderInterval))) continue;
                if (ClearEachPass) NativeMethods.MemSet(target.BackBuffer, 0, buffer.Length);
                Marshal.Copy(target.BackBuffer, buffer, 0, buffer.Length);
                while (cnt-- > 0)
                    renderCallbacks.Values[cnt](buffer, target.PixelWidth, target.PixelHeight, target.BackBufferStride);
                target.Unlock();
            }
        }

        /// <summary>
        /// Gets or sets the bitmap.
        /// </summary>
        /// <value>
        /// The bitmap.
        /// </value>
        public IWriteableBitmapWrapper Bitmap
        {
            get { return target; }
            set
            {
                Stop();
                target = value;
                Start();
            }
        }

        /// <summary>
        /// Adjusts the priority.
        /// </summary>
        /// <param name="priority">The priority.</param>
        /// <param name="breaker">if set to <c>true</c> [breaker].</param>
        protected virtual void AdjustPriority(ref int priority, bool breaker)
        {
            while (renderCallbacks.ContainsKey(priority))
                priority += (breaker ? 1 : -1);
            // Invert priority to allow the renderthread to go through the callbacks in reverse order
            priority *= -1;
        }

        /// <summary>
        /// Registers the render callback.
        /// </summary>
        /// <param name="callback">The callback.</param>
        /// <param name="priority">The priority.</param>
        /// <param name="tieBreaker">if set to <c>true</c> [tie breaker].</param>
        public void RegisterRenderCallback(RenderCallback callback, int priority, bool tieBreaker = false)
        {
            var prio = priority * 100;
            lock (renderCallbacks)
            {
                AdjustPriority(ref prio, tieBreaker);
                renderCallbacks.Add(prio, callback);
            }
        }

        /// <summary>
        /// Removes the render callback.
        /// </summary>
        /// <param name="callback">The callback.</param>
        public void RemoveRenderCallback(RenderCallback callback)
        {
            lock (renderCallbacks)
            {
                var exists = renderCallbacks.Values.Contains(callback);
                if (!exists) return;
                var kvp = renderCallbacks.FirstOrDefault(k => k.Value == callback);
                renderCallbacks.Remove(kvp.Key);
            }
        }
    }
}
