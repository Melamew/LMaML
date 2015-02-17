using System;
using System.IO;
using System.Security.RightsManagement;
using System.Windows;
using iLynx.Configuration;
using LMaML.Infrastructure;
using LMaML.Infrastructure.Commands;
using LMaML.Infrastructure.Events;
using LMaML.Infrastructure.Services.Interfaces;
using iLynx.Common;
using Xceed.Wpf.AvalonDock.Layout.Serialization;

namespace LMaML
{
    public class WindowInfo
    {
        public Rect Bounds { get; set; }
        public WindowState State { get; set; }
    }
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class Shell
    {
        private IConfigurationManager configurationManager;
        private const string LayoutFile = ".\\Layout.xml";
        public Shell()
        {
            InitializeComponent();
            LoadLayout();
        }

        //[Conditional("PERSIST_LAYOUT")]
        private void SaveLayout()
        {
            SaveWindowInfo();
            using (var output = File.Open(LayoutFile, FileMode.Create, FileAccess.ReadWrite))
            {
                var layoutSerializer = new XmlLayoutSerializer(DockingManager);
                layoutSerializer.Serialize(output);
            }
        }

        private IConfigurableValue<WindowInfo> GetBoundsValue(IConfigurationManager source)
        {
            return source.GetValue("Window Bounds", new WindowInfo
            {
                Bounds = RestoreBounds,
                State = WindowState.Normal
            }, KnownConfigSections.Hidden);
        }

        //[Conditional("PERSIST_LAYOUT")]
        private void LoadLayout()
        {
            if (!File.Exists(LayoutFile)) return;
            using (var input = File.Open(LayoutFile, FileMode.Open, FileAccess.Read))
            {
                var layoutSerializer = new XmlLayoutSerializer(DockingManager);
                layoutSerializer.Deserialize(input);
            }
        }

        public IPublicTransport PublicTransport { get; set; }

        public IConfigurationManager ConfigurationManager
        {
            get { return configurationManager; }
            set
            {
                configurationManager = value;
                if (null == configurationManager) return;
                LoadWindowInfo();
            }
        }

        private void LoadWindowInfo()
        {
            var windowInfo = GetBoundsValue(configurationManager).Value;
            var bounds = windowInfo.Bounds;
            Left = bounds.Left;
            Top = bounds.Top;
            Width = bounds.Width;
            Height = bounds.Height;
            WindowState = windowInfo.State;
        }

        private void SaveWindowInfo()
        {
            var value = GetBoundsValue(ConfigurationManager);
            value.Value = new WindowInfo
            {
                Bounds = RestoreBounds,
                State = WindowState,
            };
            value.Store();
        }

        public ILogger Logger { get; set; }

        protected override void OnClosing(System.ComponentModel.CancelEventArgs e)
        {
            base.OnClosing(e);
            try
            {
                SaveLayout();
                PublicTransport.ApplicationEventBus.PublishWait(new ShutdownEvent());
            }
            catch (Exception ex)
            {
                if (null == Logger) return;
                Logger.Log(LogLevel.Error, this, ex.ToString());
            }
        }
    }
}
