using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;

namespace IronAHK.Rusty.Linux.X11.Events
{
    [StructLayout(LayoutKind.Sequential)]
    internal struct XErrorEvent
    {
        internal XEventName type;
        internal IntPtr display;
        internal IntPtr resourceid;
        internal IntPtr serial;
        internal byte error_code;
        internal XRequest request_code;
        internal byte minor_code;
    }
}
