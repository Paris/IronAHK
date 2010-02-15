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
            writer.Write('(');
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

            writer.Write(')');
            WriteSpace();
            writer.Write('{');

            depth++;
            WriteSpace();
            writer.Write("global");
            EmitStatements(method.Statements);
            depth--;

            WriteSpace();
            writer.Write('}');
        }
    }
}
