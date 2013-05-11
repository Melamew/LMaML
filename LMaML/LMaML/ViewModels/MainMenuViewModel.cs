using System.Collections.ObjectModel;
using System.Windows.Controls;
using LMaML.Infrastructure.Events;
using LMaML.Infrastructure.Services.Interfaces;
using iLynx.Common;
using iLynx.Common.WPF;

namespace LMaML.ViewModels
{
    public class MainMenuViewModel : NotificationBase
    {
        private readonly IMenuService menuService;
        private readonly IDispatcher dispatcher;
        private readonly ObservableCollection<MenuItem> menuItems = new ObservableCollection<MenuItem>();

        /// <summary>
        /// Initializes a new instance of the <see cref="MainMenuViewModel" /> class.
        /// </summary>
        /// <param name="menuService">The menu service.</param>
        /// <param name="publicTransport">The public transport.</param>
        /// <param name="dispatcher">The dispatcher.</param>
        /// <param name="logger">The logger.</param>
        public MainMenuViewModel(IMenuService menuService, IPublicTransport publicTransport, IDispatcher dispatcher, ILogger logger)
            : base(logger)
        {
            menuService.Guard("menuService");
            publicTransport.Guard("publicTransport");
            dispatcher.Guard("dispatcher");
            this.menuService = menuService;
            this.dispatcher = dispatcher;
            BuildMenu();
            publicTransport.ApplicationEventBus.Subscribe<MainMenuChangedEvent>(OnMainMenuChanged);
        }

        private void OnMainMenuChanged(MainMenuChangedEvent mainMenuChangedEvent)
        {
            dispatcher.Invoke(BuildMenu);
        }

        private void BuildMenu()
        {
            menuItems.Clear();
            foreach (var root in menuService.RootMenus)
                MenuItems.Add(ExpandTree(root));
        }

        /// <summary>
        /// Expands the tree.
        /// </summary>
        /// <param name="item">The item.</param>
        /// <returns></returns>
        private static MenuItem ExpandTree(IMenuItem item)
        {
            var root = new MenuItem { Header = item.Name, Command = item.Command };
            foreach (var subNode in item.SubItems)
                root.Items.Add(ExpandTree(subNode));
            return root;
        }

        /// <summary>
        /// Gets the menu items.
        /// </summary>
        /// <value>
        /// The menu items.
        /// </value>
        public ObservableCollection<MenuItem> MenuItems { get { return menuItems; } }
    }
}
