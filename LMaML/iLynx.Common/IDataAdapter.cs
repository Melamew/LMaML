using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace iLynx.Common
{
    /// <summary>
    /// An interface that can be used to implement a simple data store adapter
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface IDataAdapter<T> : IDataPersister<T>
    {
        /// <summary>
        /// Gets the first instance of <typeparamref name="T"/> available in the datastore
        /// </summary>
        /// <returns></returns>
        T GetFirst();

        /// <summary>
        /// Attempts to get all instances of <typeparamref name="T"/>
        /// </summary>
        /// <returns></returns>
        IEnumerable<T> GetAll();

        /// <summary>
        /// Queries this instance.
        /// </summary>
        /// <returns></returns>
        IQueryable<T> Query();

        /// <summary>
        /// Distincts the by.
        /// </summary>
        /// <typeparam name="TK">The type of the K.</typeparam>
        /// <param name="keyExpression">The key expression.</param>
        /// <returns></returns>
        IEnumerable<T> DistinctBy<TK>(Expression<Func<T, TK>> keyExpression);

        /// <summary>
        /// Deletes the specified value.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value">The value.</param>
        void Delete(T value);

        /// <summary>
        /// Gets the first.
        /// </summary>
        /// <param name="predicate">The predicate.</param>
        /// <returns></returns>
        T GetFirst(Expression<Func<T, bool>> predicate);

        /// <summary>
        /// Gets the first by id.
        /// </summary>
        /// <param name="id">The id.</param>
        /// <returns></returns>
        T GetFirstById(object id);
    }
}
