using System;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using IronAHK.Rusty;
using IronAHK.Scripting;

namespace IronAHK.Setup
{
    partial class Program
    {
        static void MergePortable()
        {
            var ext = new string[2];
            var n = -1;

            foreach (var dll in new[] { typeof(Core), typeof(IACodeProvider) })
                ext[++n] = dll.Assembly.Location;

            var bin = Path.Combine(Output, string.Format("{0}-{1}.exe", Name, Version));

            using (var ilm = new Process())
            {
                ilm.StartInfo = new ProcessStartInfo
                {
                    FileName = "ilmerge",
                    UseShellExecute = false,
                    Arguments = string.Format("/target:winexe /closed /ndebug /v2 \"/out:{1}\" \"{0}.exe\" \"{2}\" \"{3}\"", Name, bin, ext[0], ext[1])
                };

                try
                {
                    ilm.Start();
                    ilm.WaitForExit();
                }
                catch (Win32Exception e)
                {
                    Console.Error.WriteLine(ExecFailed, ilm.StartInfo.FileName, e.Message);
                }
            }
        }
    }
}
