using System;
using System.Collections.Generic;
using System.Text;
using IronAHK.Rusty.Cores.SystemWindow;
using IronAHK.Rusty.Linux.Proxies;
using System.Drawing;
using IronAHK.Rusty.Linux.X11;
using IronAHK.Rusty.Linux.X11.Types;

namespace IronAHK.Rusty.Linux
{
    public class LinuxWindow : SystemWindow
    {
        #region Fields

        XWindow _xwindow = null;

        #endregion

        #region Constructors

        LinuxWindow(IntPtr handle) : base(handle) { }

        internal LinuxWindow(XWindow uxwindow)
            : this(new IntPtr(uxwindow.ID)) {
            _xwindow = uxwindow;
        }

        #endregion

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
                var attr = _xwindow.Attributes;
                return new Point(attr.x, attr.y);
            }
            set {
                throw new NotImplementedException();
            }
        }

        public override Size Size {
            get {
                var attr = _xwindow.Attributes;
                return new Size(attr.width, attr.height);
            }
            set {
                throw new NotImplementedException();
            }
        }

        public override string Title {
            get {
				XTextProperty Prop = new XTextProperty();
				Xlib.XGetTextProperty(_xwindow.XDisplay.Handle, _xwindow.ID, ref Prop, Atom.XA_WM_NAME);
				return Prop.value;
            }
            set {
				var prop = new XTextProperty();
                prop.value = value;
                Xlib.XSetTextProperty(_xwindow.XDisplay.Handle, _xwindow.ID, ref prop, Atom.XA_WM_NAME);
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
