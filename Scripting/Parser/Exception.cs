using System;

namespace IronAHK.Scripting
{
    [Serializable]
    class ParseException : Exception
    {
        string message;
        int line;
        string source;

        public ParseException(string message)
            : this(message, default(int)) { }

        public ParseException(string message, CodeLine line)
            : this(message, line.LineNumber, line.FileName) { }

        public ParseException(string message, int line)
            : this(message, line, null) { }

        public ParseException(string message, int line, string source)
        {
            this.message = message;
            this.line = line;
            this.source = source;
        }

        public override string Message
        {
            get { return message; }
        }

        public int Line
        {
            get { return line; }
        }

        public override string Source
        {
            get { return source; }
        }
    }
}
