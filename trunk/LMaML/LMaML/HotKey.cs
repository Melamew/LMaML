using System;
using System.Collections.Generic;
using System.Windows.Input;
using iLynx.Common;

namespace LMaML
{
    /// <summary>
    /// 
    /// </summary>
    public class HotKey
    {
        /// <summary>
        /// Gets or sets the id.
        /// </summary>
        /// <value>
        /// The id.
        /// </value>
        public int Id { get; private set; }


        private readonly ModifierKeys modifiers;
        private readonly Key key;
        private readonly List<Action> callbacks = new List<Action>();

        /// <summary>
        /// Initializes a new instance of the <see cref="HotKey" /> class.
        /// </summary>
        /// <param name="modifiers">The modifiers.</param>
        /// <param name="key">The key.</param>
        /// <param name="id">The id.</param>
        public HotKey(ModifierKeys modifiers, Key key, int id)
        {
            Id = id;
            this.modifiers = modifiers;
            this.key = key;
        }

        /// <summary>
        /// Registers the callback.
        /// </summary>
        /// <param name="callback">The callback.</param>
        public void RegisterCallback(Action callback)
        {
            callback.Guard("callback");
            callbacks.Remove(callback);
            callbacks.Add(callback);
        }

        /// <summary>
        /// Uns the register callback.
        /// </summary>
        /// <param name="callback">The callback.</param>
        public void UnRegisterCallback(Action callback)
        {
            callbacks.Remove(callback);
        }

        /// <summary>
        /// Gets a value indicating whether this instance is empty.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this instance is empty; otherwise, <c>false</c>.
        /// </value>
        public bool IsEmpty
        {
            get { return callbacks.Count <= 0; } // You never know...
        }

        /// <summary>
        /// Determines whether [is] [the specified mods].
        /// </summary>
        /// <param name="mods">The mods.</param>
        /// <param name="k">The k.</param>
        /// <returns>
        ///   <c>true</c> if [is] [the specified mods]; otherwise, <c>false</c>.
        /// </returns>
        public bool Is(ModifierKeys mods, Key k)
        {
            return mods == modifiers && k == key;
        }

        /// <summary>
        /// Calls this instance.
        /// </summary>
        public void Call()
        {
            foreach (var callback in callbacks)
                callback();
        }

        /// <summary>
        /// Gets the modifiers.
        /// </summary>
        /// <value>
        /// The modifiers.
        /// </value>
        public ModifierKeys Modifiers { get { return modifiers; } }

        /// <summary>
        /// Gets the key.
        /// </summary>
        /// <value>
        /// The key.
        /// </value>
        public Key Key { get { return key; } }
    }
}