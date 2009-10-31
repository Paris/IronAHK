using System.CodeDom;

namespace IronAHK.Scripting
{
    partial class Parser
    {
        int loops = 0;
        CodeStatementCollection lastelse = null;

        CodeStatement ParseFlow(CodeLine line, out bool rewind)
        {
            #region Variables

            string code = line.Code;

            char[] delimiters = new char[Spaces.Length + 2];
            delimiters[0] = Multicast;
            delimiters[1] = ParenOpen;
            Spaces.CopyTo(delimiters, 2);

            string[] parts = code.TrimStart(Spaces).Split(delimiters, 2);

            if (parts.Length > 1)
                parts[1] = parts[1].Trim(Spaces);

            char sym;
            int n;

            bool gosub = false;

            rewind = false;

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
                        CodeConditionStatement ifelse = new CodeConditionStatement();
                        ifelse.Condition = condition;
                        var block = new CodeBlock(line, Scope, ifelse.TrueStatements);
                        block.Type = blockOpen ? CodeBlock.BlockType.Within : CodeBlock.BlockType.Expect;
                        blocks.Push(block);

                        lastelse = ifelse.FalseStatements;
                        return ifelse;
                    }

                case FlowElse:
                    {
                        if (lastelse == null)
                            throw new ParseException("Else with no preceeding if block");

                        bool blockOpen = parts.Length > 1 && parts[1][0] == BlockOpen;
                        var block = new CodeBlock(line, Scope, lastelse);
                        block.Type = blockOpen ? CodeBlock.BlockType.Within : CodeBlock.BlockType.Expect;
                        blocks.Push(block);

                        lastelse = null;

                        line.Code = line.Code.Substring(FlowElse.Length).TrimStart(Spaces);
                        rewind = true;
                    }
                    break;

                #endregion

                #region Goto

                case FlowGosub:
                    gosub = true;
                    goto case FlowGoto;

                case FlowGoto:
                    if (parts.Length < 1)
                        throw new ParseException("No label specified");
                    for (int i = 0; i < parts[1].Length; i++)
                    {
                        sym = parts[1][i];
                        if (sym == Resolve)
                            throw new ParseException("Dynamic label references are not supported"); // TODO: dynamic goto label references
                        else if (!IsIdentifier(sym))
                            throw new ParseException("Illegal character in label name");
                    }
                    return new CodeGotoStatement(parts[1]); // TODO: gosub

                #endregion

                #region Loops

                case FlowLoop:
                    {
                        bool blockOpen = false;
                        int i;

                        if (parts.Length > 0)
                        {
                            string arg = parts[1];
                            arg = StripComment(arg);
                            n = arg.Length - 1;
                            if (arg.Length > 1 && arg[n] == BlockOpen)
                            {
                                blockOpen = true;
                                arg = arg.Substring(0, n);
                            }
                            arg = arg.Trim(Spaces);
                            if (arg.Length == 0)
                                i = 0;
                            else
                            {
                                if (!int.TryParse(arg, out i))
                                    throw new ParseException("Loop iterations is not a valid integer");
                            }
                        }
                        else
                            i = 0;

                        string id = "loop" + loops.ToString();
                        loops++;

                        var init = new CodeAssignStatement(new CodeVariableReferenceExpression(id), new CodePrimitiveExpression(0));

                        var condition = new CodeBinaryOperatorExpression(new CodeVariableReferenceExpression(id),
                            CodeBinaryOperatorType.LessThan, new CodePrimitiveExpression(i));

                        var inc = new CodeAssignStatement(new CodeVariableReferenceExpression(id),
                            new CodeBinaryOperatorExpression(new CodeVariableReferenceExpression(id), CodeBinaryOperatorType.Add, new CodePrimitiveExpression(1)));

                        CodeIterationStatement loop = new CodeIterationStatement();
                        loop.InitStatement = init;
                        loop.IncrementStatement = inc;
                        loop.TestExpression = condition;

                        var block = new CodeBlock(line, Scope, loop.Statements);

                        block.Type = blockOpen ? CodeBlock.BlockType.Within : CodeBlock.BlockType.Expect;
                        blocks.Push(block);
                        return loop;
                    }

                case FlowWhile:
                    {
                        bool blockOpen = false;
                        CodeExpression condition = parts.Length > 1 ? ParseFlowParameter(parts[1], true, out blockOpen) : new CodePrimitiveExpression(true);
                        CodeIterationStatement loop = new CodeIterationStatement(); //null, condition, null, statements);
                        loop.TestExpression = condition;
                        loop.InitStatement = null;
                        var block = new CodeBlock(line, Scope, loop.Statements);
                        block.Type = blockOpen ? CodeBlock.BlockType.Within : CodeBlock.BlockType.Expect;
                        blocks.Push(block);
                    }
                    break;

                case FlowBreak:
                    if (parts.Length > 1)
                        throw new ParseException(ExFlowArgNotReq);
                    return new CodeGotoStatement(); // TODO: break labels

                case FlowContinue:
                    if (parts.Length > 1)
                        throw new ParseException(ExFlowArgNotReq);
                    return new CodeGotoStatement(); // TODO: continue labels

                #endregion

                case FlowReturn:
                    if (Scope == mainScope)
                    {
                        if (parts.Length > 1)
                            throw new ParseException("Cannot have return parameter for entry point method");
                        return new CodeMethodReturnStatement();
                    }
                    else
                        return new CodeMethodReturnStatement(parts.Length > 1 ? ParseSingleExpression(parts[1]) : new CodePrimitiveExpression(null));

                default:
                    throw new ParseException(ExUnexpected);
            }

            return null;
        }

        CodeExpression ParseFlowParameter(string code, bool inequality, out bool blockOpen)
        {
            blockOpen = false;
            code = code.Trim(Spaces);
            if (code.Length == 0)
                return null;
            else if (code.Length > 1 && code[0] == Resolve && IsSpace(code[1]))
                return ParseSingleExpression(code.Substring(2));
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
                code = StripComment(code);

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

            string word = code.Split(Spaces, 1)[0];
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

        CodeBinaryOperatorExpression ParseInequality(string code)
        {
            string var;
            CodeBinaryOperatorType op;
            CodeExpression value;
            int i;
            char sym;

            for (i = 0; i < code.Length; i++)
            {
                sym = code[i];

                if (!IsIdentifier(sym))
                    break;
            }

            if (i == 0)
                throw new ParseException(ExUnexpected);

            var = code.Substring(0, i);

            while (i < code.Length && IsSpace(code[i]))
                i++;

            if (i == code.Length)
            {
                op = CodeBinaryOperatorType.IdentityInequality;
                value = new CodePrimitiveExpression(null);
            }
            else
            {
                int n = i + 1;
                sym = n < code.Length ? code[n] : '\0';
                bool doubleOp = false;

                switch (code[i])
                {
                    case Equal:
                        if (sym == Equal)
                        {
                            doubleOp = true;
                            op = CodeBinaryOperatorType.IdentityInequality;
                        }
                        else
                            op = CodeBinaryOperatorType.ValueEquality;
                        break;

                    case Not:
                        if (sym != Equal)
                            throw new ParseException(ExUnexpected);
                        doubleOp = true;
                        op = CodeBinaryOperatorType.IdentityInequality;
                        break;

                    case Greater:
                        if (sym == Equal)
                        {
                            doubleOp = true;
                            op = CodeBinaryOperatorType.GreaterThanOrEqual;
                        }
                        else
                            op = CodeBinaryOperatorType.GreaterThan;
                        break;

                    case Less:
                        doubleOp = true;
                        if (sym == Equal)
                            op = CodeBinaryOperatorType.LessThanOrEqual;
                        else if (sym == Greater)
                            op = CodeBinaryOperatorType.IdentityInequality;
                        else
                        {
                            doubleOp = false;
                            op = CodeBinaryOperatorType.LessThan;
                        }
                        break;

                    default:
                        throw new ParseException(ExUnexpected);
                }

                i++;
                if (doubleOp)
                    i++;

                if (i == code.Length)
                    value = new CodePrimitiveExpression(null);
                else
                {
                    string valueStr = code.Substring(i);
                    value = VarNameOrBasicString(valueStr, true);
                }
            }

            return new CodeBinaryOperatorExpression(VarNameOrBasicString(var, false), op, value);
        }
    }
}
