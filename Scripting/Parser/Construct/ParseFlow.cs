using System;
using System.Collections.Generic;
using System.CodeDom;
using System.Text;

namespace IronAHK.Scripting
{
    partial class Parser
    {

        CodeStatement[] ParseFlow(List<CodeLine> lines, int index)
        {
            #region Variables

            var line = lines[index];
            string code = line.Code;

            char[] delimiters = new char[Spaces.Length + 2];
            delimiters[0] = Multicast;
            delimiters[1] = ParenOpen;
            Spaces.CopyTo(delimiters, 2);

            string[] parts = code.TrimStart(Spaces).Split(delimiters, 2);

            if (parts.Length > 1)
                parts[1] = parts[1].Trim(Spaces);

            #endregion

            switch (parts[0].ToLowerInvariant())
            {
                #region If/Else

                case FlowIf:
                    {
                        if (parts.Length < 1)
                            throw new ParseException("If requires a parameter");

                        bool blockOpen = false;
                        CodeExpression condition = ParseFlowParameter(parts[1], true, out blockOpen);
                        CodeConditionStatement ifelse = new CodeConditionStatement { Condition = condition };

                        var block = new CodeBlock(line, Scope, ifelse.TrueStatements, CodeBlock.BlockKind.IfElse);
                        block.Type = blockOpen ? CodeBlock.BlockType.Within : CodeBlock.BlockType.Expect;
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
                        var block = new CodeBlock(lines[index], Scope, elses.Pop(), CodeBlock.BlockKind.IfElse) { Type = type };
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
                        bool skip = false;

                        #region Loop types
                        if (parts.Length > 0)
                        {
                            switch (parts[0].ToUpperInvariant())
                            {
                                case "READ":
                                    skip = true;
                                    iterator = (CodeMethodInvokeExpression)InternalMethods.LoopRead;
                                    break;

                                case "PARSE":
                                    skip = true;
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
                                    skip = true;
                                    iterator = (CodeMethodInvokeExpression)InternalMethods.LoopRegistry;
                                    break;

                                default:
                                    // TODO: file and normal loops
                                    iterator = (CodeMethodInvokeExpression)InternalMethods.Loop;
                                    break;
                            }

                            string args = parts[1];

                            if (skip)
                            {
                                int z = args.IndexOf(Multicast);
                                if (z == -1)
                                    throw new ParseException("Loop type must have an argument");
                                args = args.Substring(z);
                            }

                            foreach (string arg in SplitCommandParameters(args))
                                iterator.Parameters.Add(ParseCommandParameter(arg));
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
                        init.Type = new CodeTypeReference(typeof(System.Collections.IEnumerable));
                        init.InitExpression = new CodeMethodInvokeExpression(iterator, "GetEnumerator", new CodeExpression[] { });

                        var condition = new CodeMethodInvokeExpression();
                        condition.Method.TargetObject = new CodeVariableReferenceExpression(id);
                        condition.Method.MethodName = "MoveNext";

                        CodeIterationStatement loop = new CodeIterationStatement();
                        loop.InitStatement = init;
                        loop.IncrementStatement = new CodeCommentStatement(string.Empty); // for C# display
                        loop.TestExpression = condition;

                        var block = new CodeBlock(line, Scope, loop.Statements, CodeBlock.BlockKind.Loop);
                        block.Type = blockOpen ? CodeBlock.BlockType.Within : CodeBlock.BlockType.Expect;
                        blocks.Push(block);

                        var label = new BreakLabels(InternalID, InternalID);
                        breakLabels.Push(label);

                        return new CodeStatement[] { loop, new CodeLabeledStatement(label.Break) };
                    }

                case FlowWhile:
                    {
                        bool blockOpen = false;
                        CodeExpression condition = parts.Length > 1 ? ParseFlowParameter(parts[1], true, out blockOpen) : new CodePrimitiveExpression(true);
                        CodeIterationStatement loop = new CodeIterationStatement();
                        loop.TestExpression = condition;
                        loop.InitStatement = new CodeCommentStatement(string.Empty);

                        var block = new CodeBlock(line, Scope, loop.Statements, CodeBlock.BlockKind.Loop);
                        block.Type = blockOpen ? CodeBlock.BlockType.Within : CodeBlock.BlockType.Expect;
                        blocks.Push(block);

                        var label = new BreakLabels(InternalID, InternalID);
                        breakLabels.Push(label);

                        return new CodeStatement[] { loop, new CodeLabeledStatement(label.Break) };
                    }

                case FlowBreak:
                    if (parts.Length > 1)
                        throw new ParseException(ExFlowArgNotReq);
                    if (breakLabels.Count == 0)
                        throw new ParseException("Cannot break outside a loop");
                    return new CodeStatement[] { new CodeGotoStatement(breakLabels.Peek().Break) };

                case FlowContinue:
                    if (parts.Length > 1)
                        throw new ParseException(ExFlowArgNotReq);
                    if (breakLabels.Count == 0)
                        throw new ParseException("Cannot continue outside a loop");
                    return new CodeStatement[] { new CodeGotoStatement(breakLabels.Peek().Continue) };

                #endregion

                #region Return

                case FlowReturn:
                    if (Scope == mainScope)
                    {
                        if (parts.Length > 1 && !IsEmptyStatement(parts[1]))
                            throw new ParseException("Cannot have return parameter for entry point method");
                        return new CodeStatement[] { new CodeMethodReturnStatement() };
                    }
                    else
                    {
                        var result = parts.Length > 1 && !IsEmptyStatement(parts[1]) ? ParseSingleExpression(parts[1]) : new CodePrimitiveExpression(null);
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

        #region Flow argument

        CodeExpression ParseFlowParameter(string code, bool inequality, out bool blockOpen)
        {
            blockOpen = false;
            code = code.Trim(Spaces);
            if (code.Length == 0)
                return new CodePrimitiveExpression(false);
            else if (IsExpression(code))
            {
                int l = code.Length - 1;
                if (code.Length > 0 && code[l] == BlockOpen)
                {
                    blockOpen = true;
                    code = code.Substring(0, l);
                }
                return ParseSingleExpression(code);
            }
            else
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
        }

        bool IsExpression(string code)
        {
            char sym = code[0];

            if (sym == ParenOpen)
                return true;

            if (!IsIdentifier(sym) && !char.IsLetterOrDigit(sym))
                return true;

            string word = code.Split(Spaces, 2)[0];
            switch (word.ToLowerInvariant())
            {
                case AndTxt:
                case OrTxt:
                case NotTxt:
                    return true;
            }

            foreach (char token in code)
            {
                if (token == ParenOpen)
                    return true;
                else if (!IsIdentifier(token))
                    return false;
            }

            return false;
        }

        CodeExpression ParseInequality(string code)
        {
            var buf = new StringBuilder(code.Length);
            int i = 0;

            while (i < code.Length && IsSpace(code[i])) i++;

            while (i < code.Length && (IsIdentifier(code[i]) || code[i] == Resolve))
                buf.Append(code[i++]);

            while (i < code.Length && IsSpace(code[i])) i++;

            if (i == code.Length)
            {
                buf.Append(Not);
                buf.Append(Equal);
                buf.Append(StringBound);
                buf.Append(StringBound);
            }
            else
            {
                char[] op = new char[] { Equal, Not, Greater, Less };
                
                if (Array.IndexOf<char>(op, code[i]) == -1)
                    throw new ParseException(ExUnexpected);

                buf.Append(code[i++]);

                if (i < code.Length && Array.IndexOf<char>(op, code[i]) != -1)
                    buf.Append(code[i++]);

                buf.Append(StringBound);

                while (i < code.Length && IsSpace(code[i])) i++;

                if (i < code.Length)
                    buf.Append(code.Substring(i));

                while (i < code.Length && IsSpace(code[i])) i++;

                buf.Append(StringBound);
            }

            var iftest = (CodeMethodInvokeExpression)InternalMethods.IfElse;
            var expr = ParseSingleExpression(buf.ToString());
            iftest.Parameters.Add(expr);
            return iftest;
        }

        #endregion
    }
}
