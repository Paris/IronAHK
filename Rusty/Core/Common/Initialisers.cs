using System;
using System.Collections.Generic;

namespace IronAHK.Rusty
{
    partial class Core
    {
        static void InitKeyboardHook()
        {
            if (hotkeys == null)
                hotkeys = new Dictionary<string, HotkeyDefinition>();

            if (keyboardHook != null)
                return;

            if (Environment.OSVersion.Platform == PlatformID.Win32NT)
                keyboardHook = new Windows.KeyboardHook();
            else
                keyboardHook = new Linux.KeyboardHook();
        }
    }
}
