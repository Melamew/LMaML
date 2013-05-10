using LMaML.ViewModels;
using Microsoft.Practices.Unity;

namespace LMaML.Views
{
    /// <summary>
    /// Interaction logic for StatusView.xaml
    /// </summary>
    public partial class StatusView
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="StatusView" /> class.
        /// </summary>
        public StatusView()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Gets or sets the data context for an element when it participates in data binding.
        /// </summary>
        /// <returns>The object to use as data context.</returns>
        [Dependency]
        public new StatusViewModel DataContext
        {
            get { return (StatusViewModel) base.DataContext; }
            set { base.DataContext = value; }
        }
    }
}
