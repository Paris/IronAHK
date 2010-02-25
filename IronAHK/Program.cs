using System;
using System.CodeDom.Compiler;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Windows.Forms;
using IronAHK.Scripting;

[assembly: CLSCompliant(true)]

namespace IronAHK
{
    static partial class Program
    {
        const int ExitSuccess = 0;
        static bool gui;

        static int Main(string[] args)
        {
            //args = new string[] { string.Format("..{0}..{0}..{0}Tests{0}Scripting{0}Code{0}isolated.ahk", Path.DirectorySeparatorChar) };
            Start(ref args);

            #region Constants

            const int ExitInvalidFunction = 1;
            const int ExitFileNotFound = 2;

            const string ErrorNoPaths = "No source path specified.";
            const string ErrorSourceFileNotFound = "Specified file path not found.";
            const string ErrorDuplicatePaths = "Duplicate paths to source file.";
            const string ErrorOutputUnspecified = "No output path specified.";
            const string ErrorUnrecognisedSwitch = "Unrecognised switch.";

            #endregion

            #region Command line

            string self = Assembly.GetExecutingAssembly().Location;

            string script = null;
            string exe = null;
            gui = Environment.OSVersion.Platform == PlatformID.Unix || Environment.OSVersion.Platform == PlatformID.MacOSX ? false : true;

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
                        Message(ErrorDuplicatePaths, ExitInvalidFunction);
                }
                else
                {
                    switch (option.ToUpperInvariant())
                    {
                        case "GUI":
                            gui = true;
                            break;

                        case "OUT":
                            int n = i + 1;
                            if (n < args.Length)
                            {
                                exe = args[n];
                                i = n;
                            }
                            else
                                return Message(ErrorOutputUnspecified, ExitInvalidFunction);
                            break;

                        case "HELP":
                        case "?":
                            string txt = string.Format("Usage: {0} [{1}gui] [{1}out filename] <source file>",
                                Path.GetFileName(self), Environment.OSVersion.Platform == PlatformID.Unix ? "--" : "/");
                            return Message(txt, ExitSuccess);

                        default:
                            return Message(ErrorUnrecognisedSwitch, ExitInvalidFunction);
                    }
                }
            }

            if (script == null)
            {
                if (gui)
                {
                    string[] docs = new[] 
                    {
                        Path.Combine(Path.GetDirectoryName(self), Path.Combine("docs", "index.html")),
                        string.Format("..{0}..{0}Site{0}docs{0}index.html", Path.DirectorySeparatorChar),
                        null
                    };

                    for (int i = 0; i < 2;i++)
                        if (File.Exists(docs[i]))
                            docs[2] = docs[i];

                    bool help = !string.IsNullOrEmpty(docs[2]);

                    string text = (args.Length > 0 || !help ? "Error: " + ErrorNoPaths + "\n\n" : string.Empty) +
                        (help ? "To get started, would you like to view the help documentation?" : string.Empty);

                    var result = MessageBox.Show(text, typeof(Program).Namespace, help ? MessageBoxButtons.YesNo : MessageBoxButtons.OK, MessageBoxIcon.Information);

                    if (help && result == DialogResult.Yes)
                        Process.Start(Path.GetFullPath(docs[2]));

                    return ExitFileNotFound;
                }
                else
                    return Message(ErrorNoPaths, ExitFileNotFound);
            }
            else if (!File.Exists(script))
                return Message(ErrorSourceFileNotFound, ExitFileNotFound);

            #endregion

            #region Compile

            var ahk = new IACodeProvider();

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

        static int Message(string text, int exit)
        {
            bool error = exit != ExitSuccess;

            if (gui)
                MessageBox.Show(text, typeof(Program).Namespace, MessageBoxButtons.OK, error ? MessageBoxIcon.Exclamation : MessageBoxIcon.Information);

            var stdout = error ? Console.Error : Console.Out;
            stdout.WriteLine(text);

            return exit;
        }
    }
}
