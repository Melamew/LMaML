using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Windows.Controls;
using LMaML.Infrastructure;
using LMaML.Infrastructure.Events;
using LMaML.Infrastructure.Services.Interfaces;
using iLynx.Common;
using Microsoft.Practices.Prism.Regions;
using Xceed.Wpf.AvalonDock.Layout;
using Xceed.Wpf.AvalonDock.Layout.Serialization;

namespace LMaML
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class Shell
    {
        private const string LayoutFile = ".\\Layout.xml";
        public Shell()
        {
            InitializeComponent();
            LoadLayout();
        }

        [Conditional("PERSIST_LAYOUT")]
        private void SaveLayout()
        {
            using (var output = File.Open(LayoutFile, FileMode.Create, FileAccess.ReadWrite))
            {
                var layoutSerializer = new XmlLayoutSerializer(DockingManager);
                layoutSerializer.Serialize(output);
            }
        }

        [Conditional("PERSIST_LAYOUT")]
        private void LoadLayout()
        {
            if (!File.Exists(LayoutFile)) return;
            using (var input = File.Open(LayoutFile, FileMode.Open, FileAccess.Read))
            {
                var layoutSerializer = new XmlLayoutSerializer(DockingManager);
                layoutSerializer.Deserialize(input);
                //foreach (var anchorable in DockingManager.Layout.Descendents().OfType<LayoutAnchorable>())
                //{
                //    // TODO: Replace with dictionary lookup?
                //    ContentControl control;
                //    switch (anchorable.Title)
                //    {
                //        case "Playlist":
                //            anchorable.Content = control = new ContentControl();
                //            RegionManager.SetRegionName(control, RegionNames.PlaylistRegion);
                //            break;
                //        case "Browser":
                //            anchorable.Content = control = new ContentControl();
                //            RegionManager.SetRegionName(control, RegionNames.BrowserRegion);
                //            break;
                //        case "Player":
                //            anchorable.Content = control = new ContentControl();
                //            RegionManager.SetRegionName(control, RegionNames.ControlsRegion);
                //            break;
                //        case "Visualization":
                //            anchorable.Content = control = new ContentControl();
                //            RegionManager.SetRegionName(control, RegionNames.VisualizationRegion);
                //            break;
                //    }
                //}
            }
        }

        public IPublicTransport PublicTransport { get; set; }
        public ILogger Logger { get; set; }

        protected override void OnClosing(System.ComponentModel.CancelEventArgs e)
        {
            base.OnClosing(e);
            try
            {
                SaveLayout();
                PublicTransport.ApplicationEventBus.Send(new ShutdownEvent());
            }
            catch (Exception ex)
            {
                if (null == Logger) return;
                Logger.Log(LogLevel.Error, this, ex.ToString());
            }
        }
    }
}
