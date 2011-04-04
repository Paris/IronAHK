using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;

namespace IronAHK.Rusty.Linux.X11.Events
{
    [StructLayout(LayoutKind.Sequential)]
    internal struct XCirculateEvent
    {
        internal XEventName type;
        internal IntPtr serial;
        internal bool send_event;
        internal IntPtr display;
        internal IntPtr xevent;
        internal IntPtr window;
        internal int place;
    }

    [StructLayout(LayoutKind.Sequential)]
    internal struct XCirculateRequestEvent
    {
        internal XEventName type;
        internal IntPtr serial;
        internal bool send_event;
        internal IntPtr display;
        internal IntPtr parent;
        internal IntPtr window;
        internal int place;
    }
}
