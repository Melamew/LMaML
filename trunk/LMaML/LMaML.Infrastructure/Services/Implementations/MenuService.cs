using System;
using System.Collections.Generic;
using LMaML.Infrastructure.Events;
using LMaML.Infrastructure.Services.Interfaces;
using iLynx.Common;

namespace LMaML.Infrastructure.Services.Implementations
{
    /// <summary>
    /// MenuService
    /// </summary>
    public class MenuService : IMenuService
    {
        private readonly IPublicTransport publicTransport;
        private readonly Dictionary<string, IMenuItem> items;

        /// <summary>
        /// Initializes a new instance of the <see cref="MenuService" /> class.
        /// </summary>
        /// <param name="publicTransport">The public transport.</param>
        public MenuService(IPublicTransport publicTransport)
        {
            this.publicTransport = publicTransport;
            publicTransport.Guard("publicTransport");
            items = new Dictionary<string, IMenuItem>();
        }

        /// <summary>
        /// Gets the root menus.
        /// </summary>
        /// <value>
        /// The root menus.
        /// </value>
        public IEnumerable<IMenuItem> RootMenus { get { return items.Values; } }

        /// <summary>
        /// Registers the root.
        /// </summary>
        /// <param name="root">The root.</param>
        public void Register(IMenuItem root)
        {
            root.Guard("root");
            if (items.ContainsKey(root.Name))
                items[root.Name] = root;
            else
                items.Add(root.Name, root);
            root.Changed += Changed;
            Changed();
        }

        /// <summary>
        /// Roots the on changed.
        /// </summary>
        private void Changed()
        {
            publicTransport.ApplicationEventBus.Send(new MainMenuChangedEvent());
        }
    }
}
