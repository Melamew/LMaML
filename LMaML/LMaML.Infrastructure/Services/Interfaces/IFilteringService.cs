using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LMaML.Infrastructure.Domain.Concrete;
using LMaML.Infrastructure.Util;

namespace LMaML.Infrastructure.Services.Interfaces
{
    /// <summary>
    /// IFilteringService
    /// </summary>
    public interface IFilteringService
    {
        /// <summary>
        /// Gets the filter columns.
        /// </summary>
        /// <value>
        /// The filter columns.
        /// </value>
        IEnumerable<string> FilterColumns { get; }

        /// <summary>
        /// Gets the full column.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <returns></returns>
        Task<IQueryable<TagReference>> GetFullColumnAsync(string name);

        /// <summary>
        /// Gets the column async.
        /// </summary>
        /// <param name="target">The target.</param>
        /// <param name="basedOn">The based on.</param>
        /// <returns></returns>
        Task<IQueryable<TagReference>> GetColumnAsync(string target, params IColumnSetup[] basedOn);

        /// <summary>
        /// Gets the files async.
        /// </summary>
        /// <param name="basedOn">The based on.</param>
        /// <returns></returns>
        Task<IQueryable<StorableTaggedFile>> GetFilesAsync(params IColumnSetup[] basedOn);

        /// <summary>
        /// Gets the files.
        /// </summary>
        /// <param name="basedOn">The based on.</param>
        /// <returns></returns>
        IQueryable<StorableTaggedFile> GetFiles(params IColumnSetup[] basedOn);
    }
}