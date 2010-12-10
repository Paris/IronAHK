using System;
using System.Runtime.CompilerServices;

[assembly: CLSCompliant(true)]
[assembly: InternalsVisibleTo("IronAHK.Tests, PublicKey=00240000048000009400000006020000002400005253413100040000010001006f42907133e2242e06eabdfa63e1b16acf48785fe74422f1ea05be42caa221ed08d66accd396c9bfc03c7728b89c5afdbbdb40ad1fce58bcd66da4b0ebcd2d8388d9be0f7f4052904438ab74130aa44becab88338435563a70633cf87e8360c8416961d767346ba3bb5e7df6dc6038282fcf7e5924954e2e930a7d02f8737ca5")]

namespace IronAHK.Rusty
{
    /// <summary>
    /// A library of commands useful for desktop scripting.
    /// </summary>
    public partial class Core
    {
        const bool Debug =
#if DEBUG
 true
#else
 false
#endif
;
    }
}
