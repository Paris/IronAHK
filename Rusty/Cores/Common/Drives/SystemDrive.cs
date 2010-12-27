using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace IronAHK.Rusty.Cores.Common.Drives
{
    /// <summary>
    /// Status of the CD
    /// </summary>
    public enum StatusCD
    {
        NotReady,
        Open,
        Playing,
        Paused,
        Seeking,
        Stopped
    }

    /// <summary>
    /// Encapsulates raw Drive Methods over InterOP
    /// </summary>
    public abstract class SystemDrive
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

        /// <summary>
        /// Ejects the CD Drive
        /// </summary>
        public abstract void Eject();

        /// <summary>
        /// Retracts the CD Drive
        /// </summary>
        public abstract void Retract();

        public abstract StatusCD Status { get; }

        public abstract long Serial { get; }
    }
}
