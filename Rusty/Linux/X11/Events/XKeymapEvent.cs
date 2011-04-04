using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;

namespace IronAHK.Rusty.Linux.X11.Events
{
    [StructLayout(LayoutKind.Sequential)]
    internal struct XKeymapEvent
    {
        internal XEventName type;
        internal IntPtr serial;
        internal bool send_event;
        internal IntPtr display;
        internal IntPtr window;
        internal byte key_vector0;
        internal byte key_vector1;
        internal byte key_vector2;
        internal byte key_vector3;
        internal byte key_vector4;
        internal byte key_vector5;
        internal byte key_vector6;
        internal byte key_vector7;
        internal byte key_vector8;
        internal byte key_vector9;
        internal byte key_vector10;
        internal byte key_vector11;
        internal byte key_vector12;
        internal byte key_vector13;
        internal byte key_vector14;
        internal byte key_vector15;
        internal byte key_vector16;
        internal byte key_vector17;
        internal byte key_vector18;
        internal byte key_vector19;
        internal byte key_vector20;
        internal byte key_vector21;
        internal byte key_vector22;
        internal byte key_vector23;
        internal byte key_vector24;
        internal byte key_vector25;
        internal byte key_vector26;
        internal byte key_vector27;
        internal byte key_vector28;
        internal byte key_vector29;
        internal byte key_vector30;
        internal byte key_vector31;
    }
}
