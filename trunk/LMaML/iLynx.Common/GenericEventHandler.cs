namespace iLynx.Common
{
    /// <summary>
    /// This delegate is used to notify a receiver of an event that something has happened
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <typeparam name="TSource"></typeparam>
    /// <param name="val">The even data</param>
    /// <param name="sender">The sender of the event</param>
    public delegate void GenericEventHandler<in TSource, in T>(TSource sender, T val);
}