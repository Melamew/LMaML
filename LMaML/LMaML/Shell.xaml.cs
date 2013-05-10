using System;
using LMaML.Infrastructure.Events;
using LMaML.Infrastructure.Services.Interfaces;
using Microsoft.Practices.ServiceLocation;
using iLynx.Common;

namespace LMaML
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class Shell
    {
        public Shell()
        {
            InitializeComponent();
        }

        public IPublicTransport PublicTransport { get; set; }
        public ILogger Logger { get; set; }

        protected override void OnClosing(System.ComponentModel.CancelEventArgs e)
        {
            base.OnClosing(e);
            try
            {
                PublicTransport.ApplicationEventBus.Send(new ShutdownEvent());
            }
            catch (Exception ex)
            {
                if (null == Logger) return;
                Logger.Log(LoggingType.Error, this, ex.ToString());
            }
        }
    }
}
