using System;
using System.Windows;
using System.Windows.Data;
using System.Windows.Input;

namespace LMaML.Infrastructure.Behaviours
{
    public class CollectionViewFilterBehaviour
    {
        public static readonly DependencyProperty FilterCommandProperty =
            DependencyProperty.RegisterAttached("FilterCommand", typeof (ICommand), typeof (CollectionViewFilterBehaviour), new PropertyMetadata(default(ICommand), PropertyChangedCallback));

        private static void PropertyChangedCallback(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs dependencyPropertyChangedEventArgs)
        {
            var view = dependencyObject as CollectionViewSource;
            if (null == view) return;
            view.Filter -= ViewOnFilter;
            if (null == dependencyPropertyChangedEventArgs.NewValue) return;
            view.Filter += ViewOnFilter;
        }

        private static void ViewOnFilter(object sender, FilterEventArgs filterEventArgs)
        {
            var view = sender as CollectionViewSource;
            if (null == view) return;
            var command = GetFilterCommand(view);
            command.Execute(filterEventArgs);
        }

        public static void SetFilterCommand(CollectionViewSource element, ICommand value)
        {
            element.SetValue(FilterCommandProperty, value);
        }

        public static ICommand GetFilterCommand(CollectionViewSource element)
        {
            return (ICommand) element.GetValue(FilterCommandProperty);
        }
    }
}
