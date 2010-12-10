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


        static void InitKeyboardHook()
        {
            if (hotkeys == null)
                hotkeys = new Dictionary<string, HotkeyDefinition>();

            if (hotstrings == null)
                hotstrings = new Dictionary<string, HotstringDefinition>();

            if (keyboardHook != null)
                return;

            if (Environment.OSVersion.Platform == PlatformID.Win32NT)
                keyboardHook = new WindowsAPI.KeyboardHook();
            else
                keyboardHook = new LinuxAPI.KeyboardHook();
        }

        static void InitGui()
        {
            if (imageLists == null)
                imageLists = new Dictionary<long, ImageList>();
        }


        static void InitDialoges() {

            if(progressDialgos == null)
                progressDialgos = new Dictionary<int, ProgressDialog>();

            if(splashDialogs == null)
                splashDialogs = new Dictionary<int, SplashDialog>();
        }

    }
}
