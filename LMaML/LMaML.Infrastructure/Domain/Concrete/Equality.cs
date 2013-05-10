using System.Collections.Generic;

namespace LMaML.Infrastructure.Domain.Concrete
{
    public class EntityEqualityComparer : IEqualityComparer<ILibraryEntity>
    {
        /// <summary>
        /// The singleton
        /// </summary>
        public static readonly EntityEqualityComparer Singleton = new EntityEqualityComparer();

        /// <summary>
        /// Equalses the specified x.
        /// </summary>
        /// <param name="x">The x.</param>
        /// <param name="y">The y.</param>
        /// <returns></returns>
        public bool Equals(ILibraryEntity x, ILibraryEntity y)
        {
            if (null == x && null != y) return false;
            if (null == y && null != x) return false;
            if (null == y) return true;
            return x.Id == y.Id;
        }

        /// <summary>
        /// Returns a hash code for this instance.
        /// </summary>
        /// <param name="obj">The obj.</param>
        /// <returns>
        /// A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table. 
        /// </returns>
        public int GetHashCode(ILibraryEntity obj)
        {
            return obj.Id.GetHashCode();
        }
    }
}
