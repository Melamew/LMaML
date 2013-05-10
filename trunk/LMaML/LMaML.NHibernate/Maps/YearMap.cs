using FluentNHibernate.Mapping;
using LMaML.Infrastructure.Domain.Concrete;

namespace LMaML.NHibernate.Maps
{
    /// <summary>
    /// YearMap
    /// </summary>
    public class YearMap : ClassMap<Year>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="YearMap" /> class.
        /// </summary>
        public YearMap()
        {
            Id(x => x.Id).GeneratedBy.Assigned().Index("Year_Idx");
            Map(x => x.Name);
            Map(x => x.Value);
        }
    }
}