using System;
using System.CodeDom.Compiler;
using System.IO;
using IronAHK.Scripting;

[assembly: CLSCompliant(true)]

namespace IronAHK
{
    static class Program
    {
        static int Main(string[] args)
        {
            #region Test run

#if DEBUG
            const string test = "if";
            const string output = ""; // "/out test.exe";
            const string extra = "";
            const string cmd = extra + " " + output + " ..{0}..{0}..{0}Tests{0}Scripting{0}Code{0}" + test + ".ia";
          
            args = string.Format(cmd, Path.DirectorySeparatorChar.ToString()).Split(new char[] { ' ', '\t' }, StringSplitOptions.RemoveEmptyEntries);
#endif

            #endregion

            #region Constants

            const int ExitSuccess = 0;

            const int ExitInvalidFunction = 1;
            const string ErrorInvalidFunction = "Incorrect function.";

            const int ExitFileNotFound = 2;
            const string ErrorFileNotFound = "The system cannot find the file specified.";

            #endregion

            #region Command line

            string script = null;
            string exe = null;
            bool csc = false;

            for (int i = 0; i < args.Length; i++)
            {
                string option = args[i];
                bool file = false;

                switch (option[0])
                {
                    case '/':
                        option = option.Substring(1);
                        break;

                    case '-':
                        int n = 1;
                        if (option.Length > 1 && option[1] == option[0])
                            n++;
                        option = option.Substring(n);
                        break;

                    default:
                        file = true;
                        break;
                }

                if (file)
                {
                    if (script == null)
                        script = option;
                    else
                    {
                        Console.Error.WriteLine(ErrorFileNotFound);
                        return ExitInvalidFunction;
                    }
                }
                else
                {
                    switch (option.ToUpperInvariant())
                    {
                        case "OUT":
                            int n = i + 1;
                            if (n < args.Length)
                            {
                                exe = args[n];
                                i = n;
                            }
                            else
                            {
                                Console.Error.WriteLine(ErrorFileNotFound);
                                return ExitInvalidFunction;
                            }
                            break;

                        case "HELP":
                        case "?":
                            Console.WriteLine("Usage: {0} [{1}out filename] <source file>",
                                GetExecutingFile(true), Environment.OSVersion.Platform == PlatformID.Unix ? "--" : "/");
                            return ExitSuccess;

                        default:
                            Console.Error.WriteLine(ErrorInvalidFunction);
                            return ExitInvalidFunction;
                    }
                }
            }

            if (script == null || !File.Exists(script))
            {
                Console.Error.WriteLine(ErrorFileNotFound);
                return ExitInvalidFunction;
            }

            #endregion

            #region Compile

            var ahk = new IACodeProvider { UseCSharpCompiler = csc };

            var options = new CompilerParameters();
            options.ReferencedAssemblies.Add(typeof(IronAHK.Rusty.Core).Namespace + ".dll");

            bool reflect = exe == null;

            if (!reflect)
            {
                if (File.Exists(exe))
                    File.Delete(exe);
                options.OutputAssembly = exe;
            }

            options.GenerateExecutable = !reflect;
            options.GenerateInMemory = reflect;

            var results = ahk.CompileAssemblyFromFile(options, script);

            foreach (CompilerError error in results.Errors)
                Console.Error.WriteLine("{0} ({1}): ==> {2}", error.FileName, error.Line.ToString(), error.ErrorText);

            #endregion

            #region Run

            if (!options.GenerateExecutable)
            {
                try { File.Delete(options.OutputAssembly); } // TODO: safe delete temp executable
                catch (UnauthorizedAccessException) { Console.Error.WriteLine("Unable to delete temporary assembly"); }
            }

            if (reflect)
                results.CompiledAssembly.EntryPoint.Invoke(null, null);

#if DEBUG
            Console.Read();
#endif

            #endregion

            return ExitSuccess;
        }

        #region Helpers

        static string GetExecutingFile(bool quote)
        {
            string cli = Environment.CommandLine;
            const char str = '"';
            bool esc = cli[0] == str;
            int z;

            for (int i = 1; i < cli.Length; i++)
            {
                if (cli[i] == str)
                    esc = !esc;
                if (!esc && char.IsWhiteSpace(cli, i))
                {
                    z = cli[0] == str ? 1 : 0;
                    cli = cli.Substring(z, i - z * 2);
                }
            }

            z = cli.LastIndexOf(Path.DirectorySeparatorChar);
            if (z != -1 && z + 1 < cli.Length)
                cli = cli.Substring(z + 1);

            if (quote)
            {
                foreach (char letter in cli)
                {
                    if (char.IsWhiteSpace(letter))
                    {
                        cli = string.Concat(str, cli, str);
                        break;
                    }
                }
            }

            return cli;
        }

        #endregion
    }
}
