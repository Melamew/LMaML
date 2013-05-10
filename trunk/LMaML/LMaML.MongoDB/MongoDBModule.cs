using System.Diagnostics;
using System.Linq;
using LMaML.Infrastructure;
using LMaML.Infrastructure.Domain.Concrete;
using Microsoft.Practices.Prism.Modularity;
using Microsoft.Practices.Unity;
using MongoDB.Bson.Serialization;
using iLynx.Common;

namespace LMaML.MongoDB
{
    [Module(ModuleName = "MongoDBModule")]
    public class MongoDBModule : ModuleBase
    {
        public MongoDBModule(IUnityContainer container)
            : base(container)
        {

        }

        protected override void RegisterTypes()
        {
            BsonClassMap.RegisterClassMap<TagReference>(map =>
                                                          {
                                                              map.SetIsRootClass(true);
                                                              map.AddKnownType(typeof(Genre));
                                                              map.AddKnownType(typeof(Year));
                                                              map.AddKnownType(typeof(Title));
                                                              map.AddKnownType(typeof(Artist));
                                                              map.AddKnownType(typeof(Album));
                                                              map.MapIdProperty(x => x.Id);
                                                              map.MapProperty(x => x.Name);
                                                          });
            BsonClassMap.RegisterClassMap<StorableTaggedFile>(map =>
                                                                  {
                                                                      map.UnmapProperty(f => f.Album);
                                                                      map.MapProperty(f => f.AlbumId);
                                                                      map.UnmapProperty(f => f.Artist);
                                                                      map.MapProperty(f => f.ArtistId);
                                                                      map.UnmapProperty(f => f.Genre);
                                                                      map.MapProperty(f => f.GenreId);
                                                                      map.UnmapProperty(f => f.Title);
                                                                      map.MapProperty(f => f.TitleId);
                                                                      map.UnmapProperty(f => f.Year);
                                                                      map.MapProperty(f => f.YearId);
                                                                      map.MapProperty(f => f.Comment);
                                                                      map.MapProperty(f => f.Filename);
                                                                      map.MapProperty(f => f.TrackNo);
                                                                      map.MapIdProperty(f => f.Id);
                                                                  });
            Container.RegisterType<IMongoWrapper, MongoWrapper>(new ContainerControlledLifetimeManager());
            Container.RegisterType(typeof(IDataAdapter<>), typeof(MongoDBAdapter<>), new PerResolveLifetimeManager());
            //var adapter = Container.Resolve<MongoDBAdapter<StorableTaggedFile>>();
            //var res =
            //    adapter.DistinctBy(f => f.AlbumId)
            //           .Select(f => f.LazyLoadReferences(Container.Resolve<IReferenceAdapters>()));
            //foreach (var item in res)
            //    Trace.WriteLine(item);
            //Trace.WriteLine("Done");
        }
    }

    ///// <summary>
    ///// InterfaceSerializer
    ///// </summary>
    //public class InterfaceSerializer : IBsonSerializer
    //{
    //    private readonly Dictionary<Type, Type> maps = new Dictionary<Type, Type>();
    //    private readonly Dictionary<Type, Func<BsonReader, object>> deserializers = new Dictionary<Type, Func<BsonReader, object>>();
    //    private readonly Dictionary<Type, Action<BsonWriter, object>> serializers = new Dictionary<Type, Action<BsonWriter, object>>();

    //    /// <summary>
    //    /// Maps this instance.
    //    /// </summary>
    //    /// <typeparam name="TInterface">The type of the interface.</typeparam>
    //    /// <typeparam name="TClass">The type of the class.</typeparam>
    //    public InterfaceSerializer Map<TInterface, TClass>() where TClass : TInterface
    //    {
    //        if (maps.ContainsKey(typeof(TInterface)))
    //            maps[typeof(TInterface)] = typeof(TClass);
    //        else
    //            maps.Add(typeof(TInterface), typeof(TClass));
    //        return this;
    //    }

    //    /// <summary>
    //    /// Deserializes an object from a BsonReader.
    //    /// </summary>
    //    /// <param name="bsonReader">The BsonReader.</param>
    //    /// <param name="nominalType">The nominal type of the object.</param>
    //    /// <param name="options">The serialization options.</param>
    //    /// <returns>
    //    /// An object.
    //    /// </returns>
    //    public object Deserialize(BsonReader bsonReader, Type nominalType, IBsonSerializationOptions options)
    //    {
    //        Func<BsonReader, object> callback;
    //        if (!deserializers.TryGetValue(nominalType, out callback))
    //            deserializers.Add(nominalType, callback = GetSerializer(nominalType));
    //        return callback(bsonReader);
    //    }

    //    /// <summary>
    //    /// Gets the serializer.
    //    /// </summary>
    //    /// <param name="nominalType">Type of the nominal.</param>
    //    /// <returns></returns>
    //    /// <exception cref="System.NotSupportedException"></exception>
    //    private Func<BsonReader, object> GetSerializer(Type nominalType)
    //    {
    //        Type type;
    //        if (!maps.TryGetValue(nominalType, out type))
    //            throw new NotSupportedException(string.Format("Unable to find a mapping for the type {0}", nominalType));
    //        var info = typeof(BsonSerializer).GetMethod("Deserialize", new[] { typeof(BsonReader), typeof(Type), typeof(IBsonSerializationOptions) });
    //        info = info.MakeGenericMethod(type);
    //        return (reader) => info.Invoke(null, new object[] { reader });
    //    }

    //    /// <summary>
    //    /// Deserializes an object from a BsonReader.
    //    /// </summary>
    //    /// <param name="bsonReader">The BsonReader.</param>
    //    /// <param name="nominalType">The nominal type of the object.</param>
    //    /// <param name="actualType">The actual type of the object.</param>
    //    /// <param name="options">The serialization options.</param>
    //    /// <returns>
    //    /// An object.
    //    /// </returns>
    //    public object Deserialize(BsonReader bsonReader, Type nominalType, Type actualType, IBsonSerializationOptions options)
    //    {
    //        return Deserialize(bsonReader, actualType, options);
    //    }

    //    /// <summary>
    //    /// Gets the default serialization options for this serializer.
    //    /// </summary>
    //    /// <returns>
    //    /// The default serialization options for this serializer.
    //    /// </returns>
    //    public IBsonSerializationOptions GetDefaultSerializationOptions()
    //    {
    //        return new DocumentSerializationOptions(true);
    //    }

    //    /// <summary>
    //    /// Serializes an object to a BsonWriter.
    //    /// </summary>
    //    /// <param name="bsonWriter">The BsonWriter.</param>
    //    /// <param name="nominalType">The nominal type.</param>
    //    /// <param name="value">The object.</param>
    //    /// <param name="options">The serialization options.</param>
    //    public void Serialize(BsonWriter bsonWriter, Type nominalType, object value, IBsonSerializationOptions options)
    //    {
    //        Action<BsonWriter, object> writer;
    //        if (!serializers.TryGetValue(nominalType, out writer))
    //            serializers.Add(nominalType, writer = MakeSerializer(nominalType));
    //        writer(bsonWriter, value);
    //    }

    //    private Action<BsonWriter, object> MakeSerializer(Type forType)
    //    {
    //        var info = typeof (BsonSerializer).GetMethod("Serialize");
    //        Type type;
    //        if (!maps.TryGetValue(forType, out type))
    //            throw new NotSupportedException(string.Format("Unable to find a mapping for the specified type {0}", forType));
    //        info = info.MakeGenericMethod(type);
    //        return (writer, arg1) => info.Invoke(null, new[] {writer, arg1 });
    //    }
    //}
}
