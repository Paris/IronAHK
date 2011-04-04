using System;
using System.Collections.Generic;
using System.Text;
using IronAHK.Rusty.Linux.X11.Types;
using IronAHK.Rusty.Linux.X11;

namespace IronAHK.Rusty.Linux.Proxies
{
    /// <summary>
    /// represents a single xwindow - proxy for actions affecting x windows
    /// </summary>
    internal class XWindow
    {
        XWindowAttributes _attributes;
        XDisplay _display = null;
        int _id;

        public XWindow(XDisplay display, int window) {
            _display = display;
            _id = window;
        }

        /// <summary>
        /// ID of the window
        /// </summary>
        public int ID {
            get { return _id; }
            set { _id = value; }
        }

        /// <summary>
        /// Backreference to the XDisplay from this Window
        /// </summary>
        public XDisplay XDisplay {
            get {
                return _display;
            }
        }

        public XWindowAttributes Attributes {
            get {
                if(Xlib.XGetWindowAttributes(this._display.Handle, this.ID, ref _attributes) == 0) {
                    throw new XWindowException();
                }
                return _attributes;
            }
        }
    }


    internal class XWindowException : Exception
    {
        //
    }
}
