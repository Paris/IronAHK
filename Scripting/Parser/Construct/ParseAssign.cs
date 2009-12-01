using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace IronAHK.Scripting
{
    partial class Parser
    {
        const string ScopeVar = ".";

        CodeComplexAssignStatement ParseAssign(string code)
        {
            string name, value;
            var buf = new StringBuilder(code.Length);
            int i = 0;
            char sym;

            #region Name

            bool bound = false;

            for (i = 0; i < code.Length; i++)
            {
                sym = code[i];

                if (IsIdentifier(sym) || sym == Resolve)
                    buf.Append(sym);
                else if (sym == Equal)
                {
                    i++;
                    bound = true;
                    break;
                }
                else if (IsSpace(sym))
                    break;
                else
                    throw new ParseException(ExUnexpected);
            }

            if (!bound)
            {
                while (i < code.Length)
                {
                    sym = code[i];
                    i++;
                    if (sym == Equal)
                    {
                        bound = true;
                        break;
                    }
                }
                if (!bound)
                    throw new ParseException(ExUnexpected);
            }

            name = buf.ToString();
            buf.Length = 0;

            #endregion

            #region Value

            value = code.Substring(i);

            if (value.Length == 0)
                value = null;

            value = StripCommentSingle(value);

            #endregion

            CodeExpression result = VarNameOrBasicString(value, true);
            return new CodeComplexAssignStatement(VarId(name), result);
        }

        public static bool IsIdentifier(char symbol)
        {
            return char.IsLetterOrDigit(symbol) || VarExt.IndexOf(symbol) != -1;
        }

        bool IsIdentifier(string token)
        {
            if (string.IsNullOrEmpty(token))
                return false;

            foreach (char sym in token)
                if (!IsIdentifier(sym))
                    return false;

            return true;
        }

        bool IsPrimativeObject(string code, out object result)
        {
            if (string.IsNullOrEmpty(code))
            {
                result = null;
                return true;
            }

            string codeTrim = code.Trim(Spaces);
            var info = CultureInfo.CreateSpecificCulture("en-GB");

            decimal d;
            if (decimal.TryParse(codeTrim, NumberStyles.Any, info, out d))
            {
                result = d;
                return true;
            }

            int i;
            const string hex = "0x";
            int z = codeTrim.IndexOf(hex);
            bool negative = false;
            if (z == 1 && codeTrim[0] == Minus)
            {
                negative = true;
                codeTrim = codeTrim.Substring(1);
            }
            if ((z == 0 || negative) && int.TryParse(codeTrim.Replace(hex, string.Empty), NumberStyles.HexNumber, info, out i))
            {
                result = (decimal)(negative ? -i : i);
                return true;
            }

            result = null;
            return false;
        }

        bool IsPrimativeObject(string code)
        {
            object result;
            return IsPrimativeObject(code, out result);
        }

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
                return (CodeMethodInvokeExpression)(new CodeComplexVariableReferenceExpression(all));
        }

        CodeMethodInvokeExpression StringConcat(params CodeExpression[] parts)
        {
            Type str = typeof(string);
            var method = (CodeMethodReferenceExpression)InternalMethods.Concat;
            var all = new CodeArrayCreateExpression(str, parts);
            return new CodeMethodInvokeExpression(method, all);
        }

        CodeComplexVariableReferenceExpression VarId(string name)
        {
            return new CodeComplexVariableReferenceExpression(new CodePrimitiveExpression(Scope + ScopeVar), VarNameOrBasicString(name, true));
        }
    }
}
