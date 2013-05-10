using FluentNHibernate.Mapping;
using LMaML.Infrastructure.Domain.Concrete;

namespace LMaML.NHibernate.Maps
{
    /// <summary>
    /// GenreMap
    /// </summary>
    public class GenreMap : ClassMap<Genre>
    {
        public GenreMap()
        {
            Id(x => x.Id).GeneratedBy.Assigned().Index("Genre_Idx");
            Map(x => x.Name);
        }
    }
}