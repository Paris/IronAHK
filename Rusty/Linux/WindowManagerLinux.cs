using System;
using System.Windows.Forms;
using IronAHK.Rusty.Cores.SystemWindow;
using System.Collections.Generic;
using System.Drawing;

namespace IronAHK.Rusty.Linux
{
    public class WindowManagerLinux : WindowManager
    {
        public override SystemWindow LastFound {
            get {
                throw new NotImplementedException();
            }
            set {
                throw new NotImplementedException();
            }
        }

        public override void MinimizeAll() {
            throw new NotImplementedException();
        }

        public override void MinimizeAllUndo() {
            throw new NotImplementedException();
        }

        public override IEnumerable<SystemWindow> AllWindows {
            get { throw new NotImplementedException(); }
        }

        public override SystemWindow ActiveWindow {
            get { throw new NotImplementedException(); }
        }

        public override SystemWindow FindWindow(SearchCriteria criteria) {
            throw new NotImplementedException();
        }

        public override SystemWindow CreateWindow(IntPtr id) {
            throw new NotImplementedException();
        }

        public override SystemWindow GetForeGroundWindow() {
            throw new NotImplementedException();
        }



        public override SystemWindow WindowFromPoint(Point location) {
            throw new NotImplementedException();
        }
    }

}
