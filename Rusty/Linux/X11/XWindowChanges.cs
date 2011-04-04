using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;

namespace IronAHK.Rusty.Linux.X11
{

    [StructLayout(LayoutKind.Sequential)]
    internal struct XWindowChanges
    {
        internal int x;
        internal int y;
        internal int width;
        internal int height;
        internal int border_width;
        internal IntPtr sibling;
        internal StackMode stack_mode;
    }
}
