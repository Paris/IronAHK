using System.CodeDom;

namespace IronAHK.Scripting
{
    class CodeLine : CodeLinePragma
    {
        string code;

        public CodeLine(string fileName, int lineNumber, string code)
            : base(fileName, lineNumber)
        {
            this.code = code;
        }

        public string Code
        {
            get { return code; }
            set { code = value; }
        }
    }
}
