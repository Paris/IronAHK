using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;

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

        static string Name
        {
            get { return typeof(Program).Namespace.Split(new[] { '.' }, 2)[0]; }
        }

        static string Version
        {
            get { return File.ReadAllText(Path.Combine(WorkingDir, "version.txt")).Trim(); }
        }

        static string WorkingDir
        {
            get { return Path.GetFullPath(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)); }
        }

        static string Output
        {
            get
            {
                string path = string.Format("..{0}..{0}..{0}bin", Path.DirectorySeparatorChar.ToString());
                path = Path.GetFullPath(Path.Combine(WorkingDir, path));

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
