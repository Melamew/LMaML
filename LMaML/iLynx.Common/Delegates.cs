
namespace iLynx.Common
{
    /// <summary>
    /// This delegate is used to notify a receiver of an event that something has happened
    /// </summary>
    /// <typeparam name="TSource"></typeparam>
    /// <param name="sender">The sender of this event</param>
    public delegate void GenericEventHandler<in TSource>(TSource sender);
}