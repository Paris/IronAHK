using System;
using System.Collections.Generic;
using System.Text;

namespace IronAHK.Rusty.Linux.X11
{
    internal enum XKeySym : uint
    {
        XK_BackSpace = 0xFF08,
        XK_Tab = 0xFF09,
        XK_Clear = 0xFF0B,
        XK_Return = 0xFF0D,
        XK_Home = 0xFF50,
        XK_Left = 0xFF51,
        XK_Up = 0xFF52,
        XK_Right = 0xFF53,
        XK_Down = 0xFF54,
        XK_Page_Up = 0xFF55,
        XK_Page_Down = 0xFF56,
        XK_End = 0xFF57,
        XK_Begin = 0xFF58,
        XK_Menu = 0xFF67,
        XK_Shift_L = 0xFFE1,
        XK_Shift_R = 0xFFE2,
        XK_Control_L = 0xFFE3,
        XK_Control_R = 0xFFE4,
        XK_Caps_Lock = 0xFFE5,
        XK_Shift_Lock = 0xFFE6,
        XK_Meta_L = 0xFFE7,
        XK_Meta_R = 0xFFE8,
        XK_Alt_L = 0xFFE9,
        XK_Alt_R = 0xFFEA,
        XK_Super_L = 0xFFEB,
        XK_Super_R = 0xFFEC,
        XK_Hyper_L = 0xFFED,
        XK_Hyper_R = 0xFFEE,
    }
}
