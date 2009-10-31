using System;
using System.CodeDom.Compiler;
using System.IO;
using IronAHK.Scripting;

[assembly: CLSCompliant(true)]

namespace IronAHK
{
    class Program
    {
        static int Main(string[] args)
        {
            #region Setup

            const int ExitSuccess = 0;
            const int ExitError = 1;

#if DEBUG
            const string test = "CompoundingAssignment";
            const string opt = ""; // "/csc";
            const string cmd = opt + " /out test.exe ..{0}..{0}..{0}Scripting{0}Tests{0}" + test + ".ia";
            args = string.Format(cmd, Path.DirectorySeparatorChar.ToString()).Split(new char[] { ' ', '\t' }, StringSplitOptions.RemoveEmptyEntries);
#endif

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
                        Console.Error.WriteLine("Specify one file name.");
                        return ExitError;
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
                                Console.Error.WriteLine("No output file specified.");
                                return ExitError;
                            }
                            break;

                        case "CSC":
                            csc = true;
                            break;

                        case "HELP":
                        case "?":
                            Console.WriteLine("Usage: {0} [{1}out filename] <source file>",
                                GetExecutingFile(true), Environment.OSVersion.Platform == PlatformID.Unix ? "--" : "/");
                            return ExitSuccess;

                        default:
                            Console.Error.WriteLine("Unrecognised option.");
                            return ExitError;
                    }
                }
            }

            if (script == null)
            {
                Console.Error.WriteLine("No input file specified.");
                return ExitError;
            }
            else if (!File.Exists(script))
            {
                Console.Error.WriteLine("Input file not found.");
                return ExitError;
            }

            #endregion

            #region Compile

            var ahk = new IACodeProvider();
            ahk.UseCSharpCompiler = csc;

            CompilerParameters options = new CompilerParameters();

            options.ReferencedAssemblies.Add(Path.GetFullPath(typeof(IronAHK.Rusty.Core).Namespace + ".dll"));

            if (exe != null)
            {
                if (File.Exists(exe))
                    File.Delete(exe);
                options.OutputAssembly = exe;
            }

            CompilerResults results = ahk.CompileAssemblyFromFile(options, script);

            if (results.Errors.HasErrors)
            {
                foreach (CompilerError error in results.Errors)
                {
                    Console.Error.WriteLine("{0} ({1}): ==> {2}", error.FileName, error.Line.ToString(), error.ErrorText);
                }
            }

            #endregion

#if DEBUG
            if (!string.IsNullOrEmpty(options.OutputAssembly))
            {
                AppDomain domain = AppDomain.CreateDomain(Path.GetFileNameWithoutExtension(options.OutputAssembly));
                domain.ExecuteAssembly(options.OutputAssembly);
            }
#endif

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
