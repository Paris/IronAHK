using System;
using System.Drawing;
using System.Windows.Forms;

namespace IronAHK.Rusty
{
    partial class Windows
    {
        public class WindowManager : Core.WindowManager
        {
            #region Find

            public override IntPtr LastFound
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

            public override IntPtr[] AllWindows
            {
                get { throw new NotImplementedException(); }
            }

            public override IntPtr[] ActiveWindows
            {
                get { throw new NotImplementedException(); }
            }

            public override IntPtr[] LastActiveWindows
            {
                get { throw new NotImplementedException(); }
            }

            public override IntPtr[] FindWindow(Core.WindowManager.SearchCriteria criteria)
            {
                throw new NotImplementedException();
            }

            #endregion

            #region Window

            public override Core.WindowManager CreateWindow(IntPtr id)
            {
                return new WindowManager { ID = id };
            }

            protected override IntPtr PID
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
                throw new NotImplementedException();
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
                    throw new NotImplementedException();
                }
                set
                {
                    throw new NotImplementedException();
                }
            }

            public override bool Bottom
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

            public override bool Enabled
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

            public override bool Redraw()
            {
                if (!IsSpecified)
                    return false;

                return InvalidateRect(ID, IntPtr.Zero, true);
            }

            public override long Style
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

            public override long ExStyle
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

            public override byte Transparency
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

            public override Color TransparencyColor
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
                    throw new NotImplementedException();
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
