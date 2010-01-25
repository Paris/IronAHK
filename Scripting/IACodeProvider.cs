using System;
using System.CodeDom;
using System.CodeDom.Compiler;
using System.Diagnostics;
using System.IO;

[assembly: CLSCompliant(true)]

namespace IronAHK.Scripting
{
    public sealed partial class IACodeProvider : CodeDomProvider
    {
        #region Extras

        bool csc = false;

        public bool UseCSharpCompiler
        {
            get { return csc; }
            set { csc = value; }
        }

        public override string FileExtension
        {
            get { return "ahk"; }
        }

        #endregion

        #region Generator

        [Obsolete()]
        public override ICodeGenerator CreateGenerator()
        {
            return new Generator();
        }

        #endregion

        #region Compiler

        #region Wrappers

        [Obsolete()]
        public override ICodeCompiler CreateCompiler()
        {
            throw new NotImplementedException();
        }

        public override CompilerResults CompileAssemblyFromFile(CompilerParameters options, params string[] fileNames)
        {
            var readers = new TextReader[fileNames.Length];

            for (int i = 0; i < fileNames.Length; i++)
                readers[i] = new StreamReader(fileNames[i]);

            return CompileAssemblyFromReader(options, readers);
        }

        public override CompilerResults CompileAssemblyFromSource(CompilerParameters options, params string[] sources)
        {
            var readers = new TextReader[sources.Length];

            for (int i = 0; i < sources.Length; i++)
                readers[i] = new StringReader(sources[i]);

            return CompileAssemblyFromReader(options, readers);
        }

        #endregion

        CompilerResults CompileAssemblyFromReader(CompilerParameters options, params TextReader[] readers)
        {
            var units = new CodeCompileUnit[readers.Length];
            var errors = new CompilerErrorCollection();
            var syntax = new Parser();

            for (int i = 0; i < readers.Length; i++)
            {
                try
                {
                    units[i] = syntax.Parse(readers[i]);
                }
#if !DEBUG
                catch (ParseException e)
                {
                    errors.Add(new CompilerError(e.Source, e.Line, 0, e.Message.GetHashCode().ToString(), e.Message));
                }
#endif
                finally { }
            }

            var results = CompileAssemblyFromDom(options, units);
            results.Errors.AddRange(errors);

            return results;
        }

        public override CompilerResults CompileAssemblyFromDom(CompilerParameters options, params CodeCompileUnit[] compilationUnits)
        {
            PrintCSharpCode(compilationUnits, Console.Out);

            CompilerResults results;

#if !DEBUG
            csc = false;
#endif

            if (csc)
            {
                options.GenerateExecutable = true;
                options.GenerateInMemory = false;
                options.IncludeDebugInformation = false;
                options.CompilerOptions = "/optimize+";
                var cs = new Microsoft.CSharp.CSharpCodeProvider();
                results = cs.CompileAssemblyFromDom(options, compilationUnits);
            }
            else
            {
                Compiler compiler = new Compiler();
                results = compiler.CompileAssemblyFromDomBatch(options, compilationUnits);
            }

            return results;
        }

        #region Helpers

        [Conditional("DEBUG")]
        void PrintCSharpCode(CodeCompileUnit[] units, TextWriter writer)
        {
            var provider = new Microsoft.CSharp.CSharpCodeProvider();
            var options = new CodeGeneratorOptions { BracingStyle = "C", ElseOnClosing = false, IndentString = "  " };
            var buf = new StringWriter();

            foreach (CodeCompileUnit code in units)
            {
                try { provider.GenerateCodeFromCompileUnit(code, buf, options); }
                catch (Exception e) { buf.WriteLine(e.Message); }
                finally { }
            }

            const string ext = ".", prefix = "namespace ";
            string space = typeof(Script).Namespace + ext;
            string output = buf.ToString().Replace(typeof(Rusty.Core).Namespace + ext, string.Empty).Replace(space, string.Empty);
            output = output.Substring(output.IndexOf(prefix)).Replace(prefix, prefix + space);
            writer.Write(output);
        }

        #endregion

        #endregion
    }
}
