using System;
using System.CodeDom;

namespace IronAHK.Scripting
{
    public class CompileException : Exception
    {
        public CodeObject Offending;

        public CompileException (CodeObject Offending, string Message) : base(Message)
        {
            this.Offending = Offending;
        }
    }
}
