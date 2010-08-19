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
                #region Line

                string code = lines[i].Code;

                if (string.IsNullOrEmpty(code))
                    continue;

                line = lines[i].LineNumber;
                fileName = lines[i].FileName;

                #endregion

                #region Blocks

                var parent = blocks.Count > 0 ? blocks.Peek().Statements : main.Statements;
                string codeTrim = code.TrimStart(Spaces);
                int blocksCount = -1;

                if (codeTrim.Length > 0)
                {
                    CodeBlock block;
                    char sym = codeTrim[0];
                    bool skip = false;

                    switch (sym)
                    {
                        case BlockOpen:
                            if (blocks.Count == 0)
                            {
                                block = new CodeBlock(lines[i], Scope, new CodeStatementCollection(), CodeBlock.BlockKind.Dummy, blocks.Count == 0 ? null : blocks.Peek());
                                CloseTopSingleBlock();
                                blocks.Push(block);
                            }
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
                                blocksCount = blocks.Count;
                                block = blocks.Peek();
                                block.Type = CodeBlock.BlockType.Within;
                                block.Level = blocksCount;
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

                #region Tokens

                var token = GetToken(code);

                try
                {
                    switch (token)
                    {
                        case Token.Assign:
                            var assign = ParseAssign(code);
                            assign.LinePragma = lines[i];
                            parent.Add(assign);
                            break;

                        case Token.Command:
                            var command = new CodeExpressionStatement(OptimiseExpression(ParseCommand(code)));

                            if (command.Expression == null)
                                continue;

                            command.LinePragma = lines[i];
                            parent.Add(command);
                            break;

                        case Token.Label:
                            var label = new CodeExpressionStatement(ParseLabel(lines[i]));
                            label.LinePragma = lines[i];
                            parent.Add(label);
                            break;

                        case Token.Hotkey:
                            var hotkey = ParseHotkey(lines, i);
                            hotkey.LinePragma = lines[i];
                            parent.Add(hotkey);
                            break;

                        case Token.Flow:
                            {
                                var result = ParseFlow(lines, i);
                                if (result != null)
                                {
                                    for (int n = 0; n < result.Length; n++)
                                        result[n].LinePragma = lines[i];
                                    parent.AddRange(result);
                                }
                            }
                            break;

                        case Token.Expression:
                            {
                                int n = i + 1;
                                if (IsFunction(code, n < lines.Count ? lines[n].Code : string.Empty))
                                    ParseFunction(lines[i]);
                                else
                                {
                                    var statements = ParseMultiExpression(code);

                                    for (n = 0; n < statements.Length; n++)
                                    {
                                        var expr = OptimiseLoneExpression(statements[n].Expression);

                                        if (expr == null)
                                            continue;
                                        else
                                            statements[n] = new CodeExpressionStatement(expr);

                                        statements[n].LinePragma = lines[n];
                                        parent.Add(statements[n]);
                                    }
                                }
                            }
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

                #endregion

                #region Blocks

                if (blocks.Count == blocksCount && blocks.Peek().IsSingle)
                    CloseBlock(blocksCount, blocks.Count > blocksCount && blocksCount != -1);

                #endregion
            }

            #region Blocks

            CloseTopSingleBlocks();
            CloseTopLabelBlock();
            CloseTopSingleBlocks();
            CloseSingleLoopBlocks();

            if (blocks.Count > 0)
                throw new ParseException(ExUnclosedBlock, blocks.Peek().Line);

            #endregion
        }
    }
}
