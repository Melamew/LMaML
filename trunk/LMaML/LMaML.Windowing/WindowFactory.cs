using LMaML.Infrastructure;
using LMaML.Infrastructure.Services.Interfaces;
using iLynx.Common.WPF;

namespace LMaML.Windowing
{
    /// <summary>
    /// WindowFactory
    /// </summary>
    public class WindowFactory : IWindowFactoryService
    {
        /// <summary>
        /// Creates this instance.
        /// </summary>
        /// <returns></returns>
        public IWindowWrapper CreateNew()
        {
            return new BorderlessWindowWrapper(new BorderlessWindow());
        }
    }
}
