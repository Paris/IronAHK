using System;
using System.Diagnostics;

[assembly: CLSCompliant(true)]

namespace IronAHK.Setup
{
    static partial class Program
    {
        static void Main(string[] args)
        {
            TransformDocs();
            PackageZip();
            AppBundle();

            if (Environment.OSVersion.Platform == PlatformID.Win32NT)
                BuildMsi();

            Cleanup();
        }

        [Conditional("DEBUG")]
        static void Cleanup()
        {
            Console.Read();
        }
    }
}
