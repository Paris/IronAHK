using System;
using System.Windows.Forms;

namespace IronAHK.Rusty
{
    partial class Linux
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
                throw new NotImplementedException();
            }

            protected override IntPtr PID
            {
                get { throw new NotImplementedException(); }
            }

            public override bool Active
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

            public override bool Close()
            {
                throw new NotImplementedException();
            }

            public override bool Exists
            {
                get { throw new NotImplementedException(); }
            }

            public override string ClassName
            {
                get { throw new NotImplementedException(); }
            }

            public override System.Drawing.Point Location
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

            public override System.Drawing.Size Size
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

            public override bool SelectMenuItem(params string[] items)
            {
                throw new NotImplementedException();
            }

            public override string Title
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
                throw new NotImplementedException();
            }

            public override bool Kill()
            {
                throw new NotImplementedException();
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
                throw new NotImplementedException();
            }

            public override int Style
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

            public override int ExStyle
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

            public override void SetTransparency(byte level, System.Drawing.Color color)
            {
                throw new NotImplementedException();
            }

            public override bool Show()
            {
                throw new NotImplementedException();
            }

            public override FormWindowState WindowState
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

            #endregion
        }
    }
}
