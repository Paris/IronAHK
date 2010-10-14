using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace IronAHK.Rusty
{
    partial class Windows
    {
        public class WindowManager : Core.WindowManager
        {
            #region Find

            public override IntPtr LastFound { get; set; }

            public override IntPtr[] AllWindows
            {
                get
                {
                    var ids = new List<IntPtr>();
                    EnumWindows(delegate(IntPtr hwnd, int lParam)
                    {
                        ids.Add(hwnd);
                        return true;
                    }, 0);
                    return ids.ToArray();
                }
            }

            public override IntPtr[] ActiveWindows
            {
                get { throw new NotImplementedException(); }
            }

            public override IntPtr PreviousWindow
            {
                get
                {
                    if (!IsSpecified)
                        return IntPtr.Zero;

                    return GetWindow(ID, GW_HWNDPREV);
                }
            }

            public override IntPtr FindWindow(Core.WindowManager.SearchCriteria criteria)
            {
                if (criteria.IsEmpty)
                    return LastFound;

                var found = IntPtr.Zero;

                if (!string.IsNullOrEmpty(criteria.ClassName) && !criteria.HasExcludes && !criteria.HasID && string.IsNullOrEmpty(criteria.Text))
                    found = Windows.FindWindow(criteria.ClassName, criteria.Title);
                else
                {
                    foreach (var id in AllWindows)
                    {
                        if (CreateWindow(id).Equals(criteria))
                        {
                            found = id;
                            break;
                        }
                    }
                }

                if (found != IntPtr.Zero)
                    LastFound = found;

                return found;
            }

            #endregion

            #region Window

            public override Core.WindowManager CreateWindow(IntPtr id)
            {
                return new WindowManager { ID = id };
            }

            public override IntPtr PID
            {
                get
                {
                    uint n;
                    return new IntPtr(GetWindowThreadProcessId(ID, out n));
                }
            }

            public override bool Active
            {
                get
                {
                    return IsSpecified && Array.IndexOf(ActiveWindows, ID) != -1;
                }
                set
                {
                    if (IsSpecified)
                        SetActiveWindow(ID);
                }
            }

            public override bool Close()
            {
                if (!IsSpecified)
                    return false;

                return DestroyWindow(ID);
            }

            public override bool Exists
            {
                get
                {
                    return IsSpecified ? IsWindow(ID) : false;
                }
            }

            public override string ClassName
            {
                get
                {
                    return IsSpecified ? GetClassName(ID) : string.Empty;
                }
            }

            public override Point Location
            {
                get
                {
                    RECT rect;

                    if (!IsSpecified || !GetWindowRect(ID, out rect))
                        return Point.Empty;

                    return new Point(rect.Left, rect.Top);
                }
                set
                {
                    RECT rect;

                    if (!IsSpecified || !GetWindowRect(ID, out rect))
                        return;

                    MoveWindow(ID, value.X, value.Y, rect.Right - rect.Left, rect.Bottom - rect.Top, true);
                }
            }

            public override Size Size
            {
                get
                {
                    RECT rect;

                    if (!IsSpecified || !GetWindowRect(ID, out rect))
                        return Size.Empty;

                    return new Size(rect.Right - rect.Left, rect.Bottom - rect.Top);
                }
                set
                {
                    RECT rect;

                    if (!IsSpecified || !GetWindowRect(ID, out rect))
                        return;

                    MoveWindow(ID, rect.Left, rect.Right, value.Width, value.Height, true);
                }
            }

            public override bool SelectMenuItem(params string[] items)
            {
                if (!IsSpecified)
                    return false;

                var menu = GetMenu(ID);

                if (menu == IntPtr.Zero || GetMenuItemCount(menu) == 0)
                    return false;

                foreach (var item in items)
                {
                    int n, l = item.Length;

                    if (l > 1 && item[l] == '&' && int.TryParse(item.Substring(0, l), out n))
                    {
                        menu = GetSubMenu(menu, n);

                        if (menu == IntPtr.Zero)
                            return false;
                    }
                    else
                    {
                        for (var i = 0; i < GetMenuItemCount(menu); i++)
                        {
                            var buf = new StringBuilder(1024);
                            var result = GetMenuString(menu, (uint)i, buf, buf.Length - 1, (uint)MF_BYPOSITION);

                            if (result == 0)
                                return false;

                            var name = buf.ToString();

                            if (name.Equals(item, StringComparison.OrdinalIgnoreCase) || name.Replace("&", string.Empty).Equals(item, StringComparison.OrdinalIgnoreCase))
                            {
                                menu = GetSubMenu(menu, i);
                                continue;
                            }
                        }

                        return false;
                    }
                }

                return true;
            }

            public override string Title
            {
                get
                {
                    if (!IsSpecified)
                        return string.Empty;

                    return GetWindowText(ID);
                }
                set
                {
                    if (IsSpecified)
                        SetWindowText(ID, value ?? string.Empty);
                }
            }

            public override string Text
            {
                get
                {
                    throw new NotImplementedException();
                }
                set
                {
                    throw new NotImplementedException();
                }
            }

            public override bool Hide()
            {
                if (!IsSpecified)
                    return false;

                return ShowWindow(ID, SW_HIDE);
            }

            public override bool Kill()
            {
                Close();

                if (!Exists)
                    return true;

                var pid = (uint)PID.ToInt32();
                var prc = pid != 0 ? OpenProcess(PROCESS_ALL_ACCESS, false, pid) : IntPtr.Zero;

                if (prc != IntPtr.Zero)
                {
                    TerminateProcess(prc, 0);
                    CloseHandle(prc);
                }

                return !Exists;
            }

            public override bool AlwaysOnTop
            {
                get
                {
                    return IsSpecified ? (ExStyle & WS_EX_TOPMOST) != 0 : false;
                }
                set
                {
                    if (!IsSpecified)
                        return;

                    var type = new IntPtr(value ? HWND_TOPMOST : HWND_NOTOPMOST);
                    SetWindowPos(ID, type, 0, 0, 0, 0, SWP_NOMOVE | SWP_NOSIZE | SWP_NOACTIVATE);
                }
            }

            public override bool Bottom
            {
                set
                {
                    if (!IsSpecified)
                        return;
                    
                    var type = new IntPtr(value ? HWND_BOTTOM : HWND_TOP);
                    SetWindowPos(ID, type, 0, 0, 0, 0, SWP_NOMOVE | SWP_NOSIZE | SWP_NOACTIVATE);
                }
            }

            public override bool Enabled
            {
                get
                {
                    if (!IsSpecified)
                        return false;

                    return IsWindowEnabled(ID);
                }
                set
                {
                    if (!IsSpecified)
                        return;

                    EnableWindow(ID, value);
                }
            }

            public override bool Redraw()
            {
                if (!IsSpecified)
                    return false;

                return InvalidateRect(ID, IntPtr.Zero, true);
            }

            public override int Style
            {
                get
                {
                    return IsSpecified ? GetWindowLong(ID, GWL_STYLE) : 0;
                }
                set
                {
                    if (IsSpecified)
                        SetWindowLong(ID, GWL_STYLE, value);
                }
            }

            public override int ExStyle
            {
                get
                {
                    return IsSpecified ? GetWindowLong(ID, GWL_EXSTYLE) : 0;
                }
                set
                {
                    if (IsSpecified)
                        SetWindowLong(ID, GWL_EXSTYLE, value);
                }
            }

            public override void SetTransparency(byte level, Color color)
            {
                if (!IsSpecified)
                    return;

                if (level == byte.MaxValue)
                    ExStyle &= ~WS_EX_LAYERED;
                else
                {
                    var flags = LWA_ALPHA;
                    var c = color.B << 16 | color.G << 8 | color.R;

                    if (c != 0)
                        flags |= LWA_COLORKEY;

                    ExStyle |= WS_EX_LAYERED;
                    SetLayeredWindowAttributes(ID, (uint)c, level, (uint)flags);
                }
            }

            public override bool Show()
            {
                if (!IsSpecified)
                    return false;

                return ShowWindow(ID, SW_SHOWDEFAULT);
            }

            public override FormWindowState WindowState
            {
                get
                {
                    if (!IsSpecified)
                        return FormWindowState.Normal;

                    var info = WINDOWPLACEMENT.Default;

                    if (!GetWindowPlacement(ID, out info))
                        return FormWindowState.Normal;

                    switch ((int)info.showCmd)
                    {
                        case SW_MAXIMIZE: return FormWindowState.Maximized;
                        case SW_MINIMIZE: return FormWindowState.Minimized;
                        default: return FormWindowState.Normal;
                    }
                }
                set
                {
                    if (!IsSpecified)
                        return;

                    var cmd = SW_NORMAL;

                    switch (value)
                    {
                        case FormWindowState.Maximized: cmd = SW_MAXIMIZE; break;
                        case FormWindowState.Minimized: cmd = SW_MINIMIZE; break;
                    }

                    ShowWindow(ID, cmd);
                }
            }

            #endregion
        }
    }
}
