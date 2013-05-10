using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using CSharpTest.Net.Collections;
using CSharpTest.Net.Serialization;
using LMaML.Infrastructure.Domain;
using iLynx.Common;
using iLynx.Common.Configuration;
using iLynx.Common.Serialization;

namespace LMaML.BPlusTree
{
    /// <summary>
    /// CSharpTestSerializerProxy
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class CSharpTestSerializerProxy<T> : CSharpTest.Net.Serialization.ISerializer<T> where T : new()
    {
        private readonly ISerializerService serializerService;

        /// <summary>
        /// Initializes a new instance of the <see cref="CSharpTestSerializerProxy{T}" /> class.
        /// </summary>
        /// <param name="serializerService">The serializer service.</param>
        public CSharpTestSerializerProxy(ISerializerService serializerService)
        {
            this.serializerService = serializerService;
        }

        /// <summary>
        /// Writes to.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <param name="stream">The stream.</param>
        public void WriteTo(T value, Stream stream)
        {
            serializerService.Serialize(value, stream);
        }

        /// <summary>
        /// Reads from.
        /// </summary>
        /// <param name="stream">The stream.</param>
        /// <returns></returns>
        public T ReadFrom(Stream stream)
        {
            return serializerService.Deserialize<T>(stream);
        }
    }

    /// <summary>
    /// A data adapter for a BPlusTree
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class BPlusTreeAdapter<T> : ComponentBase, IDataAdapter<T>, IDisposable where T : ILibraryEntity, new()
    {
        private readonly ISerializerService serializerService;
        private readonly BPlusTree<Guid, T> tree;
        private readonly Dictionary<string, BPlusTree<Guid, List<Guid>>> indices = new Dictionary<string, BPlusTree<Guid, List<Guid>>>();

        /// <summary>
        /// Initializes a new instance of the <see cref="BPlusTreeAdapter{T}" /> class.
        /// </summary>
        /// <param name="configurationManager">The configuration manager.</param>
        /// <param name="serializerService">The serializer service.</param>
        /// <param name="logger">The logger.</param>
        public BPlusTreeAdapter(IConfigurationManager configurationManager, ISerializerService serializerService, ILogger logger)
            : base(logger)
        {
            configurationManager.Guard("configurationManager");
            serializerService.Guard("serializerService");
            this.serializerService = serializerService;

            var fileBase = configurationManager.GetValue("BPlusTree.BaseFile", "Storage.bin");
            var pathBase = configurationManager.GetValue("BPlusTree.BasePath", "Trees");
            tree = new BPlusTree<Guid, T>(GetOptions(fileBase.Value, pathBase.Value));
            LogWarning("The BPlusTreeAdapter is currently very much experimental, which means not much is supported");
            LogWarning("Indices are NOT supported, Queries are NOT supported");
            LogWarning("The only thing that is really supported is GetFirstById(object) where the parameter is a Guid");
        }

        private BPlusTree<Guid, T>.OptionsV2 GetOptions(string fileBase, string pathBase)
        {
            if (!Path.IsPathRooted(pathBase))
                pathBase = Path.Combine(Environment.CurrentDirectory, pathBase);
            if (Path.IsPathRooted(fileBase))
                fileBase = Path.GetFileName(fileBase);
            if (string.IsNullOrEmpty(fileBase))
                fileBase = "Storage.bin";
            if (!Directory.Exists(pathBase))
                Directory.CreateDirectory(pathBase);
            return new BPlusTree<Guid, T>.OptionsV2(PrimitiveSerializer.Guid,
                                                    new CSharpTestSerializerProxy<T>(serializerService))
                       {
                           CreateFile = CreatePolicy.IfNeeded,
                           FileName = string.Format("{0}\\{1}.{2}", pathBase, typeof(T).Name, fileBase),
                           ExistingLogAction = ExistingLogAction.ReplayAndCommit,
                           StoragePerformance = StoragePerformance.LogFileInCache,
                           TransactionLogLimit = 100 * 1024 * 1024,
                       }.CalcBTreeOrder(16, 512); // TODO: Adjust the value size if necessary
        }

        public void Dispose()
        {
            tree.Commit();
            tree.Dispose();
        }

        /// <summary>
        /// Saves the specified value.
        /// </summary>
        /// <param name="value">The value.</param>
        public void Save(T value)
        {
            tree.AddOrUpdate(value.Id, value, (key, original) => value);
        }

        /// <summary>
        /// Saves the or update.
        /// </summary>
        /// <param name="value">The value.</param>
        public void SaveOrUpdate(T value)
        {
            tree.AddOrUpdate(value.Id, value, (key, original) => value);
        }

        /// <summary>
        /// Transacts the specified a.
        /// </summary>
        /// <param name="a">A.</param>
        /// <param name="flushAfter">if set to <c>true</c> [flush after].</param>
        public void Transact(Action a, bool flushAfter)
        {
            a();
            if (flushAfter)
                tree.Commit();
        }

        /// <summary>
        /// Creates the index.
        /// </summary>
        /// <param name="fields">The fields.</param>
        public void CreateIndex(params string[] fields)
        {

            foreach (var field in fields)
            {
                try
                {
                    var memberInfo = typeof(T).GetMember(field);
                    if (memberInfo.Length <= 0)
                    {
                        LogWarning("Tried to create index on non-existing field or property ({0})", field);
                        continue;
                    }
                    if (memberInfo.Length > 1)
                    {
                        LogWarning("Unable to determine which field or property to create an index for, multiple members match the name ({0})", field);
                        continue;
                    }
                    var mi = memberInfo[0];
                    if (mi.MemberType != MemberTypes.Field && mi.MemberType != MemberTypes.Property)
                    {
                        LogWarning("Unable to create an index for the field ({0}), Not a property or a field", mi.Name);
                        continue;
                    }
                    BPlusTree<Guid, List<Guid>> index;
                    if (!TryGetIndex(mi.Name, out index))
                        LogWarning("Unable to create index for for the field ({0})", mi.Name);
                }
                catch (Exception e) { LogException(e, MethodBase.GetCurrentMethod()); }
                //if (
                //    !typeof (T).GetMembers(BindingFlags.Public | BindingFlags.GetField | BindingFlags.SetField)
                //               .Select(mi => mi.Name)
                //               .Contains(field)
                //    &&
                //    !typeof(T).GetMember())
            }
        }

        private bool TryGetIndex(string name, out BPlusTree<Guid, List<Guid>> index)
        {
            return (!indices.TryGetValue(name, out index) && !TryCreateIndex(name, out index));
        }

        private bool TryCreateIndex(string name, out BPlusTree<Guid, List<Guid>> index)
        {
            LogWarning("Indices are not yet implemented");
            index = null;
            return false;
        }

        /// <summary>
        /// </summary>
        /// <param name="data">The data.</param>
        public void BulkInsert(IEnumerable<T> data)
        {
            tree.BulkInsert(data.Select(arg => new KeyValuePair<Guid, T>(arg.Id, arg)));
        }

        /// <summary>
        /// Gets the first.
        /// </summary>
        /// <returns></returns>
        public T GetFirst()
        {
            KeyValuePair<Guid, T> first;
            return !tree.TryGetFirst(out first) ? default(T) : first.Value;
        }

        /// <summary>
        /// Gets all.
        /// </summary>
        /// <returns></returns>
        public IEnumerable<T> GetAll()
        {
            return tree.Values;
        }

        /// <summary>
        /// Queries this instance.
        /// </summary>
        /// <returns></returns>
        public IQueryable<T> Query()
        {
            return tree.Values.AsQueryable();
        }

        /// <summary>
        /// Distincts the by.
        /// </summary>
        /// <typeparam name="TK">The type of the K.</typeparam>
        /// <param name="keyExpression">The key expression.</param>
        /// <returns></returns>
        /// <exception cref="System.NotImplementedException"></exception>
        public IEnumerable<T> DistinctBy<TK>(Expression<Func<T, TK>> keyExpression)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Deletes the specified value.
        /// </summary>
        /// <param name="value">The value.</param>
        public void Delete(T value)
        {
            T res;
            tree.TryRemove(value.Id, out res);
        }

        /// <summary>
        /// Gets the first by id.
        /// </summary>
        /// <param name="id">The id.</param>
        /// <returns></returns>
        public T GetFirstById(object id)
        {
            if (!(id is Guid))
            {
                LogError("Attempted to get first by id, but id wasn't the expected type ({0})", typeof(Guid));
                return default(T);
            }
            T val;
            return !tree.TryGetValue((Guid)id, out val) ? default(T) : val;
        }

        /// <summary>
        /// Gets the first.
        /// </summary>
        /// <param name="predicate">The predicate.</param>
        /// <returns></returns>
        public T GetFirst(Expression<Func<T, bool>> predicate)
        {
            return tree.Values.FirstOrDefault(predicate.Compile());
        }
    }
}
