using System.Windows.Controls;
using LMaML.Visualization.ViewModels;
using Microsoft.Practices.Unity;

namespace LMaML.Visualization.Views
{
    /// <summary>
    /// Interaction logic for VisualizationView.xaml
    /// </summary>
    public partial class VisualizationView
    {
        public VisualizationView()
        {
            InitializeComponent();
        }

        [Dependency]
        public new VisualizationViewModel DataContext
        {
            get { return (VisualizationViewModel) base.DataContext; }
            set { base.DataContext = value; }
        }
    }
}
