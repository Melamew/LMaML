namespace iLynx.Common.WPF.Imaging
{
    /// <summary>
    /// UnsafeFunctions
    /// </summary>
    public unsafe  static class UnsafeFunctions
    {
        private const int PixelSize = 4;
        #region Thanks to WriteableBitmapEx (http://writeablebitmapex.codeplex.com/SourceControl/changeset/view/99832#1157213)
        
        /// <summary>
        /// Fills the rect.
        /// </summary>
        /// <param name="buffer">The buffer.</param>
        /// <param name="width">The width.</param>
        /// <param name="height">The height.</param>
        /// <param name="x1">The x1.</param>
        /// <param name="y1">The y1.</param>
        /// <param name="x2">The x2.</param>
        /// <param name="y2">The y2.</param>
        /// <param name="colour">The colour.</param>
        public static void FillRect(byte* buffer, int width, int height, int x1, int y1, int x2, int y2, int colour)
        {
            // Use refs for faster access (really important!) speeds up a lot!
            var w = width;
            var h = height;
            var pixels = (int*)buffer;

            // Check boundaries
            if ((x1 < 0 && x2 < 0) || (y1 < 0 && y2 < 0)
             || (x1 >= w && x2 >= w) || (y1 >= h && y2 >= h))
            {
                return;
            }

            // Clamp boundaries
            if (x1 < 0) { x1 = 0; }
            if (y1 < 0) { y1 = 0; }
            if (x2 < 0) { x2 = 0; }
            if (y2 < 0) { y2 = 0; }
            if (x1 >= w) { x1 = w - 1; }
            if (y1 >= h) { y1 = h - 1; }
            if (x2 >= w) { x2 = w - 1; }
            if (y2 >= h) { y2 = h - 1; }

            // Fill first line
            var startY = y1 * w;
            var startYPlusX1 = startY + x1;
            var endOffset = startY + x2;
            for (var x = startYPlusX1; x <= endOffset; x++)
            {
                pixels[x] = colour;
            }

            // Copy first line
            var len = (x2 - x1 + 1) * PixelSize;
            var srcOffsetBytes = startYPlusX1 * PixelSize;
            var offset2 = y2 * w + x1;
            for (var y = startYPlusX1 + w; y <= offset2; y += w)
            {
                NativeMethods.MemCpy(buffer, srcOffsetBytes, buffer, y * PixelSize, len);
                //BitmapContext.BlockCopy(context, srcOffsetBytes, context, y * SizeOfArgb, len);
            }
        }
        #endregion

        /// <summary>
        /// Fills the row.
        /// </summary>
        /// <param name="buffer">The buffer.</param>
        /// <param name="width">The width.</param>
        /// <param name="height">The height.</param>
        /// <param name="xStart">The x start.</param>
        /// <param name="xEnd">The x end.</param>
        /// <param name="y">The y.</param>
        /// <param name="colour">The colour.</param>
        public static void FillRow(byte* buffer, int width, int height, int xStart, int xEnd, int y, int colour)
        {
            // TODO: Make this a simple memset if possible?
            FillRect(buffer, width, height, xStart, y, xEnd, y, colour);
        }
    }
}
