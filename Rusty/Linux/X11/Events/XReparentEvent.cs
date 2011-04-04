using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;

namespace IronAHK.Rusty.Linux.X11.Events
{
    [StructLayout(LayoutKind.Sequential)]
    internal struct XReparentEvent
    {
        internal XEventName type;
        internal IntPtr serial;
        internal bool send_event;
        internal IntPtr display;
        internal IntPtr xevent;
        internal IntPtr window;
        internal IntPtr parent;
        internal int x;
        internal int y;
        internal bool override_redirect;
    }
}
