using System;
using System.Collections.Generic;
using System.Text;

namespace IronAHK.Rusty.Linux.X11
{
    [Flags]
    internal enum ChangeWindowFlags
    {
        CWX = 1 << 0,
        CWY = 1 << 1,
        CWWidth = 1 << 2,
        CWHeight = 1 << 3,
        CWBorderWidth = 1 << 4,
        CWSibling = 1 << 5,
        CWStackMode = 1 << 6
    }
}
