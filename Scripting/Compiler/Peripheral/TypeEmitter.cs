using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;

namespace IronAHK.Scripting
{
    partial class Compiler
    {
        void EmitType(ModuleBuilder Parent, CodeTypeDeclaration Decl)
        {
            TypeBuilder Type = Parent.DefineType(Decl.Name, TypeAttributes.Public);

            // Allow for late binding
            var LocalMethods = new Dictionary<string, MethodWriter>();
            var LocalParameters = new Dictionary<string, Type[]>();

            foreach(CodeMemberMethod Method in Decl.Members)
            {
                var Writer = new MethodWriter(Type, Method, Methods, Mirror);
                LocalParameters.Add(Method.Name, GetParameterTypes(Method.Parameters));
                LocalMethods.Add(Method.Name, Writer);
            }

            foreach(var Writer in LocalMethods.Values)
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
            {
                if (param[i].UserData.Contains(Parser.RawData))
                    types[i] = param[i].UserData[Parser.RawData] as Type;
                else
                    types[i] = Type.GetType(param[i].Type.BaseType);
            }

            return types;
        }
    }
}
