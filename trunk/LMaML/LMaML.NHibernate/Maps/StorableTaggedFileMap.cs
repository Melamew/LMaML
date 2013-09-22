using FluentNHibernate.Mapping;
using LMaML.Infrastructure.Domain.Concrete;

namespace LMaML.NHibernate.Maps
{
    /// <summary>
    /// StorableID3FileMap
    /// </summary>
    public class StorableTaggedFileMap : ClassMap<StorableTaggedFile>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="StorableTaggedFileMap" /> class.
        /// </summary>
        public StorableTaggedFileMap()
        {
            Id(x => x.Id).GeneratedBy.Assigned().Index("File_Idx");
            Map(x => x.Comment);
            Map(x => x.Filename);
            Map(x => x.TrackNo);
            Map(x => x.Duration);
            Map(x => x.AlbumId).Index("Idx_Album_id");
            Map(x => x.ArtistId).Index("Idx_Artist_id");
            Map(x => x.GenreId).Index("Idx_Genre_id");
            Map(x => x.TitleId).Index("Idx_Title_id");
            Map(x => x.YearId).Index("Idx_Year_id");
            //References(x => x.Title).Class<Title>().Cascade.SaveUpdate();
            //References(x => x.Album).Class<Album>().Cascade.SaveUpdate();
            //References(x => x.Artist).Class<Artist>().Cascade.SaveUpdate();
            //References(x => x.Genre).Class<Genre>().Cascade.SaveUpdate();
            //References(x => x.Year).Class<Year>().Cascade.SaveUpdate();
        }
    }
}
