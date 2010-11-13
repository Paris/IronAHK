using System;
using System.CodeDom.Compiler;

namespace IronAHK.Scripting
{
    public class IACompilerParameters : CompilerParameters
    {
        public bool Merge { get; set; }
        public bool MergeFallbackToLink { get; set; }

        public IACompilerParameters()
            : base()
        {
            MergeFallbackToLink = true;
        }
    }
}
