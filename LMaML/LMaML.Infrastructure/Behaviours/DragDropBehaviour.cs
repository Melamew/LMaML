using System.Windows;
using System.Windows.Input;

namespace LMaML.Infrastructure.Behaviours
{
    public class DragDropBehaviour
    {
        public static readonly DependencyProperty DragDropCommandProperty =
            DependencyProperty.RegisterAttached("DragDropCommand", typeof (ICommand), typeof (DragDropBehaviour), new PropertyMetadata(default(ICommand), PropertyChangedCallback));

        private static void PropertyChangedCallback(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs dependencyPropertyChangedEventArgs)
        {
            var element = dependencyObject as FrameworkElement;
            if (null == element) return;
            element.Drop -= ElementOnDrop;
            if (null == dependencyPropertyChangedEventArgs.NewValue)
                return;
            element.Drop += ElementOnDrop;
        }

        private static void ElementOnDrop(object sender, DragEventArgs dragEventArgs)
        {
            var element = sender as FrameworkElement;
            if (null == element) return;
            var command = GetDragDropCommand(element);
            if (null == command) return;
            command.Execute(dragEventArgs);
        }

        public static void SetDragDropCommand(FrameworkElement element, ICommand value)
        {
            element.SetValue(DragDropCommandProperty, value);
        }

        public static ICommand GetDragDropCommand(FrameworkElement element)
        {
            return (ICommand) element.GetValue(DragDropCommandProperty);
        }
    }
}
