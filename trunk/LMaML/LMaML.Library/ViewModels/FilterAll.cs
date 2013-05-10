using LMaML.Infrastructure;
using LMaML.Infrastructure.Domain.Concrete;

namespace LMaML.Library.ViewModels
{
    /// <summary>
    /// FilterAll
    /// </summary>
    public sealed class FilterAll : TagReference
    {
        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        /// <value>
        /// The name.
        /// </value>
        public override string Name
        {
            get { return "All"; }
            set
            {
                //base.Name = value;
            }
        }

        public static readonly FilterAll Singleton = new FilterAll();

        /// <summary>
        /// Returns a <see cref="System.String" /> that represents this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="System.String" /> that represents this instance.
        /// </returns>
        public override string ToString()
        {
            return "All";
        }
    }
}