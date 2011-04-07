using System.CodeDom;

namespace IronAHK.Scripting
{
    class CodeLine : CodeLinePragma
    {
        public CodeLine(string fileName, int lineNumber, string code)
            : base(fileName, lineNumber)
        {
            Code = code;
        }

        public string Code { get; set; }

        public override string ToString() {
            return string.Format("{0}: {1}", LineNumber, Code);
        }
    }
}
