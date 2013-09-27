using System;
using System.Windows;
using System.Windows.Input;
using iLynx.Common.WPF;

namespace LMaML.Infrastructure.Behaviours
{
    public class ResizeBehaviour
    {
        public static readonly DependencyProperty ResizeBeginCommandProperty =
            DependencyProperty.RegisterAttached("ResizeBeginCommand", typeof (ICommand), typeof (ResizeBehaviour), new PropertyMetadata(default(ICommand), OnResizeBeginCommandChanged));

        private static void OnResizeBeginCommandChanged(DependencyObject dependencyObject,
                                                    DependencyPropertyChangedEventArgs dependencyPropertyChangedEventArgs)
        {
            var win = dependencyObject as BorderlessWindow;
            if (null == win) return;
            win.ResizeBegin -= WinOnResizeBegin;
            if (null == dependencyPropertyChangedEventArgs.NewValue) return;
            win.ResizeBegin += WinOnResizeBegin;
        }

        private static void WinOnResizeBegin(object sender,
                                             EventArgs eventArgs)
        {
            var win = sender as BorderlessWindow;
            if (null == win) return;
            var cmd = GetResizeBeginCommand(win);
            if (null == cmd) return;
            cmd.Execute(null);
        }

        public static readonly DependencyProperty ResizeEndCommandProperty =
            DependencyProperty.RegisterAttached("ResizeEndCommand", typeof (ICommand), typeof (ResizeBehaviour), new PropertyMetadata(default(ICommand), OnResizeEndCommandChanged));

        private static void OnResizeEndCommandChanged(DependencyObject dependencyObject,
                                                    DependencyPropertyChangedEventArgs dependencyPropertyChangedEventArgs)
        {
            var win = dependencyObject as BorderlessWindow;
            if (null == win) return;
            win.ResizeEnd -= WinOnResizeEnd;
            if (null == dependencyPropertyChangedEventArgs.NewValue) return;
            win.ResizeEnd += WinOnResizeEnd;
        }

        private static void WinOnResizeEnd(object sender,
                                           EventArgs eventArgs)
        {
            var win = sender as BorderlessWindow;
            if (null == win) return;
            var cmd = GetResizeEndCommand(win);
            if (null == cmd) return;
            cmd.Execute(null);
        }

        public static void SetResizeEndCommand(UIElement element,
                                ICommand value)
        {
            element.SetValue(ResizeEndCommandProperty, value);
        }

        public static ICommand GetResizeEndCommand(UIElement element)
        {
            return (ICommand) element.GetValue(ResizeEndCommandProperty);
        }

        public static void SetResizeBeginCommand(UIElement element,
                                ICommand value)
        {
            element.SetValue(ResizeBeginCommandProperty, value);
        }

        public static ICommand GetResizeBeginCommand(UIElement element)
        {
            return (ICommand) element.GetValue(ResizeBeginCommandProperty);
        }
    }
}
