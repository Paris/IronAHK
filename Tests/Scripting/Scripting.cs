using System;
using System.CodeDom.Compiler;
using System.IO;
using System.Text;
using IronAHK.Scripting;
using NUnit.Framework;

namespace IronAHK.Tests
{
    [TestFixture]
    public class Scripting
    {
        [Test]
        public void RunScripts()
        {
            string path = string.Format("..{0}..{0}Scripting{0}Code", Path.DirectorySeparatorChar.ToString());
            string line;
            var sources = new StreamReader(Path.Combine(path, "order.txt"));

            while ((line = sources.ReadLine()) != null)
            {
                if (line == "!")
                    break;

                string file = Path.Combine(path, line + ".ia");
                var provider = new IACodeProvider();
                var options = new CompilerParameters { GenerateExecutable = false, GenerateInMemory = true, };
                options.ReferencedAssemblies.Add(typeof(IronAHK.Rusty.Core).Namespace + ".dll");
                var results = provider.CompileAssemblyFromFile(options, file);

                var buffer = new StringBuilder();
                var writer = new StringWriter(buffer);
                Console.SetOut(writer);

                results.CompiledAssembly.EntryPoint.Invoke(null, null);

                writer.Flush();
                string output = buffer.ToString();
                var stdout = new StreamWriter(Console.OpenStandardOutput());
                stdout.AutoFlush = true;
                Console.SetOut(stdout);

                Assert.IsNotEmpty(output, line + " results");

                const string pass = "pass";
                foreach (string remove in new string[] { pass, " ", "\n" })
                    output = output.Replace(remove, string.Empty);

                Assert.IsEmpty(output, line + " " + pass);
            }
        }
    }
}
