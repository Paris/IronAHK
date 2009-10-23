using System;
using System.CodeDom;
using System.CodeDom.Compiler;
using System.IO;

[assembly: CLSCompliant(true)]

namespace IronAHK.Scripting
{
    public sealed partial class IACodeProvider : CodeDomProvider
    {
        Generator generator;

        public IACodeProvider()
        {
            generator = new Generator();
        }

        public override string FileExtension
        {
            get
            {
#if LEGACY
                return "ahk";
#endif
#if !LEGACY
                return "ia";
#endif
            }
        }

        #region Generator

        [Obsolete()]
        public override ICodeGenerator CreateGenerator()
        {
            return new Generator();
        }

        #endregion

        #region Compiler

        [Obsolete()]
        public override ICodeCompiler CreateCompiler()
        {
            throw new NotImplementedException();
        }

        public override CompilerResults CompileAssemblyFromFile(CompilerParameters options, params string[] fileNames)
        {
            var errors = new CompilerErrorCollection();
            var syntax = new Parser();

            foreach (string file in fileNames)
            {
                try
                {
                    syntax.Parse(new StreamReader(file));
                }
#if !DEBUG
                catch (Exception e)
                {
                    throw e;
                    errors.Add(new CompilerError(file, 0, 0, e.GetHashCode().ToString(), e.ToString()));
                }
#endif
                finally { }
            }

            var results = CompileAssemblyFromDom(options, syntax.CompileUnit);
            results.Errors.AddRange(errors);

            return results;
        }

        public override CompilerResults CompileAssemblyFromSource(CompilerParameters options, params string[] sources)
        {
            string[] fileNames = new string[sources.Length];
            var temp = new TempFileCollection();
            var errors = new CompilerErrorCollection();

            for (int i = 0; i < sources.Length; i++)
            {
                try
                {
                    string file = Path.GetTempFileName();
                    File.WriteAllText(file, sources[i]);
                    temp.AddFile(file, false);
                    fileNames[i] = file;
                }
#if !DEBUG
                catch (Exception e)
                {
                    errors.Add(new CompilerError(string.Empty, 0, 0, e.GetHashCode().ToString(), e.Message));
                }
#endif
                finally { }
            }

            var results = CompileAssemblyFromFile(options, fileNames);
            results.Errors.AddRange(errors);
            results.TempFiles = temp;
            temp.Delete();

            return results;
        }

        public override CompilerResults CompileAssemblyFromDom(CompilerParameters options, params CodeCompileUnit[] compilationUnits)
        {
            Compiler compiler = new Compiler();
            CompilerResults results = compiler.CompileAssemblyFromDomBatch(options, compilationUnits);
            return results;
        }
        
        #endregion
    }
}
