using LMaML.Infrastructure.Domain.Concrete;
using LMaML.Tests.Helpers;
using NUnit.Framework;
using Telerik.JustMock;
using iLynx.Common;
using iLynx.Common.Configuration;

namespace LMaML.Tests.LMaML.Infrastructure
{
    [TestFixture]
    public class TaggedFilePersisterFixture
    {
        [Test]
        public void NullParameterTest()
        {
            TestHelper.NullParameterTest<TaggedFilePersister>();
        }

        [Test]
        public void WhenInitializedIndicesCreated()
        {
            IDataAdapter<Album> albumAdapterMock;
            IDataAdapter<Genre> genreAdapterMock;
            IDataAdapter<Artist> artistAdapterMock;
            IDataAdapter<Title> titleAdapterMock;
            IDataAdapter<Year> yearAdapterMock;
            var adaptersMock = TestHelper.CreateReferenceAdaptersMock(
                out albumAdapterMock,
                out artistAdapterMock,
                out titleAdapterMock,
                out genreAdapterMock,
                out yearAdapterMock);
            var fileAdapterMock = Mock.Create<IDataAdapter<StorableTaggedFile>>();

            // Act
            new Builder<TaggedFilePersister>().With(adaptersMock).With(fileAdapterMock).Build();

            // Assert
            Mock.Assert(() => albumAdapterMock.CreateIndex(Arg.IsAny<string[]>()));
            Mock.Assert(() => genreAdapterMock.CreateIndex(Arg.IsAny<string[]>()));
            Mock.Assert(() => artistAdapterMock.CreateIndex(Arg.IsAny<string[]>()));
            Mock.Assert(() => titleAdapterMock.CreateIndex(Arg.IsAny<string[]>()));
            Mock.Assert(() => yearAdapterMock.CreateIndex(Arg.IsAny<string[]>()));
            Mock.Assert(() => fileAdapterMock.CreateIndex(Arg.IsAny<string[]>()));
        }

        [Test]
        public void WhenSaveReferencesSaved()
        {
            // Arrange
            IDataAdapter<Album> albumAdapterMock;
            IDataAdapter<Genre> genreAdapterMock;
            IDataAdapter<Artist> artistAdapterMock;
            IDataAdapter<Title> titleAdapterMock;
            IDataAdapter<Year> yearAdapterMock;
            var adaptersMock = TestHelper.CreateReferenceAdaptersMock(
                out albumAdapterMock,
                out artistAdapterMock,
                out titleAdapterMock,
                out genreAdapterMock,
                out yearAdapterMock);
            var fileAdapterMock = Mock.Create<IDataAdapter<StorableTaggedFile>>();
            var file = new StorableTaggedFile
            {
                Album = new Album(),
                Artist = new Artist(),
                Comment = "Blah",
                Filename = "Blah2",
                Genre = new Genre(),
                Title = new Title(),
                TrackNo = 50000,
                Year = new Year(),
            };

            var target = new Builder<TaggedFilePersister>().With(adaptersMock).With(fileAdapterMock).Build();

            // Act
            target.Save(file);

            // Assert
            // ReSharper disable ImplicitlyCapturedClosure
            Mock.Assert(() => albumAdapterMock.Save(file.Album));
            Mock.Assert(() => genreAdapterMock.Save(file.Genre));
            Mock.Assert(() => artistAdapterMock.Save(file.Artist));
            Mock.Assert(() => titleAdapterMock.Save(file.Title));
            Mock.Assert(() => yearAdapterMock.Save(file.Year));
            Mock.Assert(() => fileAdapterMock.Save(file));
            // ReSharper restore ImplicitlyCapturedClosure
        }

        [Test]
        public void WhenSaveReferencesQueried()
        {
            // Arrange
            IDataAdapter<Album> albumAdapterMock;
            IDataAdapter<Genre> genreAdapterMock;
            IDataAdapter<Artist> artistAdapterMock;
            IDataAdapter<Title> titleAdapterMock;
            IDataAdapter<Year> yearAdapterMock;
            var adaptersMock = TestHelper.CreateReferenceAdaptersMock(
                out albumAdapterMock,
                out artistAdapterMock,
                out titleAdapterMock,
                out genreAdapterMock,
                out yearAdapterMock);
            var fileAdapterMock = Mock.Create<IDataAdapter<StorableTaggedFile>>();
            var configManagerMock = Mock.Create<IConfigurationManager>();
            Mock.Arrange(() => configManagerMock.GetValue(Arg.IsAny<string>(), Arg.IsAny<int>(), Arg.IsAny<string>()))
                .Returns(Mock.Create<IConfigurableValue<int>>());
            var file = new StorableTaggedFile
            {
                Album = new Album { Name = "Album" },
                Artist = new Artist { Name = "Artist" },
                Comment = "Blah",
                Filename = "Blah2",
                Genre = new Genre { Name = "Genre" },
                Title = new Title { Name = "Title" },
                TrackNo = 50000,
                Year = new Year { Name = "Year" },
            };
            var target = new Builder<TaggedFilePersister>().With(adaptersMock).With(fileAdapterMock).With(configManagerMock).Build();

            // Act
            target.Save(file);

            // Assert
            // ReSharper disable ImplicitlyCapturedClosure
            Mock.Assert(
                () =>
                albumAdapterMock.GetFirstById(StorableTaggedFile.GenerateLowerCaseId(file.Album.Name, StorableTaggedFile.AlbumNamespace)));
            Mock.Assert(
                () =>
                genreAdapterMock.GetFirstById(StorableTaggedFile.GenerateLowerCaseId(file.Genre.Name, StorableTaggedFile.GenreNamespace)));
            Mock.Assert(
                () =>
                artistAdapterMock.GetFirstById(StorableTaggedFile.GenerateLowerCaseId(file.Artist.Name, StorableTaggedFile.ArtistNamespace)));
            Mock.Assert(
                () =>
                titleAdapterMock.GetFirstById(StorableTaggedFile.GenerateLowerCaseId(file.Title.Name, StorableTaggedFile.TitleNamespace)));
            Mock.Assert(
                () =>
                yearAdapterMock.GetFirstById(StorableTaggedFile.GenerateLowerCaseId(file.Year.Name, StorableTaggedFile.YearNamespace)));
            // ReSharper restore ImplicitlyCapturedClosure
        }

        [Test]
        public void WhenFileExistsItIsUpdated()
        {
            // Arrange
            IDataAdapter<Album> albumAdapterMock;
            IDataAdapter<Genre> genreAdapterMock;
            IDataAdapter<Artist> artistAdapterMock;
            IDataAdapter<Title> titleAdapterMock;
            IDataAdapter<Year> yearAdapterMock;
            var adaptersMock = TestHelper.CreateReferenceAdaptersMock(
                out albumAdapterMock,
                out artistAdapterMock,
                out titleAdapterMock,
                out genreAdapterMock,
                out yearAdapterMock);
            var fileAdapterMock = Mock.Create<IDataAdapter<StorableTaggedFile>>();
            const string fileName = "Filename";
            var existingId = StorableTaggedFile.GenerateLowerCaseId(fileName, StorableTaggedFile.FilenameNamespace);
            var existingFile = new StorableTaggedFile {Filename = fileName};
            Mock.Arrange(() => fileAdapterMock.GetFirstById(existingId)).Returns(existingFile);
            var file = new StorableTaggedFile
            {

                Album = new Album { Name = "Album" },
                Artist = new Artist { Name = "Artist" },
                Comment = "Blah",
                Filename = fileName,
                Genre = new Genre { Name = "Genre" },
                Title = new Title { Name = "Title" },
                TrackNo = 50000,
                Year = new Year { Name = "Year" },
                Id = existingId,
            };
            var target = new Builder<TaggedFilePersister>().With(adaptersMock).With(fileAdapterMock).Build();

            target.Save(file);

            Assert.AreEqual(file.AlbumId, existingFile.AlbumId);
            Assert.AreEqual(file.ArtistId, existingFile.ArtistId);
            Assert.AreEqual(file.Comment, existingFile.Comment);
            Assert.AreEqual(file.Filename, existingFile.Filename);
            Assert.AreEqual(file.GenreId, existingFile.GenreId);
            Assert.AreEqual(file.TitleId, existingFile.TitleId);
            Assert.AreEqual(file.TrackNo, existingFile.TrackNo);
            Assert.AreEqual(file.YearId, existingFile.YearId);
        }
    }
}
