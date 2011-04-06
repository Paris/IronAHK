using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Windows.Forms;
using IronAHK.Rusty.Common;

namespace IronAHK.Rusty.Windows
{

    /// <summary>
    /// represents a window under windows operating system
    /// </summary>
    class WindowItem : Common.Window.WindowItemBase
    {
        #region Constructor

        public WindowItem(IntPtr handle) : base(handle) { }

        #endregion

        #region Public Properties

        public override IntPtr PID {
            get {
                uint n;
                return new IntPtr(WindowsAPI.GetWindowThreadProcessId(this.Handle, out n));
            }
        }

        public override Window.WindowItemBase ParentWindow
        {
            get {
                return new WindowItem(WindowsAPI.GetAncestor(this.Handle, WindowsAPI.gaFlags.GA_PARENT));
            }
        }

        /// <summary>
        /// returns the previous Window
        /// </summary>
        public override Window.WindowItemBase PreviousWindow
        {
            get {
                if(!IsSpecified)
                    return null;

                return new WindowItem(WindowsAPI.GetWindow(this.Handle, WindowsAPI.GW_HWNDPREV));
            }
        }

        public override bool Active {
            get {
                return IsSpecified && Window.WindowItemProvider.Instance.ActiveWindow == this;
            }
            set {
                if(IsSpecified)
                    WindowsAPI.SetActiveWindow(Handle);
            }
        }

        public override bool Exists {
            get {
                return IsSpecified ? WindowsAPI.IsWindow(this.Handle) : false;
            }
        }

        public override string ClassName {
            get {
                return IsSpecified ? WindowsAPI.GetClassName(this.Handle) : string.Empty;
            }
        }


        public override Point Location {
            get {
                WindowsAPI.RECT rect;

                if(!IsSpecified || !WindowsAPI.GetWindowRect(this.Handle, out rect))
                    return Point.Empty;

                return new Point(rect.Left, rect.Top);
            }
            set {
                WindowsAPI.RECT rect;

                if(!IsSpecified || !WindowsAPI.GetWindowRect(this.Handle, out rect))
                    return;

                WindowsAPI.MoveWindow(this.Handle, value.X, value.Y, rect.Right - rect.Left, rect.Bottom - rect.Top, true);
            }
        }


        public override Size Size {
            get {
                WindowsAPI.RECT rect;

                if(!IsSpecified || !WindowsAPI.GetWindowRect(this.Handle, out rect))
                    return Size.Empty;

                return new Size(rect.Right - rect.Left, rect.Bottom - rect.Top);
            }
            set {
                WindowsAPI.RECT rect;

                if(!IsSpecified || !WindowsAPI.GetWindowRect(this.Handle, out rect))
                    return;
                WindowsAPI.MoveWindow(this.Handle, rect.Left, rect.Right, value.Width, value.Height, true);
            }
        }

        public override string Title {
            get {
                if(!IsSpecified)
                    return string.Empty;

                return WindowsAPI.GetWindowText(this.Handle);
            }
            set {
                if(IsSpecified)
                    WindowsAPI.SetWindowText(this.Handle, value ?? string.Empty);
            }
        }

        public override string[] Text {
            get {
                if(!IsSpecified)
                    return new string[0];

                var items = new List<string>();

                WindowsAPI.EnumChildWindows(this.Handle, delegate(IntPtr hwnd, int lParam)
                {
                    var text = WindowsAPI.GetWindowText(hwnd);
                    items.Add(text);
                    return true;
                }, 0);

                return items.ToArray();
            }
        }


        public override bool AlwaysOnTop {
            get {
                return IsSpecified ? (ExStyle & WindowsAPI.WS_EX_TOPMOST) != 0 : false;
            }
            set {
                if(!IsSpecified)
                    return;

                var type = new IntPtr(value ? WindowsAPI.HWND_TOPMOST : WindowsAPI.HWND_NOTOPMOST);
                WindowsAPI.SetWindowPos(this.Handle, type, 0, 0, 0, 0, WindowsAPI.SWP_NOMOVE | WindowsAPI.SWP_NOSIZE | WindowsAPI.SWP_NOACTIVATE);
            }
        }


        public override bool Bottom {
            set {
                if(!IsSpecified)
                    return;

                var type = new IntPtr(value ? WindowsAPI.HWND_BOTTOM : WindowsAPI.HWND_TOP);
                WindowsAPI.SetWindowPos(this.Handle, type, 0, 0, 0, 0, WindowsAPI.SWP_NOMOVE | WindowsAPI.SWP_NOSIZE | WindowsAPI.SWP_NOACTIVATE);
            }
        }

        public override bool Enabled {
            get {
                if(!IsSpecified)
                    return false;

                return WindowsAPI.IsWindowEnabled(this.Handle);
            }
            set {
                if(!IsSpecified)
                    return;

                WindowsAPI.EnableWindow(this.Handle, value);
            }
        }

        public override int Style {
            get {
                return IsSpecified ? WindowsAPI.GetWindowLong(this.Handle, WindowsAPI.GWL_STYLE) : 0;
            }
            set {
                if(IsSpecified)
                    WindowsAPI.SetWindowLong(this.Handle, WindowsAPI.GWL_STYLE, value);
            }
        }

        public override int ExStyle {
            get {
                return IsSpecified ? WindowsAPI.GetWindowLong(this.Handle, WindowsAPI.GWL_EXSTYLE) : 0;
            }
            set {
                if(IsSpecified)
                    WindowsAPI.SetWindowLong(this.Handle, WindowsAPI.GWL_EXSTYLE, value);
            }
        }

        public override FormWindowState WindowState {
            get {
                if(!IsSpecified)
                    return FormWindowState.Normal;

                var info = WindowsAPI.WINDOWPLACEMENT.Default;

                if(!WindowsAPI.GetWindowPlacement(this.Handle, out info))
                    return FormWindowState.Normal;

                switch((int)info.showCmd) {
                    case WindowsAPI.SW_MAXIMIZE: return FormWindowState.Maximized;
                    case WindowsAPI.SW_MINIMIZE: return FormWindowState.Minimized;
                    default: return FormWindowState.Normal;
                }
            }
            set {
                if(!IsSpecified)
                    return;

                var cmd = WindowsAPI.SW_NORMAL;

                switch(value) {
                    case FormWindowState.Maximized: cmd = WindowsAPI.SW_MAXIMIZE; break;
                    case FormWindowState.Minimized: cmd = WindowsAPI.SW_MINIMIZE; break;
                }

                WindowsAPI.ShowWindow(this.Handle, cmd);
            }
        }

        public override IEnumerable<Window.WindowItemBase> ChildWindows
        {
            get {
                var childs = new List<Window.WindowItemBase>();
                if(this.IsSpecified) {
                    WindowsAPI.EnumChildWindows(this.Handle, delegate(IntPtr hwnd, int lParam)
                    {
                        childs.Add(new WindowItem(hwnd));
                        return true;
                    }, 0);
                }
                return childs;
            }
        }

        #endregion

        public override string ClassNN {
            get {
                string className = this.ClassName;
                string classNN = className;
                // to get the classNN we must know the enumeration
                // of our parent window:
                var parent = this.ParentWindow;

                if(parent.IsSpecified){
                    int nn = 1; // Class NN counter
                    // now we must know the postion of our "control"
                    foreach(var c in parent.ChildWindows) {
                        if(c.IsSpecified) {
                            if(c.ClassName == className) {
                                if(c.Equals(this)) {
                                    break;
                                } else
                                    ++nn;  // if its the same class but not our control
                            }
                        }
                    }
                    classNN += nn.ToString(); // if its the same class and our control
                }
                return classNN;
            }
        }



        #region Methods

        public override bool Close() {
            if(!IsSpecified)
                return false;
            return WindowsAPI.PostMessage(this.Handle, WindowsAPI.WM_CLOSE, IntPtr.Zero, IntPtr.Zero);
        }

        public override bool SelectMenuItem(params string[] items) {
            if(!IsSpecified)
                return false;

            var menu = WindowsAPI.GetMenu(this.Handle);

            if(menu == IntPtr.Zero || WindowsAPI.GetMenuItemCount(menu) == 0)
                return false;

            foreach(var item in items) {
                int n, l = item.Length;

                if(l > 1 && item[l] == '&' && int.TryParse(item.Substring(0, l), out n)) {
                    menu = WindowsAPI.GetSubMenu(menu, n);

                    if(menu == IntPtr.Zero)
                        return false;
                } else {
                    for(var i = 0; i < WindowsAPI.GetMenuItemCount(menu); i++) {
                        var buf = new StringBuilder(1024);
                        var result = WindowsAPI.GetMenuString(menu, (uint)i, buf, buf.Length - 1, (uint)WindowsAPI.MF_BYPOSITION);

                        if(result == 0)
                            return false;

                        var name = buf.ToString();

                        if(name.Equals(item, StringComparison.OrdinalIgnoreCase) || name.Replace("&", string.Empty).Equals(item, StringComparison.OrdinalIgnoreCase)) {
                            menu = WindowsAPI.GetSubMenu(menu, i);
                            continue;
                        }
                    }

                    return false;
                }
            }

            return true;
        }

        public override bool Hide() {
            if(!IsSpecified)
                return false;
            return WindowsAPI.ShowWindow(this.Handle, WindowsAPI.SW_HIDE);
        }

        public override bool Kill() {
            Close();

            if(!Exists)
                return true;

            var pid = (uint)PID.ToInt32();
            var prc = pid != 0 ? WindowsAPI.OpenProcess(WindowsAPI.PROCESS_ALL_ACCESS, false, pid) : IntPtr.Zero;

            if(prc != IntPtr.Zero) {
                WindowsAPI.TerminateProcess(prc, 0);
                WindowsAPI.CloseHandle(prc);
            }

            return !Exists;
        }

        public override bool Redraw() {
            if(!IsSpecified)
                return false;

            return WindowsAPI.InvalidateRect(this.Handle, IntPtr.Zero, true);
        }

        public override void SetTransparency(byte level, Color color) {
            if(!IsSpecified)
                return;

            if(level == byte.MaxValue)
                ExStyle &= ~WindowsAPI.WS_EX_LAYERED;
            else {
                var flags = WindowsAPI.LWA_ALPHA;
                var c = color.B << 16 | color.G << 8 | color.R;

                if(c != 0)
                    flags |= WindowsAPI.LWA_COLORKEY;

                ExStyle |= WindowsAPI.WS_EX_LAYERED;
                WindowsAPI.SetLayeredWindowAttributes(this.Handle, (uint)c, level, (uint)flags);
            }
        }

        public override bool Show() {
            if(!IsSpecified)
                return false;

            return WindowsAPI.ShowWindow(this.Handle, WindowsAPI.SW_SHOWDEFAULT);
        }

        #endregion

        /// <summary>
        /// Searches for a child window/control at <paramref name="location"/> 
        /// </summary>
        /// <param name="location"></param>
        /// <returns></returns>
        public override Window.WindowItemBase RealChildWindowFromPoint(Point location)
        {
            Window.WindowItemBase child = null;
            if(this.IsSpecified)
                child = new WindowItem(WindowsAPI.RealChildWindowFromPoint(this.Handle, location));
            return child;  
        }

        public override void SendMouseEvent(WindowsAPI.MOUSEEVENTF mouseevent, Point? location = null) {
            var click = new Point();
            if(location.HasValue) {
                click = location.Value;
            } else {
                // if not specified find middlepoint of this window/control
                var size = this.Size;
                click.X = size.Width / 2;
                click.Y = size.Height / 2;
            }
            var lparam = new IntPtr(WindowsAPI.MakeInt((short)click.X, (short)click.Y));
            WindowsAPI.PostMessage(this.Handle, (uint)mouseevent, new IntPtr(1), lparam);
        }


    }

}
