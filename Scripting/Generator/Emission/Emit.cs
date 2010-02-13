using System;
using System.CodeDom;
using System.CodeDom.Compiler;
using System.IO;

namespace IronAHK.Scripting
{
    class Emit
    {
        TextWriter writer;
        CodeGeneratorOptions options;
        int depth;

        public Emit(TextWriter writer, CodeGeneratorOptions options, int depth)
        {
            this.writer = writer;
            this.options = options;
            this.depth = depth;
        }

        public void Convert(CodeObject code)
        {
            if (code is CodeCompileUnit)
            {
                foreach (CodeTypeMember member in ((CodeCompileUnit)code).Namespaces[0].Types[0].Members)
                    if (member is CodeEntryPointMethod || code is CodeMemberMethod)
                        EmitMethod((CodeMemberMethod)member);
            }
            else if (code is CodeEntryPointMethod || code is CodeMemberMethod)
                EmitMethod((CodeMemberMethod)code);
            else if (code is CodeStatement)
                EmitStatement((CodeStatement)code);
        }

        void WriteSpace()
        {
            writer.WriteLine();

            for (int i = 0; i < depth; i++)
                writer.Write(options.IndentString);
        }

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

        void EmitStatements(CodeStatementCollection statements)
        {
            foreach (CodeStatement statement in statements)
                EmitStatement(statement);
        }

        void EmitStatement(CodeStatement statement)
        {
            if (statement is CodeConditionStatement)
                EmitConditionStatement((CodeConditionStatement)statement);
        }

        void EmitExpression(CodeExpression expr)
        {
            writer.Write(expr.ToString());
        }

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
    }
}
