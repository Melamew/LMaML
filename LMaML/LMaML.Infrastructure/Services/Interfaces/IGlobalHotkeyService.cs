using System;

namespace LMaML.Infrastructure.Services.Interfaces
{
    /// <summary>
    /// IGlobalHotkeyService
    /// </summary>
    public interface IGlobalHotkeyService
    {
        /// <summary>
        /// Registers the hotkey.
        /// </summary>
        /// <param name="hotkeyDescriptor">The hotkey registration.</param>
        /// <param name="callback">The callback.</param>
        void RegisterHotkey(HotkeyDescriptor hotkeyDescriptor, Action callback);

        /// <summary>
        /// Unregisters the hotkey.
        /// </summary>
        /// <param name="hotkeyDescriptor">The hotkey registration.</param>
        /// <param name="callback">The callback.</param>
        void UnregisterHotkey(HotkeyDescriptor hotkeyDescriptor, Action callback);
    }
}
