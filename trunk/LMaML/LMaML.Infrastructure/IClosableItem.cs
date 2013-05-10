using System;

namespace LMaML.Infrastructure
{
    /// <summary>
    /// IClosableItem
    /// </summary>
    public interface IClosableItem
    {
        /// <summary>
        /// Occurs when [request close].
        /// </summary>
        event Action<IClosableItem> RequestClose;
    }
}
