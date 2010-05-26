using System.CodeDom;
using System.Collections.Generic;

namespace IronAHK.Scripting
{
    partial class Parser
    {
        CodeMethodInvokeExpression ParseCommand(string code)
        {
            var parts = SplitCommandStatement(code);
            var invoke = new CodeMethodInvokeExpression();
            invoke.Method = new CodeMethodReferenceExpression(null, parts[0]);
            CheckPersistent(parts[0]);

            if (parts.Length > 1 && parts[1].Length != 0)
            {
                foreach (var param in SplitCommandParameters(parts[1]))
                    invoke.Parameters.Add(ParseCommandParameter(param));
            }

            invoke.UserData.Add(invokeCommand, true);
            invokes.Add(invoke);
            return invoke;
        }

        #region Parameters

        string[] SplitCommandStatement(string code)
        {
            int i = 0;
            bool d = false;

            code = code.TrimStart(Spaces);

            for (; i < code.Length; i++)
            {
                if (code[i] == Multicast)
                    break;
                else if (IsSpace(code[i]))
                    d = true;
                else if (d)
                {
                    i--;
                    break;
                }
            }

            int n = i + 1;
            var parts = new[] { code.Substring(0, i).TrimEnd(Spaces), n >= code.Length ? string.Empty : code.Substring(n).TrimStart(Spaces) };

            return parts;
        }

        string[] SplitCommandParameters(string code)
        {
            var parts = new List<string>();
            bool start = true, expr = false, str = false;
            int last = 0;
            int[] levels = { 0, 0, 0 }; // parentheses, objects, arrays

            for (int i = 0; i < code.Length; i++)
            {
                char sym = code[i];

                if (str)
                {
                    if (sym == StringBound)
                        str = !str;
                    continue;
                }
                else if (IsCommentAt(code, i))
                    break;

                if (start)
                {
                    if (IsSpace(sym))
                        continue;
                    else
                    {
                        start = false;
                        int n = i + 1;
                        expr = sym == Resolve && (n < code.Length ? IsSpace(code[n]) : true);
                    }
                }

                if (expr)
                {
                    switch (sym)
                    {
                        case StringBound: str = !str; break;
                        case ParenOpen: levels[0]++; break;
                        case ParenClose: levels[0]--; break;
                        case BlockOpen: levels[1]++; break;
                        case BlockClose: levels[1]--; break;
                        case ArrayOpen: levels[2]++; break;
                        case ArrayClose: levels[2]--; break;
                    }
                }
                
                if (sym == Multicast && (i == 0 || code[i - 1] != Escape) && !str && levels[0] == 0 && levels[1] == 0 && levels[2] == 0)
                {
                    parts.Add(code.Substring(last, i - last));
                    last = i + 1;
                    start = true;
                    expr = false;
                }
            }

            int d = code.Length - last;
            if (d != 0)
                parts.Add(code.Substring(last, d));

            return parts.ToArray();
        }

        CodeExpression ParseCommandParameter(string code)
        {
            code = code.Trim(Spaces); // should depend on AutoTrim

            if (code.Length == 0)
                return new CodePrimitiveExpression(null);

            if (IsExpressionParameter(code))
                return ParseSingleExpression(code.Substring(2));
            else
                return VarNameOrBasicString(StripComment(code), true);
        }

        #endregion
    }
}
