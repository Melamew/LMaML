using iLynx.Configuration;

namespace LMaML.Infrastructure
{
    /// <summary>
    /// KnownConfigSections
    /// </summary>
    public static class KnownConfigSections
    {
        /// <summary>
        /// The global hotkeys
        /// </summary>
        public const string GlobalHotkeys = "Global Hotkeys";

        /// <summary>
        /// This category should not be shown in settings.
        /// </summary>
        public const string Hidden = "Hidden";

        /// <summary>
        /// The default
        /// </summary>
        public const string Default = ConfigSection.DefaultCategory;
    }
}
