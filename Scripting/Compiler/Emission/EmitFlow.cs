using System;
using System.CodeDom;
using System.CodeDom.Compiler;
using System.Reflection.Emit;
using System.Reflection;

namespace IronAHK.Scripting
{
    internal partial class MethodWriter
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
    }
}
