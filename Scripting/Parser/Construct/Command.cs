using System.CodeDom;
using System.Collections.Generic;

namespace IronAHK.Scripting
{
    partial class Parser
    {
        CodeMethodInvokeExpression ParseCommand(string code)
        {
            var anchors = new char[Spaces.Length + 1];
            anchors[0] = Multicast;
            Spaces.CopyTo(anchors, 1);

            code = code.TrimStart(Spaces);
            string[] parts = code.Split(anchors, 2);
            string name = parts[0];

            var invoke = new CodeMethodInvokeExpression();
            invoke.Method = new CodeMethodReferenceExpression(null, name);
            CheckPersistent(name);

            if (parts.Length > 1 && parts[1].Length != 0)
            {
                int cast = parts[1].IndexOf(Multicast);
                if (cast != -1 && IsSpace(parts[1].Substring(0, cast)))
                    parts[1] = parts[1].Substring(cast + 1);

                if (parts[1].Length != 0)
                    foreach (var param in SplitCommandParameters(parts[1]))
                        invoke.Parameters.Add(ParseCommandParameter(param));
            }

            invoke.UserData.Add(invokeCommand, true);
            invokes.Add(invoke);
            return invoke;
        }

        #region Parameters

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
