using System.CodeDom;
using System.Reflection.Emit;

namespace IronAHK.Scripting
{
    partial class MethodWriter
    {
        void EmitIterationStatement(CodeIterationStatement Iterate)
        {
            Depth++;
            Debug("Emitting iteration statement");

            // Used for break and continue later on
            var Meta = new LoopMetadata {
                Begin = Generator.DefineLabel(),
                End = Generator.DefineLabel(),
            };
            Loops.Push(Meta);

            EmitStatement(Iterate.InitStatement, false);

            // The beginning of our loop: check the limit
            Generator.MarkLabel(Meta.Begin);

            EmitExpression(Iterate.TestExpression, false);
            Generator.Emit(OpCodes.Brfalse, Meta.End);

            // Emit the actual statements within
            EmitStatementCollection(Iterate.Statements);

            // Increase the counter by one
            EmitStatement(Iterate.IncrementStatement, false);

            // Start all over again
            Generator.Emit(OpCodes.Br, Meta.Begin);
            Generator.MarkLabel(Meta.End);

            Loops.Pop();
            Depth--;
        }
    }
}
