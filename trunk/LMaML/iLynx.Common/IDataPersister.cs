using System;
using System.Collections.Generic;

namespace iLynx.Common
{
    /// <summary>
    /// IStoreData
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface IDataPersister<in T>
    {
        /// <summary>
        /// Stores the generic type <typeparamref name="T"/> in the datastore
        /// </summary>
        /// <param name="value">The value to store</param>
        void Save(T value);

        /// <summary>
        /// Saves or Updates the specified value.
        /// </summary>
        /// <param name="value">The value.</param>
        void SaveOrUpdate(T value);

        /// <summary>
        /// Transacts the specified a.
        /// </summary>
        /// <param name="a">A.</param>
        /// <param name="flushAfter">if set to <c>true</c> [flush after].</param>
        void Transact(Action a, bool flushAfter);

        /// <summary>
        /// Creates the index.
        /// </summary>
        /// <param name="fields">The fields.</param>
        void CreateIndex(params string[] fields);

        /// <summary>
        /// Bulks the insert.
        /// </summary>
        /// <param name="data">The data.</param>
        void BulkInsert(IEnumerable<T> data);
    }
}