using System;
using System.Collections.Generic;
using System.Linq;
using LMaML.Infrastructure;
using LMaML.Infrastructure.Events;
using LMaML.Infrastructure.Services.Interfaces;
using iLynx.Common;

namespace LMaML.Services
{
    public class WindowManager : IWindowManager
    {
        private readonly IWindowFactoryService windowFactory;
        private readonly Dictionary<IRequestClose, IWindowWrapper> windows = new Dictionary<IRequestClose, IWindowWrapper>();

        public WindowManager(IPublicTransport publicTransport, IWindowFactoryService windowFactory)
        {
            publicTransport.Guard("publicTransport");
            this.windowFactory = windowFactory;
            publicTransport.ApplicationEventBus.Subscribe<ShutdownEvent>(OnShutdown);
        }

        /// <summary>
        /// Called when [shutdown].
        /// </summary>
        /// <param name="shutdownEvent">The shutdown event.</param>
        private void OnShutdown(ShutdownEvent shutdownEvent)
        {
            var wins = windows.Values.ToArray();
            foreach (var window in wins)
                window.Close();
        }

        /// <summary>
        /// Opens the new.
        /// </summary>
        /// <param name="content">The content.</param>
        /// <param name="title">The title.</param>
        /// <param name="desiredWidth">Width of the desired.</param>
        /// <param name="desiredHeight">Height of the desired.</param>
        /// <param name="header">The header.</param>
        /// <returns></returns>
        public IWindowWrapper OpenNew(IRequestClose content, string title, int desiredWidth, int desiredHeight, object header = null)
        {
            content.Guard("content");
            var window = windowFactory.CreateNew();
            window.Closing += WindowOnClosing;
            content.RequestClose += ContentOnRequestClose;
            window.Content = content;
            window.Title = title;
            window.Header = header;
            window.Width = desiredWidth;
            window.Height = desiredHeight;
            windows.Add(content, window);
            window.Show();
            return window;
        }

        private void ContentOnRequestClose(IRequestClose item)
        {
            item.Guard("item");
            IWindowWrapper window;
            if (!windows.TryGetValue(item, out window)) return;
            window.Close();
            windows.Remove(item);
        }

        private void WindowOnClosing(object sender, EventArgs cancelEventArgs)
        {
            var window = sender as IWindowWrapper;
            if (null == window) return;
            var item = window.Content as IRequestClose;
            if (null == item) return;
            windows.Remove(item);
        }
    }
}
