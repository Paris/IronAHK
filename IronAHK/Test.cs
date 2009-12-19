using System;
using System.Diagnostics;
using System.IO;

namespace IronAHK
{
    static partial class Program
    {
        [Conditional("DEBUG")]
        static void Start(ref string[] args)
        {
            Sandbox();

            const string test = "pass";
            const string file = "test.exe";

            string source = string.Format("..{0}..{0}..{0}Tests{0}Scripting{0}Code{0}" + test + ".ia", Path.DirectorySeparatorChar);
            File.Delete(file);

            if (args == null || args.Length == 0)
                args = new string[] { "/out", file, source };
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
