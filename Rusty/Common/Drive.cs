using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace IronAHK.Rusty.Common
{
    abstract class Drive
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

        #region Properties

        protected DriveInfo drive;

        public Drive(DriveInfo udrive)
        {
            drive = udrive;
        }

        public bool IsCDDrive()
        {
            try
            {
                return (drive.DriveType == DriveType.CDRom);
            }
            catch
            {
                return false;
            }
        }

        #endregion

        #region Abstract methods

        /// <summary>
        /// Ejects the CD Drive
        /// </summary>
        public abstract void Eject();

        /// <summary>
        /// Retracts the CD Drive
        /// </summary>
        public abstract void Retract();

        /// <summary>
        /// Locks the drives Eject ability
        /// </summary>
        public abstract void Lock();

        /// <summary>
        /// Unlocks the drives Eject ability
        /// </summary>
        public abstract void UnLock();

        public abstract StatusCD Status { get; }

        public abstract long Serial { get; }

        #endregion

        #region Provider

        internal static class DriveProvider
        {
            // UNDONE: organise DriveProvider

            /// <summary>
            /// Creates platform specific SystemDrive Instance
            /// </summary>
            /// <param name="drive"></param>
            /// <returns></returns>
            public static Drive CreateDrive(DriveInfo drive)
            {
                if (Environment.OSVersion.Platform == PlatformID.Win32NT)
                    return new Windows.Drive(drive);
                else
                    return new Linux.Drive(drive);
            }
        }

        #endregion
    }
}
