using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;

namespace IronAHK.Rusty.Linux.X11.Events
{
    [StructLayout(LayoutKind.Sequential)]
    internal struct XTimerNotifyEvent
    {
        internal XEventName type;
        internal EventHandler handler;
    }

}
