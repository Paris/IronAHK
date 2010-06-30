using System.CodeDom;
using System.Reflection.Emit;

namespace IronAHK.Scripting
{
    partial class Compiler
    {
        void EmitNamespace(AssemblyBuilder Parent, CodeNamespace Namespace)
        {
            ModuleBuilder Module = Parent.DefineDynamicModule(Namespace.Name, AName.Name);
            Methods.Module = Module;
            
            foreach(CodeTypeDeclaration Type in Namespace.Types)
            {
                EmitType(Module, Type);
            }
            
            Module.CreateGlobalFunctions();
        }
    }
}
