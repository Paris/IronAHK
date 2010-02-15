using System;
using System.CodeDom;

namespace IronAHK.Scripting
{
    partial class Emit
    {
        void EmitMethod(CodeMemberMethod method)
        {
            WriteSpace();

            writer.Write(method.Name);

            bool first = true;
            foreach (CodeParameterDeclarationExpression param in method.Parameters)
            {
                if (!first)
                    writer.Write(" ");

                switch (param.Direction)
                {
                    case FieldDirection.Out:
                        throw new NotSupportedException();

                    case FieldDirection.Ref:
                        writer.Write("ByRef ");
                        break;
                }

                writer.Write(param.Name);

                if (first)
                    first = false;
                else
                    writer.Write(",");

            }

            depth++;
            EmitStatements(method.Statements);
            depth--;
        }
    }
}
