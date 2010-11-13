using System;
using System.CodeDom.Compiler;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Windows.Forms;
using IronAHK.Scripting;

[assembly: CLSCompliant(true)]
[assembly: InternalsVisibleTo("Setup, PublicKey=00240000048000009400000006020000002400005253413100040000010001009734282d68c536699358b36ad5636aa2d7fbd95ead0f6dc6c0708f19d400740e3aa4a0b5e6e779e5196bbefa6f12f19240a0f1a4daa3a4c8a59bf0067730915f9fcf4b3ee3844b290d39be63eb444f030ecd34570b3d784f307f10efc680ec37701e7f0008b0a8de2c6249c4896bf0cf1aa3cfadd434c40dcde17a25874cebcc")]

namespace IronAHK
{
    static partial class Program
    {
        const int ExitSuccess = 0;
        static bool gui;

        [STAThread]
        static int Main(string[] args)
        {
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

                                    using (var saveas = new SaveFileDialog())
                                    {
                                        saveas.AddExtension = true;
                                        saveas.AutoUpgradeEnabled = true;
                                        saveas.CheckPathExists = true;
                                        saveas.DefaultExt = "exe";
                                        saveas.Filter = "Application (*.exe)|*.exe";
                                        saveas.OverwritePrompt = true;
                                        saveas.ValidateNames = true;

                                        if (!string.IsNullOrEmpty(script))
                                            saveas.FileName = Path.GetFileNameWithoutExtension(script) + ".exe";

                                        if (saveas.ShowDialog() == DialogResult.OK)
                                            exe = saveas.FileName;
                                    }
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

            #region Source

            CompilerResults results;
            bool reflect = exe == null;
            var exit = ExitSuccess;

            using (var ahk = new IACodeProvider())
            {
                self = Path.GetDirectoryName(Path.GetFullPath(self));
                var options = new IACompilerParameters { MergeFallbackToLink = true };

                if (!reflect)
                {
                    if (File.Exists(exe))
                        File.Delete(exe);
                    options.OutputAssembly = exe;
                    options.Merge = !debug;
                }

                options.GenerateExecutable = !reflect;
                options.GenerateInMemory = reflect;

                results = ahk.CompileAssemblyFromFile(options, script);
            }

            #endregion

            #region Warnings

            var failed = false;
            string failure;

            using (var warnings = new StringWriter())
            {
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

                failure = warnings.ToString();
            }

            if (failed)
                return Message(gui ? failure : ErrorCompilationFailed, ExitInvalidFunction);

            #endregion

            #region Execute

            if (reflect || debug)
            {
                GC.Collect();
                GC.WaitForPendingFinalizers();

                try
                {
                    if (results.CompiledAssembly == null)
                        throw new Exception(ErrorCompilationFailed);
                    Environment.SetEnvironmentVariable("SCRIPT", script);
                    results.CompiledAssembly.EntryPoint.Invoke(null, null);
                }
                #region Error
                catch (Exception e)
                {
                    if (e is TargetInvocationException)
                        e = e.InnerException;

                    string msg;

                    using (var error = new StringWriter())
                    {
                        error.WriteLine("{0}: {1}", e.GetType().Name, e.Message);
                        error.WriteLine();
                        error.WriteLine(e.StackTrace);
                        msg = error.ToString();
                    }

#pragma warning disable 162
                    if (debug)
                    {
                        Console.WriteLine();
                        Console.Write(msg);
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

                            File.WriteAllText(trace, msg);
                        }
                    }
                    catch { }

                    if (debug)
                        Console.Read();
                }
                #endregion
            }

            #endregion

            #endregion

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

        internal static Version Version
        {
            get { return Assembly.GetExecutingAssembly().GetName().Version; }
        }
    }
}
