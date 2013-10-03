using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using Xceed.Wpf.AvalonDock.Controls;
using Xceed.Wpf.AvalonDock.Layout;
using iLynx.Common.WPF;

namespace LMaML.Infrastructure.Behaviours
{
    public class IsAnchorableVisibleBehaviour : DependencyObject
    {
        public static readonly DependencyProperty IsAnchorableVisibleChangedCommandProperty =
            DependencyProperty.RegisterAttached("IsAnchorableVisibleChangedCommand", typeof(ICommand), typeof(IsAnchorableVisibleBehaviour), new PropertyMetadata(default(ICommand), OnChanged));

        public static readonly DependencyPropertyKey VisibilityTargetsPropertyKey =
            DependencyProperty.RegisterAttachedReadOnly("VisibilityTargets", typeof(List<UIElement>), typeof(IsAnchorableVisibleBehaviour), new PropertyMetadata(default(List<UIElement>)));

        private static void SetVisibilityTargets(DependencyObject element,
                                List<UIElement> value)
        {
            element.SetValue(VisibilityTargetsPropertyKey, value);
        }

        private static List<UIElement> GetVisibilityTargets(DependencyObject element)
        {
            return (List<UIElement>)element.GetValue(VisibilityTargetsPropertyKey.DependencyProperty);
        }

        private static void OnChanged(DependencyObject dependencyObject,
                                                    DependencyPropertyChangedEventArgs dependencyPropertyChangedEventArgs)
        {
            var element = dependencyObject as UIElement;
            if (null == element) return;
            var p = element.FindVisualParent<LayoutAnchorableControl>();
            if (null == p) return;
            var parent = p.LayoutItem.LayoutElement as LayoutAnchorable;
            if (null == parent) return;
            var targets = GetVisibilityTargets(parent);
            parent.IsSelectedChanged -= ParentOnIsVisibleChanged;
            parent.IsVisibleChanged -= ParentOnIsVisibleChanged;
            if (null == dependencyPropertyChangedEventArgs.NewValue)
            {
                if (null != targets)
                    targets.Remove(element);
                return;
            }
            parent.IsSelectedChanged += ParentOnIsVisibleChanged;
            parent.IsVisibleChanged += ParentOnIsVisibleChanged;
            if (null == targets)
            {
                targets = new List<UIElement>();
                SetVisibilityTargets(parent, targets);
            }
            targets.Add(element);
        }

        private static void ParentOnIsVisibleChanged(object sender,
                                                     EventArgs dependencyPropertyChangedEventArgs)
        {
            var parent = sender as LayoutAnchorable;
            if (null == parent) return;
            var targets = GetVisibilityTargets(parent);
            if (null == targets || 0 == targets.Count)
            {
                parent.IsVisibleChanged -= ParentOnIsVisibleChanged;
                return;
            }
            targets.RemoveAll(x => null == x || null == GetIsAnchorableVisibleChangedCommand(x));
            foreach (var command in targets.Select(GetIsAnchorableVisibleChangedCommand))
                command.Execute(parent.IsVisible && parent.IsSelected);
        }

        public static void SetIsAnchorableVisibleChangedCommand(UIElement element,
                                ICommand value)
        {
            element.SetValue(IsAnchorableVisibleChangedCommandProperty, value);
        }

        public static ICommand GetIsAnchorableVisibleChangedCommand(UIElement element)
        {
            return (ICommand)element.GetValue(IsAnchorableVisibleChangedCommandProperty);
        }
    }
}
