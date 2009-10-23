using System;
using System.CodeDom;
using System.CodeDom.Compiler;
using System.Reflection.Emit;

namespace IronAHK.Scripting
{
    internal partial class Compiler
    {
        void EmitNamespace(AssemblyBuilder Parent, CodeNamespace Namespace)
        {
            ModuleBuilder Module = Parent.DefineDynamicModule(Namespace.Name, AName.Name);
            
            foreach(CodeTypeDeclaration Type in Namespace.Types)
            {
                EmitType(Module, Type);
            }
            
            Module.CreateGlobalFunctions();
        }
    }
}
