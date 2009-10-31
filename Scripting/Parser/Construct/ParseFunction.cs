using System.CodeDom;
using System.Collections.Generic;
using System.Text;

namespace IronAHK.Scripting
{
    partial class Parser
    {
        const string argv = "argv";

        public void ParseFunction(CodeLine line)
        {
            string code = line.Code;
            int i;
            var buf = new StringBuilder();

            #region Name

            string name;

            for (i = 0; i < code.Length; i++)
            {
                char sym = code[i];

                if (IsIdentifier(sym))
                    buf.Append(sym);
                else if (sym == ParenOpen)
                {
                    i++;
                    break;
                }
                else
                    throw new ParseException(ExUnexpected);
            }

            if (buf.Length == 0)
                throw new ParseException(ExUnexpected);

            name = buf.ToString();
            buf.Length = 0;

            #endregion

            #region Parameters

            CodeBlock.BlockType blockType = CodeBlock.BlockType.Expect;

            bool str = false;
            bool stop = false;

            for (; i < code.Length; i++)
            {
                char sym = code[i];

                switch (sym)
                {
                    case StringBound:
                        str = !str;
                        goto default;

                    case ParenClose:
                        if (str)
                            goto default;
                        else
                            stop = true;
                        break;

                    default:
                        buf.Append(sym);
                        break;
                }

                if (stop)
                    break;
            }

            if (!stop)
                throw new ParseException("Expected closing parenthesis");

            string param = buf.ToString();
            buf.Length = 0;
            var parameters = ParseFunctionParameters(param);
            i++;

            #region Opening brace

            for (; i < code.Length; i++)
            {
                char sym = code[i];

                if (sym == BlockOpen)
                {
                    blockType = CodeBlock.BlockType.Within;
                    break;
                }
                else if (!IsSpace(sym))
                    throw new ParseException(ExUnexpected);
            }

            #endregion

            #endregion

            #region Method

            CodeMemberMethod method = new CodeMemberMethod();
            method.Name = name;
            method.ReturnType = new CodeTypeReference(typeof(object));

            method.Parameters.Add(new CodeParameterDeclarationExpression(typeof(object[]), argv));
            method.Statements.Add(parameters);

            #endregion

            #region Block

            var block = new CodeBlock(line, method.Name, method.Statements);
            block.Type = blockType;
            blocks.Push(block);

            #endregion

            methods.Add(method.Name, method);
        }

        CodeStatement ParseFunctionParameters(string code)
        {
            #region List

            List<string> names = new List<string>(), defaults = new List<string>();
            int i = 0;

            while (i < code.Length)
            {
                int x;

                // preceeding space
                while (IsSpace(code[i])) i++;

                // name
                x = i;
                while (i < code.Length && IsIdentifier(code[i])) i++;
                
                if (x == i)
                    throw new ParseException(ExUnexpected);
                else
                    names.Add(code.Substring(x, i - x));

                while (i < code.Length && IsSpace(code[i])) i++;

                if (i == code.Length)
                    break;

                // defaults
                if (code[i] == Equal)
                {
                    i++;
                    while (IsSpace(code[i])) i++;

                    if (i == code.Length)
                        throw new ParseException(ExUnexpected);
                    else if (code[i] == Multicast)
                        defaults.Add(null);
                    else
                    {
                        bool str = false;
                        x = i;

                        while (!(i == code.Length || (code[i] == Multicast && !str)))
                        {
                            if (code[i++] == StringBound)
                                str = !str;
                        }

                        if (x == i)
                            defaults.Add(null);
                        else
                        {
                            string sub = code.Substring(x, i - x).Trim(Spaces);
                            defaults.Add(sub.Length == 0 ? null : ValidateParameterLiteral(sub));
                        }
                    }
                }
                else
                    defaults.Add(null);

                // next
                if (i < code.Length && code[i] != Multicast)
                    throw new ParseException(ExUnexpected);
                else
                    i++;
            }

            #endregion

            #region Method

            CodeMethodInvokeExpression fix = new CodeMethodInvokeExpression();
            fix.Method = new CodeMethodReferenceExpression(new CodeTypeReferenceExpression(typeof(Rusty.Core)), "Parameters");

            fix.Parameters.Add(new CodePrimitiveExpression(names.ToArray()));
            fix.Parameters.Add(new CodeArgumentReferenceExpression(argv));
            fix.Parameters.Add(new CodePrimitiveExpression(defaults.ToArray()));

            return new CodeExpressionStatement(fix);

            #endregion
        }

        string ValidateParameterLiteral(string code)
        {
            const string err = "Default parameter value expects a literal";

            if (code.Length == 0)
                return null;

            var cs = System.StringComparison.OrdinalIgnoreCase;
            if (code.Equals(TrueTxt, cs) || code.Equals(FalseTxt, cs) || code.Equals(NullTxt, cs))
                return code;

            if (code[0] == StringBound)
            {
                bool str = true;
                for (int i = 1; i < code.Length; i++)
                {
                    if (code[i] == StringBound)
                    {
                        str = !str;
                        int n = i + 1;
                        if (n < code.Length && code[n] == code[i])
                        {
                            i = n;
                        }
                        else if (n != code.Length)
                            throw new ParseException(err);
                    }
                }
                if (str)
                    throw new ParseException(err);
            }
            else if (!IsPrimativeObject(code))
                throw new ParseException(err);

            return code;
        }
    }
}
