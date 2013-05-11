using System.Windows.Input;
using LMaML.Infrastructure.Events;
using LMaML.Infrastructure.Services.Interfaces;
using iLynx.Common;
using iLynx.Common.WPF;

namespace LMaML
{
    /// <summary>
    /// ShellViewModel
    /// </summary>
    public class ShellViewModel : NotificationBase
    {
        private readonly IPublicTransport publicTransport;

        /// <summary>
        /// Initializes a new instance of the <see cref="ShellViewModel" /> class.
        /// </summary>
        /// <param name="publicTransport">The public transport.</param>
        public ShellViewModel(IPublicTransport publicTransport)
        {
            publicTransport.Guard("publicTransport");
            this.publicTransport = publicTransport;
        }

        private ICommand collapsedCommand;
        /// <summary>
        /// Gets the collapsed command.
        /// </summary>
        /// <value>
        /// The collapsed command.
        /// </value>
        public ICommand CollapsedCommand
        {
            get { return collapsedCommand ?? (collapsedCommand = new DelegateCommand(OnCollapsed)); }
        }

        private ICommand expandedCommand;
        /// <summary>
        /// Gets the expanded command.
        /// </summary>
        /// <value>
        /// The expanded command.
        /// </value>
        public ICommand ExpandedCommand
        {
            get { return expandedCommand ?? (expandedCommand = new DelegateCommand(OnExpanded)); }
        }

        /// <summary>
        /// Called when [expanded].
        /// </summary>
        private void OnExpanded()
        {
            publicTransport.ApplicationEventBus.Send(new ShellExpandedEvent());
        }

        /// <summary>
        /// Called when [collapsed].
        /// </summary>
        private void OnCollapsed()
        {
            publicTransport.ApplicationEventBus.Send(new ShellCollapsedEvent());
        }
    }
}
