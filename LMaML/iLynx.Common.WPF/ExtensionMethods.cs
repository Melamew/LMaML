using System.Windows;
using System;
using System.Threading;
using System.Windows.Media;

namespace iLynx.Common.WPF
{
    /// <summary>
    /// A set of extension methods for WPF
    /// </summary>
    public static class ExtensionMethods
    {
        /// <summary>
        /// Attempts to "safely" set the specified <see cref="DependencyProperty"/> value to the specified value
        /// </summary>
        /// <param name="obj">The object to set the value on</param>
        /// <param name="property">The property to set</param>
        /// <param name="value">The value to set</param>
        public static void SetValueSafe(this DependencyObject obj, DependencyProperty property, object value)
        {
            if (!obj.CheckAccess())
                obj.Dispatcher.Invoke(new Action<DependencyObject, DependencyProperty, object>((d, p, o) => d.SetValueSafe(p, o)), obj, property, value);
            else
                obj.SetValue(property, value);
        }

        /// <summary>
        /// Attempts to "safely" set the specified <see cref="DependencyProperty"/> value as <typeparamref name="T"/>
        /// </summary>
        /// <typeparam name="T">The type of value to retrieve</typeparam>
        /// <param name="obj">The object to retrieve the value from</param>
        /// <param name="property">The property to retrieve</param>
        /// <param name="milliTimeout">The timeout (in milliseconds) of the dispatcher operation (if Dispatcher Use is neccesary)</param>
        /// <returns></returns>
        public static T GetValueSafe<T>(this DependencyObject obj, DependencyProperty property, double milliTimeout = 50d)
        {
            object val = null;
            var done = false;
            if (!obj.CheckAccess())
            {
                obj.Dispatcher.Invoke(new Action<DependencyObject, DependencyProperty>((d, dp) =>
                {
                    val = d.GetValue(dp);
                    done = true;
                }), TimeSpan.FromMilliseconds(milliTimeout), obj, property);
            }
            else
                return (T)obj.GetValue(property);
            
            while (!done)
                Thread.Sleep(0);

            return (T)val;
        }

        /// <summary>
        /// Attempts to find the first visual child of type <typeparamref name="T"/> of the specified <see cref="DependencyObject"/>
        /// </summary>
        /// <typeparam name="T">The type of visual child to find</typeparam>
        /// <param name="obj">The root object</param>
        /// <returns></returns>
        public static T FindVisualChild<T>(this DependencyObject obj) where T : DependencyObject
        {
            T ret = null;
            for (var i = 0; i < VisualTreeHelper.GetChildrenCount(obj); ++i)
            {
                var c = VisualTreeHelper.GetChild(obj, i);
                if (c == null) continue;
                if (c is T)
                    ret = (T)c;
                else
                    ret = c.FindVisualChild<T>();
                if (ret != null)
                    break;
            }
            return ret;
        }
    }
}