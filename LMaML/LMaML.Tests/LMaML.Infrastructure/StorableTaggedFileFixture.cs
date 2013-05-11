using System;
using System.Linq.Expressions;
using LMaML.Infrastructure;
using LMaML.Infrastructure.Domain.Concrete;
using LMaML.Tests.Helpers;
using NUnit.Framework;
using Telerik.JustMock;
using iLynx.Common;

namespace LMaML.Tests.LMaML.Infrastructure
{
    [TestFixture]
    public class StorableTaggedFileFixture
    {
        [Test]
        public void NullParameterTest()
        {
            TestHelper.NullParameterTest<StorableTaggedFile>();
        }

        [Test]
        public void Todo()
        {
            Assert.Inconclusive("Not really going to create tests that verify the copy methods...");
        }

        [Test]
        public void WhenLoadReferencesAreLoaded()
        {
            // Arrange
            var adaptersMock = Mock.Create<IReferenceAdapters>();
            var albumAdapterMock = Mock.Create<IDataAdapter<Album>>();
            var artistAdapterMock = Mock.Create<IDataAdapter<Artist>>();
            var titleAdapterMock = Mock.Create<IDataAdapter<Title>>();
            var genreAdapterMock = Mock.Create<IDataAdapter<Genre>>();
            var yearAdapterMock = Mock.Create<IDataAdapter<Year>>();
            Mock.Arrange(() => adaptersMock.AlbumAdapter).Returns(albumAdapterMock);
            Mock.Arrange(() => adaptersMock.ArtistAdapter).Returns(artistAdapterMock);
            Mock.Arrange(() => adaptersMock.TitleAdapter).Returns(titleAdapterMock);
            Mock.Arrange(() => adaptersMock.GenreAdapter).Returns(genreAdapterMock);
            Mock.Arrange(() => adaptersMock.YearAdapter).Returns(yearAdapterMock);
            var target = new StorableTaggedFile();

            // Act
            target.LoadReferences(adaptersMock);

            // Assert(s)...
            Mock.Assert(() => albumAdapterMock.GetFirst(Arg.IsAny<Expression<Func<Album, bool>>>()));
            Mock.Assert(() => artistAdapterMock.GetFirst(Arg.IsAny<Expression<Func<Artist, bool>>>()));
            Mock.Assert(() => titleAdapterMock.GetFirst(Arg.IsAny<Expression<Func<Title, bool>>>()));
            Mock.Assert(() => genreAdapterMock.GetFirst(Arg.IsAny<Expression<Func<Genre, bool>>>()));
            Mock.Assert(() => yearAdapterMock.GetFirst(Arg.IsAny<Expression<Func<Year, bool>>>()));
        }

        [Test]
        public void WhenLazyLoadReferencesLazyFileReturned()
        {
            // Arrange
            var adaptersMock = Mock.Create<IReferenceAdapters>();
            var target = new StorableTaggedFile();

            // Act
            var result = target.LazyLoadReferences(adaptersMock);

            // Assert
            Assert.IsInstanceOf<LazyLoadedTaggedFile>(result);
        }

        [Test]
        public void ToStringReturnsArtistTitle()
        {
            // Arrange
            const string expectedArtist = "Artist";
            const string expectedTitle = "Title";
            const string expectedString = expectedArtist + " - " + expectedTitle;
            var target = new StorableTaggedFile
                             {
                                 Artist = new Artist {Name = expectedArtist},
                                 Title = new Title {Name = expectedTitle}
                             };

            // Act
            var result = target.ToString();
            
            // Assert
            Assert.AreEqual(expectedString, result);
        }

        [Test]
        public void WhenArtistTitleNullLoadMeReturned()
        {
            // Arrange
            var target = new StorableTaggedFile();
            const string expected = "LOADME!";

            // Act
            var result = target.ToString();

            // Assert
            Assert.AreEqual(expected, result);
        }
    }
}
