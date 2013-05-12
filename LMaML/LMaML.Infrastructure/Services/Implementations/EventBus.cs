using System;
using System.Collections.Generic;
using LMaML.Infrastructure.Events;
using LMaML.Infrastructure.Services.Interfaces;

namespace LMaML.Infrastructure.Services.Implementations
{
    /// <summary>
    ///     EventBus
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class EventBus<T> : IEventBus<T> where T : IEvent
    {
        private readonly Dictionary<Type, List<Delegate>> subscriptions =
            new Dictionary<Type, List<Delegate>>();

        /// <summary>
        ///     Sends the specified e.
        /// </summary>
        /// <param name="e">The e.</param>
        public void Send<T2>(T2 e) where T2 : T
        {
            // Need a better fix for this deadlock
            //lock (subscriptions)
            //{
                List<Delegate> targets;
                if (!subscriptions.TryGetValue(typeof (T2), out targets)) return;
                targets.RemoveAll(reference => null == reference);
                foreach (var target in targets)
                    target.DynamicInvoke(e);
            //}
        }

        /// <summary>
        ///     Subscribes the specified action.
        /// </summary>
        /// <param name="action">The action.</param>
        public void Subscribe<T2>(Action<T2> action) where T2 : T
        {
            Unsubscribe(action);
            lock (subscriptions)
            {
                List<Delegate> targets;
                if (!subscriptions.TryGetValue(typeof (T2), out targets))
                {
                    targets = new List<Delegate>();
                    subscriptions.Add(typeof (T2), targets);
                }
                targets.Add(action);
            }
        }

        /// <summary>
        ///     Unsubscribes the specified action.
        /// </summary>
        /// <param name="action">The action.</param>
        public void Unsubscribe<T2>(Action<T2> action) where T2 : T
        {
            lock (subscriptions)
            {
                List<Delegate> targets;
                if (!subscriptions.TryGetValue(typeof (T2), out targets)) return;
                targets.RemoveAll(wr => wr == (Delegate) action);
                if (0 >= targets.Count)
                    subscriptions.Remove(typeof (T2));
            }
        }
    }
}