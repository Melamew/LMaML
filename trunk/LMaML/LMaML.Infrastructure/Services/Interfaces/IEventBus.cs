using System;
using LMaML.Infrastructure.Events;

namespace LMaML.Infrastructure.Services.Interfaces
{
    /// <summary>
    /// IEventBus
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface IEventBus<in T> where T : IEvent
    {
        /// <summary>
        /// Sends the specified event.
        /// </summary>
        /// <typeparam name="T2">The type of the 2.</typeparam>
        /// <param name="e">The e.</param>
        void Send<T2>(T2 e) where T2 : T;

        /// <summary>
        /// Subscribes the specified action to the event specified in the generic argument.
        /// </summary>
        /// <typeparam name="T2">The type of the 2.</typeparam>
        /// <param name="action">The action.</param>
        void Subscribe<T2>(Action<T2> action) where T2 : T;

        /// <summary>
        /// Unsubscribes the specified action from the event specified in the generic argument.
        /// </summary>
        /// <typeparam name="T2">The type of the 2.</typeparam>
        /// <param name="action">The action.</param>
        void Unsubscribe<T2>(Action<T2> action) where T2 : T;
    }
}
