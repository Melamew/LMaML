using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Input;
using System.Windows.Interop;
using LMaML.Infrastructure.Services.Interfaces;

namespace LMaML
{
    public class GlobalHotkeyService : IGlobalHotkeyService, IDisposable
    {
        private readonly HwndSource source;
        private readonly Dictionary<int, HotKey> registeredHotkeys = new Dictionary<int, HotKey>();
        
        public GlobalHotkeyService()
        {
            var interopHelper = new WindowInteropHelper(Application.Current.MainWindow);
            interopHelper.EnsureHandle();
            source = HwndSource.FromHwnd(interopHelper.Handle);
            if (null == source)
            {
                Trace.WriteLine("Could not get HwndSource for the Shell");
                return;
            }
            source.AddHook(WndProc);
        }
    
        private int hotkeyId;
        /// <summary>
        /// Gets the hotkey id.
        /// </summary>
        /// <value>
        /// The hotkey id.
        /// </value>
        private int HotkeyId
        {
            get { return hotkeyId++; }
        }

        private IntPtr WndProc(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
        {
            if (msg == 786)
            {
                HotKey hotKey;
                if (registeredHotkeys.TryGetValue((int)wParam, out hotKey))
                    hotKey.Call();
            }
            return IntPtr.Zero;
        }

        /// <summary>
        /// Registers the hot key.
        /// </summary>
        /// <param name="hwnd">The HWND.</param>
        /// <param name="id">The id.</param>
        /// <param name="modifiers">The modifiers.</param>
        /// <param name="key">The key.</param>
        /// <returns></returns>
        [DllImport("user32", CharSet = CharSet.Ansi, SetLastError = true, ExactSpelling = true)]
        private static extern int RegisterHotKey(IntPtr hwnd, int id, int modifiers, int key);

        /// <summary>
        /// Unregisters the hot key.
        /// </summary>
        /// <param name="hwnd">The HWND.</param>
        /// <param name="id">The id.</param>
        /// <returns></returns>
        [DllImport("user32", CharSet = CharSet.Ansi, SetLastError = true, ExactSpelling = true)]
        private static extern int UnregisterHotKey(IntPtr hwnd, int id);

        /// <summary>
        /// Registers the hotkey.
        /// </summary>
        /// <param name="hotkeyDescriptor">The hotkey registration.</param>
        /// <param name="callback">The callback.</param>
        public void RegisterHotkey(HotkeyDescriptor hotkeyDescriptor, Action callback)
        {
            var hotkey = GetHotKey(hotkeyDescriptor.Modifiers, hotkeyDescriptor.Key);
            if (null == hotkey)
            {
                hotkey = new HotKey(hotkeyDescriptor.Modifiers, hotkeyDescriptor.Key, HotkeyId);
                registeredHotkeys.Add(hotkey.Id, hotkey);
                HookHotkey(hotkey, hotkey.Id);
            }
            hotkey.RegisterCallback(callback);
        }

        private HotKey GetHotKey(ModifierKeys modifiers, Key key)
        {
            return registeredHotkeys.Values.FirstOrDefault(x => x.Is(modifiers, key));
        }

        /// <summary>
        /// Unregisters the hotkey.
        /// </summary>
        /// <param name="hotkeyDescriptor">The hotkey registration.</param>
        /// <param name="callback">The callback.</param>
        public void UnregisterHotkey(HotkeyDescriptor hotkeyDescriptor, Action callback)
        {
            var hotkey = GetHotKey(hotkeyDescriptor.Modifiers, hotkeyDescriptor.Key);
            if (null == hotkey) return;
            hotkey.UnRegisterCallback(callback);
            if (!hotkey.IsEmpty) return;
            UnregisterHotKey(source.Handle, hotkey.Id);
            registeredHotkeys.Remove(hotkey.Id);
        }

        private void HookHotkey(HotKey hotkey, int id)
        {
            RegisterHotKey(source.Handle, id, (int)hotkey.Modifiers, KeyInterop.VirtualKeyFromKey(hotkey.Key));
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            foreach (var kvp in registeredHotkeys)
            {
                UnregisterHotKey(source.Handle, kvp.Key);
                var error = Marshal.GetLastWin32Error();
                if (0 != error)
                    Trace.WriteLine(string.Format("Got error: {0} while trying to unregister hotkey", error));
            }
        }
    }
}
