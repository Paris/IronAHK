﻿using System;

[assembly: CLSCompliant(true)]

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
