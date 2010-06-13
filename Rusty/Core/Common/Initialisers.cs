using System;
using System.Collections.Generic;

namespace IronAHK.Rusty
{
    partial class Core
    {
        static void InitVariables()
        {
            if (variables == null)
                variables = new Dictionary<string, object>();
        }

        static void InitKeyboardHook()
        {
            if (hotkeys == null)
                hotkeys = new Dictionary<string, HotkeyDefinition>();

            if (hotstrings == null)
                hotstrings = new Dictionary<string, HotstringDefinition>();

            if (keyboardHook != null)
                return;

            if (Environment.OSVersion.Platform == PlatformID.Win32NT)
                keyboardHook = new Windows.KeyboardHook();
            else
                keyboardHook = new Linux.KeyboardHook();
        }
    }
}
