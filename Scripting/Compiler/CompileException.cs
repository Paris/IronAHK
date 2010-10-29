using System;
using System.CodeDom;

namespace IronAHK.Scripting
{
    [Serializable]
    class CompileException : Exception
    {
        public CodeObject Offending;

        public CompileException (CodeObject Offending, string Message) : base(Message)
        {
            this.Offending = Offending;
        }
    }
}
