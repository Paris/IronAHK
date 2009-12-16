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

            const string test = "assign";
            const string output = ""; // "/out test.exe";
            const string extra = "";
            const string cmd = extra + " " + output + " ..{0}..{0}..{0}Tests{0}Scripting{0}Code{0}" + test + ".ia";

            args = string.Format(cmd, Path.DirectorySeparatorChar.ToString()).Split(new char[] { ' ', '\t' }, StringSplitOptions.RemoveEmptyEntries);
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
