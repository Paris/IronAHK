using System;
using System.CodeDom;
using System.CodeDom.Compiler;
using System.Reflection.Emit;
using System.Reflection;

namespace IronAHK.Scripting
{
    internal partial class Compiler
    {
        void EmitType(ModuleBuilder Parent, CodeTypeDeclaration Decl)
        {
            TypeBuilder Type = Parent.DefineType(Decl.Name, TypeAttributes.Public, typeof(Rusty.Script));
            
            foreach(CodeMemberMethod Method in Decl.Members)
            {
                MethodWriter Writer = new MethodWriter(Type, Method, Methods);
                Writer.Emit();
                if(Writer.IsEntryPoint) EntryPoint = Writer.Method;
            }

            Type.CreateType();
        }
    }
}
