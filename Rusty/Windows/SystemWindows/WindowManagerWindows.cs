using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using IronAHK.Rusty.Cores.SystemWindow;

namespace IronAHK.Rusty.Windows.SystemWindows
{
    /// <summary>
    /// Concrete Implementation of WindowManager for Windows Platfrom
    /// </summary>
    public class WindowManagerWindows : WindowManager
    {
        #region Find

        public override SystemWindow LastFound { get; set; }

        public override IEnumerable<SystemWindow> AllWindows {
            get {
                var windows = new List<SystemWindow>();
                WindowsAPI.EnumWindows(delegate(IntPtr hwnd, int lParam)
                {
                    windows.Add(new WindowWindows(hwnd));
                    return true;
                }, 0);
                return windows;
            }
        }

        public override SystemWindow ActiveWindow {
            get { return new WindowWindows(WindowsAPI.GetForegroundWindow()); }
        }


        public override SystemWindow FindWindow(SearchCriteria criteria) {
            SystemWindow found = null;

            if(criteria.IsEmpty)
                return found;

            if(!string.IsNullOrEmpty(criteria.ClassName) && !criteria.HasExcludes && !criteria.HasID && string.IsNullOrEmpty(criteria.Text))
                found = new WindowWindows(WindowsAPI.FindWindow(criteria.ClassName, criteria.Title));
            else {
                foreach(var window in AllWindows) {
                    if(window.Equals(criteria)) {
                        found = window;
                        break;
                    }
                }
            }

            if(found.IsSpecified)
                LastFound = found;

            return found;
        }

        #endregion

        public override void MinimizeAll() {
            
            var window = this.FindWindow(new SearchCriteria { ClassName = "Shell_TrayWnd" });
            WindowsAPI.PostMessage(window.Handle, (uint)WindowsAPI.WM_COMMAND, new IntPtr(419), IntPtr.Zero);
        }

        public override void MinimizeAllUndo() {
            var window = FindWindow(new SearchCriteria { ClassName = "Shell_TrayWnd" });
            WindowsAPI.PostMessage(window.Handle, (uint)WindowsAPI.WM_COMMAND, new IntPtr(416), IntPtr.Zero);
        }

        public override SystemWindow CreateWindow(IntPtr id) {
            return new WindowWindows(id);
        }
    }
}
