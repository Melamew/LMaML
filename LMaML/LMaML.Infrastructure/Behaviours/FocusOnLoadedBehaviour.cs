using System.Windows;
using System.Windows.Input;

namespace LMaML.Infrastructure.Behaviours
{
    public class FocusOnLoadedBehaviour
    {
        public static readonly DependencyProperty FocusOnLoadedProperty =
            DependencyProperty.RegisterAttached("FocusOnLoaded", typeof (bool), typeof (TextSelectionBehaviour), new PropertyMetadata(default(bool), OnFocusOnLoadedChanged));

        private static void OnFocusOnLoadedChanged(DependencyObject dependencyObject,
                                                   DependencyPropertyChangedEventArgs dependencyPropertyChangedEventArgs)
        {
            var element = dependencyObject as FrameworkElement;
            if (null == element) return;
            element.Loaded -= ElementOnLoaded;
            if (!GetFocusOnLoaded(element)) return;
            element.Loaded += ElementOnLoaded;
        }

        private static void ElementOnLoaded(object sender, RoutedEventArgs routedEventArgs)
        {
            var element = sender as FrameworkElement;
            if (null == element) return;
            FocusManager.SetFocusedElement(element, element);
        }

        public static void SetFocusOnLoaded(FrameworkElement element, bool value)
        {
            element.SetValue(FocusOnLoadedProperty, value);
        }

        public static bool GetFocusOnLoaded(FrameworkElement element)
        {
            return (bool) element.GetValue(FocusOnLoadedProperty);
        }
    }
}
