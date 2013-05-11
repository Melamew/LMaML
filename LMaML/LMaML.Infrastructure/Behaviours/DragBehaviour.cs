using System;
using System.Windows;
using System.Windows.Input;

namespace LMaML.Infrastructure.Behaviours
{
    public class DragBehaviour
    {
        /// <summary>
        /// The drag drop command property
        /// </summary>
        public static readonly DependencyProperty DragDropCommandProperty =
            DependencyProperty.RegisterAttached("DragDropCommand", typeof (ICommand), typeof (DragBehaviour), new PropertyMetadata(default(ICommand), OnDragDropCommandChanged));

        public static readonly DependencyProperty DragLeaveCommandProperty =
            DependencyProperty.RegisterAttached("DragLeaveCommand", typeof (ICommand), typeof (DragBehaviour), new PropertyMetadata(default(ICommand), OnDragLeaveCommandChanged));

        private static void OnDragLeaveCommandChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs dependencyPropertyChangedEventArgs)
        {
            var element = dependencyObject as UIElement;
            if (null == element) return;
            element.DragLeave -= ElementOnDragLeave;
            if (null == dependencyPropertyChangedEventArgs.NewValue) return;
            element.DragLeave += ElementOnDragLeave;
        }

        private static void ElementOnDragLeave(object sender, DragEventArgs dragEventArgs)
        {
            var element = sender as UIElement;
            if (null == element) return;
            var cmd = GetDragLeaveCommand(element);
            if (null == cmd) return;
            cmd.Execute(dragEventArgs);
        }

        public static void SetDragLeaveCommand(UIElement element, ICommand value)
        {
            element.SetValue(DragLeaveCommandProperty, value);
        }

        public static ICommand GetDragLeaveCommand(UIElement element)
        {
            return (ICommand) element.GetValue(DragLeaveCommandProperty);
        }

        /// <summary>
        /// Properties the changed callback.
        /// </summary>
        /// <param name="dependencyObject">The dependency object.</param>
        /// <param name="dependencyPropertyChangedEventArgs">The <see cref="DependencyPropertyChangedEventArgs" /> instance containing the event data.</param>
        private static void OnDragDropCommandChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs dependencyPropertyChangedEventArgs)
        {
            var element = dependencyObject as UIElement;
            if (null == element) return;
            element.Drop -= ElementOnDrop;
            if (null == dependencyPropertyChangedEventArgs.NewValue)
                return;
            element.Drop += ElementOnDrop;
        }

        /// <summary>
        /// Elements the on drop.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="dragEventArgs">The <see cref="DragEventArgs" /> instance containing the event data.</param>
        private static void ElementOnDrop(object sender, DragEventArgs dragEventArgs)
        {
            var element = sender as FrameworkElement;
            if (null == element) return;
            var command = GetDragDropCommand(element);
            if (null == command) return;
            command.Execute(dragEventArgs);
        }

        /// <summary>
        /// Sets the drag drop command.
        /// </summary>
        /// <param name="element">The element.</param>
        /// <param name="value">The value.</param>
        public static void SetDragDropCommand(UIElement element, ICommand value)
        {
            element.SetValue(DragDropCommandProperty, value);
        }

        /// <summary>
        /// Gets the drag drop command.
        /// </summary>
        /// <param name="element">The element.</param>
        /// <returns></returns>
        public static ICommand GetDragDropCommand(UIElement element)
        {
            return (ICommand) element.GetValue(DragDropCommandProperty);
        }
    }
}
