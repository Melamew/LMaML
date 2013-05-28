using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using LMaML.Infrastructure.Domain.Concrete;
using LMaML.Infrastructure.Services.Interfaces;
using Microsoft.Practices.Unity;
using iLynx.Common;

namespace LMaML.Library
{
    /// <summary>
    /// LibraryManagerService
    /// </summary>
    public class LibraryManagerService : ILibraryManagerService
    {
        private readonly IUnityContainer container;
        private readonly IDataPersister<StorableTaggedFile> fileStorer;

        /// <summary>
        /// Initializes a new instance of the <see cref="LibraryManagerService" /> class.
        /// </summary>
        /// <param name="container">The container.</param>
        /// <param name="fileStorer">The file storer.</param>
        public LibraryManagerService(IUnityContainer container, IDataPersister<StorableTaggedFile> fileStorer)
        {
            container.Guard("container");
            fileStorer.Guard("fileStorer");
            this.container = container;
            this.fileStorer = fileStorer;
        }

        /// <summary>
        /// Stores the specified value.
        /// </summary>
        /// <param name="value">The value.</param>
        public void Store(StorableTaggedFile value)
        {
            //StorableTaggedFile file;
            //if ((file = value) == null)
            //    file = StorableTaggedFile.Copy(value);
            fileStorer.Save(value);
        }

        /// <summary>
        /// Finds the specified predicate.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="predicate">The predicate.</param>
        /// <returns></returns>
        public IQueryable<T> Find<T>(Expression<Func<T, bool>> predicate)
        {
            return container.Resolve<IDataAdapter<T>>().Query().Where(predicate);
        }

        /// <summary>
        /// Gets the queryable.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public IQueryable<T> GetQueryable<T>()
        {
            return container.Resolve<IDataAdapter<T>>().Query();
        }

        /// <summary>
        /// Gets the adapter.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public IDataAdapter<T> GetAdapter<T>()
        {
            return container.Resolve<IDataAdapter<T>>();
        }

        /// <summary>
        /// Gets all.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public IEnumerable<T> GetAll<T>()
        {
            return container.Resolve<IDataAdapter<T>>().GetAll();
        }
    }
}
