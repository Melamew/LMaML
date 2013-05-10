using System;

namespace iLynx.Common.Serialization.Old
{
    /// <summary>
    ///     A structure used to represent a single property in <see cref="DataSerializer{T}" />
    /// </summary>
    /// <typeparam name="TOwner">Used to constrain types</typeparam>
    public struct DataProperty<TOwner> : IDataProperty<TOwner>
    {
        private Type dataType;
        private string name;
        private object value;

        /// <summary>
        ///     Initializes a new instance of <see cref="DataProperty{T}" /> and sets the <see cref="DataProperty{T}.Value" /> and
        ///     <see
        ///         cref="DataProperty{T}.PropertyName" />
        ///     to the specified values
        ///     <para />
        ///     The <see cref="DataProperty{T}.DataType" /> property is set to the <paramref name="dataType" /> argument
        /// </summary>
        /// <param name="name">The name of the property</param>
        /// <param name="value">The value of the property</param>
        /// <param name="dataType">The type of data</param>
        public DataProperty(string name, object value, Type dataType)
        {
            this.value = value;
            this.name = name;
            this.dataType = dataType;
        }

        /// <summary>
        ///     Initializes a new instance of <see cref="DataProperty{T}" /> and sets the <see cref="DataProperty{T}.Value" /> and
        ///     <see
        ///         cref="DataProperty{T}.PropertyName" />
        ///     to the specified values
        ///     <para />
        ///     The <see cref="DataProperty{T}.DataType" /> property is set to the result of value.GetType
        ///     <para />
        ///     Please note that value cannot be null for this to work
        /// </summary>
        /// <param name="name">The name of the property</param>
        /// <param name="value">The value of the property</param>
        public DataProperty(string name, object value)
        {
            this.value = value;
            this.name = name;
            if (value == null)
                throw new ArgumentNullException("value");
            dataType = value.GetType();
        }

        /// <summary>
        ///     Initializes a new instance of <see cref="DataProperty{T}" /> and sets the name and datatype to those contained in the specified template
        ///     <para />
        ///     Value is set to null
        /// </summary>
        /// <param name="template"></param>
        public DataProperty(IDataProperty<TOwner> template)
        {
            value = null;
            name = template.PropertyName;
            dataType = template.DataType;
        }

        /// <summary>
        ///     Gets or Sets the value of this property
        /// </summary>
        public object Value
        {
            get { return value; }
            set { this.value = value; }
        }

        /// <summary>
        ///     Gets or Sets the name of this property
        /// </summary>
        public string PropertyName
        {
            get { return name; }
            set { name = value; }
        }

        /// <summary>
        ///     Gets or Sets the type of data that this <see cref="DataProperty{T}" /> will eventually contain
        /// </summary>
        public Type DataType
        {
            get { return dataType; }
            set { dataType = value; }
        }
    }
}