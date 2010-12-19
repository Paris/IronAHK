using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;

namespace IronAHK.Rusty.Linux.Proxies
{
    internal class XDisplay : IDisposable
    {
        public IntPtr handle = IntPtr.Zero;

        public XDisplay(IntPtr prt) {
            Handle = prt;
        }

        public IntPtr Handle {
            get { return handle; }
            set { handle = value; }
        }

        public XWindow Root {
            get {
                return new XWindow(this, LinuxAPI.X11.XDefaultRootWindow(this.handle));
            }
        }

        public IEnumerable<XWindow> XQueryTree() {
            return XQueryTree(this.Root);
        }
        public IEnumerable<XWindow> XQueryTree(XWindow windowToObtain) {
            int root_return, parent_return;
            IntPtr children_return;
            int nchildren_return;

            LinuxAPI.X11.XQueryTree(Handle, windowToObtain.ID, out root_return, out parent_return, out children_return, out nchildren_return);
            var childs = new int[nchildren_return];
            Marshal.Copy(children_return, childs, 0, nchildren_return);

            var wins = new List<XWindow>();
            foreach(int id in childs) {
                wins.Add(new XWindow(this, id));
            }

            return wins;
        }

        public static XDisplay GetDefault() {
            return new XDisplay(LinuxAPI.X11.XOpenDisplay(IntPtr.Zero));
        }

        public void Dispose() {
            LinuxAPI.X11.XCloseDisplay(handle);
        }
    }
}
