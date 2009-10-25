using System;
using System.CodeDom;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace IronAHK.Scripting
{
    partial class Parser
    {
        CodeExpressionStatement[] ParseMultiExpression(string code)
        {
            var tokens = SplitTokens(code);
            var list = new List<CodeExpressionStatement>();
            var sub = new List<object>(tokens.Count);

            for (int i = 0; i <= tokens.Count; i++)
            {
                if (i == tokens.Count)
                    goto collect; // ffffff

                string check = tokens[i] as string;
                if (!(check != null && check.Length == 1 && check[0] == Multicast))
                {
                    sub.Add(tokens[i]);
                    continue;
                }

            collect:
                if (sub.Count > 0)
                {
                    list.Add(new CodeExpressionStatement(ParseExpression(sub)));
                    sub = new List<object>(tokens.Count);
                }
            }

            return list.ToArray();
        }

        CodeExpression ParseSingleExpression(string code)
        {
            return ParseExpression(SplitTokens(code));
        }

        CodeExpression ParseExpression(List<object> parts)
        {
            for (int i = 0; i < parts.Count; i++)
            {
                if (parts[i] is string)
                {
                    string part = (string)parts[i];
                    object result;
                    bool next = false;

                    #region Parentheses
                    if (part[0] == ParenOpen)
                    {
                        int levels = 1;
                        for (int x = i + 1; x < parts.Count; x++)
                        {
                            string current = parts[x] as string;
                            if (string.IsNullOrEmpty(current))
                                continue;
                            switch (current[0])
                            {
                                case ParenOpen:
                                    levels++;
                                    break;

                                case ParenClose:
                                    if (--levels == 0)
                                    {
                                        int count = x - i;

                                        var sub = new List<object>(count);

                                        for (int n = i + 1; n < x; n++)
                                            sub.Add(parts[n]);

                                        parts.RemoveRange(i, count + 1);

                                        if (sub.Count > 0)
                                            parts.Insert(i, ParseExpression(sub));

                                        next = true;
                                    }
                                    break;
                            }
                            if (next)
                                break;
                        }
                        if (levels != 0)
                            throw new ParseException(ExUnbalancedParens);
                        else if (next)
                            continue;
                    }
                    #endregion
                    else if (IsPrimativeObject(part, out result)) // numeric
                        parts[i] = new CodePrimitiveExpression(result);
                    else if (IsIdentifier(part[0])) // variables
                        parts[i] = VarId(part);
                    else if (part.Length > 2 && part[0] == StringBound && part[part.Length - 1] == StringBound) // string
                        parts[i] = part.Substring(1, part.Length - 2);
                    else if (IsAssignOp(part)) // assignments
                    {
                        int n = i - 1;
                        if (n < 0 || !(parts[n] is CodeComplexVariableReferenceExpression))
                            throw new ParseException("Can only assign to a variable");

                        // (x += y) => (x = x + y)
                        parts[i] = new CodeAssignExpression();
                        if (part[0] != AssignPre)
                        {
                            i++;
                            parts.Insert(i, parts[i - 2]);
                            i++;
                            parts.Insert(i, BinaryOperator(part.Substring(0, part.Length - 1)));
                        }
                    }
                    else if (part.Length == 1 && part[0] == Multicast)
                    {
                        throw new ParseException("Multiple expression statements not allowed here");
                    }
                    else // binary operators
                    {
                        parts[i] = BinaryOperator(part); // TODO: uniary operators
                    }
                }
            }

            #region Binary operators
            bool op = true;
            while (op)
            {
                op = false;
                for (int i = 0; i < parts.Count; i++)
                {
                    if (parts[i] is CodeBinaryOperatorType)
                    {
                        op = true;
                        parts[i - 1] = new CodeBinaryOperatorExpression(
                            (CodeExpression)parts[i - 1], (CodeBinaryOperatorType)parts[i], (CodeExpression)parts[i + 1]);
                        parts.RemoveAt(i + 1);
                        parts.RemoveAt(i);
                    }
                }
            }
            #endregion

            #region Assignments
            for (int i = 0; i < parts.Count; i++)
            {
                if (parts[i] is CodeAssignExpression)
                {
                    var assign = (CodeAssignExpression)parts[i];
                    assign.Left = (CodeComplexVariableReferenceExpression)parts[i - 1];
                    assign.Right = (CodeExpression)parts[i + 1];
                    parts.RemoveAt(i - 1);
                    parts.RemoveAt(i + 1 - 1);
                }
            }
            #endregion

            if (parts.Count == 1)
                return (CodeExpression)parts[0];
            else
                throw new ArgumentOutOfRangeException();
        }

        CodeBinaryOperatorType BinaryOperator(string code)
        {
            char[] op = code.ToCharArray();

            switch (op[0])
            {
                case Add:
                    switch (op.Length)
                    {
                        case 1:
                            return CodeBinaryOperatorType.Add;

                        case 2:
                            return 0; // TODO: increment operator

                        default:
                            throw new ParseException(ExUnexpected);
                    }

                case Minus:
                    switch (op.Length)
                    {
                        case 1:
                            return CodeBinaryOperatorType.Subtract;

                        case 2:
                            return 0; // TODO: decrement operator

                        default:
                            throw new ParseException(ExUnexpected);
                    }

                case Multiply:
                    switch (op.Length)
                    {
                        case 1:
                            return CodeBinaryOperatorType.Multiply;

                        case 2:
                            return 0; // TODO: power operator

                        default:
                            throw new ParseException(ExUnexpected);
                    }

                case Divide:
                    switch (op.Length)
                    {
                        case 1:
                            return CodeBinaryOperatorType.Divide;

                        case 2:
                            return 0; // TODO: floor divide operator

                        default:
                            throw new ParseException(ExUnexpected);
                    }

                case Greater:
                    switch (op.Length)
                    {
                        case 1:
                            return CodeBinaryOperatorType.GreaterThan;

                        case 2:
                            if (op[1] == op[0])
                                return 0; // TODO: bit shift right operator
                            else if (op[1] == Equal)
                                return CodeBinaryOperatorType.GreaterThanOrEqual;
                            else
                                throw new ParseException(ExUnexpected);

                        default:
                            throw new ParseException(ExUnexpected);
                    }

                case Less:
                    switch (op.Length)
                    {
                        case 1:
                            return CodeBinaryOperatorType.LessThan;

                        case 2:
                            if (op[1] == op[0])
                                return 0; // TODO: bit shift left operator
                             else if (op[1] == Equal)
                                return CodeBinaryOperatorType.LessThanOrEqual;
                            else
                                throw new ParseException(ExUnexpected);

                        default:
                            throw new ParseException(ExUnexpected);
                    }

                case BitAND:
                    switch (op.Length)
                    {
                        case 1:
                            return CodeBinaryOperatorType.BitwiseAnd;

                        case 2:
                            if (op[0] == op[1])
                                return CodeBinaryOperatorType.BooleanAnd;
                            else
                                throw new ParseException(ExUnexpected);

                        default:
                            throw new ParseException(ExUnexpected);
                    }

                case BitOR:
                    switch (op.Length)
                    {
                        case 1:
                            return CodeBinaryOperatorType.BitwiseOr;

                        case 2:
                            if (op[0] == op[1])
                                return CodeBinaryOperatorType.BooleanOr;
                            else
                                throw new ParseException(ExUnexpected);

                        default:
                            throw new ParseException(ExUnexpected);
                    }

                case Equal:
                    switch (op.Length)
                    {
                        case 1:
                            return CodeBinaryOperatorType.ValueEquality;

                        case 2:
                            if (op[1] == op[0])
                                return CodeBinaryOperatorType.IdentityEquality;
                            else
                                throw new ParseException(ExUnexpected);

                        default:
                            throw new ParseException(ExUnexpected);
                    }

                case Not:
                    switch (op.Length)
                    {
                        case 2:
                            if (op[1] == Equal)
                                return 0; // TODO: value inequality operator
                            else
                                throw new ParseException(ExUnexpected);

                        case 3:
                            if (op[1] == Equal && op[2] == Equal)
                                return CodeBinaryOperatorType.IdentityInequality;
                            else
                                throw new ParseException(ExUnexpected);

                        default:
                            throw new ParseException(ExUnexpected);
                    }
                    
                case AssignPre:
                    if (op.Length > 1 && op[1] == Equal)
                        return CodeBinaryOperatorType.Assign;
                    else
                        throw new ParseException(ExUnexpected);

                default:
                    if (code.Length == AndTxt.Length && code.Equals(AndTxt, StringComparison.OrdinalIgnoreCase))
                        return CodeBinaryOperatorType.BooleanAnd;
                    else if (code.Length == OrTxt.Length && code.Equals(OrTxt, StringComparison.OrdinalIgnoreCase))
                        return CodeBinaryOperatorType.BooleanOr;
                    break;
            }

            // CodeBinaryOperatorType.Modulus is unimplemented

            throw new ArgumentOutOfRangeException();
        }

        bool IsAssignOp(string code)
        {
            if (!(code.Length == 2 || code.Length == 3))
                return false;

            if (code[0] == Equal || code[code.Length - 1] != Equal)
                return false;

            if (code.Length == 3)
            {
                if (code[0] == code[1])
                {
                    switch (code[0])
                    {
                        case Greater:
                        case Less:
                            return true;

                        default:
                            return false;
                    }
                }
                else
                    return false;
            }
            else
                return true;
        }

        List<object> SplitTokens(string code)
        {
            var stream = new StringReader(code);
            var list = new List<object>();
            char sym;

            while (true)
            {
                int ch = stream.Read();
                if (ch == -1)
                    break;

                sym = (char)ch;

                if (IsSpace(sym))
                    continue;
                #region Identifiers
                else if (IsIdentifier(sym))
                {
                    var id = new StringBuilder(code.Length);
                    id.Append(sym);
                    while (true)
                    {
                        int next = stream.Peek();
                        if (next == -1)
                            break;
                        char symNext = (char)next;
                        if (IsIdentifier(symNext) || symNext == Resolve)
                        {
                            id.Append(symNext);
                            stream.Read();
                        }
                        else
                            break;
                    }
                    if ((char)stream.Peek() == ParenOpen)
                    {
                        id.Append(ParenOpen);
                        stream.Read();
                    }
                    list.Add(id.ToString());
                }
                #endregion
                #region Strings
                else if (sym == StringBound)
                {
                    var str = new StringBuilder(code.Length);
                    str.Append(StringBound);
                    while (true)
                    {
                        int next = stream.Peek();
                        if (next == -1)
                            throw new ParseException("Unterminated string");
                        sym = (char)next;
                        stream.Read();
                        str.Append(sym);
                        if (sym == StringBound)
                        {
                            if ((char)stream.Peek() == StringBound)
                                stream.Read();
                            else
                                break;
                        }
                    }
                    list.Add(str.ToString());
                }
                #endregion
                #region Operators
                else
                {
                    var op = new StringBuilder(3);
                    int next = stream.Peek();
                    char symNext = (char)next;

                    #region 3x
                    if (sym == symNext)
                    {
                        bool peekAssign = false;

                        switch (sym)
                        {
                            case Divide:
                            case Greater:
                            case Less:
                                peekAssign = true;
                                goto case Add;

                            case Add:
                            case Minus:
                            case Multiply:
                            case BitOR:
                                op.Append(sym);
                                op.Append(symNext);
                                stream.Read();
                                if (peekAssign)
                                {
                                    next = stream.Peek();
                                    if (next != -1)
                                    {
                                        symNext = (char)next;
                                        if (symNext == Equal)
                                        {
                                            op.Append(symNext);
                                            stream.Read();
                                        }
                                    }
                                }
                                break;
                        }
                    }
                    #endregion
                    else
                    {
                        #region 2x
                        if (symNext == Equal)
                        {
                            switch (sym)
                            {
                                case Not:
                                case AssignPre:
                                case Minus:
                                case Multiply:
                                case Divide:
                                case Concatenate:
                                case BitOR:
                                case BitAND:
                                case BitXOR:
                                    op.Append(sym);
                                    op.Append(symNext);
                                    stream.Read();
                                    break;
                            }
                        }
                        #endregion
                        #region 1x
                        else
                        {
                            switch (sym)
                            {
                                case Resolve:
                                case Add:
                                case Minus:
                                case Multiply:
                                case Not:
                                case BitNOT:
                                case BitAND:
                                case Greater:
                                case Less:
                                case BitXOR:
                                case BitOR:
                                case Multicast:
                                case ParenOpen:
                                case ParenClose:
                                    op.Append(sym);
                                    break;

                                default:
                                    throw new ParseException(ExUnexpected);
                            }
                        }
                        #endregion
                    }

                    if (op.Length == 0)
                        op.Append(sym);
                    list.Add(op.ToString());
                }
                #endregion
            }

            return list;
        }
    }
}
