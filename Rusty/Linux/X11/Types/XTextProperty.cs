using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;

namespace IronAHK.Rusty.Linux.X11.Types
{
    [StructLayout(LayoutKind.Sequential)]
    internal struct XTextProperty
    {
        public IntPtr value;
        public IntPtr encoding;
        public int format;
        public IntPtr nitems;

        public string GetText() {
            return Marshal.PtrToStringAnsi(value);
        }

        public bool SetText(String value) {
            if(value == null) {
                value = String.Empty;
            }
            IntPtr str = Marshal.StringToHGlobalAnsi(value);
            if(str == IntPtr.Zero) {
                return false;
            }
            if(Xlib.XStringListToTextProperty(ref str, 1, ref this) == 0) {
                Marshal.FreeHGlobal(str);
                return false;
            }
            Marshal.FreeHGlobal(str);
            return true;
        }

        /// <summary>
        /// Free the text
        /// </summary>
        public void Free() {
            if(value != IntPtr.Zero) {
                Xlib.XFree(value);
                value = IntPtr.Zero;
            }
        }

    }
}
