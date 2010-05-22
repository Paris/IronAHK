using System;
using System.Text;

namespace IronAHK.Scripting
{
    partial class Generator
    {
        public string CreateEscapedIdentifier(string value)
        {
            var result = new StringBuilder(value.Length * 5);

            foreach (var sym in value)
            {
                if (Parser.IsIdentifier(sym))
                    result.Append(sym);
                else
                {
                    result.Append('_');
                    result.Append(((int)sym).ToString("x"));
                }
            }

            return result.ToString();
        }

        public string CreateValidIdentifier(string value)
        {
            const string format = "x";
            return IsValidIdentifier(value) ? value : format + value.GetHashCode().ToString(format);
        }

        public bool IsValidIdentifier(string value)
        {
            if (value.Length == 0)
                return false;

            foreach (var sym in value)
                if (!Parser.IsIdentifier(sym))
                    return false;

            return true;
        }

        public void ValidateIdentifier(string value)
        {
            if (IsValidIdentifier(value))
                throw new ArgumentException();
        }
    }
}
