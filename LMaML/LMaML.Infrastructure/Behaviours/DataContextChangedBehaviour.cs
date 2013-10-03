using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace LMaML.Infrastructure.Behaviours
{
    public class DataContextChangedBehaviour
    {
        public static readonly DependencyProperty DataContextChangedCommandProperty =
            DependencyProperty.RegisterAttached("DataContextChangedCommand", typeof (ICommand), typeof (DataContextChangedBehaviour), new PropertyMetadata(default(ICommand), PropertyChangedCallback));

        private static void PropertyChangedCallback(DependencyObject dependencyObject,
                                                    DependencyPropertyChangedEventArgs dependencyPropertyChangedEventArgs)
        {
            var element = dependencyObject as FrameworkElement;
            if (null == element) return;
            element.DataContextChanged -= ElementOnDataContextChanged;
            if (null == dependencyPropertyChangedEventArgs.NewValue) return;
            element.DataContextChanged += ElementOnDataContextChanged;
        }

        private static void ElementOnDataContextChanged(object sender, DependencyPropertyChangedEventArgs dependencyPropertyChangedEventArgs)
        {
            var element = sender as FrameworkElement;
            if (null == element) return;
            var command = GetDataContextChangedCommand(element);
            if (null == command) return;
            command.Execute(element);
        }

        public static void SetDataContextChangedCommand(FrameworkElement element,
                                ICommand value)
        {
            element.SetValue(DataContextChangedCommandProperty, value);
        }

        public static ICommand GetDataContextChangedCommand(FrameworkElement element)
        {
            return (ICommand) element.GetValue(DataContextChangedCommandProperty);
        }
    }
}
