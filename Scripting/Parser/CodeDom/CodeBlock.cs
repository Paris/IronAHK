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
        bool loop;

        public CodeBlock(CodeLine line, string method, CodeStatementCollection statements)
            : this(line, method, statements, false) { }

        public CodeBlock(CodeLine line, string method, CodeStatementCollection statements, bool loop)
        {
            this.line = line;
            this.method = method;
            this.statements = statements;
            this.type = BlockType.Expect;
            this.loop = loop;
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

        public bool Loop
        {
            get { return loop; }
        }
    }
}
