using FluentNHibernate.Mapping;
using LMaML.Infrastructure.Domain.Concrete;

namespace LMaML.NHibernate.Maps
{
    /// <summary>
    /// ArtistMap
    /// </summary>
    public class ArtistMap : ClassMap<Artist>
    {
        public ArtistMap()
        {
            Id(x => x.Id).GeneratedBy.Assigned().Index("Artist_Idx");
            Map(x => x.Name);
        }
    }
}