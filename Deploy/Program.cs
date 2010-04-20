using System;
using System.Diagnostics;
using System.IO;

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

        static string Output
        {
            get
            {
                string path = string.Format("..{0}..{0}..{0}bin", Path.DirectorySeparatorChar.ToString());
                path = Path.GetFullPath(path);

                if (!Directory.Exists(path))
                    Directory.CreateDirectory(path);

                const string gitignore = ".gitignore";
                string git = Path.Combine(path, gitignore);

                if (!File.Exists(git))
                    File.WriteAllText(git, "*");

                return path;
            }
        }
    }
}
