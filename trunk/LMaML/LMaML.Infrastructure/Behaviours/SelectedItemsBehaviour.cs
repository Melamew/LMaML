using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;

namespace LMaML.Infrastructure.Behaviours
{
    public class SelectedItemsBehaviour
    {
        public static readonly DependencyProperty SynchronizationTargetProperty =
            DependencyProperty.RegisterAttached("SynchronizationTarget", typeof(IList), typeof(SelectedItemsBehaviour), new PropertyMetadata(new List<object>(), PropertyChangedCallback));

        public static readonly DependencyProperty SynchronizationManagerProperty =
            DependencyProperty.RegisterAttached("SynchronizationManager", typeof(SynchronizationManager), typeof(SelectedItemsBehaviour), new PropertyMetadata(default(SynchronizationManager)));

        public static void SetSynchronizationManager(UIElement element, SynchronizationManager value)
        {
            element.SetValue(SynchronizationManagerProperty, value);
        }

        public static SynchronizationManager GetSynchronizationManager(UIElement element)
        {
            return (SynchronizationManager)element.GetValue(SynchronizationManagerProperty);
        }

        public class SynchronizationManager : IDisposable
        {
            private readonly IList target;
            private readonly ObservableCollection<object> source;

            public SynchronizationManager(IList target, ObservableCollection<object> source)
            {
                this.target = target;
                this.source = source;
                source.CollectionChanged += SourceOnCollectionChanged;
            }

            private void SourceOnCollectionChanged(object sender, NotifyCollectionChangedEventArgs notifyCollectionChangedEventArgs)
            {
                switch (notifyCollectionChangedEventArgs.Action)
                {
                    case NotifyCollectionChangedAction.Remove:
                        foreach (var item in notifyCollectionChangedEventArgs.OldItems)
                            target.Remove(item);
                        break;
                    case NotifyCollectionChangedAction.Add:
                        foreach (var item in notifyCollectionChangedEventArgs.NewItems)
                            target.Add(item);
                        break;
                }
            }

            public void Dispose()
            {
                source.CollectionChanged -= SourceOnCollectionChanged;
            }
        }

        private static void PropertyChangedCallback(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs dependencyPropertyChangedEventArgs)
        {
            var element = dependencyObject as UIElement;
            if (null == element) return;
            if (null != dependencyPropertyChangedEventArgs.OldValue)
            {
                var manager = GetSynchronizationManager(element);
                if (null != manager)
                {
                    manager.Dispose();
                    SetSynchronizationManager(element, null);
                }
            }
            if (null == dependencyPropertyChangedEventArgs.NewValue) return;
            var selector = dependencyObject as MultiSelector;
            if (null == selector)
            {
                TryListBox(element, dependencyPropertyChangedEventArgs);
                return;
            }
            var observable = selector.SelectedItems as ObservableCollection<object>;
            if (null == observable) return;
            var target = dependencyPropertyChangedEventArgs.NewValue as IList;
            if (null == target) return;
            Link(element, target, observable);
        }

        private static void Link(UIElement obj, IList target, ObservableCollection<object> source)
        {
            SetSynchronizationManager(obj, new SynchronizationManager(target, source));
        }

        private static void TryListBox(UIElement element,
                                DependencyPropertyChangedEventArgs dependencyPropertyChangedEventArgs)
        {
            var lb = element as ListBox;
            if (null == lb) return;
            var observable = lb.SelectedItems as ObservableCollection<object>;
            if (null == observable) return;
            var target = dependencyPropertyChangedEventArgs.NewValue as IList;
            if (null == target) return;
            Link(element, target, observable);
        }

        public static void SetSynchronizationTarget(UIElement element, ICollection value)
        {
            element.SetValue(SynchronizationTargetProperty, value);
        }

        public static ICollection GetSynchronizationTarget(UIElement element)
        {
            return (ICollection)element.GetValue(SynchronizationTargetProperty);
        }
    }
}
