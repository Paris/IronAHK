using System;
using System.CodeDom.Compiler;
using System.IO;
using System.Reflection;
using IronAHK.Scripting;

[assembly: CLSCompliant(true)]

namespace IronAHK
{
    static partial class Program
    {
        static int Main(string[] args)
        {
            //args = new string[] { string.Format("..{0}..{0}..{0}Tests{0}Scripting{0}Code{0}isolated.ahk", Path.DirectorySeparatorChar) };
            Start(ref args);

            #region Constants

            const int ExitSuccess = 0;

            const int ExitInvalidFunction = 1;
            const string ErrorInvalidFunction = "Incorrect function.";

            const int ExitFileNotFound = 2;
            const string ErrorFileNotFound = "The system cannot find the file specified.";

            #endregion

            #region Command line

            string self = Assembly.GetExecutingAssembly().Location;

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
                        if (Path.DirectorySeparatorChar == '/')
                            goto case '-';
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
                                Path.GetFileName(self), Environment.OSVersion.Platform == PlatformID.Unix ? "--" : "/");
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
                return script == null ? ExitInvalidFunction : ExitFileNotFound;
            }

            #endregion

            #region Compile

            var ahk = new IACodeProvider { UseCSharpCompiler = csc };

            self = Path.GetDirectoryName(Path.GetFullPath(self));
            const string ext = ".dll";

            var options = new CompilerParameters();
            options.ReferencedAssemblies.Add(Path.Combine(self, typeof(IronAHK.Rusty.Core).Namespace + ext));
            options.ReferencedAssemblies.Add(Path.Combine(self, typeof(IACodeProvider).Namespace + ext));

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

#if DEBUG
            reflect = true;
#endif

            if (reflect)
            {
                try
                {
                    if (results.CompiledAssembly == null)
                        throw new Exception("compilation failed");
                    results.CompiledAssembly.EntryPoint.Invoke(null, null);
                }
#if DEBUG
                catch (TargetInvocationException e)
                {
                    Console.Error.WriteLine(e.ToString());
                    throw;
                }
#endif
#if !DEBUG
                catch (Exception e) { Console.Error.WriteLine("Could not execute: {0}", e.Message); }
#endif
                finally { }
            }

            #endregion

            Cleanup();

            return ExitSuccess;
        }
    }
}
