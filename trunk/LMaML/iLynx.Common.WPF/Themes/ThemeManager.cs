using System;
using System.Collections.Generic;
using System.Windows;

namespace iLynx.Common.WPF.Themes
{
    public class ThemeManager : IThemeManager
    {
        /// <summary>
        /// The loaded themes
        /// </summary>
        private static readonly Dictionary<Guid, ResourceDictionary> LoadedThemes = new Dictionary<Guid, ResourceDictionary>();

        /// <summary>
        /// The theme property
        /// </summary>
        public static readonly DependencyProperty ThemeProperty =
            DependencyProperty.RegisterAttached("Theme", typeof(Theme), typeof(ThemeManager), new FrameworkPropertyMetadata(default(Theme), FrameworkPropertyMetadataOptions.AffectsRender, OnThemeChanged, CoerceValueCallback));

        /// <summary>
        /// Coerces the value callback.
        /// </summary>
        /// <param name="dependencyObject">The dependency object.</param>
        /// <param name="baseValue">The base value.</param>
        /// <returns></returns>
        private static object CoerceValueCallback(DependencyObject dependencyObject, object baseValue)
        {
            var element = dependencyObject as FrameworkElement;
            if (null == element) return baseValue;
            var old = GetTheme(element);
            RemoveTheme(element, old);
            ApplyTheme(element, baseValue as Theme);
            return baseValue;
        }

        /// <summary>
        /// Called when [theme changed].
        /// </summary>
        /// <param name="dependencyObject">The dependency object.</param>
        /// <param name="dependencyPropertyChangedEventArgs">The <see cref="DependencyPropertyChangedEventArgs" /> instance containing the event data.</param>
        private static void OnThemeChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs dependencyPropertyChangedEventArgs)
        {
            var element = dependencyObject as FrameworkElement;
            if (null == element) return;
            var old = dependencyPropertyChangedEventArgs.OldValue as Theme;
            if (null != old)
                RemoveTheme(element, old);
            var newTheme = dependencyPropertyChangedEventArgs.NewValue as Theme;
            if (null == newTheme) return;
            ApplyTheme(element, newTheme);
        }

        /// <summary>
        /// Sets the theme.
        /// </summary>
        /// <param name="element">The element.</param>
        /// <param name="value">The value.</param>
        public static void SetTheme(FrameworkElement element, Theme value)
        {
            element.SetValue(ThemeProperty, value);
        }

        /// <summary>
        /// Gets the theme.
        /// </summary>
        /// <param name="element">The element.</param>
        /// <returns></returns>
        public static Theme GetTheme(FrameworkElement element)
        {
            return (Theme)element.GetValue(ThemeProperty);
        }

        /// <summary>
        /// Removes the theme.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <param name="theme">The theme.</param>
        public static void RemoveTheme(FrameworkElement source, Theme theme)
        {
            if (null == theme) return;
            if (null == source) return;
            lock (LoadedThemes)
            {
                ResourceDictionary dict;
                if (!LoadedThemes.TryGetValue(theme.UniqueId, out dict)) return;
                source.Resources.MergedDictionaries.Remove(dict);
            }
        }

        /// <summary>
        /// Applies the theme.
        /// </summary>
        /// <param name="target">The target.</param>
        /// <param name="theme">The theme.</param>
        public static void ApplyTheme(FrameworkElement target, Theme theme)
        {
            if (null == theme) return;
            if (null == target) return;
            try
            {
                lock (LoadedThemes)
                {
                    ResourceDictionary dict;
                    var resource = theme.GetResourceLocation();
                    var sourceAssembly = theme.GetType().Assembly;
                    if (!LoadedThemes.TryGetValue(theme.UniqueId, out dict))
                        dict = new ResourceDictionary { Source = RuntimeHelper.MakePackUri(sourceAssembly, resource) };
                    else
                        target.Resources.MergedDictionaries.Remove(dict);
                    target.Resources.MergedDictionaries.Add(dict);
                }
            }
            catch (Exception e)
            {
                RuntimeCommon.DefaultLogger.Log(LoggingType.Error, null, e.ToString());
            }
        }

        /// <summary>
        /// Applies the theme.
        /// </summary>
        /// <param name="target"></param>
        /// <param name="theme">The theme.</param>
        public void ApplyTheme(ResourceDictionary target, Theme theme)
        {
            target.Guard("target");
            theme.Guard("theme");
            lock (LoadedThemes)
            {
                ResourceDictionary dict;
                RemoveTheme(target, theme);
                if (!LoadedThemes.TryGetValue(theme.UniqueId, out dict))
                {
                    var resource = theme.GetResourceLocation();
                    var sourceAssembly = theme.GetType().Assembly;
                    dict = new ResourceDictionary { Source = RuntimeHelper.MakePackUri(sourceAssembly, resource) };
                }
                target.MergedDictionaries.Add(dict);
            }
        }

        /// <summary>
        /// Removes the theme.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <param name="theme">The theme.</param>
        public void RemoveTheme(ResourceDictionary source, Theme theme)
        {
            source.Guard("source");
            theme.Guard("theme");
            lock (LoadedThemes)
            {
                ResourceDictionary dict;
                if (!LoadedThemes.TryGetValue(theme.UniqueId, out dict))
                    return;
                source.MergedDictionaries.Remove(dict);
            }
        }
    }
}
