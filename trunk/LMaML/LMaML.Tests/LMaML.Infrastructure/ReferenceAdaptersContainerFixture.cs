using LMaML.Infrastructure;
using LMaML.Infrastructure.Domain.Concrete;
using LMaML.Tests.Helpers;
using NUnit.Framework;
using Telerik.JustMock;
using iLynx.Common;

namespace LMaML.Tests.LMaML.Infrastructure
{
    [TestFixture]
    public class ReferenceAdaptersContainerFixture
    {
        [Test]
        public void NullParameterTest()
        {
            TestHelper.NullParameterTest<ReferenceAdaptersContainer>();
        }

        [Test]
        public void WhenInitializedValuesSet()
        {
            // Arrange
            var artistAdapter = Mock.Create<IDataAdapter<Artist>>();
            var albumAdapter = Mock.Create<IDataAdapter<Album>>();
            var genreAdapter = Mock.Create<IDataAdapter<Genre>>();
            var titleAdapter = Mock.Create<IDataAdapter<Title>>();
            var yearAdapter = Mock.Create<IDataAdapter<Year>>();
            
            // Act
            var target = new Builder<ReferenceAdaptersContainer>()
                .With(artistAdapter)
                .With(albumAdapter)
                .With(genreAdapter)
                .With(titleAdapter)
                .With(yearAdapter)
                .Build();

            // Assert
            Assert.AreSame(artistAdapter, target.ArtistAdapter);
            Assert.AreSame(albumAdapter, target.AlbumAdapter);
            Assert.AreSame(genreAdapter, target.GenreAdapter);
            Assert.AreSame(titleAdapter, target.TitleAdapter);
            Assert.AreSame(yearAdapter, target.YearAdapter);
        }
    }
}
