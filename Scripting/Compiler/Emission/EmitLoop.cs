using System;
using System.CodeDom;
using System.CodeDom.Compiler;
using System.Reflection.Emit;
using System.Reflection;

namespace IronAHK.Scripting
{
    internal partial class MethodWriter
    {
        void EmitIterationStatement(CodeIterationStatement Iterate)
        {
            // Used for break and continue later on
            LoopMetadata Meta = new LoopMetadata {
                Begin = Generator.DefineLabel(),
                End = Generator.DefineLabel(),
            };
            Loops.Push(Meta);

            EmitStatement(Iterate.InitStatement);

            // The beginning of our loop: check the limit
            Generator.MarkLabel(Meta.Begin);

            EmitExpression(Iterate.TestExpression);
            Generator.Emit(OpCodes.Brfalse, Meta.End);

            // Emit the actual statements within
            EmitStatementCollection(Iterate.Statements);

            // Increase the counter by one
            EmitStatement(Iterate.IncrementStatement);

            // Start all over again
            Generator.Emit(OpCodes.Br, Meta.Begin);
            Generator.MarkLabel(Meta.End);

            Loops.Pop();
        }
    }
}
