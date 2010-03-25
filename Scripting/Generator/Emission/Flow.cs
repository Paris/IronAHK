using System.CodeDom;

namespace IronAHK.Scripting
{
    partial class Emit
    {
        void EmitConditionStatement(CodeConditionStatement cond)
        {
            writer.Write(Parser.FlowIf);
            writer.Write(Parser.SingleSpace);
            writer.Write(Parser.ParenOpen);
            EmitExpression(cond.Condition);
            writer.Write(Parser.ParenClose);

            if (cond.TrueStatements.Count > 1)
            {
                WriteSpace();
                writer.Write(Parser.BlockOpen);
            }

            depth++;
            EmitStatements(cond.TrueStatements);
            depth--;

            if (cond.TrueStatements.Count > 1)
            {
                WriteSpace();
                writer.Write(Parser.BlockClose);
            }

            if (cond.FalseStatements.Count > 0)
            {
                if (options.ElseOnClosing)
                    writer.Write(Parser.SingleSpace);
                else
                    WriteSpace();

                writer.Write(Parser.FlowElse);

                if (cond.FalseStatements.Count > 1)
                {
                    if (options.ElseOnClosing)
                        writer.Write(Parser.SingleSpace);
                    else
                        WriteSpace();

                    writer.Write(Parser.BlockOpen);
                }

                depth++;
                EmitStatements(cond.FalseStatements);
                depth--;

                if (cond.FalseStatements.Count > 1)
                {
                    WriteSpace();
                    writer.Write(Parser.BlockClose);
                }
            }
        }

        void EmitIteration(CodeIterationStatement iteration)
        {
            EmitStatement(iteration.InitStatement);
            WriteSpace();
            writer.Write(Parser.FlowWhile);
            writer.Write(Parser.ParenOpen);

            depth++;
            EmitExpression(iteration.TestExpression);
            depth--;

            writer.Write(Parser.ParenClose);
            WriteSpace();
            writer.Write(Parser.BlockOpen);

            depth++;
            EmitStatements(iteration.Statements);
            EmitStatement(iteration.IncrementStatement);
            depth--;

            WriteSpace();
            writer.Write(Parser.BlockClose);
        }

        void EmitGoto(CodeGotoStatement go)
        {
            writer.Write(Parser.FlowGoto);
            writer.Write(Parser.SingleSpace);
            writer.Write(go.Label);
        }

        void EmitLabel(CodeLabeledStatement label)
        {
            writer.Write(label.Label);
            writer.Write(Parser.HotkeyBound);
        }
    }
}
