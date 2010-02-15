using System.CodeDom;

namespace IronAHK.Scripting
{
    partial class Emit
    {
        void EmitConditionStatement(CodeConditionStatement cond)
        {
            WriteSpace();
            writer.Write("if (");
            EmitExpression(cond.Condition);
            writer.Write(")");
            WriteSpace();
            writer.Write("{");
            depth++;
            EmitStatements(cond.TrueStatements);
            depth--;
            WriteSpace();
            writer.Write("}");
            if (options.ElseOnClosing)
                writer.Write(" ");
            else
                WriteSpace();
            writer.Write("else");
            if (options.ElseOnClosing)
                writer.Write(" ");
            else
                WriteSpace();
            writer.Write("{");
            depth++;
            EmitStatements(cond.FalseStatements);
            depth--;
            WriteSpace();
            writer.Write("}");
        }

        void EmitIteration(CodeIterationStatement iteration)
        {
            EmitStatement(iteration.InitStatement);
            WriteSpace();
            writer.Write("white (");

            depth++;
            EmitExpression(iteration.TestExpression);
            depth--;

            writer.Write(')');
            WriteSpace();
            writer.Write('{');

            depth++;
            EmitStatements(iteration.Statements);
            EmitStatement(iteration.IncrementStatement);
            depth--;

            WriteSpace();
            writer.Write('{');
        }

        void EmitGoto(CodeGotoStatement go)
        {
            writer.Write("goto ");
            writer.Write(go.Label);
        }

        void EmitLabel(CodeLabeledStatement label)
        {
            writer.Write(label.Label);
            writer.Write(':');
        }
    }
}
