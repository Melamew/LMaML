using System;

namespace iLynx.Common.Serialization.Old
{
    /// <summary>
    /// This interface describes a single property of a given type <typeparamref name="TOwner"/>
    /// <para/>
    /// Please note that <typeparamref name="TOwner"/> is the owner of the property, not the type of the property
    /// <para/>
    /// The type of the property can be found in <see cref="IDataProperty{TOwner}.DataType"/>
    /// </summary>
    /// <typeparam name="TOwner"></typeparam>
    public interface IDataProperty<TOwner>
    {
        /// <summary>
        /// Gets or Sets the type of this property
        /// </summary>
        Type DataType { get; set; }

        /// <summary>
        /// Gets or Sets the name of this property
        /// </summary>
        string PropertyName { get; set; }

        /// <summary>
        /// Gets or Sets the actual value of this property
        /// </summary>
        object Value { get; set; }
    }
}
