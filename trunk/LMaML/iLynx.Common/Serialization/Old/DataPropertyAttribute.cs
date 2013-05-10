using System;

namespace iLynx.Common.Serialization.Old
{
    /// <summary>
    ///     Attribute that should be used on properties that should be "serialized" by the <see cref="DataSerializer{T}" />
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = false)]
    public class DataPropertyAttribute : Attribute
    {
    }
}