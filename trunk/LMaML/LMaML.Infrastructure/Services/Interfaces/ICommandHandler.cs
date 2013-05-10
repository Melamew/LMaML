namespace LMaML.Infrastructure.Services.Interfaces
{
    /// <summary>
    /// ICommandHandler
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface ICommandHandler<T>
    {
        /// <summary>
        /// Handles the specified envelope.
        /// </summary>
        /// <param name="envelope">The envelope.</param>
        void Handle(Envelope<T> envelope);
    }
}
