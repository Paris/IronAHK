using System;
using System.Windows.Forms;
using System.Collections.Generic;
using System.Drawing;
using IronAHK.Rusty.Linux.Proxies;
using IronAHK.Rusty.Common;

namespace IronAHK.Rusty.Linux
{

    class WindowManager : Common.Window.WindowManagerBase
    {
        // ToDo: There may be more than only one xDisplay
        XDisplay _display = null;

        public WindowManager() {
            _display = XDisplay.Default;
        }

        public override Window.WindowItemBase LastFound
        {
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

        public override IEnumerable<Window.WindowItemBase> AllWindows
        {
            get { throw new NotImplementedException(); }
        }

        public override Window.WindowItemBase ActiveWindow
        {
            get {
                return new WindowItem(_display.XGetInputFocus());
            }
        }

        public override Window.WindowItemBase FindWindow(Window.SearchCriteria criteria)
        {
            throw new NotImplementedException();
        }

        public override Window.WindowItemBase CreateWindow(IntPtr id)
        {
            throw new NotImplementedException();
        }

        public override Window.WindowItemBase GetForeGroundWindow()
        {
            throw new NotImplementedException();
        }

        public override Window.WindowItemBase WindowFromPoint(Point location)
        {
            throw new NotImplementedException();
        }
    }

}
