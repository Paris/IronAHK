using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace IronAHK.Rusty
{
    partial class Core
    {
        static void InitVariables()
        {
            if (variables == null)
                variables = new Dictionary<string, object>();
        }

        static void InitWindowManager()
        {
            if (windowGroups == null)
                windowGroups = new Dictionary<string, Stack<WindowManager>>();

            if (windowManager != null)
                return;

            if (Environment.OSVersion.Platform == PlatformID.Win32NT)
                windowManager = new Windows.WindowManager();
            else
                windowManager = new Linux.WindowManager();
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

        static void InitGui()
        {
            if (imageLists == null)
                imageLists = new Dictionary<long, ImageList>();
        }
    }
}
