using System;
using System.Runtime;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace iLynx.Common.WPF.Imaging
{
    /// <summary>
    /// WriteableBitmapWrapper
    /// </summary>
    public class WriteableBitmapWrapper : IWriteableBitmapWrapper
    {
        private readonly WriteableBitmap source;

        /// <summary>
        /// Initializes a new instance of the <see cref="WriteableBitmapWrapper" /> class.
        /// </summary>
        /// <param name="pixelWidth">Width of the pixel.</param>
        /// <param name="pixelHeight">Height of the pixel.</param>
        /// <param name="dpiX">The dpi X.</param>
        /// <param name="dpiY">The dpi Y.</param>
        /// <param name="pixelFormat">The pixel format.</param>
        /// <param name="palette">The palette.</param>
        public WriteableBitmapWrapper(int pixelWidth, int pixelHeight, PixelFormat pixelFormat, int dpiX = 96, int dpiY = 96, BitmapPalette palette = null)
        {
            pixelFormat.Guard("pixelFormat");
            source = new WriteableBitmap(pixelWidth, pixelHeight, dpiX, dpiY, pixelFormat, palette);
        }
        
        /// <summary>
        /// Bitmaps the source.
        /// </summary>
        /// <param name="wrapper">The wrapper.</param>
        /// <returns></returns>
        public static implicit operator BitmapSource(WriteableBitmapWrapper wrapper)
        {
            return wrapper.source;
        }

        /// <summary>
        /// Writeables the bitmap.
        /// </summary>
        /// <param name="wrapper">The wrapper.</param>
        /// <returns></returns>
        public static implicit operator WriteableBitmap(WriteableBitmapWrapper wrapper)
        {
            return wrapper.source;
        }

        /// <summary>
        /// Gets the bitmap source.
        /// </summary>
        /// <returns></returns>
        public WriteableBitmap GetSourceReference()
        {
            return source;
        }

        public PixelFormat Format
        {
            [TargetedPatchingOptOut("Inlined because this is a wrapper around a .NET object")]
            get { return source.Format; }
        }
        public double Height
        {
            [TargetedPatchingOptOut("Inlined because this is a wrapper around a .NET object")]
            get { return source.Height; }
        }

        public bool IsDownloading
        {
            [TargetedPatchingOptOut("Inlined because this is a wrapper around a .NET object")]
            get { return source.IsDownloading; }
        }

        public ImageMetadata Metadata
        {
            [TargetedPatchingOptOut("Inlined because this is a wrapper around a .NET object")]
            get { return source.Metadata; }
        }
        public BitmapPalette Palette
        {
            [TargetedPatchingOptOut("Inlined because this is a wrapper around a .NET object")]
            get { return source.Palette; }
        }
        public int PixelHeight
        {
            [TargetedPatchingOptOut("Inlined because this is a wrapper around a .NET object")]
            get { return source.PixelHeight; }
        }
        public int PixelWidth
        {
            [TargetedPatchingOptOut("Inlined because this is a wrapper around a .NET object")]
            get { return source.PixelWidth; }
        }
        public double Width
        {
            [TargetedPatchingOptOut("Inlined because this is a wrapper around a .NET object")]
            get { return source.Width; }
        }
        
        public event EventHandler<ExceptionEventArgs> DecodeFailed
        {
            [TargetedPatchingOptOut("Inlined because this is a wrapper around a .NET object")]
            add { source.DecodeFailed += value; }
            [TargetedPatchingOptOut("Inlined because this is a wrapper around a .NET object")]
            remove { source.DecodeFailed -= value; }
        }

        public event EventHandler DownloadCompleted
        {
            [TargetedPatchingOptOut("Inlined because this is a wrapper around a .NET object")]
            add { source.DownloadCompleted += value; }
            [TargetedPatchingOptOut("Inlined because this is a wrapper around a .NET object")]
            remove { source.DownloadCompleted -= value; }
        }

        public event EventHandler<ExceptionEventArgs> DownloadFailed
        {
            [TargetedPatchingOptOut("Inlined because this is a wrapper around a .NET object")]
            add { source.DownloadFailed += value; }
            [TargetedPatchingOptOut("Inlined because this is a wrapper around a .NET object")]
            remove { source.DownloadFailed -= value; }
        }

        public event EventHandler<DownloadProgressEventArgs> DownloadProgress
        {
            [TargetedPatchingOptOut("Inlined because this is a wrapper around a .NET object")]
            add { source.DownloadProgress += value; }
            [TargetedPatchingOptOut("Inlined because this is a wrapper around a .NET object")]
            remove { source.DownloadProgress -= value; }
        }
        
        [TargetedPatchingOptOut("Inlined because this is a wrapper around a .NET object")]
        public void CopyPixels(Array pixels, int stride, int offset)
        {
            source.CopyPixels(pixels, stride, offset);
        }

        [TargetedPatchingOptOut("Inlined because this is a wrapper around a .NET object")]
        public void CopyPixels(Int32Rect sourceRect, Array pixels, int stride, int offset)
        {
            source.CopyPixels(sourceRect, pixels, stride, offset);
        }

        [TargetedPatchingOptOut("Inlined because this is a wrapper around a .NET object")]
        public void CopyPixels(Int32Rect sourceRect, IntPtr buffer, int bufferSize, int stride)
        {
            source.CopyPixels(sourceRect, buffer, bufferSize, stride);
        }

        public IntPtr BackBuffer
        {
            [TargetedPatchingOptOut("Inlined because this is a wrapper around a .NET object")]
            get { return source.BackBuffer; }
        }
        public int BackBufferStride
        {
            [TargetedPatchingOptOut("Inlined because this is a wrapper around a .NET object")]
            get { return source.BackBufferStride; }
        }
        public double DpiX
        {
            [TargetedPatchingOptOut("Inlined because this is a wrapper around a .NET object")]
            get { return source.DpiX; }
        }
        public double DpiY
        {
            [TargetedPatchingOptOut("Inlined because this is a wrapper around a .NET object")]
            get { return source.DpiY; }
        }

        [TargetedPatchingOptOut("Inlined because this is a wrapper around a .NET object")]
        public void AddDirtyRect(Int32Rect dirtyRect)
        {
            source.AddDirtyRect(dirtyRect);
        }

        [TargetedPatchingOptOut("Inlined because this is a wrapper around a .NET object")]
        public WriteableBitmap Clone()
        {
            return source.Clone();
        }

        [TargetedPatchingOptOut("Inlined because this is a wrapper around a .NET object")]
        public WriteableBitmap CloneCurrentValue()
        {
            return source.CloneCurrentValue();
        }

        [TargetedPatchingOptOut("Inlined because this is a wrapper around a .NET object")]
        public void Lock()
        {
            source.Lock();
        }

        [TargetedPatchingOptOut("Inlined because this is a wrapper around a .NET object")]
        public bool TryLock(Duration timeout)
        {
            return source.TryLock(timeout);
        }

        [TargetedPatchingOptOut("Inlined because this is a wrapper around a .NET object")]
        public void Unlock()
        {
            source.Unlock();
        }

        [TargetedPatchingOptOut("Inlined because this is a wrapper around a .NET object")]
        public void WritePixels(Int32Rect sourceRect, Array pixels, int stride, int offset)
        {
            source.WritePixels(sourceRect, pixels, stride, offset);
        }

        [TargetedPatchingOptOut("Inlined because this is a wrapper around a .NET object")]
        public void WritePixels(Int32Rect sourceRect, IntPtr buffer, int bufferSize, int stride)
        {
            source.WritePixels(sourceRect, buffer, bufferSize, stride);
        }

        [TargetedPatchingOptOut("Inlined because this is a wrapper around a .NET object")]
        public void WritePixels(Int32Rect sourceRect, Array sourceBuffer, int sourceBufferStride, int destinationX, int destinationY)
        {
            source.WritePixels(sourceRect, sourceBuffer, sourceBufferStride, destinationX, destinationY);
        }

        [TargetedPatchingOptOut("Inlined because this is a wrapper around a .NET object")]
        public void WritePixels(Int32Rect sourceRect, IntPtr sourceBuffer, int sourceBufferSize, int sourceBufferStride,
                                int destinationX, int destinationY)
        {
            source.WritePixels(sourceRect, sourceBuffer, sourceBufferSize, sourceBufferStride,
                               destinationX, destinationY);
        }
    }

    /// <summary>
    /// IRenderProxy
    /// </summary>
    public interface IRenderProxy
    {
        /// <summary>
        /// Gets the height.
        /// </summary>
        /// <value>
        /// The height.
        /// </value>
        int Height { get; }
        /// <summary>
        /// Gets the width.
        /// </summary>
        /// <value>
        /// The width.
        /// </value>
        int Width { get; }
        /// <summary>
        /// Gets the back buffer stride.
        /// </summary>
        /// <value>
        /// The back buffer stride.
        /// </value>
        int BackBufferStride { get; }
    }

    /// <summary>
    /// RenderProxy
    /// </summary>
    public class RenderProxy : IRenderProxy
    {
        public int Height { get; private set; }
        public int Width { get; private set; }
        public int BackBufferStride { get; private set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="RenderProxy" /> class.
        /// </summary>
        /// <param name="height">The height.</param>
        /// <param name="width">The width.</param>
        /// <param name="backBufferStride">The back buffer stride.</param>
        public RenderProxy(int height, int width, int backBufferStride)
        {
            Height = height;
            Width = width;
            BackBufferStride = backBufferStride;
        }
    }
}
