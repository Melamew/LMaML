using System;
using System.Collections.Generic;
using System.Windows.Input;
using LMaML.Infrastructure.Services.Interfaces;
using Microsoft.Practices.Prism.Commands;
using iLynx.Common;

namespace LMaML.Infrastructure.Util
{
    public class CallbackMenuItem : IMenuItem
    {
        private readonly Action callback;

        /// <summary>
        /// Gets the name.
        /// </summary>
        /// <value>
        /// The name.
        /// </value>
        public string Name { get; private set; }

        /// <summary>
        /// Gets the sub items.
        /// </summary>
        /// <value>
        /// The sub items.
        /// </value>
        public IEnumerable<IMenuItem> SubItems { get; private set; }
        
        private ICommand command;

        /// <summary>
        /// Clicks this instance.
        /// </summary>
        public ICommand Command { get { return callback == null ? null : command ?? (command = new DelegateCommand(callback)); } }

        /// <summary>
        /// Initializes a new instance of the <see cref="CallbackMenuItem" /> class.
        /// </summary>
        /// <param name="callback">The callback.</param>
        /// <param name="name">The name.</param>
        /// <param name="subItems">The sub items.</param>
        public CallbackMenuItem(Action callback, string name, params IMenuItem[] subItems)
        {
            // callback may be null, not necessary to react to some items
            name.GuardString("name");
            subItems.Guard("subItems"); // params arguments will be an empty array if no arguments are supplied
            Name = name;
            this.callback = callback;
            SubItems = subItems;
            foreach (var item in subItems)
            {
                item.Guard("subItem");
                item.Changed += ItemOnChanged;
            }
        }

        /// <summary>
        /// Items the on changed.
        /// </summary>
        private void ItemOnChanged()
        {
            OnTreeChanged();
        }

        /// <summary>
        /// Called when [changed].
        /// </summary>
        protected virtual void OnTreeChanged()
        {
            if (null != Changed)
                Changed();
        }

        /// <summary>
        /// Occurs when [changed].
        /// </summary>
        public event Action Changed;
    }
}
