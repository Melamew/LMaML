using System.Windows;
using System.Windows.Input;

namespace LMaML.Infrastructure.Behaviours
{
    public class ElementLoadedBehaviour
    {
        /// <summary>
        /// The element loaded command property
        /// <para/>
        /// Note that the command paramter will be the element that has finished loading
        /// </summary>
        public static readonly DependencyProperty ElementLoadedCommandProperty =
            DependencyProperty.RegisterAttached("ElementLoadedCommand", typeof (ICommand), typeof (ElementLoadedBehaviour), new FrameworkPropertyMetadata(default(ICommand), PropertyChangedCallback));

        /// <summary>
        /// Properties the changed callback.
        /// </summary>
        /// <param name="dependencyObject">The dependency object.</param>
        /// <param name="dependencyPropertyChangedEventArgs">The <see cref="DependencyPropertyChangedEventArgs" /> instance containing the event data.</param>
        private static void PropertyChangedCallback(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs dependencyPropertyChangedEventArgs)
        {
            var element = dependencyObject as FrameworkElement;
            if (null == element) return;
            element.Loaded -= ElementOnLoaded;
            if (null == dependencyPropertyChangedEventArgs.NewValue) return;
            element.Loaded += ElementOnLoaded;
        }

        private static void ElementOnLoaded(object sender, RoutedEventArgs routedEventArgs)
        {
            var element = sender as FrameworkElement;
            if (null == element) return;
            var command = GetElementLoadedCommand(element);
            if (null == command)
            {
                element.Loaded -= ElementOnLoaded;
                return;
            }
            command.Execute(sender);
        }

        /// <summary>
        /// Sets the element loaded command.
        /// </summary>
        /// <param name="element">The element.</param>
        /// <param name="value">The value.</param>
        public static void SetElementLoadedCommand(FrameworkElement element, ICommand value)
        {
            element.SetValue(ElementLoadedCommandProperty, value);
        }

        /// <summary>
        /// Gets the element loaded command.
        /// </summary>
        /// <param name="element">The element.</param>
        /// <returns></returns>
        public static ICommand GetElementLoadedCommand(FrameworkElement element)
        {
            return (ICommand) element.GetValue(ElementLoadedCommandProperty);
        }
    }
}
