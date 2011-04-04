using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;

namespace IronAHK.Rusty.Linux.X11.Events
{
    [StructLayout(LayoutKind.Sequential)]
    internal struct XUnmapEvent
    {
        internal XEventName type;
        internal IntPtr serial;
        internal bool send_event;
        internal IntPtr display;
        internal IntPtr xevent;
        internal IntPtr window;
        internal bool from_configure;
    }

    [StructLayout(LayoutKind.Sequential)]
    internal struct XMapEvent
    {
        internal XEventName type;
        internal IntPtr serial;
        internal bool send_event;
        internal IntPtr display;
        internal IntPtr xevent;
        internal IntPtr window;
        internal bool override_redirect;
    }

    [StructLayout(LayoutKind.Sequential)]
    internal struct XMapRequestEvent
    {
        internal XEventName type;
        internal IntPtr serial;
        internal bool send_event;
        internal IntPtr display;
        internal IntPtr parent;
        internal IntPtr window;
    }
}
