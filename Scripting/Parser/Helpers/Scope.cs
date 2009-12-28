
namespace IronAHK.Scripting
{
    partial class Parser
    {
        string Scope
        {
            get
            {
                if (blocks.Count == 0)
                    return mainScope;
                var block = blocks.Peek();
                return block.Kind == CodeBlock.BlockKind.Label ? mainScope : block.Method;
            }
        }
    }
}
