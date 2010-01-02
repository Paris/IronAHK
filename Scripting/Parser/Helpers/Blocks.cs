using System.CodeDom;
using System.Collections.Generic;

namespace IronAHK.Scripting
{
    partial class Parser
    {
        Stack<CodeBlock> blocks = new Stack<CodeBlock>();
        Stack<CodeStatementCollection> elses = new Stack<CodeStatementCollection>();
        Stack<BreakLabels> breakLabels = new Stack<BreakLabels>();

        void CloseBlock()
        {
            if (blocks.Count == 0)
                return;

            var top = blocks.Pop();

            if (top.Kind == CodeBlock.BlockKind.Loop)
                top.Statements.Add(new CodeLabeledStatement(breakLabels.Pop().Continue));
        }
    }
}
