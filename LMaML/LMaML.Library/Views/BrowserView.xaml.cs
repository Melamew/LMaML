using LMaML.Library.ViewModels;
using Microsoft.Practices.Unity;

namespace LMaML.Library.Views
{
    /// <summary>
    /// Interaction logic for BrowserView.xaml
    /// </summary>
    public partial class BrowserView
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="BrowserView" /> class.
        /// </summary>
        public BrowserView()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Gets or sets the data context for an element when it participates in data binding.
        /// </summary>
        /// <returns>The object to use as data context.</returns>
        [Dependency]
        public new BrowserViewModel DataContext
        {
            get { return (BrowserViewModel)base.DataContext; }
            set { base.DataContext = value; }
        }
    }
}
