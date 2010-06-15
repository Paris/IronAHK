using System;
using System.Windows.Forms;

namespace IronAHK.Scripting
{
    partial class Script
    {
        public static void Init()
        {
            if (Environment.OSVersion.Platform == PlatformID.Unix)
                Environment.SetEnvironmentVariable("MONO_VISUAL_STYLES", "gtkplus");

            Application.EnableVisualStyles();
        }

        public static void Run()
        {
            Application.Run();
        }
    }
}
