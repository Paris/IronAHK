using System.CodeDom;
using System.Collections.Generic;
using System.Text;

namespace IronAHK.Scripting
{
    partial class Parser
    {
        CodeMethodInvokeExpression ParseCommand(string code)
        {
            string name = null;
            var part = new StringBuilder();
            var param = new List<CodeExpression>();
            bool skipDelimt = false;

            for (int i = 0; i < code.Length; i++)
            {
                char sym = code[i];

                if (name == null)
                {
                    bool spaced = IsSpace(sym);
                    bool symbol = !char.IsLetterOrDigit(sym);

                    if (char.IsLetter(sym))
                        part.Append(sym);
                    else if (sym == Multicast || spaced || symbol)
                    {
                        if (spaced)
                            skipDelimt = true;
                        name = part.ToString();
                        part.Length = 0;
                        if (symbol && sym != Multicast && !spaced)
                            part.Append(sym);
                    }
                    else
                        throw new ParseException(ExCommand);
                }
                else
                {
                    if (sym == Multicast)
                    {
                        if (skipDelimt)
                            skipDelimt = false;
                        else
                        {
                            param.Add(ParseCommandParameter(part.ToString()));
                            part.Length = 0;
                        }
                    }
                    else
                        part.Append(sym);
                }
            }

            if (part.Length != 0)
            {
                if (name == null)
                    name = part.ToString();
                else
                {
                    param.Add(ParseCommandParameter(part.ToString()));
                }
            }

            if (name == null)
                return null;

            var method = new CodeMethodReferenceExpression(null, name);
            var expr = new CodeMethodInvokeExpression(method, param.ToArray());
            return expr;
        }

        CodeExpression ParseCommandParameter(string code)
        {
            code = code.Trim();

            if (code.Length == 0)
                return new CodePrimitiveExpression(null);

            if (code.Length > 2 && code[0] == Resolve && IsSpace(code[1]))
                return ParseSingleExpression(code.Substring(2));
            else
                return VarNameOrBasicString(StripComment(code), true);
        }
    }
}
