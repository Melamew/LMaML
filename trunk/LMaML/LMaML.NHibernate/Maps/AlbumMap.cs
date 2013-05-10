using FluentNHibernate.Mapping;
using LMaML.Infrastructure.Domain.Concrete;

namespace LMaML.NHibernate.Maps
{
    /// <summary>
    /// ID3AlbumMap
    /// </summary>
    public class AlbumMap : ClassMap<Album>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AlbumMap" /> class.
        /// </summary>
        public AlbumMap()
        {
            Id(x => x.Id).GeneratedBy.Assigned().Index("Album_Idx");
            Map(x => x.Name);
            Map(x => x.TrackCount);
        }
    }
}
