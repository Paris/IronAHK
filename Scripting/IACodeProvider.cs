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
                catch (Exception e)
                {
                    errors.Add(new CompilerError { ErrorText = e.Message });
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
            PrintCode(compilationUnits, Console.Out);

            CompilerResults results;
            var compiler = new Compiler();

            try
            {
                results = compiler.CompileAssemblyFromDomBatch(options, compilationUnits);
            }
#if !DEBUG
            catch (Exception e)
            {
                results = new CompilerResults(null);
                results.Errors.Add(new CompilerError { ErrorText = e.Message });
            }
#endif
            finally { }

            return results;
        }

        #region Helpers

        [Conditional("DEBUG")]
        void PrintCode(CodeCompileUnit[] units, TextWriter writer)
        {
            var gen = new Generator();
            var options = new CodeGeneratorOptions { IndentString = "  " };

            try
            {
                foreach (var unit in units)
                    gen.GenerateCodeFromCodeObject(unit, writer, options);
            }
            catch (Exception e)
            {
                Console.Error.WriteLine(e.Message);
            }
        }

        #endregion

        #endregion
    }
}
