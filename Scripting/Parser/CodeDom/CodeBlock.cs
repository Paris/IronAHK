using System.CodeDom;

namespace IronAHK.Scripting
{
    class CodeBlock
    {
        public enum BlockType { None, Expect, Within };

        CodeLine line;
        string method;
        CodeStatementCollection statements;
        BlockType type;

        public CodeBlock(CodeLine line, string method, CodeStatementCollection statements)
        {
            this.line = line;
            this.method = method;
            this.statements = statements;
            this.type = BlockType.Expect;
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
    }
}
