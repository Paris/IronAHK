using System;

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

#if DEBUG
            Console.Read();
#endif
        }
    }
}
