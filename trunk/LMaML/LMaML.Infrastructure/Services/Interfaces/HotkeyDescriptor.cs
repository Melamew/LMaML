using System.Windows.Input;

namespace LMaML.Infrastructure.Services.Interfaces
{
    /// <summary>
    /// HotkeyRegistration
    /// </summary>
    public class HotkeyDescriptor
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="HotkeyDescriptor" /> class.
        /// </summary>
        /// <param name="modifiers">The modifiers.</param>
        /// <param name="key">The key.</param>
        public HotkeyDescriptor(ModifierKeys modifiers, Key key)
        {
            Modifiers = modifiers;
            Key = key;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="HotkeyDescriptor" /> class.
        /// </summary>
        public HotkeyDescriptor() { }

        /// <summary>
        /// Gets or sets the modifiers.
        /// </summary>
        /// <value>
        /// The modifiers.
        /// </value>
        public ModifierKeys Modifiers { get; set; }

        /// <summary>
        /// Gets or sets the key.
        /// </summary>
        /// <value>
        /// The key.
        /// </value>
        public Key Key { get; set; }
    }
}