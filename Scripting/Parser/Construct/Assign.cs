using System.CodeDom;
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
            else
                value = StripCommentSingle(value);

            #endregion

            CodeExpression result = value == null ? new CodePrimitiveExpression(null) :
                IsExpressionParameter(value) ? ParseSingleExpression(value.TrimStart(Spaces).Substring(2)) : VarNameOrBasicString(value, true);
            return new CodeComplexAssignStatement(VarId(name), result);
        }
    }
}
