namespace LMaML.Infrastructure.Domain.Concrete
{
    /// <summary>
    ///     A container class for informations about an album
    /// </summary>
    public class Album : TagReference
    {
        /// <summary>
        /// Gets or sets the year.
        /// </summary>
        /// <value>
        /// The year.
        /// </value>
        public virtual int Year { get; set; }
        
        /// <summary>
        /// Gets or sets the track count.
        /// </summary>
        /// <value>
        /// The track count.
        /// </value>
        public virtual int TrackCount { get; set; }
    }
}