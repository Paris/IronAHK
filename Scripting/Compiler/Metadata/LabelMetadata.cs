using System.CodeDom;
using System.Reflection.Emit;

namespace IronAHK.Scripting
{
    // Must be reference type for EmitLabel.cs
    class LabelMetadata
    {
        public bool Resolved;
        public Label Label;
        public string Name;

        public CodeObject Bound;
        public CodeObject From;
    }
}
