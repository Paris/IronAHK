using System.CodeDom;
using System.Collections.Generic;

namespace IronAHK.Scripting
{
    partial class Parser
    {
        List<string> labels = new List<string>();

        CodeLabeledStatement ParseLabel(string code)
        {
            int z = code.Length - 1;
            string name = z > 0 ? code.Substring(0, z) : null;
            if (code.Length < 2 || code[z] != HotkeyBound || !IsIdentifier(name))
                throw new ParseException("Invalid label name");

            if (labels.Contains(name))
                throw new ParseException("Duplicate label");

            labels.Add(name);
            return new CodeLabeledStatement(name);
        }
    }
}
