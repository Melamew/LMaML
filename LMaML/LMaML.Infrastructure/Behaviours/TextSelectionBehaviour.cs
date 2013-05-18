using System.Windows;
using System.Windows.Controls;

namespace LMaML.Infrastructure.Behaviours
{
    public class TextSelectionBehaviour
    {
        /// <summary>
        /// The select on focus property
        /// </summary>
        public static readonly DependencyProperty SelectOnFocusProperty =
            DependencyProperty.RegisterAttached("SelectOnFocus", typeof(bool), typeof(TextSelectionBehaviour), new PropertyMetadata(default(bool), SelectOnFocusPropertyChanged));

        private static void SelectOnFocusPropertyChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs dependencyPropertyChangedEventArgs)
        {
            var textBox = dependencyObject as TextBox;
            if (null == textBox) return;
            var val = GetSelectOnFocus(textBox);
            if (!val)
                textBox.GotKeyboardFocus -= SelectTextEvent;
            else
                textBox.GotKeyboardFocus += SelectTextEvent;
        }

        private static void SelectTextEvent(object sender, RoutedEventArgs e)
        {
            var textBox = e.OriginalSource as TextBox;
            if (null == textBox)
                return;
            if (textBox.AcceptsReturn) return;
            textBox.SelectAll();
        }

        /// <summary>
        /// Gets the select on focus.
        /// </summary>
        /// <param name="obj">The obj.</param>
        /// <returns></returns>
        public static bool GetSelectOnFocus(TextBox obj)
        {
            return (bool)obj.GetValue(SelectOnFocusProperty);
        }

        /// <summary>
        /// Sets the select on focus.
        /// </summary>
        /// <param name="obj">The obj.</param>
        /// <param name="value">if set to <c>true</c> [value].</param>
        public static void SetSelectOnFocus(TextBox obj, bool value)
        {
            obj.SetValue(SelectOnFocusProperty, value);
        }
    }
}
