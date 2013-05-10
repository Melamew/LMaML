using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using LMaML.Infrastructure.Domain;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Driver;
using MongoDB.Driver.Linq;
using iLynx.Common;
using iLynx.Common.Serialization.Old;
using Query2 = MongoDB.Driver.Builders.Query;

namespace LMaML.MongoDB
{
    /// <summary>
    ///     A data adapter for a mongo collection
    ///     <para />
    ///     Please note that objects stored in the collection have to be "compliant" with the mongodb driver,
    ///     <para />
    ///     this means they should have ONE property named "_objectid" of type <see cref="ObjectId" />
    ///     <para />
    ///     (Or have the attribute <see cref="BsonIdAttribute" /> on a property of the same type)
    ///     <para />
    ///     Please note that you may need to use <see cref="DataSerializer{T}" /> in order to search for a specific object
    /// </summary>
    /// <typeparam name="T">The type of object to store</typeparam>
    public class MongoDBAdapter<T> : IDataAdapter<T> where T : ILibraryEntity
    {
        private readonly MongoCollection<T> collection;
        private readonly Type type = typeof(T);
// ReSharper disable StaticFieldInGenericType
        private static readonly IEnumerable<BsonElement> ClassMap; // Intentional
// ReSharper restore StaticFieldInGenericType

        /// <summary>
        /// Initializes a new instance of <see cref="MongoDBAdapter{T}" />
        /// </summary>
        /// <param name="wrapper">The wrapper.</param>
        public MongoDBAdapter(IMongoWrapper wrapper)
        {
            collection = wrapper.Database.GetCollection<T>(CollectionName);
        }

        static MongoDBAdapter()
        {
            ClassMap = BuildMap();
        }

        private static IEnumerable<BsonElement> BuildMap()
        {
            var memberMaps = BsonClassMap.LookupClassMap(typeof(T)).AllMemberMaps;
            var ret = new BsonElement[memberMaps.Count];
            for (var i = 0; i < ret.Length; ++i)
                ret[i] = new BsonElement(memberMaps[i].MemberName, "$" + memberMaps[i].ElementName);
            return ret;
        }

        /// <summary>
        ///     Gets the name of the underlying collection
        /// </summary>
        public string CollectionName
        {
            get { return type.FullName; }
        }

        /// <summary>
        ///     Stores the specified value in the database
        ///     <para />
        ///     Same as Update
        /// </summary>
        /// <param name="value"></param>
        public virtual void Save(T value)
        {
            collection.Save(value);
        }

        /// <summary>
        /// Bulk insert...
        /// </summary>
        /// <param name="data">The data.</param>
        public virtual void BulkInsert(IEnumerable<T> data)
        {
            collection.InsertBatch(data);
        }

        public virtual void SaveOrUpdate(T value)
        {
            collection.Save(value);
        }

        /// <summary>
        /// Transacts the specified a.
        /// </summary>
        /// <param name="a">A.</param>
        /// <param name="flushAfter">if set to <c>true</c> [flush after].</param>
        public virtual void Transact(Action a, bool flushAfter)
        {
            a();
        }

        /// <summary>
        /// Deletes the specified value.
        /// </summary>
        /// <param name="value">The value.</param>
        public virtual void Delete(T value)
        {
            collection.Remove(Query2.EQ("Id", value.Id));
        }

        /// <summary>
        /// Gets the first.
        /// </summary>
        /// <param name="predicate">The predicate.</param>
        /// <returns></returns>
        public T GetFirst(Expression<Func<T, bool>> predicate)
        {
            return collection.AsQueryable().FirstOrDefault(predicate);
        }

        /// <summary>
        /// Gets the first by id.
        /// </summary>
        /// <param name="id">The id.</param>
        /// <returns></returns>
        public T GetFirstById(object id)
        {
            if (!(id is Guid))
                return default(T);
            return collection.AsQueryable().FirstOrDefault(x => x.Id == (Guid) id);
            //return collection.FindOne(Query2.EQ("_id", (Guid)id));
        }

        /// <summary>
        /// Distincts the by.
        /// </summary>
        /// <typeparam name="TKey">The type of the key.</typeparam>
        /// <param name="keySelector">The key selector.</param>
        /// <returns></returns>
        /// <exception cref="System.NullReferenceException"></exception>
        public IEnumerable<T> DistinctBy<TKey>(Expression<Func<T, TKey>> keySelector)
        {
            throw new NotSupportedException("... I tried, you know, I really tried ...");
            //var ex = keySelector.Body as MemberExpression;
            //if (null == ex)
            //    throw new NullReferenceException();
            //var selectorDocument = new BsonDocument
            //                           {
            //                               {ex.Member.Name + "__","$" + ex.Member.Name},
            //                           };
            //selectorDocument.AddRange(ClassMap);
            //var group = new BsonDocument
            //                {
            //                    {
            //                        "$group",
            //                        new BsonDocument
            //                            {
            //                                {
            //                                    "_id", selectorDocument
            //                                },
            //                                {
            //                                    "Limit", new BsonDocument
            //                                                 {
            //                                                     { "$first", 1 }
            //                                                 }
            //                                },
            //                            }
            //                    },
            //                };
            //var pipeline = new[] {group};//, project};//, limit};
            //return
            //    collection.Aggregate(pipeline)
            //              .ResultDocuments
            //              .Select(x => BsonSerializer.Deserialize<T>(x["_id"].AsBsonDocument));
        }

        /// <summary>
        /// Creates the index.
        /// </summary>
        /// <param name="fields">The fields.</param>
        public virtual void CreateIndex(params string[] fields)
        {
            foreach (var field in fields.SkipWhile(f => collection.IndexExists(f)))
                collection.CreateIndex(field);
        }

        /// <summary>
        ///     Simply returns <see cref="MongoCollection{T}.FindOne()" />
        /// </summary>
        /// <returns></returns>
        public virtual T GetFirst()
        {
            return collection.FindOne();
        }

        /// <summary>
        ///     Executes <see cref="MongoCollection{TDefaultDocument}.FindAll()" />.ToArray()
        /// </summary>
        /// <returns></returns>
        public virtual IEnumerable<T> GetAll()
        {
            return collection.FindAll();
        }

        /// <summary>
        /// Queries this instance.
        /// </summary>
        /// <returns></returns>
        public virtual IQueryable<T> Query()
        {
            return collection.AsQueryable();
        }
    }

}