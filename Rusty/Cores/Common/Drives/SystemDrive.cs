using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace IronAHK.Rusty.Cores.Common.Drives
{
    internal enum StatusCD
    {
        NotReady,
        Open,
        Playing,
        Paused,
        Seeking,
        Stopped
    }

    internal abstract class SystemDrive
    {
        protected DriveInfo drive;

        public SystemDrive(DriveInfo udrive) {
            drive = udrive;
        }

        public bool IsCDDrive() {
            try {
                return (drive.DriveType == DriveType.CDRom);
            } catch {
                return false;
            }
        }

        public abstract void Eject();

        public abstract void Retract();

        public abstract StatusCD Status { get; }

        public abstract long Serial { get; }
    }
}
