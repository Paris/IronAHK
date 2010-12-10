using System;
using System.Collections.Generic;
using System.Text;
using IronAHK.Rusty.Cores.SystemWindow;
using System.Drawing;
using System.Windows.Forms;

namespace IronAHK.Rusty.Windows.SystemWindows
{

    /// <summary>
    /// represents a window under windows operating system
    /// </summary>
    public class WindowWindows : SystemWindow
    {

        public WindowWindows(IntPtr handle) : base(handle) { }


        public override IntPtr PID {
            get {
                uint n;
                return new IntPtr(WindowsAPI.GetWindowThreadProcessId(this.Handle, out n));
            }
        }

        /// <summary>
        /// returns the previous Window
        /// </summary>
        public override SystemWindow PreviousWindow {
            get {
                if(!IsSpecified)
                    return null;

                return new WindowWindows(WindowsAPI.GetWindow(this.Handle, WindowsAPI.GW_HWNDPREV));
            }
        }

        public override bool Active {
            get {
                return IsSpecified && SysWindowManager.Instance.ActiveWindow == this;
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
    }

}
