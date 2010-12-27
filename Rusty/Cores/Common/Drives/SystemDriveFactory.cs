using System;
using System.IO;
using IronAHK.Rusty.Linux.Drives;
using IronAHK.Rusty.Windows.Drives;

namespace IronAHK.Rusty.Cores.Common.Drives
{
    internal static class SystemDriveFactory
    {
        /// <summary>
        /// Creates platform specific SystemDrive Instance
        /// </summary>
        /// <param name="drive"></param>
        /// <returns></returns>
        public static SystemDrive CreateSystemDrive(DriveInfo drive) {
            if(Environment.OSVersion.Platform == PlatformID.Win32NT)
                return new SystemDriveWindows(drive);
            else
                return new SystemDriveLinux(drive);
        }
    }
}
