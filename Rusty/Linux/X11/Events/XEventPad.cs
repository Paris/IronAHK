using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;

namespace IronAHK.Rusty.Linux.X11.Events
{

    [StructLayout(LayoutKind.Sequential)]
    internal struct XEventPad
    {
        internal IntPtr pad0;
        internal IntPtr pad1;
        internal IntPtr pad2;
        internal IntPtr pad3;
        internal IntPtr pad4;
        internal IntPtr pad5;
        internal IntPtr pad6;
        internal IntPtr pad7;
        internal IntPtr pad8;
        internal IntPtr pad9;
        internal IntPtr pad10;
        internal IntPtr pad11;
        internal IntPtr pad12;
        internal IntPtr pad13;
        internal IntPtr pad14;
        internal IntPtr pad15;
        internal IntPtr pad16;
        internal IntPtr pad17;
        internal IntPtr pad18;
        internal IntPtr pad19;
        internal IntPtr pad20;
        internal IntPtr pad21;
        internal IntPtr pad22;
        internal IntPtr pad23;
    }
}
