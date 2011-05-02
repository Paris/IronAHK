using System;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Reflection;

[assembly: CLSCompliant(true)]

namespace IronAHK.Setup
{
    static partial class Program
    {
        const string ExecFailed = "Cannot execute \"{0}\": {1}";

        static void Main(string[] args)
        {
            Environment.CurrentDirectory = WorkingDir;

            bool all = false, docs = false, zip = false, app = false, ptb = false;
            string cmd = args.Length > 0 ? args[0].Trim().ToUpperInvariant() : string.Empty;

            switch (cmd)
            {
                case "HELP":
                    Console.WriteLine("{0} [all|docs|zip|app|std]", Path.GetFileName(Assembly.GetEntryAssembly().Location));
                    break;

                case "DOCS": docs = true; break;
                case "ZIP": zip = true; break;
                case "APP": app = true; break;
                case "STD": ptb = true; break;

                case "ALL":
                default:
                    all = true;
                    break;
            }

            if (all)
                docs = zip = app = ptb = true;

            if (docs)
                TransformDocs();

            if (zip)
                PackageZip();

            if (app)
                AppBundle();

            if (Environment.OSVersion.Platform == PlatformID.Win32NT)
            {
                if (ptb)
                    MergePortable();
            }

            Metadata();
        }

        static void Metadata()
        {
            var path = Path.Combine(Output, "version.txt");

            if (File.Exists(path))
                File.Delete(path);

            File.WriteAllText(path, Version);
        }

        static string Name
        {
            get { return typeof(Program).Namespace.Split(new[] { '.' }, 2)[0]; }
        }

        static string Version
        {
            get { return IronAHK.Program.Version.ToString(); }
        }

        static string WorkingDir
        {
            get { return Path.GetFullPath(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)); }
        }

        static string Output
        {
            get
            {
                string path = string.Format("..{0}..{0}..{0}bin", Path.DirectorySeparatorChar);
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
