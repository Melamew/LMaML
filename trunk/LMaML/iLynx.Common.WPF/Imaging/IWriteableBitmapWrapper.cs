using System;
using System.Security;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace iLynx.Common.WPF.Imaging
{
    /// <summary>
    /// IWriteableBitmapWrapper
    /// </summary>
    public interface IWriteableBitmapWrapper
    {
        // Summary:
        //     Gets a pointer to the back buffer.
        //
        // Returns:
        //     An System.IntPtr that points to the base address of the back buffer.
        IntPtr BackBuffer { get; }
        //
        // Summary:
        //     Gets a value indicating the number of bytes in a single row of pixel data.
        //
        // Returns:
        //     An integer indicating the number of bytes in a single row of pixel data.
        int BackBufferStride { get; }

        // Summary:
        //     Gets the horizontal dots per inch (dpi) of the image.
        //
        // Returns:
        //     The horizontal dots per inch (dpi) of the image; that is, the dots per inch
        //     (dpi) along the x-axis.
        double DpiX { get; }
        //
        // Summary:
        //     Gets the vertical dots per inch (dpi) of the image.
        //
        // Returns:
        //     The vertical dots per inch (dpi) of the image; that is, the dots per inch
        //     (dpi) along the y-axis.
        double DpiY { get; }
        //
        // Summary:
        //     Gets the native System.Windows.Media.PixelFormat of the bitmap data.
        //
        // Returns:
        //     The pixel format of the bitmap data.
        PixelFormat Format { get; }
        //
        // Summary:
        //     Gets the height of the source bitmap in device-independent units (1/96th
        //     inch per unit).
        //
        // Returns:
        //     The height of the bitmap in device-independent units (1/96th inch per unit).
        double Height { get; }
        //
        // Summary:
        //     Gets a value that indicates whether the System.Windows.Media.Imaging.BitmapSource
        //     content is currently downloading.
        //
        // Returns:
        //     true if the bitmap source is currently downloading; otherwise, false.
        bool IsDownloading { get; }
        //
        // Summary:
        //     Gets the metadata that is associated with this bitmap image.
        //
        // Returns:
        //     The metadata that is associated with the bitmap image.
        ImageMetadata Metadata { get; }
        //
        // Summary:
        //     Gets the color palette of the bitmap, if one is specified.
        //
        // Returns:
        //     The color palette of the bitmap.
        BitmapPalette Palette { get; }
        //
        // Summary:
        //     Gets the height of the bitmap in pixels.
        //
        // Returns:
        //     The height of the bitmap in pixels.
        int PixelHeight { get; }
        //
        // Summary:
        //     Gets the width of the bitmap in pixels.
        //
        // Returns:
        //     The width of the bitmap in pixels.
        int PixelWidth { get; }
        //
        // Summary:
        //     Gets the width of the bitmap in device-independent units (1/96th inch per
        //     unit).
        //
        // Returns:
        //     The width of the bitmap in device-independent units (1/96th inch per unit).
        double Width { get; }

        // Summary:
        //     Occurs when the image fails to load, due to a corrupt image header.
        event EventHandler<ExceptionEventArgs> DecodeFailed;
        //
        // Summary:
        //     Occurs when the bitmap content has been completely downloaded.
        event EventHandler DownloadCompleted;
        //
        // Summary:
        //     Occurs when the bitmap content failed to download.
        event EventHandler<ExceptionEventArgs> DownloadFailed;
        //
        // Summary:
        //     Occurs when the download progress of the bitmap content has changed.
        event EventHandler<DownloadProgressEventArgs> DownloadProgress;
        //
        // Summary:
        //     Copies the bitmap pixel data into an array of pixels with the specified stride,
        //     starting at the specified offset.
        //
        // Parameters:
        //   pixels:
        //     The destination array.
        //
        //   stride:
        //     The stride of the bitmap.
        //
        //   offset:
        //     The pixel location where copying starts.
        [SecurityCritical]
        void CopyPixels(Array pixels, int stride, int offset);
        //
        // Summary:
        //     Copies the bitmap pixel data within the specified rectangle into an array
        //     of pixels that has the specified stride starting at the specified offset.
        //
        // Parameters:
        //   sourceRect:
        //     The source rectangle to copy. An System.Windows.Int32Rect.Empty value specifies
        //     the entire bitmap.
        //
        //   pixels:
        //     The destination array.
        //
        //   stride:
        //     The stride of the bitmap.
        //
        //   offset:
        //     The pixel location where copying begins.
        [SecurityCritical]
        void CopyPixels(Int32Rect sourceRect, Array pixels, int stride, int offset);
        //
        // Summary:
        //     Copies the bitmap pixel data within the specified rectangle
        //
        // Parameters:
        //   sourceRect:
        //     The source rectangle to copy. An System.Windows.Int32Rect.Empty value specifies
        //     the entire bitmap.
        //
        //   buffer:
        //     A pointer to the buffer.
        //
        //   bufferSize:
        //     The size of the buffer.
        //
        //   stride:
        //     The stride of the bitmap.
        [SecurityCritical]
        void CopyPixels(Int32Rect sourceRect, IntPtr buffer, int bufferSize, int stride);
        
        /// <summary>
        /// Gets the bitmap source.
        /// </summary>
        /// <returns></returns>
        WriteableBitmap GetSourceReference();

        // Summary:
        //     Specifies the area of the bitmap that changed.
        //
        // Parameters:
        //   dirtyRect:
        //     An System.Windows.Int32Rect representing the area that changed. Dimensions
        //     are in pixels.
        //
        // Exceptions:
        //   System.InvalidOperationException:
        //     The bitmap has not been locked by a call to the System.Windows.Media.Imaging.WriteableBitmap.Lock()
        //     or System.Windows.Media.Imaging.WriteableBitmap.TryLock(System.Windows.Duration)
        //     methods.
        //
        //   System.ArgumentOutOfRangeException:
        //     dirtyRect falls outside the bounds of the System.Windows.Media.Imaging.WriteableBitmap.
        [SecurityCritical]
        void AddDirtyRect(Int32Rect dirtyRect);
        //
        // Summary:
        //     Creates a modifiable clone of this System.Windows.Media.Imaging.WriteableBitmap,
        //     making deep copies of this object's values. When copying dependency properties,
        //     this method copies resource references and data bindings (but they might
        //     no longer resolve) but not animations or their current values.
        //
        // Returns:
        //     A modifiable clone of the current object. The cloned object's System.Windows.Freezable.IsFrozen
        //     property will be false even if the source's System.Windows.Freezable.IsFrozen
        //     property was true.
        WriteableBitmap Clone();
        //
        // Summary:
        //     Creates a modifiable clone of this System.Windows.Media.Animation.ByteAnimationUsingKeyFrames
        //     object, making deep copies of this object's current values. Resource references,
        //     data bindings, and animations are not copied, but their current values are.
        //
        // Returns:
        //     A modifiable clone of the current object. The cloned object's System.Windows.Freezable.IsFrozen
        //     property will be false even if the source's System.Windows.Freezable.IsFrozen
        //     property was true.
        WriteableBitmap CloneCurrentValue();
        //
        // Summary:
        //     Reserves the back buffer for updates.
        void Lock();
        //
        // Summary:
        //     Attempts to lock the bitmap, waiting for no longer than the specified length
        //     of time.
        //
        // Parameters:
        //   timeout:
        //     A System.Windows.Duration that represents the length of time to wait. A value
        //     of 0 returns immediately. A value of System.Windows.Duration.Forever blocks
        //     indefinitely.
        //
        // Returns:
        //     true if the lock was acquired; otherwise, false.
        //
        // Exceptions:
        //   System.ArgumentOutOfRangeException:
        //     timeout is set to System.Windows.Duration.Automatic.
        [SecurityCritical]
        bool TryLock(Duration timeout);
        //
        // Summary:
        //     Releases the back buffer to make it available for display.
        //
        // Exceptions:
        //   System.InvalidOperationException:
        //     The bitmap has not been locked by a call to the System.Windows.Media.Imaging.WriteableBitmap.Lock()
        //     or System.Windows.Media.Imaging.WriteableBitmap.TryLock(System.Windows.Duration)
        //     methods.
        [SecurityCritical]
        void Unlock();
        //
        // Summary:
        //     Updates the pixels in the specified region of the bitmap.
        //
        // Parameters:
        //   sourceRect:
        //     The rectangle of the System.Windows.Media.Imaging.WriteableBitmap to update.
        //
        //   pixels:
        //     The pixel array used to update the bitmap.
        //
        //   stride:
        //     The stride of the update region in pixels.
        //
        //   offset:
        //     The input buffer offset.
        //
        // Exceptions:
        //   System.ArgumentOutOfRangeException:
        //     One or more of the following conditions is true.sourceRect falls outside
        //     the bounds of the System.Windows.Media.Imaging.WriteableBitmap. stride <
        //     1 offset < 0
        //
        //   System.ArgumentNullException:
        //     pixels is null.
        //
        //   System.ArgumentException:
        //     pixels has a rank other than 1 or 2, or its length is less than or equal
        //     to 0.
        [SecurityCritical]
        void WritePixels(Int32Rect sourceRect, Array pixels, int stride, int offset);
        //
        // Summary:
        //     Updates the pixels in the specified region of the bitmap.
        //
        // Parameters:
        //   sourceRect:
        //     The rectangle of the System.Windows.Media.Imaging.WriteableBitmap to update.
        //
        //   buffer:
        //     The input buffer used to update the bitmap.
        //
        //   bufferSize:
        //     The size of the input buffer.
        //
        //   stride:
        //     The stride of the update region in buffer.
        //
        // Exceptions:
        //   System.ArgumentOutOfRangeException:
        //     One or more of the following conditions is true.sourceRect falls outside
        //     the bounds of the System.Windows.Media.Imaging.WriteableBitmap.bufferSize
        //     < 1 stride < 1
        //
        //   System.ArgumentNullException:
        //     buffer is null.
        [SecurityCritical]
        void WritePixels(Int32Rect sourceRect, IntPtr buffer, int bufferSize, int stride);
        //
        // Summary:
        //     Updates the pixels in the specified region of the bitmap.
        //
        // Parameters:
        //   sourceRect:
        //     The rectangle in sourceBuffer to copy.
        //
        //   sourceBuffer:
        //     The input buffer used to update the bitmap.
        //
        //   sourceBufferStride:
        //     The stride of the input buffer, in bytes.
        //
        //   destinationX:
        //     The destination x-coordinate of the left-most pixel in the back buffer.
        //
        //   destinationY:
        //     The destination y-coordinate of the top-most pixel in the back buffer.
        //
        // Exceptions:
        //   System.ArgumentOutOfRangeException:
        //     One or more of the following conditions is true.sourceRect falls outside
        //     the bounds of the System.Windows.Media.Imaging.WriteableBitmap.destinationX
        //     or destinationY is outside the bounds of the System.Windows.Media.Imaging.WriteableBitmap.
        //     sourceBufferStride < 1
        //
        //   System.ArgumentNullException:
        //     sourceBuffer is null.
        //
        //   System.ArgumentException:
        //     sourceBuffer has a rank other than 1 or 2, or its length is less than or
        //     equal to 0.
        [SecurityCritical]
        void WritePixels(Int32Rect sourceRect, Array sourceBuffer, int sourceBufferStride, int destinationX, int destinationY);
        //
        // Summary:
        //     Updates the pixels in the specified region of the bitmap.
        //
        // Parameters:
        //   sourceRect:
        //     The rectangle in sourceBuffer to copy.
        //
        //   sourceBuffer:
        //     The input buffer used to update the bitmap.
        //
        //   sourceBufferSize:
        //     The size of the input buffer.
        //
        //   sourceBufferStride:
        //     The stride of the input buffer, in bytes.
        //
        //   destinationX:
        //     The destination x-coordinate of the left-most pixel in the back buffer.
        //
        //   destinationY:
        //     The destination y-coordinate of the top-most pixel in the back buffer.
        //
        // Exceptions:
        //   System.ArgumentOutOfRangeException:
        //     One or more of the following conditions is true.sourceRect falls outside
        //     the bounds of the System.Windows.Media.Imaging.WriteableBitmap. destinationX
        //     or destinationY is outside the bounds of the System.Windows.Media.Imaging.WriteableBitmap.sourceBufferSize
        //     < 1 sourceBufferStride < 1
        //
        //   System.ArgumentNullException:
        //     sourceBuffer is null.
        [SecurityCritical]
        void WritePixels(Int32Rect sourceRect, IntPtr sourceBuffer, int sourceBufferSize, int sourceBufferStride, int destinationX, int destinationY);
    }
}
