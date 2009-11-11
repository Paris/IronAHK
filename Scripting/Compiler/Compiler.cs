using System.CodeDom;
using System.CodeDom.Compiler;
using System.IO;
using System.Reflection.Emit;

namespace IronAHK.Scripting
{
    internal partial class Compiler : ICodeCompiler
    {
        public CompilerResults CompileAssemblyFromDomBatch(CompilerParameters options, CodeCompileUnit[] compilationUnits)
        {
            Setup(options);
                
            foreach(CodeCompileUnit Unit in compilationUnits)
            {
                EmitNamespace(ABuilder, Unit.Namespaces[0]);
            }
            
            ABuilder.SetEntryPoint(EntryPoint, PEFileKinds.WindowApplication);
            Save();

            return new CompilerResults(null);
        }

        #region Inherited methods

        public CompilerResults CompileAssemblyFromDom(CompilerParameters options, CodeCompileUnit compilationUnit)
        {
            return CompileAssemblyFromDomBatch(options, new CodeCompileUnit[] { compilationUnit });
        }
        
        public CompilerResults CompileAssemblyFromFile(CompilerParameters options, string fileName)
        {
            return CompileAssemblyFromFileBatch(options, new string[] { fileName });
        }
    
        public CompilerResults CompileAssemblyFromFileBatch(CompilerParameters options, string[] fileNames)
        {
            var readers = new TextReader[fileNames.Length];

            for (int i = 0; i < fileNames.Length; i++)
                readers[i] = new StringReader(fileNames[i]);

            return CompileAssemblyFromReaderBatch(options, readers);
        }

        public CompilerResults CompileAssemblyFromSource(CompilerParameters options, string source)
        {
            return CompileAssemblyFromSourceBatch(options, new string[] { source });
        }
    
        public CompilerResults CompileAssemblyFromSourceBatch(CompilerParameters options, string[] sources)
        {
            var readers = new TextReader[sources.Length];

            for (int i = 0; i < sources.Length; i++)
                readers[i] = new StringReader(sources[i]);

            return CompileAssemblyFromReaderBatch(options, readers);
        }

        CompilerResults CompileAssemblyFromReaderBatch(CompilerParameters options, TextReader[] readers)
        {
            var units = new CodeCompileUnit[readers.Length];
            var syntax = new Parser();

            for (int i = 0; i < readers.Length; i++)
                units[i] = syntax.Parse(readers[i]);

            return CompileAssemblyFromDomBatch(options, units);
        }

        #endregion
    }
}
