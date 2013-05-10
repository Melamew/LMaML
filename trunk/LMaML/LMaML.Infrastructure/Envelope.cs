namespace LMaML.Infrastructure
{
    /// <summary>
    /// Envelope
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class Envelope<T>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Envelope{T}" /> class.
        /// </summary>
        /// <param name="item">The item.</param>
        /// <param name="sender">The sender.</param>
        public Envelope(T item, object sender)
        {
            Sender = sender;
            Item = item;
        }

        /// <summary>
        /// Gets the item.
        /// </summary>
        /// <value>
        /// The item.
        /// </value>
        public T Item { get; private set; }

        /// <summary>
        /// Gets the sender.
        /// </summary>
        /// <value>
        /// The sender.
        /// </value>
        public object Sender { get; private set; }
    }
}
