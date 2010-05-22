using System;
using System.CodeDom;
using System.CodeDom.Compiler;
using System.IO;

namespace IronAHK.Scripting
{
    partial class Emit
    {
        #region Properties

        TextWriter writer;
        CodeGeneratorOptions options;
        int depth;

        #endregion

        #region Constructor

        public Emit(TextWriter writer, CodeGeneratorOptions options, int depth)
        {
            this.writer = writer;
            this.options = options;
            this.depth = depth;
        }

        #endregion

        #region Extra

        public void Convert(CodeObject code)
        {
            if (code is CodeCompileUnit)
            {
                foreach (CodeTypeMember member in ((CodeCompileUnit)code).Namespaces[0].Types[0].Members)
                {
                    if (member is CodeEntryPointMethod)
                        EmitStatements(((CodeEntryPointMethod)member).Statements);
                    else
                        EmitMethod((CodeMemberMethod)member);
                }
            }
            else if (code is CodeEntryPointMethod || code is CodeMemberMethod)
                EmitMethod((CodeMemberMethod)code);
            else if (code is CodeStatement)
                EmitStatement((CodeStatement)code);

            writer.WriteLine();
        }

        void WriteSpace()
        {
            writer.WriteLine();
            
            for (int i = 0; i < depth; i++)
                writer.Write(options.IndentString);
        }

        #endregion

        #region Statements

        void EmitStatements(CodeStatementCollection statements)
        {
            foreach (CodeStatement statement in statements)
            {
                WriteSpace();
                EmitStatement(statement);
            }
        }

        void EmitStatement(CodeStatement statement)
        {
            if (statement is CodeAssignStatement)
                EmitAssignment((CodeAssignStatement)statement);
            else if (statement is CodeExpressionStatement)
                EmitExpressionStatement(((CodeExpressionStatement)statement).Expression);
            else if (statement is CodeIterationStatement)
                EmitIteration((CodeIterationStatement)statement);
            else if (statement is CodeConditionStatement)
                EmitConditionStatement((CodeConditionStatement)statement);
            else if (statement is CodeGotoStatement)
                EmitGoto((CodeGotoStatement)statement);
            else if (statement is CodeLabeledStatement)
                EmitLabel((CodeLabeledStatement)statement);
            else if (statement is CodeMethodReturnStatement)
                EmitReturn((CodeMethodReturnStatement)statement);
            else if (statement is CodeVariableDeclarationStatement)
                EmitVariableDeclaration((CodeVariableDeclarationStatement)statement);
            else if (statement is CodeCommentStatement)
                EmitComment((CodeCommentStatement)statement);
            else
                throw new ArgumentException("Unrecognised statement: " + statement.GetType());
        }

        #region Misc.

        void EmitComment(CodeCommentStatement comment)
        {
            if (string.IsNullOrEmpty(comment.Comment.Text))
                return;

            writer.Write(Parser.SingleSpace);
            writer.Write(Parser.DefaultComment);
            writer.Write(Parser.SingleSpace);
            writer.Write(comment.Comment.Text);
        }

        #endregion

        #endregion
    }
}
