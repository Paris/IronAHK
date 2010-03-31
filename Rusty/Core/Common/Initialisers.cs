using System;
using System.Collections.Generic;

namespace IronAHK.Rusty
{
    partial class Core
    {
        static void InitKeyboardHook()
        {
            if (keyboardHook != null)
                return;

            if (Environment.OSVersion.Platform == PlatformID.Win32NT)
                keyboardHook = new Windows.KeyboardHook();
            else
                keyboardHook = new Linux.KeyboardHook();

            hotkeys = new Dictionary<string, HotkeyDefinition>();
        }
    }
}
