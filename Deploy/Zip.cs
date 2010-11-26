using System;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using Microsoft.Win32;

namespace IronAHK.Setup
{
    partial class Program
    {
        static void Zip(string output, string paths, string working)
        {
            if (File.Exists(output))
                File.Delete(output);

            SevenZip(output, paths, working);

            if (!File.Exists(output))
                GnuZip(output, paths, working);

            if (File.Exists(output))
                AdvZip(output);
        }

        static void AdvZip(string output)
        {
            using (var adv = new Process())
            {
                adv.StartInfo = new ProcessStartInfo { FileName = "advzip", UseShellExecute = true, Arguments = string.Format("-z -4 \"{0}\"", output) };

                try
                {
                    adv.Start();
                    adv.WaitForExit();
                }
                catch (Win32Exception)
                {

                }
            }
        }

        static void GnuZip(string output, string paths, string working)
        {
            using (var zip = new Process())
            {
                zip.StartInfo = new ProcessStartInfo
                {
                    FileName = "zip",
                    UseShellExecute = true,
                    Arguments = string.Format("-9 -r -X \"{0}\" \"{1}\"", output, paths),
                    WorkingDirectory = working
                };

                try
                {
                    zip.Start();
                    zip.WaitForExit();
                }
                catch (Win32Exception e)
                {
                    Console.Error.WriteLine(ExecFailed, zip.StartInfo.FileName, e.Message);
                }
            }
        }

        static void SevenZip(string output, string paths, string working)
        {
            using (var sz = new Process())
            {
                sz.StartInfo = new ProcessStartInfo
                {
                    FileName = "7za",
                    UseShellExecute = true,
                    Arguments = string.Format("a \"{0}\" \"{1}\" -mx=9", output, paths),
                    WorkingDirectory = working
                };

                if (Environment.OSVersion.Platform == PlatformID.Win32NT)
                {
                    var install = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles), "7-Zip");
                    var dir = (string)Registry.CurrentUser.OpenSubKey("SOFTWARE\\7-Zip").GetValue("Path", install);
                    var exe = Path.Combine(dir, "7z.exe");

                    if (File.Exists(exe))
                    {
                        sz.StartInfo.FileName = exe;
                        sz.StartInfo.UseShellExecute = false;
                    }
                }

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
        }
    }
}
