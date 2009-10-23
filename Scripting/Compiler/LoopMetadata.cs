using System;
using System.Reflection.Emit;

namespace IronAHK.Scripting
{
    internal struct LoopMetadata
    {
        public Label Begin;
        public Label End;
    }
}
