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
            BuildMsi();

        }

        [Conditional("DEBUG")]
        static void Cleanup()
        {
            Console.Read();
        }
    }
}
