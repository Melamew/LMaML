using System.Windows;

namespace iLynx.Common.WPF.Themes
{
    /// <summary>
    /// IThemeManager
    /// </summary>
    public interface IThemeManager
    {
        /// <summary>
        /// Applies the theme.
        /// </summary>
        /// <param name="theme">The theme.</param>
        void ApplyTheme(ResourceDictionary target, Theme theme);

        /// <summary>
        /// Removes the theme.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <param name="theme">The theme.</param>
        void RemoveTheme(ResourceDictionary source, Theme theme);
    }
}
