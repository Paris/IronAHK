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
                var low = parts[0].ToLowerInvariant();
                var info = libMethods.ContainsKey(low) ? libMethods[low] : null;
                var exp = info == null ? new bool[] { } : new bool[info.Length];

                for (var i = 0; info != null && i < info.Length; i++)
                    exp[i] = Script.IsNumeric(info[i].ParameterType);

                var split = SplitCommandParameters(parts[1], exp);

                if (parts[0].Equals(MsgBox, System.StringComparison.OrdinalIgnoreCase) && split.Length > 1)
                {
                    int n;

                    if (split[0].Length != 0 && !int.TryParse(split[0], out n))
                        split = new[] { parts[1] };
                }

                for (var i = 0; i < split.Length; i++)
                {
                    bool byref = false, expr = false;

                    if (info != null && i < info.Length)
                    {
                        byref = info[i].IsOut || info[i].ParameterType.IsByRef;
                        expr = exp[i];
                    }

                    invoke.Parameters.Add(ParseCommandParameter(split[i], byref, expr));
                }
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

        string[] SplitCommandParameters(string code, bool[] exp = null)
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
                    else if (sym == Multicast)
                        goto delim;
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
                        n = parts.Count;
                        if (exp != null && exp.Length > n && exp[n])
                            expr = true;
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
                
            delim:
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

        CodeExpression ParseCommandParameter(string code, bool byref = false, bool expr = false)
        {
            code = code.Trim(Spaces);

            if (code.Length == 0)
                return new CodePrimitiveExpression(null);

            if (expr && code.Length > 2 && code[0] == Resolve && code[code.Length - 1] == Resolve)
                code = code.Substring(1, code.Length - 2);

            var explicitExpr = false;

            if (IsExpressionParameter(code))
            {
                code = code.Substring(2);
                expr = true;
                explicitExpr = true;
            }

            if (expr)
            {
                try
                {
                    return ParseSingleExpression(code);
                }
                catch (ParseException)
                {
                    // soft failure for implicit expression mode only
                    if (explicitExpr)
                        throw;
                    
                    return new CodePrimitiveExpression(null);
                }
            }

            code = StripComment(code);

            if (byref)
                return VarId(code);

            return VarIdExpand(code);
        }

        #endregion
    }
}
