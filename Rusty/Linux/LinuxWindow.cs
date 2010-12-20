using System;
using System.Collections.Generic;
using System.Text;
using IronAHK.Rusty.Cores.SystemWindow;
using IronAHK.Rusty.Linux.Proxies;
using System.Drawing;

namespace IronAHK.Rusty.Linux
{
    public class LinuxWindow : SystemWindow
    {
        private XWindow xwindow = null;

        public LinuxWindow(IntPtr handle) : base(handle) { }

        internal LinuxWindow(XWindow uxwindow)
            : this(new IntPtr(uxwindow.ID)) {
            xwindow = uxwindow;
        }

        public override IntPtr PID {
            get { throw new NotImplementedException(); }
        }

        public override SystemWindow ParentWindow {
            get { throw new NotImplementedException(); }
        }

        public override SystemWindow PreviousWindow {
            get { throw new NotImplementedException(); }
        }

        public override bool Active {
            get {
                throw new NotImplementedException();
            }
            set {
                throw new NotImplementedException();
            }
        }

        public override bool Exists {
            get { throw new NotImplementedException(); }
        }

        public override string ClassName {
            get { throw new NotImplementedException(); }
        }

        public override Point Location {
            get {
                var attr = xwindow.Attributes;
                return new Point(attr.x, attr.y);
            }
            set {
                throw new NotImplementedException();
            }
        }

        public override Size Size {
            get {
                var attr = xwindow.Attributes;
                return new Size(attr.width, attr.height);
            }
            set {
                throw new NotImplementedException();
            }
        }

        public override string Title {
            get {
				LinuxAPI.XTextProperty Prop = new LinuxAPI.XTextProperty();
				LinuxAPI.X11.XGetTextProperty(xwindow.XDisplay.Handle, xwindow.ID, ref Prop, LinuxAPI.Atom.XA_WM_NAME);
				return Prop.value;
            }
            set {
				LinuxAPI.XTextProperty Prop = new LinuxAPI.XTextProperty();
				Prop.value = value;
				LinuxAPI.X11.XSetTextProperty(xwindow.XDisplay.Handle, xwindow.ID, ref Prop, LinuxAPI.Atom.XA_WM_NAME);
            }
        }

        public override string[] Text {
            get { throw new NotImplementedException(); }
        }

        public override bool AlwaysOnTop {
            get {
                throw new NotImplementedException();
            }
            set {
                throw new NotImplementedException();
            }
        }

        public override bool Bottom {
            set { throw new NotImplementedException(); }
        }

        public override bool Enabled {
            get {
                throw new NotImplementedException();
            }
            set {
                throw new NotImplementedException();
            }
        }

        public override int Style {
            get {
                throw new NotImplementedException();
            }
            set {
                throw new NotImplementedException();
            }
        }

        public override int ExStyle {
            get {
                throw new NotImplementedException();
            }
            set {
                throw new NotImplementedException();
            }
        }

        public override IEnumerable<SystemWindow> ChildWindows {
            get { throw new NotImplementedException(); }
        }

        public override string ClassNN {
            get { throw new NotImplementedException(); }
        }

        public override System.Windows.Forms.FormWindowState WindowState {
            get {
                throw new NotImplementedException();
            }
            set {
                throw new NotImplementedException();
            }
        }

        public override bool Close() {
            throw new NotImplementedException();
        }

        public override bool SelectMenuItem(params string[] items) {
            throw new NotImplementedException();
        }

        public override bool Hide() {
            throw new NotImplementedException();
        }

        public override bool Kill() {
            throw new NotImplementedException();
        }

        public override bool Redraw() {
            throw new NotImplementedException();
        }

        public override void SetTransparency(byte level, System.Drawing.Color color) {
            throw new NotImplementedException();
        }

        public override bool Show() {
            throw new NotImplementedException();
        }

        public override SystemWindow RealChildWindowFromPoint(System.Drawing.Point location) {
            throw new NotImplementedException();
        }

        public override void SendMouseEvent(WindowsAPI.MOUSEEVENTF mouseevent, System.Drawing.Point? location = null) {
            throw new NotImplementedException();
        }
    }
}
