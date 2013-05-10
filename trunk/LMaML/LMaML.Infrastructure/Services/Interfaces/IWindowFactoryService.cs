namespace LMaML.Infrastructure.Services.Interfaces
{
    /// <summary>
    /// IWindowFactoryService
    /// </summary>
    public interface IWindowFactoryService
    {
        /// <summary>
        /// Creates this instance.
        /// </summary>
        /// <returns></returns>
        IWindowWrapper CreateNew();
    }
}
