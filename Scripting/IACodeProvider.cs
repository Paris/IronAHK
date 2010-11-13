using System;
using System.CodeDom;
using System.CodeDom.Compiler;
using System.Diagnostics;
using System.Security.Permissions;
using System.IO;

[assembly: CLSCompliant(true)]

namespace IronAHK.Scripting
{
    public sealed class IACodeProvider : CodeDomProvider
    {
        #region Extras

        public override string FileExtension
        {
            [PermissionSet(SecurityAction.LinkDemand, Name = "FullTrust")]
            get { return "ahk"; }
        }

        #endregion

        #region Generator

        [Obsolete, PermissionSet(SecurityAction.LinkDemand, Name = "FullTrust")]
        public override ICodeGenerator CreateGenerator()
        {
            return new Generator();
        }

        #endregion

        #region Compiler

        #region Wrappers

        [Obsolete, PermissionSet(SecurityAction.LinkDemand, Name = "FullTrust")]
        public override ICodeCompiler CreateCompiler()
        {
            throw new NotImplementedException();
        }

        [PermissionSet(SecurityAction.LinkDemand, Name = "FullTrust")]
        public override CompilerResults CompileAssemblyFromSource(CompilerParameters options, params string[] sources)
        {
            var readers = new TextReader[sources.Length];

            for (int i = 0; i < sources.Length; i++)
                readers[i] = new StringReader(sources[i]);

            return CompileAssemblyFromReader(options, readers);
        }

        CompilerResults CompileAssemblyFromReader(CompilerParameters options, params TextReader[] readers)
        {
            var tempFiles = new TempFileCollection(Path.GetTempPath(), false);

            for (int i = 0; i < readers.Length; i++)
            {
                string file = tempFiles.AddExtension(FileExtension, false);
                File.WriteAllText(file, readers[i].ReadToEnd());
            }

            var fileNames = new string[tempFiles.Count];
            tempFiles.CopyTo(fileNames, 0);
            var results = CompileAssemblyFromFile(options, fileNames);
            tempFiles.Delete();
            return results;
        }

        #endregion

        [PermissionSet(SecurityAction.LinkDemand, Name = "FullTrust")]
        public override CompilerResults CompileAssemblyFromFile(CompilerParameters options, params string[] fileNames)
        {
            var units = new CodeCompileUnit[fileNames.Length];
            var errors = new CompilerErrorCollection();
            var syntax = new Parser(options);

            for (int i = 0; i < fileNames.Length; i++)
            {
                try
                {
                    units[i] = syntax.Parse(new StreamReader(fileNames[i]), fileNames[i]);
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

        [PermissionSet(SecurityAction.LinkDemand, Name = "FullTrust")]
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
