using LMaML.Infrastructure;
using Microsoft.Practices.Prism.Modularity;
using Microsoft.Practices.Unity;
using iLynx.Common;
using iLynx.Common.Serialization;

namespace LMaML.BPlusTree
{
    [Module(ModuleName = "BPlusTreeModule")]
    public class BPlusTreeModule : ModuleBase
    {
        public BPlusTreeModule(IUnityContainer container) : base(container)
        {
        }

        protected override void RegisterTypes()
        {
            Container.RegisterType<ISerializerService, Serializer>(new ContainerControlledLifetimeManager());
            Container.RegisterType(typeof (IDataAdapter<>), typeof (BPlusTreeAdapter<>), new ContainerControlledLifetimeManager());
        }
    }
}
