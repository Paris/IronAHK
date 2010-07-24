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
        const bool debug =
#if DEBUG
 true
#else
 false
#endif
;

        const int ExitSuccess = 0;
        static bool gui;

        [STAThread]
        static int Main(string[] args)
        {
            //args = new string[] { string.Format("..{0}..{0}..{0}Tests{0}Code{0}isolated.ahk", Path.DirectorySeparatorChar) };
            Start(ref args);

            #region Constants

            const int ExitInvalidFunction = 1;
            const int ExitFileNotFound = 2;

            const string ErrorNoPaths = "No source path specified.";
            const string ErrorSourceFileNotFound = "Specified file path not found.";
            const string ErrorDuplicatePaths = "Duplicate paths to source file.";
            const string ErrorOutputUnspecified = "No output path specified.";
            const string ErrorUnrecognisedSwitch = "Unrecognised switch.";
            const string ErrorErrorsOccurred = "The following errors occurred:";
            const string ErrorCompilationFailed = "Compilation failed.";

            #endregion

            #region Command line

            var asm = Assembly.GetExecutingAssembly();
            string self = asm.Location;
            var name = typeof(Program).Namespace;
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
                        if (Path.DirectorySeparatorChar == option[0])
                            goto default;
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

                                if (exe == "!")
                                {
                                    exe = null;

                                    var saveas = new SaveFileDialog
                                                     {
                                        AddExtension = true,
                                        AutoUpgradeEnabled = true,
                                        CheckPathExists = true,
                                        DefaultExt = "exe",
                                        Filter = "Application (*.exe)|*.exe",
                                        OverwritePrompt = true,
                                        ValidateNames = true,
                                    };

                                    if (!string.IsNullOrEmpty(script))
                                        saveas.FileName = Path.GetFileNameWithoutExtension(script) + ".exe";

                                    if (saveas.ShowDialog() == DialogResult.OK)
                                        exe = saveas.FileName;
                                }
                            }
                            if (exe == null)
                                return Message(ErrorOutputUnspecified, ExitInvalidFunction);
                            break;

                        case "VERSION":
                        case "V":
                            string vers = string.Format("{0} {1}",
                                name, Assembly.GetExecutingAssembly().GetName().Version);
                            return Message(vers, ExitSuccess);

                        case "HELP":
                        case "?":
                            string txt = string.Format("Usage: {0} [{1}gui] [{1}out filename] <source file>",
                                Path.GetFileName(self), Environment.OSVersion.Platform == PlatformID.Unix ? "--" : "/");
                            return Message(txt, ExitSuccess);

                        case "ABOUT":
                            var license = asm.GetManifestResourceStream(typeof(Program).Namespace + ".license.txt");
                            return Message(new StreamReader(license).ReadToEnd(), ExitSuccess);

                        default:
                            return Message(ErrorUnrecognisedSwitch, ExitInvalidFunction);
                    }
                }
            }

            if (script == null)
            {
                if (gui)
                {
                    var docs = new[] 
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
            var options = new CompilerParameters();
            bool reflect = exe == null;
            var exit = ExitSuccess;

            if (!reflect)
            {
                if (File.Exists(exe))
                    File.Delete(exe);
                options.OutputAssembly = exe;
            }

            options.GenerateExecutable = !reflect;
            options.GenerateInMemory = reflect;

            var results = ahk.CompileAssemblyFromFile(options, script);
            bool failed = false;

            var warnings = new StringWriter();
            warnings.WriteLine(ErrorErrorsOccurred);
            warnings.WriteLine();

            foreach (CompilerError error in results.Errors)
            {
                string file = string.IsNullOrEmpty(error.FileName) ? script : error.FileName;

                if (!error.IsWarning)
                {
                    failed = true;
                    warnings.WriteLine("{0}:{1} - {2}", Path.GetFileName(file), error.Line, error.ErrorText);
                }

                Console.Error.WriteLine("{0} ({1}): ==> {2}", file, error.Line, error.ErrorText);
            }

            if (failed)
                return Message(gui ? warnings.ToString() : ErrorCompilationFailed, ExitInvalidFunction);

#if DEBUG
            reflect = true;
#endif

            if (reflect)
            {
                GC.Collect();
                GC.WaitForPendingFinalizers();

                try
                {
                    if (results.CompiledAssembly == null)
                        throw new Exception(ErrorCompilationFailed);
                    results.CompiledAssembly.EntryPoint.Invoke(null, null);
                }
                catch (Exception e)
                {
                    if (e is TargetInvocationException)
                        e = e.InnerException;

                    var error = new StringWriter();
                    error.WriteLine("{0}: {1}", e.GetType().Name, e.Message);
                    error.WriteLine();
                    error.WriteLine(e.StackTrace);

#pragma warning disable 162
                    if (debug)
                    {
                        Console.WriteLine();
                        Console.Write(error.ToString());
                    }
                    else
                        exit = Message("Could not execute: " + e.Message, ExitInvalidFunction);
#pragma warning restore

                    var trace = Environment.GetEnvironmentVariable(name.ToUpperInvariant() + "_TRACE");

                    try
                    {
                        if (!string.IsNullOrEmpty(trace) && Directory.Exists(Path.GetDirectoryName(trace)))
                        {
                            if (File.Exists(trace))
                                File.Delete(trace);

                            File.WriteAllText(trace, error.ToString());
                        }
                    }
                    catch { }
                }
            }

            #endregion

            Cleanup();

            return exit;
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
