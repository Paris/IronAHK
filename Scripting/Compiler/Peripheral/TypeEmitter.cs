using System;
using System.CodeDom;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Reflection.Emit;
using System.Reflection;

namespace IronAHK.Scripting
{
    internal partial class Compiler
    {
        void EmitType(ModuleBuilder Parent, CodeTypeDeclaration Decl)
        {
            TypeBuilder Type = Parent.DefineType(Decl.Name, TypeAttributes.Public, typeof(Script));

            // Allow for late binding
            var LocalMethods = new Dictionary<string, MethodWriter>();
            foreach(CodeMemberMethod Method in Decl.Members)
            {
                MethodWriter Writer = new MethodWriter(Type, Method, Methods);
                LocalMethods.Add(Method.Name, Writer);
            }

            foreach(MethodWriter Writer in LocalMethods.Values)
            {
                Writer.Methods = LocalMethods;
                Writer.Emit();
                if(Writer.IsEntryPoint) EntryPoint = Writer.Method;
            }

            Type.CreateType();
        }
    }
}
