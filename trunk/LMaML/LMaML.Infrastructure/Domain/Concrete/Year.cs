using System.Globalization;

namespace LMaML.Infrastructure.Domain.Concrete
{
    /// <summary>
    /// Year
    /// </summary>
    public class Year : TagReference
    {
        private uint value;
        /// <summary>
        /// Gets or sets the value.
        /// </summary>
        /// <value>
        /// The value.
        /// </value>
        public virtual uint Value
        {
            get { return value; }
            set
            {
                this.value = value;
                Name = value.ToString(CultureInfo.InvariantCulture);
            }
        }
    }
}
