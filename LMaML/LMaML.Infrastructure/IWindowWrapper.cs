using System;

namespace LMaML.Infrastructure
{
    /// <summary>
    /// IWindowWrapper
    /// </summary>
    public interface IWindowWrapper
    {
        /// <summary>
        /// Gets or sets the width.
        /// </summary>
        /// <value>
        /// The width.
        /// </value>
        double Width { get; set; }
        /// <summary>
        /// Gets or sets the height.
        /// </summary>
        /// <value>
        /// The height.
        /// </value>
        double Height { get; set; }
        /// <summary>
        /// Gets or sets the content.
        /// </summary>
        /// <value>
        /// The content.
        /// </value>
        object Content { get; set; }
        /// <summary>
        /// Gets or sets the header.
        /// </summary>
        /// <value>
        /// The header.
        /// </value>
        object Header { get; set; }

        /// <summary>
        /// Gets or sets the title.
        /// </summary>
        /// <value>
        /// The title.
        /// </value>
        string Title { get; set; }

        /// <summary>
        /// Closes this instance.
        /// </summary>
        void Close();
        /// <summary>
        /// Shows this instance.
        /// </summary>
        void Show();
        /// <summary>
        /// Hides this instance.
        /// </summary>
        void Hide();

        /// <summary>
        /// Occurs when [closing].
        /// </summary>
        event EventHandler Closing;

        /// <summary>
        /// Activates this instance.
        /// </summary>
        void Activate();

        /// <summary>
        /// Focuses this instance.
        /// </summary>
        void Focus();
    }
}
