using System;
using System.CodeDom;
using System.CodeDom.Compiler;
using System.Reflection.Emit;
using System.Reflection;

namespace IronAHK.Scripting
{
    internal partial class MethodWriter
    {
        void EmitAssignment(CodeExpression Left, CodeExpression Right)
        {
            Depth++;
            Debug("Emitting assignment statement");

            if(Left is CodeComplexVariableReferenceExpression)
            {
                EmitComplexVariable(Left as CodeComplexVariableReferenceExpression, 3);
                EmitExpression(Right);
                if (Right is CodePrimitiveExpression && ((CodePrimitiveExpression)Right).Value is decimal)
                    Generator.Emit(OpCodes.Box, typeof(float));
                Generator.Emit(OpCodes.Call, SetEnv);
            }
            else if(Left is CodeVariableReferenceExpression)
            {
                var Reference = Left as CodeVariableReferenceExpression;

                LocalBuilder Var;
                if(Locals.ContainsKey(Reference.VariableName))
                    Var = Locals[Reference.VariableName];
                else
                {
                    Var = Generator.DeclareLocal(typeof(int));
                    Locals.Add(Reference.VariableName, Var);
                }

                EmitExpression(Right);
                Generator.Emit(OpCodes.Stloc, Var);
            }
            else throw new CompileException(Left, "Left hand is unassignable");


            Depth--;
        }

        void EmitComplexVariable(CodeComplexVariableReferenceExpression Complex, int Get)
        {
            Depth++;

            Debug("Emitting complex variable reference");

            Generator.Emit(OpCodes.Ldc_I4, Complex.Parts.Length);
            Generator.Emit(OpCodes.Newarr, typeof(string));

            if(Get > 0)
                Get--;

            for(int i = 0; i < Complex.Parts.Length; i++)
            {
                Generator.Emit(OpCodes.Dup);
                Generator.Emit(OpCodes.Ldc_I4, i);
                EmitExpression(Complex.Parts[i], Get);
                Generator.Emit(OpCodes.Stelem_Ref);
            }

            Generator.Emit(OpCodes.Call, typeof(string).GetMethod("Concat", new Type[] { typeof(string[]) }));

            Depth++;
            if(Get == 0)
            {
                Debug("Getting as instructed");
                Generator.Emit(OpCodes.Call, GetEnv);
            }
            else Debug("I'm a good boy and not getting");

            Depth--;
            Depth--;
        }

        void EmitDynamicName(CodeArrayCreateExpression Dynamic)
        {
            Depth++;
            Debug("Emitting dynamic name expression");
            // Bail if it's not a string, we only use it for that
            if(Dynamic.CreateType.BaseType != "System.String")
                throw new CompileException(Dynamic, "CodeArrayCreateExpression is only used for dynamic names. System.String only");

            Generator.Emit(OpCodes.Ldc_I4, Dynamic.Initializers.Count);
            Generator.Emit(OpCodes.Newarr, typeof(string));

            for(int i = 0; i < Dynamic.Initializers.Count; i++)
            {
                Generator.Emit(OpCodes.Dup);
                Generator.Emit(OpCodes.Ldc_I4, i);
                EmitExpression(Dynamic.Initializers[i]);
                Generator.Emit(OpCodes.Stelem_Ref);
            }
            Depth--;
        }

        string ResolveName(CodeVariableReferenceExpression Var)
        {
            const string sep = ".";
            if (IsEntryPoint)
                return string.Concat(sep, Var.VariableName);
            else
                return string.Concat(Member.Name, sep, Var.VariableName);
        }
    }
}
