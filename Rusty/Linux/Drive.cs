using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace IronAHK.Rusty.Linux
{
    /// <summary>
    /// Implementation for native Linux Drive Operations
    /// </summary>
    class Drive : Common.Drive
    {
        public Drive(DriveInfo drv)
            : base(drv) { }

        public override void Eject() {
            throw new NotImplementedException();
        }

        public override void Retract() {
            throw new NotImplementedException();
        }

        public override StatusCD Status {
            get { throw new NotImplementedException(); }
        }

        public override long Serial {
            get { throw new NotImplementedException(); }
        }

        public override void Lock() {
            throw new NotImplementedException();
        }

        public override void UnLock() {
            throw new NotImplementedException();
        }
    }
}
