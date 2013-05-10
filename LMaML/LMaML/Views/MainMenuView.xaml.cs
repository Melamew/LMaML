using LMaML.ViewModels;
using Microsoft.Practices.Unity;

namespace LMaML.Views
{
    /// <summary>
    /// Interaction logic for MainMenuView.xaml
    /// </summary>
    public partial class MainMenuView
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MainMenuView" /> class.
        /// </summary>
        public MainMenuView()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Gets or sets the data context for an element when it participates in data binding.
        /// </summary>
        /// <returns>The object to use as data context.</returns>
        [Dependency]
        public new MainMenuViewModel DataContext
        {
            get { return (MainMenuViewModel) base.DataContext; }
            set { base.DataContext = value; }
        }
    }
}
