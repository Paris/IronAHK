
namespace IronAHK.Scripting
{
    partial class Parser
    {
        const string ScopeVar = ".";
        int internalID = 0;

        string InternalID
        {
            get { return "e" + internalID++.ToString(); }
        }

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
