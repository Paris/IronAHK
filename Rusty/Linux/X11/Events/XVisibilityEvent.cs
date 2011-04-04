
using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;

namespace IronAHK.Rusty.Linux.X11.Events
{
    [StructLayout(LayoutKind.Sequential)]
    internal struct XVisibilityEvent
    {
        internal XEventName type;
        internal IntPtr serial;
        internal bool send_event;
        internal IntPtr display;
        internal IntPtr window;
        internal int state;
    }
}
