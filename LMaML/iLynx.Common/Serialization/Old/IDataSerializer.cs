using System.Collections.Generic;

namespace iLynx.Common.Serialization.Old
{
    /// <summary>
    /// An interface that, when implemented provides a <see cref="IDataAdapter{T}"/> with usable data about the generic type <typeparamref name="T"/>
    /// </summary>
    /// <typeparam name="T">The type this serializer will serialize</typeparam>
    public interface IDataSerializer<T>
    {
        /// <summary>
        /// Attempts to deserialize the specified collection of <see cref="DataProperty{T}"/> in to a new instance of <typeparamref name="T"/>
        /// </summary>
        /// <param name="fields"></param>
        /// <returns></returns>
        T Deserialize(IEnumerable<IDataProperty<T>> fields);

        /// <summary>
        /// Gets a collection of <see cref="DataProperty{T}"/> that describe the data layout
        /// </summary>
        /// <returns></returns>
        IDictionary<string, IDataProperty<T>> GetDataTemplate();

        /// <summary>
        /// When implemented in a derrived class, returns a collection of <see cref="DataProperty{T}"/> objects that can be used to search a <see cref="IDataAdapter{T}"/>
        /// <para/>
        /// - Any properties who's name is contained in <paramref name="excludeProperties"/> will not be returned in the filter array
        /// </summary>
        /// <param name="val"></param>
        /// <param name="excludeProperties"></param>
        /// <returns></returns>
        IDataProperty<T>[] GetUsableFilter(T val, params string[] excludeProperties);

        /// <summary>
        /// When implemented in a derrived class, returns a collection of <see cref="DataProperty{T}"/> that can be used to store the specified value
        /// <para/>
        /// (<typeparamref name="T"/>) using a see <see cref="IDataAdapter{T}"/>
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        IEnumerable<IDataProperty<T>> Serialize(T value);
    }
}
