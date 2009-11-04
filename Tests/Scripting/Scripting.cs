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
                    AppDomain domain = AppDomain.CreateDomain(name);

                    var buffer = new StringBuilder();
                    var writer = new StringWriter(buffer);
                    Console.SetOut(writer);

                    domain.ExecuteAssembly(options.OutputAssembly);
                    AppDomain.Unload(domain);

                    string output = buffer.ToString();
                    Assert.AreEqual("pass", output, name);

                    var standardOutput = new StreamWriter(Console.OpenStandardOutput());
                    standardOutput.AutoFlush = true;
                    Console.SetOut(standardOutput);
                }
            }
        }
    }
}
