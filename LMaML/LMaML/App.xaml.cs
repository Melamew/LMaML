using System;
using System.Windows;
using System.Windows.Threading;
using iLynx.Configuration;
using LMaML.Infrastructure;
using LMaML.Infrastructure.Events;
using LMaML.Infrastructure.Services.Interfaces;
using Microsoft.Practices.Unity;
using iLynx.Common;

namespace LMaML
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App
    {
        private IPublicTransport publicTransport;
        private ILogger logger;

        protected override void OnStartup(StartupEventArgs e)
        {
            DispatcherUnhandledException += OnDispatcherUnhandledException;
            base.OnStartup(e);
#if DEBUG
            RunDebug();
#else
            RunRelease();
#endif
        }

        /// <summary>
        /// Raises the <see cref="E:System.Windows.Application.Exit" /> event.
        /// </summary>
        /// <param name="e">An <see cref="T:System.Windows.ExitEventArgs" /> that contains the event data.</param>
        protected override void OnExit(ExitEventArgs e)
        {
            base.OnExit(e);
            if (shutdownOnEvent) return;
            if (null == publicTransport)
                return;
            if (null == publicTransport.ApplicationEventBus)
            {
                logger.Log(LogLevel.Error, this, "Cannot find Event bus to notify application shutdown");
                return;
            }
            publicTransport.ApplicationEventBus.Send(new ShutdownEvent());
        }

        private void OnDispatcherUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs dispatcherUnhandledExceptionEventArgs)
        {
            RuntimeCommon.DefaultLogger.Log(LogLevel.Error, this, string.Format("Unhandled Exception:{0}{1}", Environment.NewLine, dispatcherUnhandledExceptionEventArgs.Exception));
        }

        private bool shutdownOnEvent = false;

        /// <summary>
        /// Runs the debug.
        /// </summary>
        private void RunDebug()
        {
            var bootstrapper = new Bootstrapper();
            bootstrapper.Run(true);
            publicTransport = bootstrapper.Container.Resolve<IPublicTransport>();
            logger = bootstrapper.Container.Resolve<ILogger>();
            publicTransport.ApplicationEventBus.Subscribe<ShutdownEvent>(OnShutdown);
        }

        private void OnShutdown(ShutdownEvent shutdownEvent)
        {
            shutdownOnEvent = true;
            Shutdown();
            ExeConfig.Save();
        }

        private void RunRelease()
        {
            var bootstrapper = new Bootstrapper();
            try
            {
                bootstrapper.Run();
                publicTransport = bootstrapper.Container.Resolve<IPublicTransport>();
                logger = bootstrapper.Container.Resolve<ILogger>();
                publicTransport.ApplicationEventBus.Subscribe<ShutdownEvent>(OnShutdown);
            }
            catch (Exception e) { RuntimeCommon.DefaultLogger.Log(LogLevel.Critical, this, string.Format("APPFAILURE:{0}{1}", Environment.NewLine, e)); }
        }
    }
}
