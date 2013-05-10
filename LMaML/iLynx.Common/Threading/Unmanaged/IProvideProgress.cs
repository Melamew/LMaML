using System;

namespace iLynx.Common.Threading.Unmanaged
{
    /// <summary>
    /// IProvideProgress
    /// </summary>
    public interface IProvideProgress
    {
        /// <summary>
        /// Occurs when [progress].
        /// </summary>
        event Action<double> Progress;

        /// <summary>
        /// Occurs when [status].
        /// </summary>
        event Action<string> Status;
    }
}