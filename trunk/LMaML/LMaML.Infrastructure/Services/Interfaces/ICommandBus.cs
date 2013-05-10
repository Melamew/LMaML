namespace LMaML.Infrastructure.Services.Interfaces
{
    /// <summary>
    /// ICommandBus
    /// </summary>
    public interface ICommandBus
    {
        /// <summary>
        /// Sends the specified item.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="item">The item.</param>
        void Send<T>(Envelope<T> item);
    }
}
