using System;
using LMaML.Infrastructure;
using LMaML.Infrastructure.Audio;
using LMaML.Infrastructure.Domain.Concrete;
using Microsoft.Practices.Unity;

namespace LMaML.NAudio
{
    public class NAudioModule : AudioModuleBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ModuleBase" /> class.
        /// </summary>
        /// <param name="container">The container.</param>
        public NAudioModule(IUnityContainer container) : base(container)
        {
        }

        protected override IAudioPlayer GetPlayer(out Guid storageType)
        {
            storageType = StorageTypes.SystemFile;
            return Container.Resolve<NAudioPlayer>();
        }
    }
}
