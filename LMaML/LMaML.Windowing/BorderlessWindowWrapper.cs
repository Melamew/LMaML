using System;
using System.ComponentModel;
using LMaML.Infrastructure;
using iLynx.Common;
using iLynx.Common.WPF;

namespace LMaML.Windowing
{
    /// <summary>
    /// BorderlessWindowWrapper
    /// </summary>
    public class BorderlessWindowWrapper : IWindowWrapper
    {
        private readonly BorderlessWindow window;

        /// <summary>
        /// Initializes a new instance of the <see cref="BorderlessWindowWrapper" /> class.
        /// </summary>
        /// <param name="window">The window.</param>
        public BorderlessWindowWrapper(BorderlessWindow window)
        {
            window.Guard("window");
            this.window = window;
            this.window.Closing += WindowOnClosing;
        }

        /// <summary>
        /// Gets or sets the title.
        /// </summary>
        /// <value>
        /// The title.
        /// </value>
        public string Title
        {
            get { return window.Title; }
            set { window.Title = value; }
        }

        /// <summary>
        /// Gets or sets the width.
        /// </summary>
        /// <value>
        /// The width.
        /// </value>
        public double Width
        {
            get { return window.Width; }
            set { window.Width = value; }
        }

        /// <summary>
        /// Gets or sets the height.
        /// </summary>
        /// <value>
        /// The height.
        /// </value>
        public double Height
        {
            get { return window.Height; }
            set { window.Height = value; }
        }

        private void WindowOnClosing(object sender, CancelEventArgs cancelEventArgs)
        {
            OnClosing(cancelEventArgs);
        }

        private void OnClosing(CancelEventArgs args)
        {
            if (null == Closing) return;
            Closing(this, args);
        }

        /// <summary>
        /// Gets or sets the content.
        /// </summary>
        /// <value>
        /// The content.
        /// </value>
        public object Content { get { return window.Content; } set { window.Content = value; } }
        /// <summary>
        /// Gets or sets the header.
        /// </summary>
        /// <value>
        /// The header.
        /// </value>
        public object Header { get { return window.Header; } set { window.Header = value; } }

        /// <summary>
        /// Closes this instance.
        /// </summary>
        public void Close()
        {
            window.Close();
        }

        /// <summary>
        /// Shows this instance.
        /// </summary>
        public void Show()
        {
            window.Show();
        }

        /// <summary>
        /// Hides this instance.
        /// </summary>
        public void Hide()
        {
            window.Hide();
        }

        /// <summary>
        /// Occurs when [closing].
        /// </summary>
        public event EventHandler Closing;

        /// <summary>
        /// Activates this instance.
        /// </summary>
        public void Activate()
        {
            window.Activate();
        }

        /// <summary>
        /// Focuses this instance.
        /// </summary>
        public void Focus()
        {
            window.Focus();
        }
    }
}