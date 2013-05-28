using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using iLynx.Common;

namespace LMaML.Infrastructure.Services.Interfaces
{
    /// <summary>
    /// ILibraryManagerService
    /// </summary>
    public interface ILibraryManagerService// : IStoreData<StorableID3File>
    {
        /// <summary>
        /// Finds the specified predicate.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="predicate">The predicate.</param>
        /// <returns></returns>
        IQueryable<T> Find<T>(Expression<Func<T, bool>> predicate);
        
        /// <summary>
        /// Gets the queryable.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        IQueryable<T> GetQueryable<T>();

        /// <summary>
        /// Gets the adapter.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        IDataAdapter<T> GetAdapter<T>();

        /// <summary>
        /// Gets all.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        IEnumerable<T> GetAll<T>();
    }
}
