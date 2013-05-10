using System.Windows;
using System.Windows.Controls;

namespace LMaML.Infrastructure.Behaviours
{
    /// <summary>
    /// ListBoxScrollBehaviour
    /// </summary>
    public class ListBoxScrollBehaviour : DependencyObject
    {
        /// <summary>
        /// The scroll mode property
        /// </summary>
        public static readonly DependencyProperty ScrollModeProperty =
            DependencyProperty.RegisterAttached("ScrollMode", typeof(ListBoxScrollMode), typeof(ListBoxScrollBehaviour), new PropertyMetadata(ListBoxScrollMode.None, OnModeChanged));

        /// <summary>
        /// Called when [mode changed].
        /// </summary>
        /// <param name="dependencyObject">The dependency object.</param>
        /// <param name="dependencyPropertyChangedEventArgs">The <see cref="DependencyPropertyChangedEventArgs" /> instance containing the event data.</param>
        private static void OnModeChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs dependencyPropertyChangedEventArgs)
        {
            var listBox = dependencyObject as ListBox;
            if (null == listBox) return;
            var value = (ListBoxScrollMode)dependencyPropertyChangedEventArgs.NewValue;
            switch (value)
            {
                case ListBoxScrollMode.None:
                    listBox.SelectionChanged -= ListBoxOnSelectionChanged;
                    break;
                case ListBoxScrollMode.ToBottom:
                    break;
                case ListBoxScrollMode.ToSelected:
                    listBox.SelectionChanged += ListBoxOnSelectionChanged;
                    break;
                case ListBoxScrollMode.ToTop:
                    break;
            }
        }

        /// <summary>
        /// Lists the box on selection changed.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="selectionChangedEventArgs">The <see cref="SelectionChangedEventArgs" /> instance containing the event data.</param>
        private static void ListBoxOnSelectionChanged(object sender, SelectionChangedEventArgs selectionChangedEventArgs)
        {
            var listBox = sender as ListBox;
            if (null == listBox) return;
            listBox.ScrollIntoView(listBox.SelectedItem);
        }

        /// <summary>
        /// Sets the scroll mode.
        /// </summary>
        /// <param name="element">The element.</param>
        /// <param name="value">The value.</param>
        public static void SetScrollMode(ListBox element, ListBoxScrollMode value)
        {
            element.SetValue(ScrollModeProperty, value);
        }

        /// <summary>
        /// Gets the scroll mode.
        /// </summary>
        /// <param name="element">The element.</param>
        /// <returns></returns>
        public static ListBoxScrollMode GetScrollMode(ListBox element)
        {
            return (ListBoxScrollMode)element.GetValue(ScrollModeProperty);
        }
    }

    /// <summary>
    /// ListBoxScrollMode
    /// </summary>
    public enum ListBoxScrollMode
    {
        None,
        ToSelected,
        ToBottom,
        ToTop,
    }
}
