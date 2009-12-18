using System.CodeDom;

namespace IronAHK.Scripting
{
    class CodeBlock
    {
        public enum BlockType { None, Expect, Within };

        public enum BlockKind { Dummy, IfElse, Function, Label, Loop };

        CodeLine line;
        string method;
        CodeStatementCollection statements;
        BlockType type;
        BlockKind kind;

        public CodeBlock(CodeLine line, string method, CodeStatementCollection statements, BlockKind kind)
        {
            this.line = line;
            this.method = method;
            this.statements = statements;
            this.type = BlockType.Expect;
            this.kind = kind;
        }

        public CodeLine Line
        {
            get { return line; }
        }

        public string Method
        {
            get { return method; }
        }

        public CodeStatementCollection Statements
        {
            get { return statements; }
        }

        public BlockType Type
        {
            get { return type; }
            set { type = value; }
        }

        public BlockKind Kind
        {
            get { return kind; }
        }
    }
}
