using System.Collections.Generic;
using System.Linq;

namespace iLynx.Common.Collections
{
    /// <summary>
    /// ExtensionMethods
    /// </summary>
    public static class ExtensionMethods
    {
        /// <summary>
        /// Wraps an asynchronous enumerable source around this IEnumerable
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source">The source.</param>
        /// <param name="maxPreBuffered">The max pre buffered.</param>
        /// <returns></returns>
        public static IEnumerable<T> WrapEnumerable<T>(this IEnumerable<T> source, int maxPreBuffered = 1)
        {
            return new AsynchronousEnumerableWrapper<T>(source, maxPreBuffered);
        }

        /// <summary>
        /// Wraps an asynchronous queryable source around this IQueryable.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source">The source.</param>
        /// <param name="maxPrebuffered">The max prebuffered.</param>
        /// <returns></returns>
        public static IQueryable<T> WrapQueryableAsync<T>(this IQueryable<T> source, int maxPrebuffered)
        {
            return new AsynchronourQueryableWrapper<T>(source, maxPrebuffered);
        }
    }
}
