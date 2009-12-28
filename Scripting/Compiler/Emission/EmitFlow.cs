using System;
using System.CodeDom;
using System.Reflection.Emit;

namespace IronAHK.Scripting
{
    partial class MethodWriter
    {
        void EmitConditionStatement(CodeConditionStatement Condition)
        {
            Label False = Generator.DefineLabel();
            Label End = Generator.DefineLabel();

            // TODO: This could probably be done more efficiently
            EmitExpression(Condition.Condition);
            Generator.Emit(OpCodes.Brfalse, False);

            // Execute code for true and jump to end
            EmitStatementCollection(Condition.TrueStatements);
            Generator.Emit(OpCodes.Br, End);

            // Execute code for false and move on
            Generator.MarkLabel(False);
            EmitStatementCollection(Condition.FalseStatements);

            Generator.MarkLabel(End);
        }

        void EmitReturnStatement(CodeMethodReturnStatement Return)
        {
            Depth++;

            Debug("Emitting return statement");
            
            if(Method.ReturnType != typeof(void))
            {
                Type Top;
                if(Return.Expression == null)
                {
                    // Default to an empty string if this method expects a return value
                    Generator.Emit(OpCodes.Ldstr, "");
                    Top = typeof(string);
                }
                else Top = EmitExpression(Return.Expression);

                if (Top != null)
                    ForceTopStack(Top, Method.ReturnType);
            }
            else if(Return.Expression != null)
                throw new CompileException(Return, "Can not return value from void method "+Method.Name);

            Generator.Emit(OpCodes.Ret);

            Depth--;
        }
    }
}
