using System;
using System.CodeDom;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace IronAHK.Scripting
{
    partial class Parser
    {
        #region Wrappers

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

        #endregion

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
                    #region Numerics
                    else if (IsPrimativeObject(part, out result))
                        parts[i] = new CodePrimitiveExpression(result);
                    #endregion
                    #region Variables
                    else if (IsIdentifier(part[0]))
                        parts[i] = VarId(part);
                    #endregion
                    #region Strings
                    else if (part.Length > 2 && part[0] == StringBound && part[part.Length - 1] == StringBound)
                        parts[i] = part.Substring(1, part.Length - 2);
                    #endregion
                    #region Assignments
                    else if (IsAssignOp(part))
                    {
                        int n = i - 1;
                        if (n < 0 || !(parts[n] is CodeComplexVariableReferenceExpression))
                            throw new ParseException("Can only assign to a variable");

                        // (x += y) => (x = x + y)
                        parts[i] = new CodeComplexAssignStatement();
                        if (part[0] != AssignPre)
                        {
                            i++;
                            parts.Insert(i, parts[i - 2]);
                            i++;
                            parts.Insert(i, BinaryOperator(part.Substring(0, part.Length - 1)));
                        }
                    }
                    #endregion
                    #region Multiple statements
                    else if (part.Length == 1 && part[0] == Multicast)
                    {
                        throw new ParseException("Multiple expression statements not allowed here");
                    }
                    #endregion
                    #region Binary operators
                    else
                    {
                        parts[i] = BinaryOperator(part); // TODO: unary operators
                    }
                    #endregion
                }
            }

            #region Binary operators

            var op = new CodeMethodReferenceExpression();
            op.TargetObject = new CodeThisReferenceExpression();
            op.MethodName = "Operate";
            bool scan = true;

            // HACK: operator precedence

            while (scan)
            {
                scan = false;
                for (int i = 0; i < parts.Count; i++)
                {
                    if (parts[i] is Script.Operator)
                    {
                        scan = true;
                        int x = i - 1, y = i + 1;
                        var invoke = new CodeMethodInvokeExpression();
                        invoke.Method = op;
                        invoke.Parameters.Add(OperatorReference((Script.Operator)parts[i]));
                        invoke.Parameters.Add((CodeExpression)parts[x]);
                        invoke.Parameters.Add((CodeExpression)parts[y]);
                        parts[x] = invoke;
                        parts.RemoveAt(y);
                        parts.RemoveAt(i);
                    }
                }
            }

            #endregion

            #region Assignments
            for (int i = parts.Count - 1; i > 0; i--)
            {
                if (parts[i] is CodeComplexAssignStatement)
                {
                    int x = i - 1, y = i + 1;
                    var assign = (CodeComplexAssignStatement)parts[i];
                    assign.Left = (CodeComplexVariableReferenceExpression)parts[x];
                    assign.Right = (CodeExpression)parts[y];
                    parts[i] = (CodeMethodInvokeExpression)assign;
                    parts.RemoveAt(x);
                    parts.RemoveAt(i);
                }
            }
            #endregion

            if (parts.Count != 1)
                throw new ArgumentOutOfRangeException();

            return (CodeExpression)parts[0];
        }

        #region Operators

        Script.Operator BinaryOperator(string code)
        {
            char[] op = code.ToCharArray();

            switch (op[0])
            {
                case Add:
                    switch (op.Length)
                    {
                        case 1:
                            return Script.Operator.Add;

                        case 2:
                            return Script.Operator.Increment;

                        default:
                            throw new ParseException(ExUnexpected);
                    }

                case Minus:
                    switch (op.Length)
                    {
                        case 1:
                            return Script.Operator.Subtract;

                        case 2:
                            return Script.Operator.Decrement;

                        default:
                            throw new ParseException(ExUnexpected);
                    }

                case Multiply:
                    switch (op.Length)
                    {
                        case 1:
                            return Script.Operator.Multiply;

                        case 2:
                            return Script.Operator.Power;

                        default:
                            throw new ParseException(ExUnexpected);
                    }

                case Divide:
                    switch (op.Length)
                    {
                        case 1:
                            return Script.Operator.Divide;

                        case 2:
                            return Script.Operator.FloorDivide;

                        default:
                            throw new ParseException(ExUnexpected);
                    }

                case Greater:
                    switch (op.Length)
                    {
                        case 1:
                            return Script.Operator.GreaterThan;

                        case 2:
                            if (op[1] == op[0])
                                return Script.Operator.BitShiftRight;
                            else if (op[1] == Equal)
                                return Script.Operator.GreaterThanOrEqual;
                            else
                                throw new ParseException(ExUnexpected);

                        default:
                            throw new ParseException(ExUnexpected);
                    }

                case Less:
                    switch (op.Length)
                    {
                        case 1:
                            return Script.Operator.LessThan;

                        case 2:
                            if (op[1] == op[0])
                                return Script.Operator.BitShiftLeft;
                            else if (op[1] == Equal)
                                return Script.Operator.LessThanOrEqual;
                            else
                                throw new ParseException(ExUnexpected);

                        default:
                            throw new ParseException(ExUnexpected);
                    }

                case BitAND:
                    switch (op.Length)
                    {
                        case 1:
                            return Script.Operator.BitwiseAnd;

                        case 2:
                            if (op[0] == op[1])
                                return Script.Operator.BooleanAnd;
                            else
                                throw new ParseException(ExUnexpected);

                        default:
                            throw new ParseException(ExUnexpected);
                    }

                case BitOR:
                    switch (op.Length)
                    {
                        case 1:
                            return Script.Operator.BitwiseOr;

                        case 2:
                            if (op[0] == op[1])
                                return Script.Operator.BooleanOr;
                            else
                                throw new ParseException(ExUnexpected);

                        default:
                            throw new ParseException(ExUnexpected);
                    }

                case Equal:
                    switch (op.Length)
                    {
                        case 1:
                            return Script.Operator.ValueEquality;

                        case 2:
                            if (op[1] == op[0])
                                return Script.Operator.IdentityEquality;
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
                                return Script.Operator.ValueInequality;
                            else
                                throw new ParseException(ExUnexpected);

                        case 3:
                            if (op[1] == Equal && op[2] == Equal)
                                return Script.Operator.IdentityInequality;
                            else
                                throw new ParseException(ExUnexpected);

                        default:
                            throw new ParseException(ExUnexpected);
                    }
                    
                case AssignPre:
                    if (op.Length > 1 && op[1] == Equal)
                        return Script.Operator.Assign;
                    else
                        throw new ParseException(ExUnexpected);

                default:
                    if (code.Length == AndTxt.Length && code.Equals(AndTxt, StringComparison.OrdinalIgnoreCase))
                        return Script.Operator.BooleanAnd;
                    else if (code.Length == OrTxt.Length && code.Equals(OrTxt, StringComparison.OrdinalIgnoreCase))
                        return Script.Operator.BooleanOr;
                    break;
            }

            throw new ArgumentOutOfRangeException();
        }

        CodeFieldReferenceExpression OperatorReference(Script.Operator op)
        {
            var field = new CodeFieldReferenceExpression();
            field.TargetObject = new CodeTypeReferenceExpression(typeof(Script.Operator));
            field.FieldName = op.ToString();
            return field;
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

        #endregion

        #region Texts

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

        #endregion
    }
}
