using System.CodeDom;

namespace IronAHK.Scripting
{
    class CodeLine : CodeLinePragma
    {
        public CodeLine(string fileName, int lineNumber, string code)
            : base(fileName, lineNumber)
        {
            this.Code = code;
        }

        public string Code { get; set; }
    }
}
