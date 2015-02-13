using System;
using LMaML.Infrastructure;
using LMaML.Infrastructure.Audio;
using LMaML.Infrastructure.Domain.Concrete;
using Microsoft.Practices.Unity;

namespace LMaML.Bass
{
    public class BassModule : AudioModuleBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ModuleBase" /> class.
        /// </summary>
        /// <param name="container">The container.</param>
        public BassModule(IUnityContainer container) : base(container)
        {
        }

        protected override IAudioPlayer GetPlayer(out Guid storageType)
        {
            storageType = StorageTypes.SystemFile;
            return Container.Resolve<BassPlayer>();
        }
    }
}
