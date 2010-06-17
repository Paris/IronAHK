using System.CodeDom;
using System.Text;

namespace IronAHK.Scripting
{
    partial class Parser
    {
        CodeArrayIndexerExpression VarRef(params CodeExpression[] name)
        {
            var vars = new CodePropertyReferenceExpression(new CodeTypeReferenceExpression(typeof(Script)), VarProperty);
            return new CodeArrayIndexerExpression(vars, name);
        }

        CodeArrayIndexerExpression VarRef(string name)
        {
            return VarRef(new CodePrimitiveExpression(name));
        }

        CodeBinaryOperatorExpression VarAssign(CodeArrayIndexerExpression name, CodeExpression value)
        {
            return new CodeBinaryOperatorExpression(name, CodeBinaryOperatorType.Assign, value);
        }

        CodeExpressionStatement ParseAssign(string code)
        {
            #region Variables

            string name, value;
            var buf = new StringBuilder(code.Length);
            int i = 0;
            char sym;

            #endregion

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
                value = StripCommentSingle(value.Trim(Spaces));

            #endregion

            #region Result

            CodeExpression result = value == null ? new CodePrimitiveExpression(null) :
                IsExpressionParameter(value) ? ParseSingleExpression(value.TrimStart(Spaces).Substring(2)) : VarNameOrBasicString(value, true);
            return new CodeExpressionStatement(VarAssign(VarId(name), result));

            #endregion
        }
    }
}
