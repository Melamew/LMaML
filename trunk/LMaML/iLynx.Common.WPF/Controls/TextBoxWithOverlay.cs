using System.Windows;
using System.Windows.Controls;

namespace iLynx.Common.WPF.Controls
{
    /// <summary>
    /// TextBoxWithOverlay
    /// </summary>
    public class TextBoxWithOverlay : TextBox
    {
        /// <summary>
        /// The overlay text property
        /// </summary>
        public static readonly DependencyProperty OverlayTextProperty =
            DependencyProperty.Register("OverlayText", typeof (string), typeof (TextBoxWithOverlay), new PropertyMetadata(default(string)));

        /// <summary>
        /// The overlay visibility property
        /// </summary>
        public static readonly DependencyProperty OverlayVisibilityProperty =
            DependencyProperty.Register("OverlayVisibility", typeof (Visibility), typeof (TextBoxWithOverlay), new PropertyMetadata(default(Visibility)));

        /// <summary>
        /// Gets or sets the overlay visibility.
        /// </summary>
        /// <value>
        /// The overlay visibility.
        /// </value>
        public Visibility OverlayVisibility
        {
            get { return (Visibility) GetValue(OverlayVisibilityProperty); }
            set { SetValue(OverlayVisibilityProperty, value); }
        }

        /// <summary>
        /// Gets or sets the overlay text.
        /// </summary>
        /// <value>
        /// The overlay text.
        /// </value>
        public string OverlayText
        {
            get { return (string) GetValue(OverlayTextProperty); }
            set { SetValue(OverlayTextProperty, value); }
        }

        /// <summary>
        /// Called when one or more of the dependency properties that exist on the element have had their effective values changed.
        /// </summary>
        /// <param name="e">Arguments for the associated event.</param>
        protected override void OnPropertyChanged(DependencyPropertyChangedEventArgs e)
        {
            base.OnPropertyChanged(e);
            if (e.Property == TextProperty || e.Property == IsKeyboardFocusedProperty)
                OverlayVisibility = (IsKeyboardFocused || HasText) ? Visibility.Collapsed : Visibility.Visible;
        }

        private bool HasText
        {
            get { return !string.IsNullOrEmpty(Text); }
        }
    }
}
