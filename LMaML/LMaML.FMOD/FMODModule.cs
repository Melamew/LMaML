using System;
using LMaML.Infrastructure;
using LMaML.Infrastructure.Audio;
using LMaML.Infrastructure.Domain.Concrete;
using Microsoft.Practices.Unity;

namespace LMaML.FMOD
{
    /// <summary>
    /// FMODModule
    /// </summary>
    public class FMODModule : AudioModuleBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="FMODModule" /> class.
        /// </summary>
        /// <param name="container">The container.</param>
        public FMODModule(IUnityContainer container) : base(container)
        {

        }

        protected override IAudioPlayer GetPlayer(out Guid storageType)
        {
            storageType = StorageTypes.SystemFile;
            return Container.Resolve<FMODPlayer>();
        }
    }
}
