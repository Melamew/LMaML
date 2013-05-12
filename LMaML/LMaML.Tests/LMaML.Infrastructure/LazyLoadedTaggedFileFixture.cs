using System;
using LMaML.Infrastructure.Domain.Concrete;
using LMaML.Tests.Helpers;
using NUnit.Framework;
using Telerik.JustMock;
using iLynx.Common;

namespace LMaML.Tests.LMaML.Infrastructure
{
    [TestFixture]
    public class LazyLoadedTaggedFileFixture
    {
        [Test]
        public void NullParameterTest()
        {
            TestHelper.NullParameterTest<LazyLoadedTaggedFile>();
        }

        [Test]
        public void WhenInitializedIdsCopied()
        {
            var original = new StorableTaggedFile
            {
                AlbumId = Guid.NewGuid(),
                GenreId = Guid.NewGuid(),
                ArtistId = Guid.NewGuid(),
                TitleId = Guid.NewGuid(),
                YearId = Guid.NewGuid(),
                Id = Guid.NewGuid(),
            };
            var target = new Builder<LazyLoadedTaggedFile>().With(original).Build();

            Assert.AreEqual(original.AlbumId, target.AlbumId);
            Assert.AreEqual(original.GenreId, target.GenreId);
            Assert.AreEqual(original.ArtistId, target.ArtistId);
            Assert.AreEqual(original.TitleId, target.TitleId);
            Assert.AreEqual(original.YearId, target.YearId);
            Assert.AreEqual(original.Id, target.Id);
        }

        [Test]
        public void WhenOtherFieldsRequestedReferenceAdaptersUsed()
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
            var original = new StorableTaggedFile
            {
                AlbumId = Guid.NewGuid(),
                GenreId = Guid.NewGuid(),
                ArtistId = Guid.NewGuid(),
                TitleId = Guid.NewGuid(),
                YearId = Guid.NewGuid(),
                Id = Guid.NewGuid(),
            };
            var target = new Builder<LazyLoadedTaggedFile>().With(original).With(adaptersMock).Build();

#pragma warning disable 219
            // ReSharper disable RedundantAssignment
            // ReSharper disable ImplicitlyCapturedClosure
            object stuff = target.Album;
            Mock.Assert(() => albumAdapterMock.GetFirstById(original.AlbumId));
            stuff = target.Genre;
            Mock.Assert(() => genreAdapterMock.GetFirstById(original.GenreId));
            stuff = target.Artist;
            Mock.Assert(() => artistAdapterMock.GetFirstById(original.ArtistId));
            stuff = target.Title;
            Mock.Assert(() => titleAdapterMock.GetFirstById(original.TitleId));
            stuff = target.Year;
            Mock.Assert(() => yearAdapterMock.GetFirstById(original.YearId));
            // ReSharper restore ImplicitlyCapturedClosure
            // ReSharper restore RedundantAssignment
#pragma warning restore 219
        }
    }
}
