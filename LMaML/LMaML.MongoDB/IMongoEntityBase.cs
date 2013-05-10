using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace LMaML.MongoDB
{
    /// <summary>
    /// IMongoEntityBase
    /// </summary>
    public interface IMongoEntity
    {
        /// <summary>
        /// Gets or sets the id.
        /// </summary>
        /// <value>
        /// The id.
        /// </value>
        [BsonId]
        ObjectId MongoId { get; set; } 
    }
}