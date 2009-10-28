using System;
using System.CodeDom;
using System.CodeDom.Compiler;
using System.Diagnostics;
using System.IO;
using Microsoft.CSharp;

namespace IronAHK.Scripting
{
    partial class IACodeProvider
    {
        [Conditional("DEBUG")]
        void PrintCSharpCode(CodeCompileUnit[] units, TextWriter write)
        {
            var provider = new CSharpCodeProvider();
            var options = new CodeGeneratorOptions { BracingStyle = "C", ElseOnClosing = false, IndentString = "  " };
            foreach (CodeCompileUnit code in units)
            {
                try { provider.GenerateCodeFromCompileUnit(code, write, options); }
                catch (Exception e) { write.WriteLine(e.Message); }
                finally { }
            }
        }
    }
}
