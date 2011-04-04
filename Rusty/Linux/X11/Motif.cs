using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;

namespace IronAHK.Rusty.Linux.X11
{

    [StructLayout(LayoutKind.Sequential)]
    internal struct MotifWmHints
    {
        internal IntPtr flags;
        internal IntPtr functions;
        internal IntPtr decorations;
        internal IntPtr input_mode;
        internal IntPtr status;
    }

    [Flags]
    internal enum MotifFlags
    {
        Functions = 1,
        Decorations = 2,
        InputMode = 4,
        Status = 8
    }

    [Flags]
    internal enum MotifFunctions
    {
        All = 0x01,
        Resize = 0x02,
        Move = 0x04,
        Minimize = 0x08,
        Maximize = 0x10,
        Close = 0x20
    }

    [Flags]
    internal enum MotifDecorations
    {
        All = 0x01,
        Border = 0x02,
        ResizeH = 0x04,
        Title = 0x08,
        Menu = 0x10,
        Minimize = 0x20,
        Maximize = 0x40,

    }

    [Flags]
    internal enum MotifInputMode
    {
        Modeless = 0,
        ApplicationModal = 1,
        SystemModal = 2,
        FullApplicationModal = 3
    }
}
