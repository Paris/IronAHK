using System;
using System.CodeDom.Compiler;
using System.IO;
using System.Text;
using IronAHK.Scripting;
using NUnit.Framework;

namespace Tests
{
    [TestFixture]
    public partial class Scripting
    {
        [Test]
        public void RunScripts()
        {
            string path = string.Format("..{0}..{0}Scripting{0}Code", Path.DirectorySeparatorChar.ToString());

            foreach (string file in Directory.GetFiles(path, "*.ia"))
            {
                string name = Path.GetFileNameWithoutExtension(file);
                var provider = new IACodeProvider();

                var options = new CompilerParameters();
                options.GenerateExecutable = true;
                options.GenerateInMemory = false;
                options.ReferencedAssemblies.Add(typeof(IronAHK.Rusty.Core).Namespace + ".dll");
                options.OutputAssembly = Path.GetTempFileName() + ".exe";

                provider.CompileAssemblyFromFile(options, file);

                bool exists = File.Exists(options.OutputAssembly);
                Assert.IsTrue(exists, name + " assembly");

                if (exists)
                {
                    var buffer = new StringBuilder();
                    var writer = new StringWriter(buffer);
                    Console.SetOut(writer);

                    AppDomain.CurrentDomain.ExecuteAssembly(options.OutputAssembly);

                    try { File.Delete(options.OutputAssembly); }
                    catch (UnauthorizedAccessException) { }

                    writer.Flush();
                    string output = buffer.ToString();
                    Assert.AreEqual("pass", output, name);

                    var stdout = new StreamWriter(Console.OpenStandardOutput());
                    stdout.AutoFlush = true;
                    Console.SetOut(stdout);
                }
            }
        }
    }
}
