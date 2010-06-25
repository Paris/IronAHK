using System;
using System.Windows.Forms;

namespace IronAHK.Scripting
{
    partial class Script
    {
        public static Variables Vars { get; private set; }

        static Script()
        {
            if (Vars == null)
                Vars = new Variables();

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
