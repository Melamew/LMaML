namespace LMaML.Infrastructure.Services.Interfaces
{
    /// <summary>
    /// IWindowManager
    /// </summary>
    public interface IWindowManager
    {
        /// <summary>
        /// Opens the new.
        /// </summary>
        /// <param name="content">The content.</param>
        /// <param name="title">The title.</param>
        /// <param name="desiredWidth">Width of the desired.</param>
        /// <param name="desiredHeight">Height of the desired.</param>
        /// <param name="header">The header.</param>
        /// <returns></returns>
        IWindowWrapper OpenNew(IClosableItem content, string title, int desiredWidth, int desiredHeight, object header = null);
    }
}
