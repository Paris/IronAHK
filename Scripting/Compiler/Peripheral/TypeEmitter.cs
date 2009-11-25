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
            var LocalParameters = new Dictionary<string, Type[]>();

            foreach(CodeMemberMethod Method in Decl.Members)
            {
                MethodWriter Writer = new MethodWriter(Type, Method, Methods);
                LocalParameters.Add(Method.Name, GetParameterTypes(Method.Parameters));
                LocalMethods.Add(Method.Name, Writer);
            }

            foreach(MethodWriter Writer in LocalMethods.Values)
            {
                Writer.ParameterTypes = LocalParameters;
                Writer.Methods = LocalMethods;
                Writer.Emit();
                if(Writer.IsEntryPoint) EntryPoint = Writer.Method;
            }

            Type.CreateType();
        }

        Type[] GetParameterTypes(CodeParameterDeclarationExpressionCollection param)
        {
            var types = new Type[param.Count];

            for (int i = 0; i < types.Length; i++)
                types[i] = Type.GetType(param[i].Type.BaseType);

            return types;
        }
    }
}
