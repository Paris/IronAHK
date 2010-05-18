using System.CodeDom;
using System.CodeDom.Compiler;
using System.IO;

namespace IronAHK.Scripting
{
    partial class Generator
    {
        public void GenerateCodeFromCodeObject(CodeObject e, TextWriter w, CodeGeneratorOptions o)
        {
            var emit = new Emit(w, o, 0);
            emit.Convert(e);
        }

        #region Wrapper methods

        public void GenerateCodeFromCompileUnit(CodeCompileUnit e, TextWriter w, CodeGeneratorOptions o)
        {
            GenerateCodeFromCodeObject(e, w, o);
        }

        public void GenerateCodeFromExpression(CodeExpression e, TextWriter w, CodeGeneratorOptions o)
        {
            GenerateCodeFromCodeObject(e, w, o);
        }

        public void GenerateCodeFromNamespace(CodeNamespace e, TextWriter w, CodeGeneratorOptions o)
        {
            GenerateCodeFromCodeObject(e, w, o);
        }

        public void GenerateCodeFromStatement(CodeStatement e, TextWriter w, CodeGeneratorOptions o)
        {
            GenerateCodeFromCodeObject(e, w, o);
        }

        public void GenerateCodeFromType(CodeTypeDeclaration e, TextWriter w, CodeGeneratorOptions o)
        {
            GenerateCodeFromCodeObject(e, w, o);
        }

        #endregion
    }
}
