using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using LMaML.Infrastructure;
using LMaML.Infrastructure.Domain.Concrete;
using iLynx.Common;

namespace LMaML.Library.ViewModels
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

    public interface IColumnSetup
    {
        string Name { get; }
        Guid Id { get; }
    }

    /// <summary>
    /// ColumnSetup
    /// </summary>
    public class ColumnSetup : IColumnSetup
    {
        public string Name { get; private set; }
        public Guid Id { get; private set; }

        public ColumnSetup(string name, Guid id)
        {
            name.GuardString("name");

            Name = name;
            Id = id;
        }
    }

    /// <summary>
    /// FilteringService
    /// </summary>
    public class FilteringService : IFilteringService
    {
        private readonly ILibraryManagerService libraryManagerService;
        private readonly IReferenceAdapters referenceAdapters;
        private static readonly string FilterGenre = typeof(Genre).Name;
        private static readonly string FilterArtist = typeof(Artist).Name;
        private static readonly string FilterAlbum = typeof(Album).Name;
        private static readonly string FilterYear = typeof(Year).Name;

        private readonly Dictionary<string, Func<Guid, Expression<Func<StorableTaggedFile, bool>>>> moreSelectors = new Dictionary<string, Func<Guid, Expression<Func<StorableTaggedFile, bool>>>>();

        private readonly Dictionary<string, Func<IReferenceAdapters, IQueryable<TagReference>>> columnInitiators = new Dictionary<string, Func<IReferenceAdapters, IQueryable<TagReference>>>();

        private readonly Dictionary<string, Expression<Func<StorableTaggedFile, Guid>>> distinctors = new Dictionary<string, Expression<Func<StorableTaggedFile, Guid>>>();

        private readonly Dictionary<string, Func<IEnumerable<Guid>, IReferenceAdapters, IQueryable<TagReference>>> subSelectors = new Dictionary<string, Func<IEnumerable<Guid>, IReferenceAdapters, IQueryable<TagReference>>>();

        private readonly IList<string> filterColumns;

        public IEnumerable<string> FilterColumns
        {
            get { return filterColumns; }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="FilteringService" /> class.
        /// </summary>
        /// <param name="libraryManagerService">The library manager service.</param>
        /// <param name="referenceAdapters">The reference adapters.</param>
        public FilteringService(ILibraryManagerService libraryManagerService, IReferenceAdapters referenceAdapters)
        {
            libraryManagerService.Guard("libraryManagerService");
            referenceAdapters.Guard("referenceAdapters");
            this.libraryManagerService = libraryManagerService;
            this.referenceAdapters = referenceAdapters;
            filterColumns = new List<string>(new[] { FilterGenre, FilterArtist, FilterAlbum, FilterYear });
            Initialize();
        }

        /// <summary>
        /// Initializes this instance.
        /// </summary>
        private void Initialize()
        {
            distinctors.Add(FilterArtist, file => file.ArtistId);
            distinctors.Add(FilterAlbum, file => file.AlbumId);
            distinctors.Add(FilterGenre, file => file.GenreId);
            distinctors.Add(FilterYear, file => file.YearId);

            columnInitiators.Add(FilterArtist, adapters => adapters.ArtistAdapter.Query());
            columnInitiators.Add(FilterAlbum, adapters => adapters.AlbumAdapter.Query());
            columnInitiators.Add(FilterGenre, adapters => adapters.GenreAdapter.Query());
            columnInitiators.Add(FilterYear, adapters => adapters.YearAdapter.Query());

            moreSelectors.Add(FilterArtist, id => f => Guid.Empty == id || id == f.ArtistId);
            moreSelectors.Add(FilterGenre, id => f => Guid.Empty == id || id == f.GenreId);
            moreSelectors.Add(FilterAlbum, id => f => Guid.Empty == id || id == f.AlbumId);
            moreSelectors.Add(FilterYear, id => f => Guid.Empty == id || id == f.YearId);

            subSelectors.Add(FilterArtist, (guids, adapters) => adapters.ArtistAdapter.Query().Where(x => guids.Contains(x.Id)));
            subSelectors.Add(FilterAlbum, (guids, adapters) => adapters.AlbumAdapter.Query().Where(x => guids.Contains(x.Id)));
            subSelectors.Add(FilterGenre, (guids, adapters) => adapters.GenreAdapter.Query().Where(x => guids.Contains(x.Id)));
            subSelectors.Add(FilterYear, (guids, adapters) => adapters.YearAdapter.Query().Where(x => guids.Contains(x.Id)));
        }

        /// <summary>
        /// Gets the full column.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <returns></returns>
        public async Task<IQueryable<TagReference>> GetFullColumnAsync(string name)
        {
            return await Task.Factory.StartNew(o => GetFullColumn((string)o), name);
        }

        public IQueryable<TagReference> GetFullColumn(string name)
        {
            Func<IReferenceAdapters, IQueryable<TagReference>> initiator;
            return !columnInitiators.TryGetValue(name, out initiator) ? new TagReference[] { }.AsQueryable() : new TagReference[] { new FilterAll() }.AsQueryable().Concat(initiator(referenceAdapters).OrderBy(x => x.Name));
        }

        private Expression<Func<StorableTaggedFile, bool>> BuildExpression(IList<IColumnSetup> basedOn)
        {
            var firstSetup = basedOn.FirstOrDefault();
            if (null == firstSetup) return null;
            Func<Guid, Expression<Func<StorableTaggedFile, bool>>> expressionBuilder;
            if (!moreSelectors.TryGetValue(firstSetup.Name, out expressionBuilder)) return null;
            var expression = expressionBuilder(firstSetup.Id);
            foreach (var setup in basedOn.Skip(1)) // Not really possible here ReSharper...
            {
                if (!moreSelectors.TryGetValue(setup.Name, out expressionBuilder)) continue;
                var body = Expression.AndAlso(expression.Body, expressionBuilder(setup.Id).Body);
                expression = Expression.Lambda<Func<StorableTaggedFile, bool>>(body, expression.Parameters);
            }
            return expression;
        } 

        /// <summary>
        /// Gets the column.
        /// </summary>
        /// <param name="target">The target.</param>
        /// <param name="basedOn">The based on.</param>
        /// <returns></returns>
        public IQueryable<TagReference> GetColumn(string target, params IColumnSetup[] basedOn)
        {
            basedOn.Guard("setups");
            
            var final = new TagReference[] { }.AsQueryable();
            Expression<Func<StorableTaggedFile, Guid>> distinctor;
            Func<IEnumerable<Guid>, IReferenceAdapters, IQueryable<TagReference>> subSelector;
            var expression = BuildExpression(basedOn);
            if (null == expression) return final;
            if (!distinctors.TryGetValue(target, out distinctor)) return final;
            if (!subSelectors.TryGetValue(target, out subSelector)) return final;
            
            return new TagReference[] { new FilterAll() }.AsQueryable().Concat(
                subSelector(libraryManagerService.Find(expression).Select(distinctor).Distinct(), referenceAdapters).OrderBy(x => x.Name)
                );//.Select(x => subSelector(referenceAdapters, x)).OrderBy(x => x.Name));
        }

        /// <summary>
        /// Gets the files.
        /// </summary>
        /// <param name="basedOn">The based on.</param>
        /// <returns></returns>
        public IQueryable<StorableTaggedFile> GetFiles(params IColumnSetup[] basedOn)
        {
            var ex = BuildExpression(basedOn);
            return libraryManagerService.Find(ex).Select(x => x.LazyLoadReferences(referenceAdapters));
        }

        /// <summary>
        /// Gets the files async.
        /// </summary>
        /// <param name="basedOn">The based on.</param>
        /// <returns></returns>
        public async Task<IQueryable<StorableTaggedFile>> GetFilesAsync(params IColumnSetup[] basedOn)
        {
            return await Task.Factory.StartNew(x => GetFiles((IColumnSetup[]) x), basedOn);
        } 

        /// <summary>
        /// Gets the column async.
        /// </summary>
        /// <param name="target">The target.</param>
        /// <param name="basedOn">The based on.</param>
        /// <returns></returns>
        public async Task<IQueryable<TagReference>> GetColumnAsync(string target, params IColumnSetup[] basedOn)
        {
            return await Task.Factory.StartNew(o => GetColumn(((dynamic)o).Target, ((dynamic)o).BasedOn), new { BasedOn = basedOn, Target = target });
        }
    }
}
