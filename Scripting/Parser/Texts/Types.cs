using System;
using System.Globalization;

namespace IronAHK.Scripting
{
    partial class Parser
    {
        #region Identifiers

        public static bool IsIdentifier(char symbol)
        {
            return char.IsLetterOrDigit(symbol) || VarExt.IndexOf(symbol) != -1;
        }

        bool IsIdentifier(string token)
        {
            return IsIdentifier(token, false);
        }

        bool IsIdentifier(string token, bool dynamic)
        {
            if (string.IsNullOrEmpty(token))
                return false;

            if (token[0] == TernaryA && (token.Length == 1 || token.Length == 2 && token[1] == TernaryA))
                return false;

            foreach (var sym in token)
            {
                if (!IsIdentifier(sym))
                {
                    if (dynamic && sym == Resolve)
                        continue;
                    return false;
                }
            }

            return true;
        }

        bool IsDynamicReference(string code)
        {
            bool d = false;

            for (int i = 0; i < code.Length; i++)
            {
                char sym = code[i];

                if (sym == Resolve)
                {
                    if (d)
                        if (code[i - 1] == Resolve)
                            return false;
                    d = !d;
                }
                else if (!IsIdentifier(sym))
                    return false;
            }

            return code.Length != 0;
        }

        bool IsVariable(string code)
        {
            return IsIdentifier(code, true) && !IsKeyword(code);
        }

        #endregion

        #region Primatives

        bool IsPrimativeObject(string code, out object result)
        {
            result = null;

            if (string.IsNullOrEmpty(code))
                return true;

            switch (code.ToLowerInvariant())
            {
                case TrueTxt:
                    result = true;
                    return true;

                case FalseTxt:
                    result = false;
                    return true;

                case NullTxt:
                    return true;
            }

            // Mono incorrectly determines "." as a numeric value
            if (code.Length == 1 && code[0] == Concatenate)
                return false;

            string codeTrim = code.Trim(Spaces);
            var info = CultureInfo.CreateSpecificCulture("en-GB");

            int e = codeTrim.IndexOfAny(new[] { 'e', 'E' });
            double x = 0;
            bool xf = false;
            if (e != -1)
            {
                int n = e + 1;
                xf = n < codeTrim.Length ? double.TryParse(codeTrim.Substring(e + 1), out x) : false;
                codeTrim = codeTrim.Substring(0, e);
            }

            double d;
            if (double.TryParse(codeTrim, NumberStyles.Any, info, out d))
            {
                result = d;
                goto exp;
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
                goto exp;
            }

            result = null;
            return false;

        exp:
            if (x != 0)
            {
                if (!xf)
                    throw new ParseException(ExInvalidExponent);
                result = (double)result * Math.Pow(10, x);
            }
            return true;
        }

        bool IsPrimativeObject(string code)
        {
            object result;
            return IsPrimativeObject(code, out result);
        }

        #endregion

        #region Expressions

        bool IsExpressionParameter(string code)
        {
            code = code.TrimStart(Spaces);
            int z = code.IndexOf(Resolve);
            return z == 0 && (code.Length == 1 || IsSpace(code[1]));
        }

        bool IsExpressionIf(string code)
        {
            code = code.TrimStart(Spaces);
            int i = 0;

            if (code.Length == 0)
                return true;

            if (code[0] == ParenOpen)
                return true;
            
            while (i < code.Length && IsIdentifier(code[i])) i++;

            if (i == 0 || IsKeyword(code.Substring(0, i)))
                return true;

            while (i < code.Length && IsSpace(code[i])) i++;

            if (i == 0 || i == code.Length)
                return false;

            switch (code[i])
            {
                case Equal:
                case Not:
                case Greater:
                case Less:
                    return false;
            }

            return true;
        }

        #endregion

        #region Misc

        bool IsKeyword(string code)
        {
            switch (code.ToLowerInvariant())
            {
                case AndTxt:
                case OrTxt:
                case NotTxt:
                case TrueTxt:
                case FalseTxt:
                case NullTxt:
                case IsTxt:
                    return true;

                default:
                    return false;
            }
        }

        bool IsKeyword(char symbol)
        {
            switch (symbol)
            {
                case TernaryA:
                    return true;

                default:
                    return false;
            }
        }

        bool IsRemap(string code)
        {
            code = code.Trim(Spaces);

            if (code.Length == 0)
                return false;

            if (IsSpace(code[0]))
                return false;

            for (int i = 1; i < code.Length; i++)
            {
                if (IsCommentAt(code, i))
                    return true;
                else if (!IsSpace(code[i]))
                    return false;
            }

            return true;
        }

        bool IsLegacyIf(string code)
        {
            string[] part = code.TrimStart(Spaces).Split(Spaces, 3);

            if (part.Length < 2 || !IsIdentifier(part[0]))
                return false;

            switch (part[1].ToLowerInvariant())
            {
                case NotTxt:
                case BetweenTxt:
                case InTxt:
                case ContainsTxt:
                case IsTxt:
                    return true;
            }

            return false;
        }

        #endregion
    }
}
