using System;
using System.CodeDom.Compiler;
using System.IO;
using System.Text;
using IronAHK.Rusty;
using IronAHK.Scripting;
using NUnit.Framework;

namespace IronAHK.Tests
{
    [TestFixture, Category("Scripting")]
    public partial class Scripting
    {
        string path = string.Format("..{0}..{0}Code{0}", Path.DirectorySeparatorChar);
        const string ext = ".ahk";

        bool TestScript(string source)
        {
            return HasPassed(RunScript(string.Concat(path, source, ext), true));
        }

        bool ValidateScript(string source)
        {
            RunScript(string.Concat(path, source, ext), false);
            return true;
        }

        bool HasPassed(string output)
        {
            if (string.IsNullOrEmpty(output))
                return false;

            const string pass = "pass";
            foreach (var remove in new[] { pass, " ", "\n" })
                output = output.Replace(remove, string.Empty);

            return output.Length == 0;
        }

        string RunScript(string source, bool execute)
        {
            var provider = new IACodeProvider();
            var options = new IACompilerParameters { GenerateExecutable = false, GenerateInMemory = true };
            options.ReferencedAssemblies.Add(typeof(Core).Namespace + ".dll");
            var results = provider.CompileAssemblyFromFile(options, source);

            var buffer = new StringBuilder();
            var writer = new StringWriter(buffer);
            Console.SetOut(writer);

            if (execute)
                results.CompiledAssembly.EntryPoint.Invoke(null, null);

            writer.Flush();
            string output = buffer.ToString();
            var stdout = new StreamWriter(Console.OpenStandardOutput());
            stdout.AutoFlush = true;
            Console.SetOut(stdout);

            return output;
        }
    }
}
