using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;

namespace IronAHK.Rusty.Linux.X11.Events
{
    [StructLayout(LayoutKind.Sequential)]
    internal struct XCrossingEvent
    {
        internal XEventName type;
        internal IntPtr serial;
        internal bool send_event;
        internal IntPtr display;
        internal IntPtr window;
        internal IntPtr root;
        internal IntPtr subwindow;
        internal IntPtr time;
        internal int x;
        internal int y;
        internal int x_root;
        internal int y_root;
        internal NotifyMode mode;
        internal NotifyDetail detail;
        internal bool same_screen;
        internal bool focus;
        internal int state;
    }
}
