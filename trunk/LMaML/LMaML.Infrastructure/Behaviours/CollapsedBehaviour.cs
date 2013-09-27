using System.Windows;
using System.Windows.Input;
using iLynx.Common.WPF;

namespace LMaML.Infrastructure.Behaviours
{
    public class CollapsedBehaviour
    {
        /// <summary>
        /// The collapsed command property
        /// </summary>
        public static readonly DependencyProperty CollapsedCommandProperty =
            DependencyProperty.RegisterAttached("CollapsedCommand", typeof (ICommand), typeof (CollapsedBehaviour), new PropertyMetadata(default(ICommand), CollapsedCommandChanged));

        private static void CollapsedCommandChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs dependencyPropertyChangedEventArgs)
        {
            var window = dependencyObject as BorderlessWindow;
            if (null == window) return;
            window.Collapsed -= WindowOnCollapsed;
            if (null == dependencyPropertyChangedEventArgs.NewValue) return;
            window.Collapsed += WindowOnCollapsed;
        }

        private static void WindowOnCollapsed(object sender, RoutedEventArgs routedEventArgs)
        {
            var window = sender as BorderlessWindow;
            if (null == window) return;
            var cmd = GetCollapsedCommand(window);
            if (null == cmd) return;
            cmd.Execute(null);
        }

        /// <summary>
        /// The expanded command property
        /// </summary>
        public static readonly DependencyProperty ExpandedCommandProperty =
            DependencyProperty.RegisterAttached("ExpandedCommand", typeof (ICommand), typeof (CollapsedBehaviour), new PropertyMetadata(default(ICommand), ExpandedCommandChanged));

        private static void ExpandedCommandChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs dependencyPropertyChangedEventArgs)
        {
            var window = dependencyObject as BorderlessWindow;
            if (null == window) return;
            window.Expanded -= WindowOnExpanded;
            if (null == dependencyPropertyChangedEventArgs.NewValue) return;
            window.Expanded += WindowOnExpanded;
        }

        private static void WindowOnExpanded(object sender, RoutedEventArgs routedEventArgs)
        {
            var window = sender as BorderlessWindow;
            if (null == window) return;
            var cmd = GetExpandedCommand(window);
            if (null == cmd) return;
            cmd.Execute(null);
        }

        /// <summary>
        /// Sets the expanded command.
        /// </summary>
        /// <param name="element">The element.</param>
        /// <param name="value">The value.</param>
        public static void SetExpandedCommand(BorderlessWindow element, ICommand value)
        {
            element.SetValue(ExpandedCommandProperty, value);
        }

        /// <summary>
        /// Gets the expanded command.
        /// </summary>
        /// <param name="element">The element.</param>
        /// <returns></returns>
        public static ICommand GetExpandedCommand(BorderlessWindow element)
        {
            return (ICommand) element.GetValue(ExpandedCommandProperty);
        }

        /// <summary>
        /// Sets the collapsed command.
        /// </summary>
        /// <param name="element">The element.</param>
        /// <param name="value">The value.</param>
        public static void SetCollapsedCommand(BorderlessWindow element, ICommand value)
        {
            element.SetValue(CollapsedCommandProperty, value);
        }

        /// <summary>
        /// Gets the collapsed command.
        /// </summary>
        /// <param name="element">The element.</param>
        /// <returns></returns>
        public static ICommand GetCollapsedCommand(BorderlessWindow element)
        {
            return (ICommand) element.GetValue(CollapsedCommandProperty);
        }
    }
}
