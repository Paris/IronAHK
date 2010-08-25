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

            bool all = false, docs = false, zip = false, app = false, msi = false, ptb = false;
            string cmd = args.Length > 0 ? args[0].Trim().ToUpperInvariant() : string.Empty;

            switch (cmd)
            {
                case "HELP":
                    Console.WriteLine("{0} [all|docs|zip|app|msi|std]", Path.GetFileName(Assembly.GetEntryAssembly().Location));
                    break;

                case "DOCS": docs = true; break;
                case "ZIP": zip = true; break;
                case "APP": app = true; break;
                case "MSI": msi = true; break;
                case "STD": ptb = true; break;

                case "ALL":
                default:
                    all = true;
                    break;
            }

            if (all)
                docs = zip = app = msi = ptb = true;

            if (docs)
                TransformDocs();

            if (zip)
                PackageZip();

            if (app)
                AppBundle();

            if (Environment.OSVersion.Platform == PlatformID.Win32NT)
            {
                if (msi)
                    BuildMsi();

                if (ptb)
                    MergePortable();
            }

            Cleanup();
        }

        static void Zip(string output, string paths, string working)
        {
            if (File.Exists(output))
                File.Delete(output);

            var sz = new Process { StartInfo = new ProcessStartInfo { FileName = "7za", UseShellExecute = false } };

            if (Environment.OSVersion.Platform == PlatformID.Win32NT)
            {
                string exe = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles), "7-Zip\\7z.exe");

                if (File.Exists(exe))
                    sz.StartInfo.FileName = exe;
            }

            sz.StartInfo.WorkingDirectory = working;
            sz.StartInfo.Arguments = string.Format("a \"{0}\" \"{1}\" -mx=9", output, paths);

            try
            {
                sz.Start();
                sz.WaitForExit();
            }
            catch (Win32Exception e)
            {
                Console.Error.WriteLine(ExecFailed, sz.StartInfo.FileName, e.Message);
            }
        }

        [Conditional("DEBUG")]
        static void Cleanup()
        {

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
