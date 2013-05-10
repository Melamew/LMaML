using System;
using LMaML.Infrastructure.Domain;
using MongoDB.Driver;
using iLynx.Common;

namespace LMaML.MongoDB
{
    /// <summary>
    /// IMongoWrapper
    /// </summary>
    public interface IMongoWrapper
    {
        /// <summary>
        /// Gets the database.
        /// </summary>
        /// <value>
        /// The database.
        /// </value>
        MongoDatabase Database { get; }

        /// <summary>
        ///     Gets a value indicating whether or not MongoDB is considered as being available
        /// </summary>
        bool MongoAvailable { get; }

        /// <summary>
        /// Attempts to Start MongoDB on the local machine
        /// <para />
        /// This method will throw an <see cref="InvalidOperationException" /> if mongo is already available
        /// </summary>
        void StartMongo();

        /// <summary>
        ///     Attempts to get an <see cref="IDataAdapter{T}" /> for the specified type <typeparamref name="T" />
        /// </summary>
        /// <typeparam name="T">The type for which to get an adapter</typeparam>
        /// <returns>
        ///     Returns a <see cref="MongoDBAdapter{T}" />
        /// </returns>
        MongoDBAdapter<T> GetAdapter<T>() where T : ILibraryEntity;
    }
}