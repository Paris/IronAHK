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

        CodeExpression[] ParseMultiExpression(object[] parts)
        {
            var expr = new List<CodeExpression>();
            var sub = new List<object>();

            for (int i = 0; i < parts.Length; i++)
            {
                if (parts[i] is string)
                {
                    string part = (string)parts[i];
                    if (part.Length == 1 && part[0] == Multicast)
                    {
                        if (sub.Count == 0)
                            expr.Add(new CodePrimitiveExpression(null));
                        else
                        {
                            expr.Add(ParseExpression(sub));
                            sub = new List<object>();
                        }
                    }
                    else
                        sub.Add(parts[i]);
                }
                else
                    sub.Add(parts[i]);
            }

            if (sub.Count != 0)
                expr.Add(ParseExpression(sub));

            return expr.ToArray();
        }

        #endregion

        #region Parser

        CodeExpression ParseExpression(List<object> parts)
        {
            #region Scanner

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
                    }
                    #endregion
                    #region Strings
                    else if (part.Length > 1 && part[0] == StringBound && part[part.Length - 1] == StringBound)
                        parts[i] = new CodePrimitiveExpression(part.Substring(1, part.Length - 2));
                    #endregion
                    #region Numerics
                    else if (IsPrimativeObject(part, out result))
                        parts[i] = new CodePrimitiveExpression(result);
                    #endregion
                    #region Variables
                    else if (IsIdentifier(part))
                        parts[i] = VarId(part);
                    #endregion
                    #region Invokes
                    else if (part.Length > 1 && part[part.Length - 1] == ParenOpen)
                    {
                        string name = part.Substring(0, part.Length - 1);
                        bool dynamic = false;

                        if (!IsIdentifier(name))
                        {
                            if (IsDynamicReference(name))
                                dynamic = true;
                            else 
                                throw new ParseException("Invalid function name");
                        }

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
                                        int count = x - i - 1;

                                        object[] sub = new object[count];

                                        for (int n = 0; n < count; n++)
                                            sub[n] = parts[i + 1 + n];

                                        CodeMethodInvokeExpression invoke;

                                        if (dynamic)
                                        {
                                            invoke = (CodeMethodInvokeExpression)InternalMethods.FunctionCall;
                                            invoke.Parameters.Add(VarNameOrBasicString(name, true));
                                        }
                                        else
                                            invoke = LocalMethodInvoke(name);

                                        if (count != 0)
                                        {
                                            var passed = ParseMultiExpression(sub);
                                            var obj = new CodeArrayCreateExpression();
                                            obj.Size = passed.Length;
                                            obj.CreateType = new CodeTypeReference(typeof(object));
                                            obj.Initializers.AddRange(passed);
                                            invoke.Parameters.Add(obj);
                                        }

                                        parts.RemoveRange(i, count + 2);
                                        parts.Insert(i, invoke);

                                        next = true;
                                    }
                                    break;
                            }
                            if (next)
                                break;
                        }
                        if (levels != 0)
                            throw new ParseException(ExUnbalancedParens);
                    }
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
                            parts.Insert(i, OperatorFromString(part.Substring(0, part.Length - 1)));
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
                        var ops = OperatorFromString(part);

                        if (ops == Script.Operator.Increment || ops == Script.Operator.Decrement)
                        {
                            int z = -1, x = i - 1, y = i + 1;

                            if (x > -1 && parts[x] is CodeComplexVariableReferenceExpression)
                                z = x;

                            if (y < parts.Count && IsIdentifier(parts[y] as string))
                            {
                                if (z != -1)
                                    throw new ParseException("Cannot use both prefix and postfix operators on the same variable");
                                z = y;
                            }

                            if (z == -1)
                                throw new ParseException("Neither left or right hand side of operator is a variable");

                            var list = new List<object>(7);
                            list.Add(parts[z]);
                            list.Add(new string(new char[] { ops == Script.Operator.Increment ? Add : Minus, Equal }));
                            const string d = "1";
                            list.Add(d);
                            if (z < i) // postfix, so adjust
                            {
                                list.Insert(0, ParenOpen.ToString());
                                list.Add(ParenClose.ToString());
                                list.Add((((string)list[2])[0] == Add ? Minus : Add).ToString());
                                list.Add(d);
                            }

                            x = Math.Min(i, z);
                            y = Math.Max(i, z);
                            parts[x] = ParseExpression(list);
                            parts.RemoveAt(y);
                            i = x;
                        }
                        else
                        {
                            parts[i] = ops;
                        }
                    }
                    #endregion
                }
            }

            #endregion

            #region Operators

            var calc = (CodeMethodReferenceExpression)InternalMethods.Operate;
            bool scan = true;
            int level = -1;

            while (scan)
            {
                scan = false;
                for (int i = 0; i < parts.Count; i++)
                {
                    if (parts[i] is Script.Operator)
                    {
                        scan = true;
                        var op = (Script.Operator)parts[i];

                        if (OperatorPrecedence(op) < level)
                            continue;

                        int x = i - 1, y = i + 1;
                        var invoke = new CodeMethodInvokeExpression();

                        #region Ternary
                        if (op == Script.Operator.TernaryA)
                        {
                            const bool TERNARY_AS_DELEGATES = false;
#pragma warning disable 0162

                            CodeTernaryOperatorExpression ternary;
                            var eval = (CodeMethodInvokeExpression)InternalMethods.IfElse;
                            eval.Parameters.Add(ExpressionNode(parts[x]));

                            const int n = 2;
                            CodeMethodReturnStatement[] r;

                            if (TERNARY_AS_DELEGATES)
                            {
                                invoke.Method = (CodeMethodReferenceExpression)InternalMethods.OperateTernary;
                                invoke.Parameters.Add(eval);
                                r = new CodeMethodReturnStatement[n];

                                for (int z = 0; z < n; z++)
                                {
                                    string id = InternalID;
                                    var d = new CodeDelegateCreateExpression(new CodeTypeReference(typeof(Script.ExpressionDelegate)), new CodeTypeReferenceExpression(className), id);
                                    invoke.Parameters.Add(d);
                                    var m = new CodeMemberMethod() { Name = id, ReturnType = new CodeTypeReference(typeof(object)), Attributes = MemberAttributes.Static };
                                    r[z] = new CodeMethodReturnStatement();
                                    m.Statements.Add(r[z]);
                                    methods.Add(id, m);
                                }
                            }
                            else
                                ternary = new CodeTernaryOperatorExpression() { Condition = eval };

                            int depth = 0, max = parts.Count - i, start = i, index = 0;
                            var branch = new List<object>[] { new List<object>(max), new List<object>(max) };

                            for (i++; i < parts.Count; i++)
                            {
                                if (parts[i] is Script.Operator)
                                {
                                    var iop = (Script.Operator)parts[i];

                                    switch (iop)
                                    {
                                        case Script.Operator.TernaryA:
                                            depth++;
                                            break;

                                        case Script.Operator.TernaryB:
                                            if (--depth == -1)
                                                index = 1;
                                            break;

                                        default:
                                            branch[index].Add(parts[i]);
                                            break;
                                    }
                                }
                                else
                                    branch[index].Add(parts[i]);
                            }

                            if (branch[0].Count == 0)
                                throw new ParseException("Ternary operator must have at least one branch");

                            if (branch[1].Count == 0)
                                branch[1].Add(new CodePrimitiveExpression(null));

                            if (TERNARY_AS_DELEGATES)
                            {
                                for (int z = 0; z < n; z++)
                                    r[z].Expression = ParseExpression(branch[z]);
                                parts[x] = invoke;
                            }
                            else
                            {
                                ternary.TrueBranch = ParseExpression(branch[0]);
                                ternary.FalseBranch = ParseExpression(branch[1]);
                                parts[x] = ternary;
                            }

                            parts.Remove(y);
                            parts.RemoveRange(start, parts.Count - start);

#pragma warning restore 0162
                        }
                        #endregion
                        #region Binary
                        else
                        {
                            invoke.Method = calc;
                            invoke.Parameters.Add(OperatorAsFieldReference(op));
                            invoke.Parameters.Add(ExpressionNode(parts[x]));
                            invoke.Parameters.Add(ExpressionNode(parts[y]));

                            parts[x] = invoke;
                            parts.RemoveAt(y);
                            parts.RemoveAt(i);
                        }
                        #endregion
                    }
                }
                level--;
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
                    assign.Right = parts[y] is CodeComplexVariableReferenceExpression ? 
                        (CodeMethodInvokeExpression)(CodeComplexVariableReferenceExpression)parts[y] : (CodeExpression)parts[y];
                    parts[i] = (CodeMethodInvokeExpression)assign;
                    parts.RemoveAt(x);
                    parts.RemoveAt(i);
                }
            }
            #endregion

            #region Variables
            for (int i = 0; i < parts.Count; i++)
                if (parts[i] is CodeComplexVariableReferenceExpression)
                    parts[i] = (CodeMethodInvokeExpression)(CodeComplexVariableReferenceExpression)parts[i];
            #endregion

            if (parts.Count != 1)
                throw new ArgumentOutOfRangeException();

            return (CodeExpression)parts[0];
        }

        CodeExpression ExpressionNode(object part)
        {
            return part is CodeComplexVariableReferenceExpression ?
                (CodeMethodInvokeExpression)(CodeComplexVariableReferenceExpression)part : (CodeExpression)part;
        }

        #endregion

        #region Operators

        Script.Operator OperatorFromString(string code)
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
                        case 1:
                            return Script.Operator.LogicalNot;

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
                        return Script.Operator.TernaryB;

                case Concatenate:
                    return Script.Operator.Concat;

                case TernaryA:
                    return Script.Operator.TernaryA;

                default:
                    if (code.Length == AndTxt.Length && code.Equals(AndTxt, StringComparison.OrdinalIgnoreCase))
                        return Script.Operator.BooleanAnd;
                    else if (code.Length == OrTxt.Length && code.Equals(OrTxt, StringComparison.OrdinalIgnoreCase))
                        return Script.Operator.BooleanOr;
                    break;
            }

            throw new ArgumentOutOfRangeException();
        }

        int OperatorPrecedence(Script.Operator op)
        {
            switch (op)
            {
                case Script.Operator.Power:
                    return -1;

                case Script.Operator.Minus:
                case Script.Operator.LogicalNot:
                case Script.Operator.BitwiseNot:
                case Script.Operator.Address:
                case Script.Operator.Dereference:
                    return -2;

                case Script.Operator.Multiply:
                case Script.Operator.Divide:
                case Script.Operator.FloorDivide:
                    return -3;

                case Script.Operator.Add:
                case Script.Operator.Subtract:
                    return -4;

                case Script.Operator.BitShiftLeft:
                case Script.Operator.BitShiftRight:
                    return -5;

                case Script.Operator.BitwiseAnd:
                case Script.Operator.BitwiseXor:
                case Script.Operator.BitwiseOr:
                    return -6;

                case Script.Operator.Concat:
                    return -7;

                case Script.Operator.GreaterThan:
                case Script.Operator.LessThan:
                case Script.Operator.GreaterThanOrEqual:
                case Script.Operator.LessThanOrEqual:
                    return -8;

                case Script.Operator.ValueEquality:
                case Script.Operator.IdentityEquality:
                case Script.Operator.ValueInequality:
                case Script.Operator.IdentityInequality:
                    return -9;

                case Script.Operator.LogicalNotEx:
                    return -10;

                case Script.Operator.BooleanAnd:
                case Script.Operator.BooleanOr:
                    return -11;

                case Script.Operator.TernaryA:
                case Script.Operator.TernaryB:
                    return -12;

                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        CodeFieldReferenceExpression OperatorAsFieldReference(Script.Operator op)
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
            {
                switch (code[0])
                {
                    case Greater:
                    case Less:
                    case Not:
                        return false;

                    default:
                        return true;
                }
            }
        }

        CodeMethodInvokeExpression LocalMethodInvoke(string name)
        {
            var invoke = new CodeMethodInvokeExpression();
            invoke.Method.MethodName = name;
            invoke.Method.TargetObject = null;
            return invoke;
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
                else if (IsIdentifier(sym) || sym == Resolve)
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
                    bool tri = false;

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
                            case BitAND:
                                op.Append(sym);
                                op.Append(symNext);
                                stream.Read();
                                tri = true;
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
                    
                    if (!tri)
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
                                case Equal:
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
                                case Equal:
                                case Concatenate:
                                case TernaryB:
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
