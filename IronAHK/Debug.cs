using System;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;

namespace IronAHK
{
    partial class Program
    {
        const bool debug =
#if DEBUG
 true
#else
 false
#endif
;

        [Conditional("DEBUG"), DllImport("kernel32.dll")]
        static extern void AllocConsole();

        [Conditional("DEBUG")]
        static void Start(ref string[] args)
        {
            if (Environment.OSVersion.Platform == PlatformID.Win32NT)
                AllocConsole();

            const string source = "..{0}..{0}..{0}Tests{0}Code{0}isolated.ahk";
            const string binary = "test.exe";

            args = string.Format(source + " --out " + binary, Path.DirectorySeparatorChar).Split(' ');
        }
    }
}
