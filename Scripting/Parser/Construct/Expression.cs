using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Text;

namespace IronAHK.Scripting
{
    partial class Parser
    {
        #region Wrappers

        CodeExpressionStatement[] ParseMultiExpression(string code)
        {
            var tokens = SplitTokens(code);

            #region Date/time

            int n = tokens.Count - 2;
            if (tokens.Count > 1 && ((string)tokens[n]).Length > 0 && ((string)tokens[n])[0] == Multicast)
            {
                string arg = ((string)tokens[n + 1]).ToUpperInvariant().Trim();
                arg = arg.Length == 1 ? arg : arg.TrimEnd('S');

                switch (arg)
                {
                    case "S":
                    case "SECOND":
                    case "M":
                    case "MINUTE":
                    case "H":
                    case "HOUR":
                    case "D":
                    case "DAY":
                        return new CodeExpressionStatement[] { new CodeExpressionStatement(ParseDateExpression(code)) };
                }
            }

            #endregion

            var result = ParseMultiExpression(tokens.ToArray());
            var statements = new CodeExpressionStatement[result.Length];

            for (int i = 0; i < result.Length; i++)
                statements[i] = new CodeExpressionStatement(result[i]);

            return statements;
        }

        CodeExpression ParseSingleExpression(string code)
        {
            var tokens = SplitTokens(code);
            return ParseExpression(tokens);
        }

        CodeExpression[] ParseMultiExpression(object[] parts)
        {
            var expr = new List<CodeExpression>();
            var sub = new List<object>();

            int level = 0;

            for (int i = 0; i < parts.Length; i++)
            {
                if (string.IsNullOrEmpty(parts[i] as string))
                    goto end;

                string check = (string)parts[i];

                if (check.Length > 1)
                {
                    if (check[check.Length - 1] == ParenOpen)
                        level++;
                    goto end;
                }

                switch (check[0])
                {
                    case ParenOpen:
                        level++;
                        break;

                    case ParenClose:
                        level--;
                        break;

                    case Multicast:
                        if (level == 0 && sub.Count != 0)
                        {
                            expr.Add(ParseExpression(sub));
                            sub.Clear();
                            continue;
                        }
                        break;
                }

            end:
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
            RemoveExcessParentheses(parts);

            #region Scanner

        start:
            bool rescan = false;

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
                            if (string.IsNullOrEmpty(parts[x] as string))
                                continue;

                            string check = (string)parts[x];

                            switch (check[check.Length - 1])
                            {
                                case ParenOpen:
                                    levels++;
                                    break;

                                case ParenClose:
                                    if (check.Length != 1)
                                        break;

                                    levels--;

                                    if (levels < 0)
                                        throw new ParseException(ExUnbalancedParens);
                                    else if (levels == 0)
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
                    else if (part[0] == ParenClose)
                        rescan = true;
                    #endregion
                    #region Strings
                    else if (part.Length > 1 && part[0] == StringBound && part[part.Length - 1] == StringBound)
                        parts[i] = new CodePrimitiveExpression(EscapedString(part.Substring(1, part.Length - 2), false));
                    #endregion
                    #region Numerics
                    else if (IsPrimativeObject(part, out result))
                        parts[i] = new CodePrimitiveExpression(result);
                    #endregion
                    #region Variables
                    else if (IsIdentifier(part, true) && !IsKeyword(part))
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
                            if (string.IsNullOrEmpty(parts[x] as string))
                                continue;

                            string current = (string)parts[x];

                            switch (current[0])
                            {
                                case ParenOpen:
                                    levels++;
                                    break;

                                default:
                                    if (current[current.Length - 1] == ParenOpen)
                                        goto case ParenOpen;
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
                                            invoke.Parameters.AddRange(passed);
                                            invokes.Add(invoke);
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
                    else if (IsAssignOp(part) || IsImplicitAssignment(parts, i))
                    {
                        int n = i - 1;
                        if (n < 0 || !(parts[n] is CodeComplexVariableReferenceExpression))
                        {
                            if (LaxExpressions)
                            {
                                if (parts[n] is CodePrimitiveExpression &&
                                    ((CodePrimitiveExpression)parts[n]).Value is decimal)
                                    parts[n] = VarId(((decimal)((CodePrimitiveExpression)parts[n]).Value).ToString());
                            }
                            else
                                throw new ParseException("Can only assign to a variable");
                        }

                        // (x += y) => (x = x + y)
                        parts[i] = new CodeComplexAssignStatement();
                        if (part[0] != AssignPre && part.Length != 1)
                        {
                            parts.Insert(++i, ParenOpen.ToString());
                            parts.Insert(++i, parts[i - 3]);
                            if (part.Length > 1)
                            {
                                parts.Insert(++i, OperatorFromString(part.Substring(0, part.Length - 1)));
                                parts.Insert(++i, ParenOpen.ToString());
                                parts.Add(ParenClose.ToString());
                            }
                            parts.Add(ParenClose.ToString());
                        }
                    }
                    #endregion
                    #region Multiple statements
                    else if (part.Length == 1 && part[0] == Multicast)
                    {
#pragma warning disable 0162
                        if (!AllowNestedMultipartExpressions)
                            throw new ParseException("Nested multipart expression not allowed.");

                        // implement as: + Dummy(expr..)

                        int z = i + 1, l = parts.Count - z;
                        var sub = new List<object>(l);

                        for (; z < parts.Count; z++)
                            sub.Add(parts[z]);

                        parts.RemoveRange(i, parts.Count - i);

                        var invoke = (CodeMethodInvokeExpression)InternalMethods.OperateZero;
                        invoke.Parameters.Add(ParseExpression(sub));

                        parts.Add(Script.Operator.Add);
                        parts.Add(invoke);
#pragma warning restore 0162
                    }
                    #endregion
                    #region Binary operators
                    else
                    {
                        var ops = OperatorFromString(part);

                        #region Increment/decrement
                        if (ops == Script.Operator.Increment || ops == Script.Operator.Decrement)
                        {
                            int z = -1, x = i - 1, y = i + 1;
                            int d = ops == Script.Operator.Increment ? 1 : -1;
                            CodeMethodInvokeExpression shadow = null;

                            #region Compounding increment/decrement operators

                            if (LaxExpressions)
                            {
                                while (y < parts.Count)
                                {
                                    Script.Operator nextOps = Script.Operator.ValueEquality;

                                    if (parts[y] is Script.Operator)
                                        nextOps = (Script.Operator)parts[y];
                                    else if (parts[y] is string)
                                    {
                                        try { nextOps = OperatorFromString((string)parts[y]); }
                                        catch { break; }
                                    }
                                    else
                                        break;

                                    if (nextOps == Script.Operator.Increment)
                                        d++;
                                    else if (nextOps == Script.Operator.Decrement)
                                        d--;
                                    else
                                        break;

                                    parts.RemoveAt(y);
                                }
                            }

                            #endregion

                            if (x > -1 && parts[x] is CodeComplexVariableReferenceExpression)
                                z = x;

                            if (y < parts.Count && parts[y] is string && !IsOperator((string)parts[y]))
                            {
                                if (z != -1)
                                {
                                    if (LaxExpressions)
                                    {
                                        parts.Insert(y, Script.Operator.Concat);
                                        z = x;
                                    }
                                    else
                                        throw new ParseException("Cannot use both prefix and postfix operators on the same variable");
                                }

                                if (z == -1)
                                    z = y;

                                if (LaxExpressions)
                                {
                                    if (parts[z] is string && ((string)parts[z]).Length == 1 && ((string)parts[z])[0] == ParenOpen)
                                    {
                                        int[] zx = new int[] { z + 1, z + 2 };
                                        if (zx[1] < parts.Count &&
                                            parts[zx[1]] is string && ((string)parts[zx[1]]).Length == 1 && ((string)parts[zx[1]])[0] == ParenClose &&
                                            (parts[zx[0]] is string && IsDynamicReference((string)parts[zx[0]]) || parts[zx[0]] is CodeComplexVariableReferenceExpression))
                                        {
                                            parts.RemoveAt(zx[1]);
                                            parts.RemoveAt(z);
                                        }
                                        else
                                        {
                                            parts.RemoveAt(i);
                                            i--;
                                            continue;
                                        }
                                    }
                                }
                            }

                            if (z == -1)
                            {
                                if (LaxExpressions)
                                {
                                    if ((x > 0 && (parts[x] is CodeBinaryOperatorExpression || parts[x] is CodeMethodInvokeExpression || parts[x] is CodePrimitiveExpression)) ||
                                        (y < parts.Count && (parts[y] is string && !IsOperator(parts[y] as string) || parts[y] is Script.Operator)))
                                    {
                                        parts.RemoveAt(i);
                                        i--;
                                        continue;
                                    }
                                }
                                else
                                    throw new ParseException("Neither left or right hand side of operator is a variable");
                            }

                            if (parts[z] is string && ((string)parts[z]).Length > 0 && ((string)parts[z])[0] == StringBound)
                            {
                                parts.RemoveAt(Math.Max(i, z));
                                parts.RemoveAt(Math.Min(i, z));
                                continue;
                            }

                            if (LaxExpressions)
                            {
                                int w = z + (z == x ? 2 : 1);
                                if (w < parts.Count && (parts[w] is string && IsAssignOp((string)parts[w]) || parts[w] is CodeComplexAssignStatement))
                                {
                                    int l = parts.Count - w;
                                    var sub = new List<object>(l + 1);

                                    sub.Add(parts[z]);
                                    for (int wx = w; wx < parts.Count; wx++)
                                        sub.Add(parts[wx]);

                                    shadow = (CodeMethodInvokeExpression)InternalMethods.OperateZero;
                                    shadow.Parameters.Add(ParseExpression(sub));

                                    parts.RemoveRange(w, l);
                                }
                            }

                            var list = new List<object>(9);
                            list.Add(parts[z]);
                            list.Add(new string(new char[] { Add, Equal }));
                            list.Add(d.ToString());
                            if (shadow != null)
                            {
                                list.Add(Add.ToString());
                                list.Add(shadow);
                            }
                            if (z < i) // postfix, so adjust
                            {
                                list.Insert(0, ParenOpen.ToString());
                                list.Add(ParenClose.ToString());
                                list.Add(Add.ToString());
                                list.Add((-d).ToString());
                            }

                            x = Math.Min(i, z);
                            y = Math.Max(i, z);
                            parts[x] = ParseExpression(list);
                            parts.RemoveAt(y);
                            i = x;
                        }
                        #endregion
                        else
                        {
                            #region Dereference
                            if (part.Length == 1 && part[0] == Dereference)
                            {
                                bool deref = false;

                                if (i == 0)
                                    deref = true;
                                else
                                {
                                    int x = i - 1;
                                    deref = parts[x] is Script.Operator || parts[x] is CodeComplexAssignStatement ||
                                        (parts[x] is string && ((string)parts[x]).Length == 1 && ((string)parts[x])[0] == '(');
                                }

                                if (deref)
                                {
                                    int y = i + 1;
                                    if (y < parts.Count && (parts[y] is CodeComplexVariableReferenceExpression ||
                                        (parts[y] is string && IsIdentifier((string)parts[y]) && !IsKeyword((string)parts[y]))))
                                        ops = Script.Operator.Dereference;
                                }
                            }
                            #endregion

                            parts[i] = ops;
                        }
                    }
                    #endregion
                }
            }

            if (rescan)
                goto start;

            #endregion

            #region Operators

            #region Unary (precedent)

            for (int i = 1; i < parts.Count; i++)
            {
                if (parts[i] is Script.Operator && (parts[i - 1] is Script.Operator || parts[i - 1] is CodeComplexAssignStatement) && IsUnaryOperator((Script.Operator)parts[i]))
                {
                    int n = i + 1, m = n + 1;

                    int u = n;
                    while (u < parts.Count && parts[u] is Script.Operator && IsUnaryOperator((Script.Operator)parts[u])) u++;
                    if (u == parts.Count)
                    {
                        if (LaxExpressions)
                        {
                            u--;
                            while (parts[u] is Script.Operator && ((Script.Operator)parts[u] == Script.Operator.Add || (Script.Operator)parts[u] == Script.Operator.Subtract))
                                parts.RemoveAt(u--);

                            if (u + 1 < n)
                            {
                                i = u;
                                continue;
                            }
                        }

                        throw new ParseException("Compounding unary operator with no operand");
                    }

                    if (u > n)
                    {
                        var sub = new List<object>(++u - n);
                        for (int x = n; x < u; x++)
                            sub.Add(parts[x]);
                        parts.RemoveRange(n, u - n);
                        parts.Insert(n, ParseExpression(sub));
                    }

                    if (m + 1 < parts.Count && parts[n] is CodeComplexVariableReferenceExpression && parts[m] is CodeComplexAssignStatement)
                        MergeAssignmentAt(parts, i + 2);

                    if (m > parts.Count)
                        throw new ParseException("Unary operator without operand");

                    var op = (Script.Operator)parts[i];

                    if (parts[n] is CodePrimitiveExpression && op == Script.Operator.Subtract)
                    {
                        var parent = ((CodePrimitiveExpression)parts[n]);

                        if (parent.Value is int)
                            parent.Value = -(int)parent.Value;
                        else if (parent.Value is decimal)
                            parent.Value = -(decimal)parent.Value;
                        else if (parent.Value is double)
                            parent.Value = -(double)parent.Value;
                        else if (parent.Value is string)
                            parent.Value = string.Concat(Minus.ToString(), (string)parent.Value);
                        else
                            throw new ArgumentOutOfRangeException();

                        parts.RemoveAt(i);
                    }
                    else if (op == Script.Operator.Add)
                    {
                        parts.RemoveAt(i);
                    }
                    else
                    {
                        var invoke = (CodeMethodInvokeExpression)InternalMethods.OperateUnary;
                        invoke.Parameters.Add(OperatorAsFieldReference(op));

                        if (LaxExpressions)
                        {
                            if (!(parts[n] is CodeComplexVariableReferenceExpression || parts[n] is CodeComplexAssignStatement))
                            {
                                invoke.Parameters.Add(new CodePrimitiveExpression(null));
                                goto next;
                            }
                        }
                        invoke.Parameters.Add(WrappedComplexVar(parts[n]));

                    next:
                        parts[i] = invoke;
                        parts.RemoveAt(n);
                    }
                }
            }

            #endregion

            #region Generic

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

                        if (i + 3 < parts.Count && parts[i + 1] is CodeComplexVariableReferenceExpression && parts[i + 2] is CodeComplexAssignStatement)
                            MergeAssignmentAt(parts, i + 2);

                        #region Ternary
                        if (op == Script.Operator.TernaryA)
                        {
                            if (x < 0)
                            {
                                if (LaxExpressions)
                                    return new CodePrimitiveExpression(null);
                                else
                                    throw new ParseException("Ternary with no condition.");
                            }

                            var eval = (CodeMethodInvokeExpression)InternalMethods.IfElse;
                            eval.Parameters.Add(WrappedComplexVar(parts[x]));
                            var ternary = new CodeTernaryOperatorExpression() { Condition = eval };

                            int depth = 1, max = parts.Count - i, start = i;
                            var branch = new List<object>[] { new List<object>(max), new List<object>(max) };

                            for (i++; i < parts.Count; i++)
                            {
                                switch (parts[i] as Script.Operator?)
                                {
                                    case Script.Operator.TernaryA:
                                        depth++;
                                        break;

                                    case Script.Operator.TernaryB:
                                        depth--;
                                        break;
                                }

                                if (depth == 0)
                                {
                                    for (int n = i + 1; n < parts.Count; n++)
                                        branch[1].Add(parts[n]);
                                    break;
                                }
                                else
                                    branch[0].Add(parts[i]);
                            }

                            if (branch[0].Count == 0)
                                throw new ParseException("Ternary operator must have at least one branch");

                            if (branch[1].Count == 0)
                                branch[1].Add(new CodePrimitiveExpression(null));

                            ternary.TrueBranch = ParseExpression(branch[0]);
                            ternary.FalseBranch = ParseExpression(branch[1]);
                            parts[x] = ternary;

                            parts.Remove(y);
                            parts.RemoveRange(start, parts.Count - start);
                        }
                        #endregion
                        #region Unary
                        else if (x == -1)
                        {
                            int z = y + 1;
                            if (op == Script.Operator.LogicalNotEx && parts[y] is CodeComplexVariableReferenceExpression && z < parts.Count)
                                MergeAssignmentAt(parts, z);

                            // TODO: raise precedence of assignments for other cases (e.g. ++var)

                            if (LaxExpressions)
                            {
                                if (y > parts.Count - 1)
                                    return new CodePrimitiveExpression(null);
                            }

                            invoke.Method = (CodeMethodReferenceExpression)InternalMethods.OperateUnary;
                            invoke.Parameters.Add(OperatorAsFieldReference(op));
                            invoke.Parameters.Add(WrappedComplexVar(parts[y]));
                            parts[i] = invoke;
                            parts.RemoveAt(y);
                        }
                        #endregion
                        #region Binary
                        else
                        {
                            if (op == Script.Operator.BooleanAnd || op == Script.Operator.BooleanOr)
                            {
                                var boolean = new CodeBinaryOperatorExpression();
                                boolean.Operator = op == Script.Operator.BooleanAnd ? CodeBinaryOperatorType.BooleanAnd : CodeBinaryOperatorType.BooleanOr;

                                var iftest = (CodeMethodInvokeExpression)InternalMethods.IfElse;
                                iftest.Parameters.Add(WrappedComplexVar(parts[x]));
                                boolean.Left = iftest;

                                iftest = (CodeMethodInvokeExpression)InternalMethods.IfElse;
                                var next = parts[y] as Script.Operator?;
                                if (next == Script.Operator.BooleanAnd || next == Script.Operator.BooleanOr)
                                {
                                    if (LaxExpressions)
                                        iftest.Parameters.Add(new CodePrimitiveExpression(false));
                                    else
                                        throw new ParseException(ExInvalidExpression);
                                }
                                else
                                {
                                    iftest.Parameters.Add(WrappedComplexVar(parts[y]));
                                    parts.RemoveAt(y);
                                }
                                boolean.Right = iftest;

                                parts[x] = boolean;
                            }
                            else
                            {
                                if (LaxExpressions)
                                {
                                    if (parts[x] is Script.Operator && (Script.Operator)parts[x] == Script.Operator.TernaryA)
                                    {
                                        parts[x] = new CodePrimitiveExpression(null);
                                        goto next;
                                    }

                                    if (y > parts.Count - 1)
                                        return new CodePrimitiveExpression(null);
                                }
                                else
                                    throw new ParseException(ExInvalidExpression);

                                invoke.Method = (CodeMethodReferenceExpression)InternalMethods.Operate;
                                invoke.Parameters.Add(OperatorAsFieldReference(op));

#pragma warning disable 0162
                                if (LaxExpressions && parts[i] is Script.Operator && (Script.Operator)parts[i] == Script.Operator.Concat && parts[x] is CodeComplexAssignStatement)
                                    invoke.Parameters.Add(new CodePrimitiveExpression(string.Empty));
                                else
                                    invoke.Parameters.Add(WrappedComplexVar(parts[x]));
#pragma warning restore 0162

                                invoke.Parameters.Add(WrappedComplexVar(parts[y]));
                                parts[x] = invoke;

                            next:
                                parts.RemoveAt(y);
                            }

                            parts.RemoveAt(i);
                        }
                        #endregion
                    }
                    else
                    {
                        int x = i - 1, y = i + 1;

                        if (x > 0 && y < parts.Count && !(parts[i] is CodeComplexAssignStatement) &&
                            (parts[x] is CodeMethodInvokeExpression || parts[x] is CodePrimitiveExpression || parts[x] is CodeComplexVariableReferenceExpression) &&
                            (parts[y] is CodeMethodInvokeExpression || parts[y] is CodePrimitiveExpression || parts[y] is CodeComplexVariableReferenceExpression))
                            parts.Insert(y, Script.Operator.Concat);
                    }
                }
                level--;
            }

            #endregion

            #endregion

            #region Assignments
            for (int i = parts.Count - 1; i > 0; i--)
                MergeAssignmentAt(parts, i);
            #endregion

            #region Variables
#pragma warning disable 0162
            if (!UseComplexVar)
            {
                for (int i = 0; i < parts.Count; i++)
                    if (parts[i] is CodeComplexVariableReferenceExpression)
                        parts[i] = (CodeMethodInvokeExpression)(CodeComplexVariableReferenceExpression)parts[i];
            }
#pragma warning restore 0162
            #endregion

            #region Result

            if (parts.Count > 1)
            {
                for (int i = 0; i < parts.Count; i++)
                {
                    bool typed = false;

                    if (LaxExpressions)
                        typed = parts[i] is CodeComplexAssignStatement || parts[i] is CodeComplexVariableReferenceExpression;

                    if (!(typed || parts[i] is CodeMethodInvokeExpression || parts[i] is CodePrimitiveExpression))
                        throw new ArgumentOutOfRangeException();

                    if (i % 2 == 1)
                        parts.Insert(i, Script.Operator.Concat);
                }
                var concat = ParseExpression(parts);
                parts.Clear();
                parts.Add(concat);
            }

            if (parts.Count != 1)
                throw new ArgumentOutOfRangeException();

#pragma warning disable 0162
            if (UseComplexVar)
            {
                if (parts[0] is CodeComplexAssignStatement)
                    return (CodeBinaryOperatorExpression)(CodeComplexAssignStatement)parts[0];
                else
                    return (CodeExpression)parts[0];
            }
            else
                return (CodeExpression)parts[0];
#pragma warning restore 0162

            #endregion
        }

        #region Helpers

        bool IsImplicitAssignment(List<object> parts, int i)
        {
            int x = i - 1, y = i + 1;

            if (!((parts[i] is string) && ((string)parts[i]).Length == 1 && ((string)parts[i])[0] == Equal))
                return false;

            if (x < 0 || !(parts[x] is CodeComplexVariableReferenceExpression))
                return false;

            if (!(y < parts.Count && parts[y] is string && IsVariable((string)parts[y])))
                return false;

            int z = x - 1;
            
            if (z < 0)
                return true;

            if (parts[z] is CodeComplexAssignStatement)
                return true;

            if (!(parts[z] is Script.Operator) || (Script.Operator)parts[z] == Script.Operator.IdentityEquality)
                return false;

            return true;
        }

        void MergeAssignmentAt(List<object> parts, int i)
        {
            if (!(parts[i] is CodeComplexAssignStatement))
                return;

            int x = i - 1, y = i + 1;
            bool right = y < parts.Count;

            var assign = (CodeComplexAssignStatement)parts[i];

#pragma warning disable 0162
            if (UseComplexVar)
                if (assign.Left != null)
                    return;
#pragma warning restore 0162

            if (parts[x] is CodeBinaryOperatorExpression)
            {
                var binary = (CodeBinaryOperatorExpression)parts[x];
                assign.Left = (CodeComplexVariableReferenceExpression)binary.Left;
            }
            else if (parts[x] is CodeComplexVariableReferenceExpression)
                assign.Left = (CodeComplexVariableReferenceExpression)parts[x];
            else
                assign.Left = VarId((CodeExpression)parts[x]);
            assign.Right = right ? WrappedComplexVar(parts[y]) : new CodePrimitiveExpression(null);

#pragma warning disable 0162
            if (UseComplexVar)
                parts[x] = assign;
            else
                parts[x] = (CodeMethodInvokeExpression)assign;
#pragma warning restore 0162

            if (right)
                parts.RemoveAt(y);
            parts.RemoveAt(i);
        }

        #endregion

        #endregion

        #region Date/time

        CodeExpression ParseDateExpression(string code)
        {
            // TODO: date/time arithmetic expressions
            return new CodeExpression();
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
                            else if (op[1] == Greater)
                                return Script.Operator.ValueInequality;
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

                case BitXOR:
                    switch (op.Length)
                    {
                        case 1:
                            return Script.Operator.BitwiseXor;

                        default:
                            throw new ParseException(ExUnexpected);
                    }

                case BitNOT:
                    switch (op.Length)
                    {
                        case 1:
                            return Script.Operator.BitwiseNot;

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
                    switch (code.ToLowerInvariant())
                    {
                        case NotTxt:
                            return Script.Operator.LogicalNotEx;

                        case AndTxt:
                            return Script.Operator.BooleanAnd;

                        case OrTxt:
                            return Script.Operator.BooleanOr;
                    }
                    throw new ArgumentOutOfRangeException();
            }
        }

        bool IsOperator(string code)
        {
            try
            {
                if (!IsAssignOp(code))
                    OperatorFromString(code);
            }
            catch (ArgumentOutOfRangeException) { return false; }
            return true;
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

                case Script.Operator.Assign:
                    return -13;

                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        bool IsUnaryOperator(Script.Operator op)
        {
            switch (op)
            {
                case Script.Operator.Subtract:
                case Script.Operator.LogicalNot:
                case Script.Operator.LogicalNotEx:
                case Script.Operator.BitwiseNot:
                case Script.Operator.BitwiseAnd:
                case Script.Operator.Dereference:
                    return true;

                case Script.Operator.Add:
                    return true;

                default:
                    return false;
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
                        case Divide:
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
            var list = new List<object>();

            for (int i = 0; i < code.Length; i++)
            {
                char sym = code[i];

                #region Spaces
                if (IsSpace(sym))
                    continue;
                #endregion
                #region Comments
                else if (IsCommentAt(code, i))
                    MoveToEOL(code, ref i);
                #endregion
                #region Identifiers
                else if (IsIdentifier(sym) || sym == Resolve || (sym == Concatenate && i + 1 < code.Length && IsIdentifier(code[i + 1])))
                {
                    var id = new StringBuilder(code.Length);
                    id.Append(sym);
                    i++;

                    for (; i < code.Length; i++)
                    {
                        sym = code[i];
                        if ((sym == 'e' || sym == 'E') && IsPrimativeObject(id.ToString()) && id.ToString().IndexOf("0x") != 0 && i + 1 < code.Length)
                        {
                            id.Append(sym);
                            sym = code[++i];
                            if (!(sym == '+' || sym == '-' || char.IsDigit(sym)))
                                throw new ParseException(ExInvalidExponent);
                            id.Append(sym);
                        }
                        else if (IsIdentifier(sym) || sym == Resolve || (sym == Concatenate && (i + 1 < code.Length ? code[i + 1] != Equal : true)))
                            id.Append(sym);
                        else
                        {
                            if (i < code.Length && code[i] == ParenOpen && !IsKeyword(id.ToString()))
                                id.Append(ParenOpen);
                            else
                                i--;
                            break;
                        }
                    }

                    list.Add(id.ToString());
                }
                #endregion
                #region Strings
                else if (sym == StringBound)
                {
                    var str = new StringBuilder(code.Length);
                    str.Append(StringBound);
                    i++;

                    for (int max = code.Length + 1; i < max; i++)
                    {
                        if (i == code.Length)
                            throw new ParseException(ExUntermStr);

                        sym = code[i];
                        str.Append(sym);

                        if (sym == StringBound)
                        {
                            int n = i + 1;
                            if (n < code.Length && code[n] == StringBound)
                                i = n;
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
                    int n = i + 1;
                    char symNext = n < code.Length ? code[n] : Reserved;
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
                                i++;
                                tri = true;
                                if (peekAssign)
                                {
                                    n = i + 1;
                                    if (n < code.Length && code[n] == Equal)
                                    {
                                        op.Append(code[n]);
                                        i = n;
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
                                case AssignPre:
                                case Add:
                                case Minus:
                                case Multiply:
                                case Divide:
                                case Concatenate:
                                case BitAND:
                                case BitXOR:
                                case BitOR:
                                case Not:
                                case Equal:
                                case Greater:
                                case Less:
                                    op.Append(sym);
                                    op.Append(symNext);
                                    i++;
                                    break;
                            }
                        }
                        else if (sym == Less && symNext == Greater)
                        {
                            op.Append(sym);
                            op.Append(symNext);
                            i++;
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
                                case Divide:
                                    op.Append(sym);
                                    break;

                                case BlockOpen:
                                    blockOpen = true;
                                    int j = i + 2;
                                    if (j < code.Length && !IsCommentAt(code, j))
                                        throw new ParseException(ExUnexpected);
                                    j--;
                                    if (j < code.Length && !IsSpace(code[j]))
                                        throw new ParseException(ExUnexpected);
                                    return list;

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

        void RemoveExcessParentheses(List<object> parts)
        {
            while (parts.Count > 1)
            {
                int level = 0;
                int last = parts.Count - 1;

                if (!(--last > 1 &&
                    parts[0] is string && ((string)parts[0]).Length == 1 && ((string)parts[0])[0] == ParenOpen &&
                    parts[last] is string && ((string)parts[last]).Length == 1 && ((string)parts[last])[0] == ParenClose))
                    return;

                for (int i = 0; i < last; i++)
                {
                    string check = parts[i] as string;

                    if (string.IsNullOrEmpty(check))
                        continue;

                    switch (check[check.Length - 1])
                    {
                        case ParenOpen:
                            level++;
                            break;

                        case ParenClose:
                            if (check.Length != 1)
                                break;
                            else if (--level < 0)
                                throw new ParseException(ExUnbalancedParens);
                            break;
                    }
                }

                if (level != 0)
                    return;

                parts.RemoveAt(last);
                parts.RemoveAt(0);
            }
        }

        #endregion
    }
}
