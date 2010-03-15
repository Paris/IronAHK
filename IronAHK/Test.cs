using System;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;

namespace IronAHK
{
    static partial class Program
    {
        [DllImport("Kernel32.dll")]
        static extern bool AllocConsole();

        [Conditional("DEBUG")]
        static void Start(ref string[] args)
        {
            if (Environment.OSVersion.Platform == PlatformID.Win32NT)
                AllocConsole();

            Sandbox();

            /* 
             * Note: to change the default test script do not edit any variables here, instead
             * create a file called select.txt in the tests directory with the file name (including extension)
             * of the script to launch (i.e. "expressions.ahk" or "vanilla-MAIN.ahk").
             */

            const string output = "test.exe";
            const string test = "pass.ahk";

            string directory = string.Format("..{0}..{0}..{0}Tests{0}Code", Path.DirectorySeparatorChar);
            string select = Path.Combine(directory, "select.txt");
            string source = null;

            if (File.Exists(select))
            {
                var reader = new StreamReader(select);
                const char comment = '#';
                string line;

                while ((line = reader.ReadLine()) != null)
                {
                    line = line.Trim();
                    int z = line.IndexOf(comment);

                    if (line.Length == 0 || z == 0)
                        continue;

                    if (z != -1)
                        line = line.Substring(0, z).Trim();
                    
                    string name = Path.Combine(directory, line.Trim());

                    if (File.Exists(name))
                        source = name;

                    break;
                }
            }

            if (source == null)
                source = Path.Combine(directory, test);

            File.Delete(output);

            if (args == null || args.Length == 0)
                args = new string[] { "--out", output, source };
        }

        [Conditional("DEBUG")]
        static void Cleanup()
        {
            Console.Read();
        }

        [Conditional("DEBUG")]
        static void Sandbox()
        {

        }
    }
}
