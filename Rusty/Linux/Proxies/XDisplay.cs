using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;
using IronAHK.Rusty.Linux.X11;

namespace IronAHK.Rusty.Linux.Proxies
{
    /// <summary>
    /// Proxy around a X11 xDisplay
    /// </summary>
    internal class XDisplay : IDisposable
    {
        static IntPtr _defaultDisp = IntPtr.Zero;
        readonly IntPtr _handle = IntPtr.Zero;

        public XDisplay(IntPtr prt) {
            _handle = prt;
        }

        public IntPtr Handle {
            get { return _handle; }
        }

        public XWindow Root {
            get {
                return new XWindow(this, Xlib.XDefaultRootWindow(this._handle));
            }
        }

        /// <summary>
        /// Returns all Windows of this XDisplay
        /// </summary>
        /// <returns></returns>
        public IEnumerable<XWindow> XQueryTree() {
            return XQueryTree(this.Root);
        }

        /// <summary>
        /// Return all child xWindows from given xWindow
        /// </summary>
        /// <param name="windowToObtain"></param>
        /// <returns></returns>
        public IEnumerable<XWindow> XQueryTree(XWindow windowToObtain) {
            int root_return, parent_return;
            IntPtr children_return;
            int nchildren_return;

            Xlib.XQueryTree(_handle, windowToObtain.ID, out root_return, out parent_return, out children_return, out nchildren_return);
            var childs = new int[nchildren_return];
            Marshal.Copy(children_return, childs, 0, nchildren_return);

            var wins = new List<XWindow>();
            foreach(int id in childs) {
                wins.Add(new XWindow(this, id));
            }

            return wins;
        }


        /// <summary>
        /// Returns the window which currently has input focus
        /// </summary>
        /// <returns></returns>
        public XWindow XGetInputFocus() {
            int hwndWnd;
            int focusState;
            Xlib.XGetInputFocus(_handle, out hwndWnd, out focusState);

            return new XWindow(this, hwndWnd);
        }


        public static XDisplay Default {
            get {
                if(_defaultDisp == IntPtr.Zero)
                    _defaultDisp = Xlib.XOpenDisplay(IntPtr.Zero);
                return new XDisplay(_defaultDisp);
            }
        }

        public void Dispose() {
            if(_handle != IntPtr.Zero && _handle != _defaultDisp)
                Xlib.XCloseDisplay(_handle);
        }
    }
}
