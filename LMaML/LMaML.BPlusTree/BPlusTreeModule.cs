using iLynx.Common.DataAccess;
using iLynx.Serialization;
using LMaML.Infrastructure;
using Microsoft.Practices.Prism.Modularity;
using Microsoft.Practices.Unity;
using iLynx.Common;

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
            Container.RegisterType<ISerializerService, BinarySerializerService>(new ContainerControlledLifetimeManager());
            Container.RegisterType(typeof (IDataAdapter<>), typeof (BPlusTreeAdapter<>), new ContainerControlledLifetimeManager());
        }
    }
}
