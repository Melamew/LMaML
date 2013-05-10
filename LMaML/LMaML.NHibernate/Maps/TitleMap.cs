using FluentNHibernate.Mapping;
using LMaML.Infrastructure.Domain.Concrete;

namespace LMaML.NHibernate.Maps
{
    /// <summary>
    /// TitleMap
    /// </summary>
    public class TitleMap : ClassMap<Title>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TitleMap" /> class.
        /// </summary>
        public TitleMap()
        {
            Id(x => x.Id).GeneratedBy.Assigned().Index("Title_Idx");
            Map(x => x.Name);
        }
    }
}