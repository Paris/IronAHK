using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Text;

namespace IronAHK.Scripting
{
    partial class Parser
    {
        CodeExpression VarNameOrBasicString(string code, bool asValue)
        {
            if (asValue)
            {
                object result;
                if (IsPrimativeObject(code, out result))
                    return new CodePrimitiveExpression(result);

                if (code.IndexOf(Resolve) == -1)
                    return new CodePrimitiveExpression(code);
            }

            var parts = new List<CodeExpression>();
            var sub = new StringBuilder();
            bool id = false;

            for (int i = 0; i < code.Length; i++)
            {
                char sym = code[i];

                if (sym == Resolve && (i == 0 || code[i - 1] != Escape))
                {
                    if (id)
                    {
                        if (sub.Length == 0)
                            throw new ParseException(ExEmptyVarRef, i);
                        parts.Add((CodeMethodInvokeExpression)VarId(sub.ToString()));
                        sub.Length = 0;
                        id = false;
                    }
                    else
                    {
                        parts.Add(new CodePrimitiveExpression(sub.ToString()));
                        sub.Length = 0;
                        id = true;
                    }
                }
                else if (id && !IsIdentifier(sym))
                    throw new ParseException(ExInvalidVarToken, i);
                else
                    sub.Append(sym);
            }

            if (sub.Length != 0)
                parts.Add(new CodePrimitiveExpression(sub.ToString()));

            if (parts.Count == 1)
                return new CodePrimitiveExpression(code);

            CodeExpression[] all = parts.ToArray();

            if (asValue)
                return StringConcat(all);
            else
                return ComplexVarRef(new CodeComplexVariableReferenceExpression(all));
        }

        CodeComplexVariableReferenceExpression VarId(string name)
        {
            return new CodeComplexVariableReferenceExpression(new CodePrimitiveExpression(Scope + ScopeVar), VarNameOrBasicString(name, true));
        }

        CodeExpression StringConcat(params CodeExpression[] parts)
        {
            var list = new List<CodeExpression>(parts.Length);

            foreach (CodeExpression part in parts)
            {
                if (part is CodePrimitiveExpression)
                {
                    var value = ((CodePrimitiveExpression)part).Value;
                    if (value is string && ((string)value).Length == 0)
                        continue;
                }

                list.Add(part);
            }

            if (list.Count == 1)
                return list[0];

            Type str = typeof(string);
            var method = (CodeMethodReferenceExpression)InternalMethods.Concat;
            var all = new CodeArrayCreateExpression(str, list.ToArray());
            return new CodeMethodInvokeExpression(method, all);
        }
    }
}
