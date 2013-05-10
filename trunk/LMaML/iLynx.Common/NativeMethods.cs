using System;
using System.Runtime;
using System.Runtime.InteropServices;

namespace iLynx.Common
{
    /// <summary>
    /// NativeMethods
    /// </summary>
    public static class NativeMethods
    {
        #region MemSet and MemCpy, thanks for these goes to the people at http://writeablebitmapex.codeplex.com/
        /// <summary>
        /// Copies the unmanaged memory.
        /// </summary>
        /// <param name="srcPtr">The SRC PTR.</param>
        /// <param name="srcOffset">The SRC offset.</param>
        /// <param name="dstPtr">The DST PTR.</param>
        /// <param name="dstOffset">The DST offset.</param>
        /// <param name="count">The count.</param>
        [TargetedPatchingOptOut("Inlined for performance reasons")]
        public static unsafe void MemCpy(byte* srcPtr, int srcOffset, byte* dstPtr, int dstOffset, int count)
        {
            srcPtr += srcOffset;
            dstPtr += dstOffset;

            memcpy(dstPtr, srcPtr, count);
        }

        /// <summary>
        /// Sets the unmanaged memory.
        /// </summary>
        /// <param name="dst">The DST.</param>
        /// <param name="filler">The filler.</param>
        /// <param name="count">The count (int bytes).</param>
        [TargetedPatchingOptOut("Inlined for performance reasons")]
        public static void MemSet(IntPtr dst, int filler, int count)
        {
            memset(dst, filler, count);
        }

        // Win32 memory copy function
        //[DllImport("ntdll.dll")]
        [DllImport("msvcrt.dll", EntryPoint = "memcpy", CallingConvention = CallingConvention.Cdecl, SetLastError = false)]
        private static extern unsafe byte* memcpy(
            byte* dst,
            byte* src,
            int count);

        // Win32 memory set function
        //[DllImport("ntdll.dll")]
        //[DllImport("coredll.dll", EntryPoint = "memset", SetLastError = false)]
        [DllImport("msvcrt.dll", EntryPoint = "memset", CallingConvention = CallingConvention.Cdecl, SetLastError = false)]
        private static extern void memset(
            IntPtr dst,
            int filler,
            int count);
        #endregion
    }
}
