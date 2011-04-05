using System;
using System.CodeDom;
using System.Collections.Generic;

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
                        return new[] { new CodeExpressionStatement(ParseDateExpression(code)) };
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

            for (int i = 0; i < parts.Length; i++)
            {
                if (!(parts[i] is string) || ((string)parts[i]).Length == 0)
                {
                    sub.Add(parts[i]);
                    continue;
                }

                int next = Set(parts, i);

                if (next > 0)
                {
                    for (; i < next; i++)
                        sub.Add(parts[i]);
                    i--;
                    continue;
                }

                var check = (string)parts[i];

                if (check.Length == 1 && check[0] == Multicast && sub.Count != 0)
                {
                    expr.Add(ParseExpression(sub));
                    sub.Clear();
                    continue;
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
            RemoveExcessParentheses(parts);

            #region Scanner

        start:
            bool rescan = false;

            for (int i = 0; i < parts.Count; i++)
            {
                if (parts[i] is string)
                {
                    var part = (string)parts[i];
                    object result;

                    #region Parentheses
                    if (part[0] == ParenOpen)
                    {
                        int n = i + 1;
                        var paren = Dissect(parts, n, Set(parts, i));
                        parts.RemoveAt(n);
                        n -= 2;

                        bool call = n > -1 && parts[n] is CodeExpression && !(parts[n] is CodePrimitiveExpression);

                        if (call && parts[n] is CodeMethodInvokeExpression && ((CodeMethodInvokeExpression)parts[n]).Parameters[0] is CodeFieldReferenceExpression)
                            call = false;

                        if (call)
                        {
                            var invoke = (CodeMethodInvokeExpression)InternalMethods.Invoke;

                            invoke.Parameters.Add((CodeExpression)parts[n]);

                            if (paren.Count != 0)
                            {
                                var passed = ParseMultiExpression(paren.ToArray());
                                invoke.Parameters.AddRange(passed);
                            }


                            parts[i] = invoke;
                            parts.RemoveAt(n);
                        }
                        else
                        {
                            if (paren.Count == 0)
                                parts.RemoveAt(i);
                            else
                                parts[i] = ParseExpression(paren);
                        }
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
                    {
                        var low = part.ToLowerInvariant();

                        if (libProperties.ContainsKey(low))
                            parts[i] = new CodePropertyReferenceExpression(new CodeTypeReferenceExpression(bcl), libProperties[low]);
                        else
                            parts[i] = VarIdOrConstant(part);
                    }
                    #endregion
                    #region JSON
                    else if (part.Length == 1 && part[0] == BlockOpen)
                    {
                        int n = i + 1;
                        var paren = Dissect(parts, n, Set(parts, i));

                        var invoke = (CodeMethodInvokeExpression)InternalMethods.Dictionary;
                        CodePrimitiveExpression[] keys;
                        CodeExpression[] values;
                        ParseObject(paren, out keys, out values);
                        invoke.Parameters.Add(new CodeArrayCreateExpression(typeof(string), keys));
                        invoke.Parameters.Add(new CodeArrayCreateExpression(typeof(object), values));

                        parts[i] = invoke;
                        parts.RemoveAt(n);
                        i--;
                    }
                    else if (part.Length == 1 && part[0] == ArrayOpen)
                    {
                        int n = i + 1;
                        var paren = Dissect(parts, n, Set(parts, i));
                        parts.RemoveAt(n);

                        if (i > 0 && parts[i - 1] is CodeExpression)
                        {
                            var invoke = (CodeMethodInvokeExpression)InternalMethods.Index;
                            n = i - 1;
                            invoke.Parameters.Add((CodeExpression)parts[n]);

                            var index = ParseMultiExpression(paren.ToArray());
                            if (index.Length > 1)
                                throw new ParseException("Cannot have multipart expression in index.");
                            else if (index.Length == 0)
                            {
                                var extend = (CodeMethodInvokeExpression)InternalMethods.ExtendArray;
                                var sub = new List<object>(1);
                                sub.Add(parts[n]);
                                extend.Parameters.Add(ParseExpression(sub));
                                invoke = extend;
                            }
                            else
                                invoke.Parameters.Add(index[0]);

                            parts[i] = invoke;
                            parts.RemoveAt(n);
                            i--;
                        }
                        else
                        {
                            var array = new CodeArrayCreateExpression(typeof(object[]), ParseMultiExpression(paren.ToArray()));
                            parts[i] = array;
                        }
                    }
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
                        else
                            CheckPersistent(name);

                        int n = i + 1;
                        var paren = Dissect(parts, n, Set(parts, i));
                        parts.RemoveAt(n);

                        CodeMethodInvokeExpression invoke;

                        if (dynamic)
                        {
                            invoke = (CodeMethodInvokeExpression)InternalMethods.FunctionCall;
                            invoke.Parameters.Add(VarIdExpand(name));
                        }
                        else
                            invoke = LocalMethodInvoke(name);

                        if (paren.Count != 0)
                        {
                            var passed = ParseMultiExpression(paren.ToArray());
                            invoke.Parameters.AddRange(passed);
                        }

                        parts[i] = invoke;
                        invokes.Add(invoke);
                    }
                    #endregion
                    #region Assignments
                    else if (IsAssignOp(part) || IsImplicitAssignment(parts, i))
                    {
                        int n = i - 1;

                        if (i > 0 && IsJsonObject(parts[n])) { }
                        else if (n < 0 || !IsVarReference(parts[n]))
                        {
                            if (LaxExpressions)
                            {
                                if (parts[n] is CodePrimitiveExpression && ((CodePrimitiveExpression)parts[n]).Value is decimal)
                                    parts[n] = VarId(((decimal)((CodePrimitiveExpression)parts[n]).Value).ToString());
                            }
                            else
                                throw new ParseException("Can only assign to a variable");
                        }
                        
                        // (x += y) => (x = x + y)
                        parts[i] = CodeBinaryOperatorType.Assign;
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
                        if (!LaxExpressions)
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

                            // UNDONE: use generic approach to ++/-- for all types of operands? 
                            if (x > -1 && parts[x] is CodeMethodInvokeExpression)
                            {
                                var sub = new List<object>(5);
                                sub.Add(parts[x]);
                                sub.Add(CodeBinaryOperatorType.Assign);
                                sub.Add(parts[x]);
                                sub.Add(Script.Operator.Add);
                                sub.Add(d);

                                parts.RemoveAt(i);
                                parts[x] = ParseExpression(sub);
                                i = x;
                                continue;
                            }

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

                            if (x > -1 && (IsVarReference(parts[x]) || parts[x] is CodePropertyReferenceExpression))
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
                                        var zx = new[] { z + 1, z + 2 };
                                        if (zx[1] < parts.Count &&
                                            parts[zx[1]] is string && ((string)parts[zx[1]]).Length == 1 && ((string)parts[zx[1]])[0] == ParenClose &&
                                            (parts[zx[0]] is string && IsDynamicReference((string)parts[zx[0]]) || IsVarReference(parts[zx[0]])))
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
                                if (w < parts.Count && (parts[w] is string && IsAssignOp((string)parts[w]) || IsVarAssignment(parts[w])))
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
                            list.Add(new string(new[] { Add, Equal }));
                            list.Add(new CodePrimitiveExpression(d));
                            if (shadow != null)
                            {
                                list.Add(Script.Operator.Add);
                                list.Add(shadow);
                            }
                            if (z < i) // postfix, so adjust
                            {
                                list.Insert(0, ParenOpen.ToString());
                                list.Add(ParenClose.ToString());
                                list.Add(d > 0 ? Script.Operator.Minus : Script.Operator.Add);
                                list.Add(new CodePrimitiveExpression(d));
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
                                    deref = parts[x] is Script.Operator || IsVarAssignment(parts[x]) ||
                                        (parts[x] is string && ((string)parts[x]).Length == 1 && ((string)parts[x])[0] == '(');
                                }

                                if (deref)
                                {
                                    int y = i + 1;
                                    if (y < parts.Count && (IsVarReference(parts[y]) ||
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
                if (parts[i] is Script.Operator &&
                    (parts[i - 1] is Script.Operator || parts[i - 1] as CodeBinaryOperatorType? == CodeBinaryOperatorType.Assign || IsVarAssignment(parts[i - 1])) &&
                    IsUnaryOperator((Script.Operator)parts[i]))
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

                    if (m + 1 < parts.Count && IsVarReference(parts[n]) && IsVarAssignment(parts[m]))
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
                            if (!(IsVarReference(parts[n]) || IsVarAssignment(parts[n])))
                            {
                                invoke.Parameters.Add(new CodePrimitiveExpression(null));
                                goto next;
                            }
                        }
                        invoke.Parameters.Add(VarMixedExpr(parts[n]));

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
                    if (parts[i] is Script.Operator && (Script.Operator)parts[i] != Script.Operator.Assign)
                    {
                        scan = true;
                        var op = (Script.Operator)parts[i];

                        if (OperatorPrecedence(op) < level)
                            continue;

                        int x = i - 1, y = i + 1;
                        var invoke = new CodeMethodInvokeExpression();

                        if (i + 3 < parts.Count && IsVarReference(parts[i + 1]) && parts[i + 2] as CodeBinaryOperatorType? == CodeBinaryOperatorType.Assign)
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
                            eval.Parameters.Add(VarMixedExpr(parts[x]));
                            var ternary = new CodeTernaryOperatorExpression { Condition = eval };

                            int depth = 1, max = parts.Count - i, start = i;
                            var branch = new[] { new List<object>(max), new List<object>(max) };

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
                        else if (op == Script.Operator.NullAssign)
                        {
                            if (x < 0)
                                throw new ParseException("Nullable assignment with no condition.");

                            int n = i + 1;

                            if (n >= parts.Count)
                                throw new ParseException("Nullable assignment with no right-hand operator");

                            var result = InternalVariable;
                            var left = new CodeBinaryOperatorExpression(result, CodeBinaryOperatorType.Assign, VarMixedExpr(parts[x]));

                            var eval = (CodeMethodInvokeExpression)InternalMethods.IfElse;
                            eval.Parameters.Add(left);
                            var ternary = new CodeTernaryOperatorExpression { Condition = eval, TrueBranch = result };

                            var right = new List<object>();

                            while (n < parts.Count)
                                right.Add(parts[n++]);

                            ternary.FalseBranch = ParseExpression(right);

                            parts[x] = ternary;
                            parts.RemoveRange(i, parts.Count - i);
                        }
                        #endregion
                        #region Unary
                        else if (x == -1)
                        {
                            int z = y + 1;
                            if (op == Script.Operator.LogicalNotEx && IsVarReference(parts[y]) && z < parts.Count)
                                MergeAssignmentAt(parts, z);

                            if (LaxExpressions)
                            {
                                if (y > parts.Count - 1)
                                    return new CodePrimitiveExpression(null);
                            }

                            invoke.Method = (CodeMethodReferenceExpression)InternalMethods.OperateUnary;
                            invoke.Parameters.Add(OperatorAsFieldReference(op));
                            invoke.Parameters.Add(VarMixedExpr(parts[y]));
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
                                iftest.Parameters.Add(VarMixedExpr(parts[x]));
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
                                    iftest.Parameters.Add(VarMixedExpr(parts[y]));
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

                                if (LaxExpressions && parts[i] is Script.Operator && (Script.Operator)parts[i] == Script.Operator.Concat && parts[x] as CodeBinaryOperatorType? == CodeBinaryOperatorType.Assign)
                                    invoke.Parameters.Add(new CodePrimitiveExpression(string.Empty));
                                else
                                    invoke.Parameters.Add(VarMixedExpr(parts[x]));

                                invoke.Parameters.Add(VarMixedExpr(parts[y]));
                                parts[x] = invoke;

                            next:
                                parts.RemoveAt(y);
                            }

                            parts.RemoveAt(i);
                        }
                        #endregion

                        i--;
                    }
                    else if (parts[i] as CodeBinaryOperatorType? != CodeBinaryOperatorType.Assign)
                    {
                        var x = i - 1;

                        if (x > 0 && !(parts[x] is Script.Operator || parts[x] is CodeBinaryOperatorType))
                        {
                            parts.Insert(i, Script.Operator.Concat);
                            i--;
                            continue;
                        }
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

            #region Result

            if (parts.Count > 1)
            {
                for (int i = 0; i < parts.Count; i++)
                {
                    bool typed = false;

                    if (LaxExpressions)
                        typed = IsVarAssignment(parts[i]) || IsVarReference(parts[i]);

                    if (!(typed || parts[i] is CodeMethodInvokeExpression || parts[i] is CodePrimitiveExpression || parts[i] is CodeTernaryOperatorExpression || parts[i] is CodeBinaryOperatorExpression || parts[i] is CodePropertyReferenceExpression))
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

            if (IsVarAssignment(parts[0]))
                return (CodeBinaryOperatorExpression)parts[0];
            else
                return (CodeExpression)parts[0];

            #endregion
        }

        #endregion
    }
}
