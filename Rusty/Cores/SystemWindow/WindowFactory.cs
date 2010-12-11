using System;
using System.Collections.Generic;
using System.Text;
using IronAHK.Rusty.Linux;
using IronAHK.Rusty.Windows;

namespace IronAHK.Rusty.Cores.SystemWindow
{

    public static class WindowFactory
    {
        /// <summary>
        /// Creates a WindowManager for the current Platform
        /// </summary>
        public static WindowManager CreateWindowManager() {
            if(Environment.OSVersion.Platform == PlatformID.Win32NT)
                return new WindowManagerWindows();
            else
                return new WindowManagerLinux();
        }

    }
}
