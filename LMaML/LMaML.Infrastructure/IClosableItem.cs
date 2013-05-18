using System;

namespace LMaML.Infrastructure
{
    /// <summary>
    /// IClosableItem
    /// </summary>
    public interface IRequestClose
    {
        /// <summary>
        /// Occurs when [request close].
        /// </summary>
        event Action<IRequestClose> RequestClose;
    }
}
