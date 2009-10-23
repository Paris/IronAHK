using System.CodeDom;

namespace IronAHK.Scripting
{
    class CodeGosubStatement : CodeGotoStatement
    {
        public CodeGosubStatement(string label)
            : base(label) { }
    }
}
