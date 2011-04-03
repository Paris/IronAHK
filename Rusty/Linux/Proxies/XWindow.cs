using System;
using System.Collections.Generic;
using System.Text;

namespace IronAHK.Rusty.Linux.Proxies
{
    /// <summary>
    /// represents a single xwindow - proxy for actions affecting x windows
    /// </summary>
    internal class XWindow
    {
        LinuxAPI.XWindowAttributes attributes;
        XDisplay display = null;
        int id;

        public XWindow(XDisplay udisplay, int uwindow) {
            display = udisplay;
            id = uwindow;
        }

        /// <summary>
        /// ID of the window
        /// </summary>
        public int ID {
            get { return id; }
            set { id = value; }
        }

        /// <summary>
        /// Backreference to the XDisplay from this Window
        /// </summary>
        public XDisplay XDisplay {
            get {
                return display;
            }
        }

        public LinuxAPI.XWindowAttributes Attributes {
            get {
                if(LinuxAPI.X11.XGetWindowAttributes(this.display.Handle, this.ID, ref attributes) == 0) {
                    throw new XWindowException();
                }
                return attributes;
            }
        }
    }


    internal class XWindowException : Exception
    {
        //
    }
}
