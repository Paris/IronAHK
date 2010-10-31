using System;
using System.CodeDom;

namespace IronAHK.Scripting
{
    partial class Emit
    {
        void EmitMethod(CodeMemberMethod method)
        {
            if (options.BlankLinesBetweenMembers)
                WriteSpace();

            WriteSpace();
            writer.Write(method.Name);
            writer.Write(Parser.ParenOpen);
            bool first = true;

            foreach (CodeParameterDeclarationExpression param in method.Parameters)
            {
                if (!first)
                    writer.Write(Parser.SingleSpace);

                switch (param.Direction)
                {
                    case FieldDirection.Out:
                        throw new NotSupportedException();

                    case FieldDirection.Ref:
                        writer.Write(Parser.FunctionParamRef);
                        writer.Write(Parser.SingleSpace);
                        break;
                }

                writer.Write(param.Name);

                if (first)
                    first = false;
                else
                    writer.Write(Parser.DefaultMulticast);

            }

            writer.Write(Parser.ParenClose);
            WriteSpace();
            writer.Write(Parser.BlockOpen);

            depth++;
            WriteSpace();
            writer.Write(Parser.FunctionGlobal);
            EmitStatements(method.Statements);
            depth--;

            WriteSpace();
            writer.Write(Parser.BlockClose);
        }
    }
}
