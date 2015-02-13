using System;
using LMaML.Infrastructure.Audio;
using Microsoft.Practices.Unity;

namespace LMaML.Infrastructure
{
    public abstract class AudioModuleBase : ModuleBase
    {
        protected AudioModuleBase(IUnityContainer container) : base(container)
        {
        }

        protected override void RegisterTypes()
        {
            base.RegisterTypes();
            var aggregate = Container.Resolve<IAggregateAudioPlayer>();
            Guid type;
            var player = GetPlayer(out type);
            aggregate.RegisterPlayer(type, player);
        }

        protected abstract IAudioPlayer GetPlayer(out Guid storageType);
    }
}
