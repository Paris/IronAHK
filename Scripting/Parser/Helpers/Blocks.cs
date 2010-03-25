using System.CodeDom;
using System.Collections.Generic;

namespace IronAHK.Scripting
{
    partial class Parser
    {
        Stack<CodeBlock> blocks = new Stack<CodeBlock>();
        Stack<CodeBlock> singleLoops = new Stack<CodeBlock>();
        Stack<CodeStatementCollection> elses = new Stack<CodeStatementCollection>();

        string PeekLoopLabel(bool exit, int n)
        {
            if (blocks.Count == 0)
                return null;

            CodeBlock parent = blocks.Peek();

            while (parent != null)
            {
                if (parent.Kind == CodeBlock.BlockKind.Loop)
                    n--;

                if (n < 1)
                    return exit ? parent.ExitLabel : parent.EndLabel;

                parent = parent.Parent;
            }

            return null;
        }

        void CloseBlock(Stack<CodeBlock> stack)
        {
            var top = stack.Pop();
            if (top.EndLabel != null)
                top.Statements.Add(new CodeLabeledStatement(top.EndLabel));
        }

        void CloseSingleLoopBlocks()
        {
            while (singleLoops.Count != 0)
                CloseBlock(singleLoops);
        }

        bool CloseTopSingleBlock()
        {
            if (blocks.Count == 0)
                return false;

            if (blocks.Peek().IsSingle)
            {
                var top = blocks.Pop();

                if (top.Kind == CodeBlock.BlockKind.Loop)
                    singleLoops.Push(top);

                return true;
            }

            return false;
        }

        void CloseTopSingleBlocks()
        {
            while (CloseTopSingleBlock()) ;
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

            CloseBlock(blocks);

        end:
            if (skip)
                blocks.Push(peek);
        }
    }
}
