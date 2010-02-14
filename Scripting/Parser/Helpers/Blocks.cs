using System.CodeDom;
using System.Collections.Generic;

namespace IronAHK.Scripting
{
    partial class Parser
    {
        Stack<CodeBlock> blocks = new Stack<CodeBlock>();
        Stack<CodeStatementCollection> elses = new Stack<CodeStatementCollection>();
        Stack<BreakLabels> breakLabels = new Stack<BreakLabels>();

        void CloseTopSingleBlock()
        {
            if (blocks.Count != 0 && blocks.Peek().IsSingle)
                blocks.Pop();
        }

        void CloseTopSingleBlocks()
        {
            while (blocks.Count != 0 && blocks.Peek().IsSingle)
                blocks.Pop();
        }

        CodeBlock CloseTopLabelBlock()
        {
            if (blocks.Count != 0 && blocks.Peek().Kind == CodeBlock.BlockKind.Label)
                return blocks.Pop();
            return null;
        }

        void CloseBlock()
        {
            CloseTopSingleBlocks();
            CloseBlock(-1, false);
        }

        void CloseBlock(int level, bool skip)
        {
            if (blocks.Count < (skip ? 2 : 1))
                return;

            var peek = skip ? blocks.Pop() : null;

            var top = blocks.Peek();

            if (top.IsSingle ? blocks.Count > top.Level : false)
                goto end;

            if (top.Kind == CodeBlock.BlockKind.Loop)
                top.Statements.Add(new CodeLabeledStatement(breakLabels.Pop().Continue));

            blocks.Pop();

        end:
            if (skip)
                blocks.Push(peek);
        }
    }
}
