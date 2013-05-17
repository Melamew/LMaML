using System;
using System.Linq;
using System.Linq.Expressions;
using LMaML.Infrastructure;
using LMaML.Infrastructure.Domain.Concrete;
using LMaML.Library;
using LMaML.Library.ViewModels;
using LMaML.Tests.Helpers;
using NUnit.Framework;
using Telerik.JustMock;
using iLynx.Common;

namespace LMaML.Tests.LMaML.Library
{
    [TestFixture]
    public class FilteringServiceFixture
    {
        [Test]
        public void NullParameterTest()
        {
            TestHelper.NullParameterTest<FilteringService>();
        }

        [Test]
        public void WhenGetFullColumnDoesNotExistEmptyQueryableReturned()
        {
            // Arrange
            var target = new Builder<FilteringService>().Build();

            // Act
            var result = target.GetFullColumn("Non-Existent");

            // Assert
            Assert.AreEqual(0, result.Count());
        }

        [Test]
        public void WhenGetFullColumnReferenceAdapterUsed()
        {
            // Arrange
            var adapterMock = Mock.Create<IDataAdapter<Artist>>();
            var referenceAdaptersMock = Mock.Create<IReferenceAdapters>();
            var libraryMock = Mock.Create<ILibraryManagerService>();
            Mock.Arrange(() => referenceAdaptersMock.ArtistAdapter).Returns(adapterMock);
            Mock.Arrange(() => libraryMock.Find(Arg.IsAny<Expression<Func<StorableTaggedFile, bool>>>()))
                .Returns(new StorableTaggedFile[] { }.AsQueryable());
            Mock.Arrange(() => adapterMock.Query()).Returns(new Artist[] { }.AsQueryable());
            var target = new Builder<FilteringService>().With(referenceAdaptersMock).Build();

            // Act
            target.GetFullColumn("Artist");

            // Assert
            Mock.Assert(() => adapterMock.Query());
        }

        [Test]
        public void WhenInitializedFilterColumnsNotEmpty()
        {
            // Arrange
            var target = new Builder<FilteringService>().Build();

            // Act / Assert
            Assert.IsTrue(target.FilterColumns.Any());
        }

        [Test]
        public void WhenGetColumnAdapterUsed()
        {
            // Arrange
            var adapterMock = Mock.Create<IDataAdapter<Artist>>();
            var referenceAdaptersMock = Mock.Create<IReferenceAdapters>();
            var libraryMock = Mock.Create<ILibraryManagerService>();
            Mock.Arrange(() => referenceAdaptersMock.ArtistAdapter).Returns(adapterMock);
            Mock.Arrange(() => libraryMock.Find(Arg.IsAny<Expression<Func<StorableTaggedFile, bool>>>()))
                .Returns(new StorableTaggedFile[] { }.AsQueryable());
            Mock.Arrange(() => adapterMock.Query()).Returns(new Artist[] {}.AsQueryable());
            var target = new Builder<FilteringService>().With(libraryMock).With(referenceAdaptersMock).Build();

            target.GetColumn("Artist", new ColumnSetup("Year", Guid.NewGuid()));

            Mock.Assert(() => adapterMock.Query());
        }

        [Test]
        public void WhenGetFilesLibraryManagerUsed()
        {
            // Arrange
            var adapterMock = Mock.Create<IDataAdapter<Artist>>();
            var referenceAdaptersMock = Mock.Create<IReferenceAdapters>();
            var libraryMock = Mock.Create<ILibraryManagerService>();
            Mock.Arrange(() => referenceAdaptersMock.ArtistAdapter).Returns(adapterMock);
            Mock.Arrange(() => libraryMock.Find(Arg.IsAny<Expression<Func<StorableTaggedFile, bool>>>()))
                .Returns(new StorableTaggedFile[] { }.AsQueryable());
            Mock.Arrange(() => adapterMock.Query()).Returns(new Artist[] { }.AsQueryable());
            var target = new Builder<FilteringService>().With(libraryMock).Build();

            // Act
            target.GetFiles(new ColumnSetup("Artist", Guid.NewGuid()));

            // Assert
            Mock.Assert(() => libraryMock.Find(Arg.IsAny<Expression<Func<StorableTaggedFile,bool>>>()));
        }
    }
}
