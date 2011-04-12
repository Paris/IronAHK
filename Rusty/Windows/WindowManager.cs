using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using IronAHK.Rusty.Common;

namespace IronAHK.Rusty.Windows
{
    /// <summary>
    /// Concrete Implementation of WindowManager for Windows Platfrom
    /// </summary>
    class WindowManager : Common.Window.WindowManagerBase
    {
        #region Find

        public override Window.WindowItemBase LastFound { get; set; }

        public override IEnumerable<Window.WindowItemBase> AllWindows
        {
            get {
                var windows = new List<Window.WindowItemBase>();
                WindowsAPI.EnumWindows(delegate(IntPtr hwnd, int lParam)
                {
                    windows.Add(new WindowItem(hwnd));
                    return true;
                }, 0);
                return windows;
            }
        }

        public override Window.WindowItemBase ActiveWindow
        {
            get { return new WindowItem(WindowsAPI.GetForegroundWindow()); }
        }


        public override Window.WindowItemBase FindWindow(Window.SearchCriteria criteria)
        {
            Window.WindowItemBase found = null;

            if(criteria.IsEmpty)
                return found;

            if(!string.IsNullOrEmpty(criteria.ClassName) && !criteria.HasExcludes && !criteria.HasID && string.IsNullOrEmpty(criteria.Text))
                found = new WindowItem(WindowsAPI.FindWindow(criteria.ClassName, criteria.Title));
            else {
                foreach(var window in AllWindows) {
                    if(window.Equals(criteria)) {
                        found = window;
                        break;
                    }
                }
            }

            if(found != null && found.IsSpecified)
                LastFound = found;

            return found;
        }

        #endregion

        public override void MinimizeAll() {

            var window = this.FindWindow(new Window.SearchCriteria { ClassName = "Shell_TrayWnd" });
            WindowsAPI.PostMessage(window.Handle, (uint)WindowsAPI.WM_COMMAND, new IntPtr(419), IntPtr.Zero);
        }

        public override void MinimizeAllUndo() {
            var window = FindWindow(new Window.SearchCriteria { ClassName = "Shell_TrayWnd" });
            WindowsAPI.PostMessage(window.Handle, (uint)WindowsAPI.WM_COMMAND, new IntPtr(416), IntPtr.Zero);
        }

        public override Window.WindowItemBase CreateWindow(IntPtr id)
        {
            return new WindowItem(id);
        }

        public override Window.WindowItemBase GetForeGroundWindow()
        {
            return  new WindowItem(WindowsAPI.GetForegroundWindow());
        }

        public override Window.WindowItemBase WindowFromPoint(Point location)
        {
            return new WindowItem(WindowsAPI.WindowFromPoint(location));
        }
    }
}
