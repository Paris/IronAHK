using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Text;

namespace IronAHK.Scripting
{
    partial class Parser
    {
        const string args = "args";

        public void ParseFunction(CodeLine line)
        {
            CloseTopLabelBlock();
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

            if (IsLocalMethodReference(name))
                throw new ParseException("Duplicate function");

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
                else if (IsCommentAt(code, i))
                    break;
                else if (!IsSpace(sym))
                    throw new ParseException(ExUnexpected);
            }

            #endregion

            #endregion

            #region Block

            var method = LocalMethod(name);
            method.Attributes = MemberAttributes.Static | MemberAttributes.AccessMask;

            var block = new CodeBlock(line, method.Name, method.Statements, CodeBlock.BlockKind.Function, blocks.Count == 0 ? null : blocks.Peek());
            block.Type = blockType;
            CloseTopSingleBlock();
            blocks.Push(block);

            var fix = ParseFunctionParameters(param);
            if (fix != null)
                method.Statements.Add(fix);

            #endregion

            methods.Add(method.Name, method);

            var type = typeof(Rusty.Core.GenericFunction);
            var typeref = new CodeTypeReference();
            typeref.UserData.Add(RawData, type);
            var del = new CodeDelegateCreateExpression(typeref, new CodeTypeReferenceExpression(className), method.Name);
            var obj = VarAssign(VarRef(mainScope + ScopeVar + method.Name), del);
            prepend.Add(new CodeExpressionStatement(obj));
        }

        #region Parameters

        CodeStatement ParseFunctionParameters(string code)
        {
            #region List

            List<CodePrimitiveExpression> names = new List<CodePrimitiveExpression>(), defaults = new List<CodePrimitiveExpression>();
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
                {
                    string part = code.Substring(x, i - x);
                    bool byref = false;

                    if (part.Equals(FunctionParamRef, StringComparison.OrdinalIgnoreCase))
                    {
                        byref = true; // TODO: handle byref variables

                        do { i++; } while (i < code.Length && IsSpace(code[i]));
                        x = i;
                        while (i < code.Length && IsIdentifier(code[i])) i++;

                        if (x == i)
                            throw new ParseException("Unspecified parameter name");

                        part = code.Substring(x, i - x);
                    }

                    names.Add(new CodePrimitiveExpression((byref ? mainScope : Scope) + ScopeVar + VarNormalisedName(part)));
                }

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
                        defaults.Add(new CodePrimitiveExpression(null));
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
                            defaults.Add(new CodePrimitiveExpression(null));
                        else
                        {
                            string sub = code.Substring(x, i - x).Trim(Spaces);
                            defaults.Add(new CodePrimitiveExpression(sub.Length == 0 ? null : ValidateParameterLiteral(sub)));
                        }
                    }
                }
                else
                    defaults.Add(new CodePrimitiveExpression(null));

                // next
                if (i < code.Length && code[i] != Multicast)
                    throw new ParseException(ExUnexpected);
                else
                    i++;
            }

            #endregion

            #region Method

            if (names.Count == 0)
                return null;

            var fix = new CodeMethodInvokeExpression();
            fix.Method = (CodeMethodReferenceExpression)InternalMethods.Parameters;

            fix.Parameters.Add(new CodeArrayCreateExpression(typeof(string), names.ToArray()));
            fix.Parameters.Add(new CodeArgumentReferenceExpression(args));
            fix.Parameters.Add(new CodeArrayCreateExpression(typeof(string), defaults.ToArray()));

            return new CodeExpressionStatement(fix);

            #endregion
        }

        string ValidateParameterLiteral(string code)
        {
            const string err = "Default parameter value expects a literal";

            if (code.Length == 0)
                return null;

            var cs = StringComparison.OrdinalIgnoreCase;
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

                code = code.Substring(1, code.Length - 2);
                code.Replace(new string(StringBound, 2), string.Empty);
            }
            else if (!IsPrimativeObject(code))
                throw new ParseException(err);

            return code;
        }

        #endregion
    }
}
