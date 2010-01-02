using System.CodeDom;
using System.Collections.Generic;

namespace IronAHK.Scripting
{
    partial class Parser
    {
        void Statements(List<CodeLine> lines)
        {
            for (int i = 0; i < lines.Count; i++)
            {
                string code = lines[i].Code;

                if (string.IsNullOrEmpty(code))
                    continue;

                var parent = blocks.Count > 0 ? blocks.Peek().Statements : main.Statements;

                #region Blocks

                string codeTrim = code.TrimStart(Spaces);
                bool blockSingle = false;
                int blocksCount = -1;

                if (codeTrim.Length > 0)
                {
                    CodeBlock block;
                    char sym = codeTrim[0];
                    bool skip = false;

                    switch (sym)
                    {
                        case BlockOpen:
                            block = blocks.Peek();
                            if (block.Type == CodeBlock.BlockType.Expect)
                                block.Type = CodeBlock.BlockType.Within;
                            skip = true;
                            break;

                        case BlockClose:
                            if (blocks.Count == 0)
                                throw new ParseException(ExUnexpected, lines[i]);
                            CloseBlock();
                            skip = true;
                            break;

                        default:
                            if (blocks.Count > 0 && blocks.Peek().Type == CodeBlock.BlockType.Expect)
                            {
                                blockSingle = true;
                                blocksCount = blocks.Count;
                                block = blocks.Peek();
                                block.Type = CodeBlock.BlockType.Within;
                            }
                            break;
                    }

                    if (skip)
                    {
                        code = codeTrim.Substring(1);
                        if (code.Length == 0)
                            continue;
                        lines[i].Code = code;
                    }
                }

                codeTrim = null;

                #endregion

                var token = GetToken(code);

                try
                {
                    switch (token)
                    {
                        case Token.Assign:
                            parent.Add((CodeMethodInvokeExpression)ParseAssign(code));
                            break;

                        case Token.Command:
                            parent.Add(new CodeExpressionStatement(ParseCommand(code)));
                            break;

                        case Token.Label:
                            parent.Add(ParseLabel(lines[i]));
                            break;

                        case Token.Hotkey:
                            parent.Add(ParseHotkey(lines, i));
                            break;

                        case Token.Flow:
                            var result = ParseFlow(lines, i);
                            if (result != null)
                                parent.AddRange(result);
                            break;

                        case Token.Expression:
                            int n = i + 1;
                            if (IsFunction(code, n < lines.Count ? lines[n].Code : string.Empty))
                                ParseFunction(lines[i]);
                            else
                                parent.AddRange(ParseMultiExpression(code));
                            break;

                        case Token.Directive:
                            ParseDirective(code);
                            break;

                        case Token.Unknown:
                        default:
                            throw new ParseException(ExUnexpected, lines[i]);
                    }
                }
#if !DEBUG
                catch (ParseException e)
                {
                    throw new ParseException(e.Message, lines[i]);
                }
#endif
                finally { }

                if (blockSingle)
                {
                    if (blocks.Count > blocksCount && blocksCount != -1)
                    {
                        var old = blocks.Pop();
                        CloseBlock();
                        blocks.Push(old);
                    }
                    else 
                        CloseBlock();
                }
            }

            CheckTopBlock();

            if (blocks.Count > 0 && lines[lines.Count - 1].LineNumber == blocks.Peek().Line.LineNumber && blocks.Peek().Type == CodeBlock.BlockType.Expect)
                CloseBlock();

            if (blocks.Count > 0)
                throw new ParseException(ExUnclosedBlock, blocks.Peek().Line);
        }
    }
}
