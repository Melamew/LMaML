using LMaML.Playlist.ViewModels;
using Microsoft.Practices.Unity;

namespace LMaML.Playlist.Views
{
    /// <summary>
    /// Interaction logic for UserControl1.xaml
    /// </summary>
    public partial class PlaylistView
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PlaylistView" /> class.
        /// </summary>
        public PlaylistView()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Gets or sets the data context for an element when it participates in data binding.
        /// </summary>
        /// <returns>The object to use as data context.</returns>
        [Dependency]
        public new PlaylistViewModel DataContext
        {
            get { return base.DataContext as PlaylistViewModel; }
            set { base.DataContext = value; }
        }
    }
}
