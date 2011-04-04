using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;

namespace IronAHK.Rusty.Linux.X11.Types
{
    [StructLayout(LayoutKind.Sequential)]
    internal struct XTextProperty
    {
        internal string value;
        internal IntPtr encoding;
        internal int format;
        internal IntPtr nitems;
    }
}
