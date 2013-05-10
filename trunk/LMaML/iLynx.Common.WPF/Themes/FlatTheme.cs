using System;

namespace iLynx.Common.WPF.Themes
{
    /// <summary>
    /// Theme
    /// </summary>
    public abstract class Theme
    {
        /// <summary>
        /// Gets the resource location.
        /// </summary>
        /// <returns></returns>
        public abstract string GetResourceLocation();

        /// <summary>
        /// Gets the name.
        /// </summary>
        /// <value>
        /// The name.
        /// </value>
        public abstract Guid UniqueId { get; }
    }

    /// <summary>
    /// FlatTheme
    /// </summary>
    public class FlatTheme : Theme
    {
        /// <summary>
        /// The id
        /// </summary>
        private static readonly Guid Id = "FlatTheme".CreateGuidV5(RuntimeHelper.LynxSpace);

        /// <summary>
        /// Gets the resource location.
        /// </summary>
        /// <returns></returns>
        public override string GetResourceLocation()
        {
            return "Themes/Generic.xaml";
        }

        /// <summary>
        /// Gets the name.
        /// </summary>
        /// <value>
        /// The name.
        /// </value>
        public override Guid UniqueId
        {
            get { return Id; }
        }
    }
}
