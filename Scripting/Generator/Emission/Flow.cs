using System.CodeDom;

namespace IronAHK.Scripting
{
    partial class Emit
    {
        void EmitConditionStatement(CodeConditionStatement cond)
        {
            writer.Write("if (");
            EmitExpression(cond.Condition);
            writer.Write(")");

            if (cond.TrueStatements.Count > 1)
            {
                WriteSpace();
                writer.Write("{");
            }

            depth++;
            EmitStatements(cond.TrueStatements);
            depth--;

            if (cond.TrueStatements.Count > 1)
            {
                WriteSpace();
                writer.Write("}");
            }

            if (cond.FalseStatements.Count > 0)
            {
                if (options.ElseOnClosing)
                    writer.Write(" ");
                else
                    WriteSpace();

                writer.Write("else");

                if (options.ElseOnClosing)
                    writer.Write(" ");
                else
                    WriteSpace();

                if (cond.FalseStatements.Count > 1)
                    writer.Write("{");

                depth++;
                EmitStatements(cond.FalseStatements);
                depth--;

                if (cond.FalseStatements.Count > 1)
                {
                    WriteSpace();
                    writer.Write("}");
                }
            }
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
            writer.Write('}');
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
