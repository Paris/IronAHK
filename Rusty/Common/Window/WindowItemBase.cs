using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using System.Drawing;
using IronAHK.Rusty;
using System.Text.RegularExpressions;

namespace IronAHK.Rusty.Common
{
    partial class Window
    {
        /// <summary>
        /// Abstraction of a single Platform independend Window
        /// </summary>
        public abstract class WindowItemBase
        {
            #region Private Fields

            private int delay = 100;
            private IntPtr windowHandle = IntPtr.Zero;

            #endregion

            #region Constructor

            public WindowItemBase(IntPtr handle)
            {
                this.Handle = handle;
            }

            #endregion

            #region Public Properties

            /// <summary>
            /// OS Handle to identify the Window.
            /// </summary>
            public IntPtr Handle
            {
                get { return windowHandle; }
                set { windowHandle = value; }
            }

            public bool IsSpecified
            {
                get { return this.Handle != IntPtr.Zero; }
            }

            public int Delay
            {
                get { return delay; }
                set { delay = value; }
            }

            public abstract IntPtr PID { get; }

            public abstract WindowItemBase ParentWindow { get; }

            public abstract WindowItemBase PreviousWindow { get; }

            public abstract bool Active { get; set; }

            public abstract bool Exists { get; }

            public abstract string ClassName { get; }

            public abstract Point Location { get; set; }

            public abstract Size Size { get; set; }

            public abstract string Title { get; set; }

            public abstract string[] Text { get; }

            public abstract bool AlwaysOnTop { get; set; }

            public abstract bool Bottom { set; }

            public abstract bool Enabled { get; set; }

            public abstract int Style { get; set; }

            public abstract int ExStyle { get; set; }

            /// <summary>
            /// Enumerates all child windows/controls
            /// </summary>
            public abstract IEnumerable<WindowItemBase> ChildWindows { get; }

            /// <summary>
            /// Get the ClassName + number of occurence of this window (control)
            /// </summary>
            public abstract string ClassNN { get; }


            public abstract FormWindowState WindowState { get; set; }

            #endregion

            #region Methods

            public abstract bool Close();

            public abstract bool SelectMenuItem(params string[] items);

            public abstract bool Hide();

            public abstract bool Kill();

            public abstract bool Redraw();

            public abstract void SetTransparency(byte level, Color color);

            public abstract bool Show();


            public abstract WindowItemBase RealChildWindowFromPoint(Point location);

            /// <summary>
            /// Sends MouseEvent to the current Window/Child/Control
            /// </summary>
            /// <param name="mouseevent">MouseEvent to send (for now we take also on abstract layer the windows enum, this might be mapped for other OS.)</param>
            /// <param name="location"></param>
            public abstract void SendMouseEvent(WindowsAPI.MOUSEEVENTF mouseevent, Point? location = null);

            /// <summary>
            /// Left-Clicks on this window/control
            /// </summary>
            /// <param name="location"></param>
            public virtual void Click(Point? location = null)
            {
                this.SendMouseEvent(WindowsAPI.MOUSEEVENTF.LEFTDOWN, location);
                this.SendMouseEvent(WindowsAPI.MOUSEEVENTF.LEFTUP, location);
            }

            /// <summary>
            /// Right-Clicks on this window/control
            /// </summary>
            /// <param name="location"></param>
            public virtual void ClickRight(Point? location = null)
            {
                this.SendMouseEvent(WindowsAPI.MOUSEEVENTF.RIGHTDOWN, location);
                this.SendMouseEvent(WindowsAPI.MOUSEEVENTF.RIGHTUP, location);
            }



            #region Wait

            public bool Wait(int timeout = -1)
            {
                if (timeout != -1)
                    timeout += Environment.TickCount;

                while (!this.Exists)
                {
                    if (timeout != -1 && Environment.TickCount >= timeout)
                        return false;

                    System.Threading.Thread.Sleep(Delay);
                }
                return true;
            }

            public bool WaitActive(int timeout = -1)
            {
                if (timeout != -1)
                    timeout += Environment.TickCount;

                while (!this.Active)
                {
                    if (timeout != -1 && Environment.TickCount >= timeout)
                        return false;

                    System.Threading.Thread.Sleep(Delay);
                }

                return true;
            }

            public bool WaitClose(int timeout = -1)
            {
                if (timeout != -1)
                    timeout += Environment.TickCount;

                while (this.Exists)
                {
                    if (timeout != -1 && Environment.TickCount >= timeout)
                        return false;

                    System.Threading.Thread.Sleep(Delay);
                }

                return true;
            }

            public bool WaitNotActive(int timeout = -1)
            {
                if (timeout != -1)
                    timeout += Environment.TickCount;

                while (this.Active)
                {
                    if (timeout != -1 && Environment.TickCount >= timeout)
                        return false;

                    System.Threading.Thread.Sleep(Delay);
                }

                return true;
            }

            #endregion


            public bool Equals(SearchCriteria criteria)
            {
                if (!IsSpecified)
                    return false;

                if (criteria.ID != IntPtr.Zero && this.Handle != criteria.ID)
                    return false;

                if (criteria.PID != IntPtr.Zero && PID != criteria.PID)
                    return false;

                var comp = StringComparison.OrdinalIgnoreCase;

                if (!string.IsNullOrEmpty(criteria.ClassName))
                {
                    if (!ClassName.Equals(criteria.ClassName, comp))
                        return false;
                }

                if (!string.IsNullOrEmpty(criteria.Title))
                {
                    if (!TitleCompare(Title, criteria.Title))
                        return false;
                }

                if (!string.IsNullOrEmpty(criteria.Text))
                {
                    foreach (var text in Text)
                        if (text.IndexOf(criteria.Text, comp) == -1)
                            return false;
                }

                if (!string.IsNullOrEmpty(criteria.ExcludeTitle))
                {
                    if (Title.IndexOf(criteria.ExcludeTitle, comp) != -1)
                        return false;
                }

                if (!string.IsNullOrEmpty(criteria.ExcludeText))
                {
                    foreach (var text in Text)
                        if (text.IndexOf(criteria.ExcludeText, comp) != -1)
                            return false;
                }

                return true;
            }

            #endregion

            #region Overrides

            /// <summary>
            /// Define Standard Equalty Opertaor
            /// </summary>
            /// <param name="obj"></param>
            /// <returns></returns>
            public override bool Equals(object obj)
            {
                if (obj is WindowItemBase)
                {
                    return (obj as WindowItemBase).Handle == this.Handle;
                }
                else
                    return base.Equals(obj);
            }

            public override int GetHashCode()
            {
                return base.GetHashCode();
            }

            public override string ToString()
            {
                return (this.IsSpecified) ? this.Title : "not specified window";
            }
            #endregion

            #region Private Helper Methods

            private static bool TitleCompare(string a, string b)
            {
                var comp = StringComparison.OrdinalIgnoreCase;

                switch (Core.A_TitleMatchMode.ToLowerInvariant())
                {
                    case "1":
                        return a.StartsWith(b, comp);

                    case "2":
                        return a.IndexOf(b, comp) != -1;

                    case "3":
                        return a.Equals(b, comp);

                    case Core.Keyword_RegEx:
                        return new Regex(b).IsMatch(a);
                }
                return false;
            }

            #endregion

        }
    }
}
