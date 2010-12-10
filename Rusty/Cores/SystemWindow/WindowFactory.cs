using System;
using System.Collections.Generic;
using System.Text;
using IronAHK.Rusty.Windows.SystemWindows;
using IronAHK.Rusty.Linux;

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
