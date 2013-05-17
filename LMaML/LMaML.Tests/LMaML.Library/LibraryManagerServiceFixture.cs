using System.Linq;
using LMaML.Infrastructure.Domain.Concrete;
using LMaML.Library;
using LMaML.Tests.Helpers;
using Microsoft.Practices.Unity;
using NUnit.Framework;
using Telerik.JustMock;
using iLynx.Common;

namespace LMaML.Tests.LMaML.Library
{
    [TestFixture]
    public class LibraryManagerServiceFixture
    {
        [Test]
        public void NullParameterTest()
        {
            TestHelper.NullParameterTest<LibraryManagerService>();
        }

        [Test]
        public void WhenStorePersisterUsed()
        {
            // Arrange
            var persister = Mock.Create<IDataPersister<StorableTaggedFile>>();
            var target = new Builder<LibraryManagerService>().With(persister).Build();
            var expected = new StorableTaggedFile();

            // Act
            target.Store(expected);

            // Assert
            Mock.Assert(() => persister.Save(expected));
        }

        [Test]
        public void WhenFindAdapterResolvedAndQueried()
        {
            // Arrange
            var adapter = Mock.Create<IDataAdapter<int>>();
            Mock.Arrange(() => adapter.Query()).Returns(new int[] { }.AsQueryable());
            var target = new Builder<LibraryManagerService>().With(adapter).Build();

            // Act
            target.Find<int>(x => x == 5);

            // Assert
            Mock.Assert(() => adapter.Query());
        }

        [Test]
        public void WhenGetAdapterIsResolved()
        {
            // Arrange
            var containerMock = Mock.Create<IUnityContainer>();
            var target = new Builder<LibraryManagerService>().With(containerMock).Build();

            // Act
            target.GetAdapter<int>();

            // Assert
            Mock.Assert(() => containerMock.Resolve(typeof(IDataAdapter<int>), Arg.IsAny<string>(), Arg.IsAny<ResolverOverride[]>()));
        }

        [Test]
        public void WhenGetAllAdapterUsed()
        {
            // Arrange
            var adapter = Mock.Create<IDataAdapter<int>>();
            var target = new Builder<LibraryManagerService>().With(adapter).Build();

            // Act
            target.GetAll<int>();

            // Assert
            Mock.Assert(() => adapter.GetAll());
        }
    }
}
