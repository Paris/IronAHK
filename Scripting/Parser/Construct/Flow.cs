using System;
using System.CodeDom;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace IronAHK.Scripting
{
    partial class Parser
    {
        bool blockOpen;

        CodeStatement[] ParseFlow(List<CodeLine> lines, int index)
        {
            #region Variables

            var line = lines[index];
            string code = line.Code.TrimStart(Spaces);
            string[] parts = { string.Empty, string.Empty };

            var delimiters = new char[Spaces.Length + 1];
            delimiters[0] = Multicast;
            Spaces.CopyTo(delimiters, 1);
            int[] d = { code.IndexOfAny(delimiters), code.IndexOfAny(new[] { BlockOpen, ParenOpen }) };

            if (d[0] == -1 && d[1] == -1)
                parts[0] = code;
            else if (d[1] != -1 && (d[1] < d[0] || d[0] == -1))
            {
                parts[0] = code.Substring(0, d[1]);
                parts[1] = code.Substring(d[1], code.Length - d[1]).TrimStart(Spaces);
            }
            else
            {
                parts[0] = code.Substring(0, d[0]);
                parts[1] = code.Substring(d[0] + 1, code.Length - d[0] - 1).TrimStart(Spaces);
            }

            if (parts.Length > 1 && IsEmptyStatement(parts[1]))
                parts = new[] { parts[0] };

            #endregion

            switch (parts[0].ToLowerInvariant())
            {
                #region If/Else

                case FlowIf:
                    {
                        if (parts.Length < 1)
                            throw new ParseException("If requires a parameter");

                        bool blockOpen = false;
                        CodeExpression condition = ParseFlowParameter(parts[1], true, out blockOpen, false);
                        var ifelse = new CodeConditionStatement { Condition = condition };

                        var block = new CodeBlock(line, Scope, ifelse.TrueStatements, CodeBlock.BlockKind.IfElse, blocks.Count == 0 ? null : blocks.Peek());
                        block.Type = blockOpen ? CodeBlock.BlockType.Within : CodeBlock.BlockType.Expect;
                        CloseTopSingleBlock();
                        blocks.Push(block);

                        elses.Push(ifelse.FalseStatements);
                        return new CodeStatement[] { ifelse };
                    }

                case FlowElse:
                    {
                        if (elses.Count == 0)
                            throw new ParseException("Else with no preceeding if block");

                        string next = line.Code.TrimStart(Spaces).Substring(FlowElse.Length).TrimStart(Spaces);

                        if (!IsEmptyStatement(next))
                            lines.Insert(index + 1, new CodeLine(lines[index].FileName, lines[index].LineNumber, next));

                        var type = parts.Length > 1 && parts[1][0] == BlockOpen ? CodeBlock.BlockType.Within : CodeBlock.BlockType.Expect;
                        var block = new CodeBlock(lines[index], Scope, elses.Pop(), CodeBlock.BlockKind.IfElse, blocks.Count == 0 ? null : blocks.Peek()) { Type = type };
                        CloseTopSingleBlock();
                        blocks.Push(block);
                    }
                    break;

                #endregion

                #region Goto

                case FlowGosub:
                    {
                        if (parts.Length < 1)
                            throw new ParseException("No label specified");
                        return new CodeStatement[] { new CodeExpressionStatement(LocalLabelInvoke(parts[1])) };
                    }

                case FlowGoto:
                    {
                        if (parts.Length < 1)
                            throw new ParseException("No label specified");
                        return new CodeStatement[] { new CodeExpressionStatement(LocalLabelInvoke(parts[1])), new CodeMethodReturnStatement() };
                    }

                #endregion

                #region Loops

                case FlowLoop:
                    {
                        bool blockOpen = false;
                        CodeMethodInvokeExpression iterator;
                        bool skip = true;
                        bool checkBrace = true;
                        bool byref = false;

                        #region Loop types
                        if (parts.Length > 1)
                        {
                            string[] sub = parts[1].Split(new[] { Multicast }, 2);
                            sub = new[] { sub[0].Trim(), sub.Length > 1 ? sub[1].Trim() : string.Empty };

                            switch (sub[0].ToUpperInvariant())
                            {
                                case "READ":
                                    byref = true;
                                    iterator = (CodeMethodInvokeExpression)InternalMethods.LoopRead;
                                    break;

                                case "PARSE":
                                    checkBrace = false;
                                    byref = true;
                                    iterator = (CodeMethodInvokeExpression)InternalMethods.LoopParse;
                                    break;

                                case "HKEY_LOCAL_MACHINE":
                                case "HKLM":
                                case "HKEY_USERS":
                                case "HKU":
                                case "HKEY_CURRENT_USER":
                                case "HKCU":
                                case "HKEY_CLASSES_ROOT":
                                case "HKCR":
                                case "HKEY_CURRENT_CONFIG":
                                case "HKCC":
                                    iterator = (CodeMethodInvokeExpression)InternalMethods.LoopRegistry;
                                    break;

                                case "EACH":
                                    byref = true;
                                    iterator = (CodeMethodInvokeExpression)InternalMethods.LoopEach;
                                    break;

                                default:
                                    {
                                        var file = false;

                                        if (parts[1].IndexOf(Multicast) != -1)
                                            file = true;

                                        // TODO: check file/iteration loop types

                                        skip = false;
                                        iterator = (CodeMethodInvokeExpression)(file ? InternalMethods.LoopFile : InternalMethods.Loop);
                                    }
                                    break;
                            }

                            if (skip)
                                parts[1] = sub[1];

                            if (checkBrace)
                            {
                                // TODO: check expression parameters before stripping comments
                                int x = parts.Length == 1 ? 0 : 1;
                                string part = StripComment(parts[x]).TrimEnd(Spaces);
                                int l = part.Length - 1;
                                if (part.Length > 0 && part[l] == BlockOpen)
                                {
                                    blockOpen = true;
                                    parts[x] = part.Substring(0, l);
                                }
                            }

                            if (skip && parts[1].Length == 0)
                                throw new ParseException("Loop type must have an argument");

                            foreach (var arg in SplitCommandParameters(parts[1]))
                                iterator.Parameters.Add(ParseCommandParameter(arg));

                            if (LegacyLoop && byref)
                                iterator.Parameters[0] = VarId(iterator.Parameters[0]);
                        }
                        else
                        {
                            iterator = (CodeMethodInvokeExpression)InternalMethods.Loop;
                            iterator.Parameters.Add(new CodePrimitiveExpression(int.MaxValue));
                        }
                        #endregion

                        string id = InternalID;

                        var init = new CodeVariableDeclarationStatement();
                        init.Name = id;
                        init.Type = new CodeTypeReference(typeof(IEnumerable));
                        init.InitExpression = new CodeMethodInvokeExpression(iterator, "GetEnumerator", new CodeExpression[] { });

                        var condition = new CodeMethodInvokeExpression();
                        condition.Method.TargetObject = new CodeVariableReferenceExpression(id);
                        condition.Method.MethodName = "MoveNext";

                        var loop = new CodeIterationStatement();
                        loop.InitStatement = init;
                        loop.IncrementStatement = new CodeCommentStatement(string.Empty); // for C# display
                        loop.TestExpression = condition;

                        var block = new CodeBlock(line, Scope, loop.Statements, CodeBlock.BlockKind.Loop, blocks.Count == 0 ? null : blocks.Peek(), InternalID, InternalID);
                        block.Type = blockOpen ? CodeBlock.BlockType.Within : CodeBlock.BlockType.Expect;
                        CloseTopSingleBlock();
                        blocks.Push(block);

                        return new CodeStatement[] { loop, new CodeLabeledStatement(block.ExitLabel) };
                    }

                case FlowWhile:
                    {
                        bool blockOpen = false;
                        CodeExpression condition = parts.Length > 1 ? ParseFlowParameter(parts[1], true, out blockOpen, true) : new CodePrimitiveExpression(true);
                        var loop = new CodeIterationStatement();
                        loop.TestExpression = condition;
                        loop.InitStatement = new CodeCommentStatement(string.Empty);

                        var block = new CodeBlock(line, Scope, loop.Statements, CodeBlock.BlockKind.Loop, blocks.Count == 0 ? null : blocks.Peek(), InternalID, InternalID);
                        block.Type = blockOpen ? CodeBlock.BlockType.Within : CodeBlock.BlockType.Expect;
                        CloseTopSingleBlock();
                        blocks.Push(block);

                        return new CodeStatement[] { loop, new CodeLabeledStatement(block.ExitLabel) };
                    }

                case FlowBreak:
                    int b = 1;
                    if (parts.Length > 1)
                    {
                        parts[1] = StripCommentSingle(parts[1]);
                        if (!int.TryParse(parts[1], out b) || b < 1)
                            throw new ParseException("Break parameter must be a static integer greater than zero.");
                    }
                    string exit = PeekLoopLabel(true, b);
                    if (exit == null)
                        throw new ParseException("Cannot break outside a loop");
                    return new CodeStatement[] { new CodeGotoStatement(exit) };

                case FlowContinue:
                    int c = 1;
                    if (parts.Length > 1)
                    {
                        parts[1] = StripCommentSingle(parts[1]);
                        if (!int.TryParse(parts[1], out c) || c < 1)
                            throw new ParseException("Continue parameter must be a static integer greater than zero.");
                    }
                    string cont = PeekLoopLabel(false, c);
                    if (cont == null)
                        throw new ParseException("Cannot continue outside a loop");
                    return new CodeStatement[] { new CodeGotoStatement(cont) };

                #endregion

                #region Return

                case FlowReturn:
                    if (Scope == mainScope)
                    {
                        if (parts.Length > 1)
                            throw new ParseException("Cannot have return parameter for entry point method");
                        return new CodeStatement[] { new CodeMethodReturnStatement() };
                    }
                    else
                    {
                        var result = parts.Length > 1 ? ParseSingleExpression(parts[1]) : new CodePrimitiveExpression(null);
                        return new CodeStatement[] { new CodeMethodReturnStatement(result) };
                    }

                #endregion

                #region Function

                case FunctionLocal:
                case FunctionGlobal:
                case FunctionStatic:
                    // TODO: function local/global/static scoping modifiers
                    break;

                #endregion

                default:
                    throw new ParseException(ExUnexpected);
            }

            return null;
        }

        #region Parameters

        CodeExpression ParseFlowParameter(string code, bool inequality, out bool blockOpen, bool expr)
        {
            blockOpen = false;
            code = code.Trim(Spaces);
            if (code.Length == 0)
                return new CodePrimitiveExpression(false);
            if (LaxExpressions && IsLegacyIf(code))
                return ParseLegacyIf(code);
            else if (expr || IsExpressionIf(code))
            {
                code = StripComment(code).TrimEnd(Spaces);
                int l = code.Length - 1;
                if (code.Length > 0 && code[l] == BlockOpen)
                {
                    blockOpen = true;
                    code = code.Substring(0, l);
                }

                this.blockOpen = false;
                var result = ParseSingleExpression(code);
                blockOpen = blockOpen || this.blockOpen;

                var iftest = (CodeMethodInvokeExpression)InternalMethods.IfElse;
                iftest.Parameters.Add(result);
                return iftest;
            }
            else if (LegacyIf)
            {
                code = StripCommentSingle(code);

                if (inequality)
                    return ParseInequality(code);

                object result;
                if (IsPrimativeObject(code, out result))
                    return new CodePrimitiveExpression(result);
                else
                    throw new ParseException(ExUnexpected);
            }
            else
                throw new ParseException("Invalid arguments for if statement");
        }

        CodeExpression ParseInequality(string code)
        {
            var buf = new StringBuilder(code.Length);
            int i = 0;

            while (i < code.Length && IsSpace(code[i])) i++;

            while (i < code.Length && (IsIdentifier(code[i]) || code[i] == Resolve))
                buf.Append(code[i++]);

            while (i < code.Length && IsSpace(code[i])) i++;

            if (i != code.Length) // if test argument is not a lone identifier then it is an expression
            {
                var op = new[] { Equal, Not, Greater, Less };
                
                if (Array.IndexOf(op, code[i]) == -1)
                    throw new ParseException(ExUnexpected);

                buf.Append(code[i++]);

                if (i < code.Length && Array.IndexOf(op, code[i]) != -1)
                    buf.Append(code[i++]);

                buf.Append(StringBound);

                while (i < code.Length && IsSpace(code[i])) i++;

                if (i < code.Length)
                {
                    string str = code.Substring(i);
                    str = str.Replace(StringBound.ToString(), new string(StringBound, 2));
                    buf.Append(str);
                }

                while (i < code.Length && IsSpace(code[i])) i++;

                buf.Append(StringBound);
            }

            var iftest = (CodeMethodInvokeExpression)InternalMethods.IfElse;
            var expr = ParseSingleExpression(buf.ToString());
            iftest.Parameters.Add(expr);
            return iftest;
        }

        CodeExpression ParseLegacyIf(string code)
        {
            string[] parts = code.TrimStart(Spaces).Split(Spaces, 3);

            if (parts.Length != 3)
                throw new ArgumentOutOfRangeException();

            if (!IsIdentifier(parts[0]))
                throw new ArgumentException();

            bool not = false;

            if (parts[1].Equals(NotTxt, StringComparison.OrdinalIgnoreCase))
            {
                not = false;
                string[] sub = parts[2].Split(Spaces, 2);
                parts[1] = sub[0];
                parts[2] = sub[1];
            }

            var invoke = (CodeMethodInvokeExpression)InternalMethods.IfLegacy;
            invoke.Parameters.Add(VarId(parts[0]));
            parts[1] = parts[1].ToLowerInvariant();

            switch (parts[1])
            {
                case BetweenTxt:
                case InTxt:
                case ContainsTxt:
                case IsTxt:
                    invoke.Parameters.Add(new CodePrimitiveExpression(parts[1]));
                    break;

                default:
                    throw new ArgumentOutOfRangeException();
            }

            invoke.Parameters.Add(ParseCommandParameter(parts[2]));

            if (not)
            {
                var flip = (CodeMethodInvokeExpression)InternalMethods.OperateUnary;
                flip.Parameters.Add(OperatorAsFieldReference(Script.Operator.BitwiseNot));
                flip.Parameters.Add(invoke);
                invoke = flip;
            }

            return invoke;
        }

        #endregion
    }
}
