using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Runtime.InteropServices;

namespace IronAHK.Rusty.Windows
{
    /// <summary>
    /// Implementation for native Windows Drive Operations
    /// </summary>
    class Drive : Common.Drive
    {
        static readonly string IOPathPrefix = @"\\.\";

        public Drive(DriveInfo drv)
            : base(drv) { }

        public override void Eject() {
            IntPtr fileHandle = IntPtr.Zero;
            try {
                // Create an handle to the drive
                fileHandle = WindowsAPI.CreateFile(this.CreateDeviceIOPath,
                WindowsAPI.GENERICREAD, 0, IntPtr.Zero,
                WindowsAPI.OPENEXISTING, 0, IntPtr.Zero);

                if((int)fileHandle != WindowsAPI.INVALID_HANDLE) {
                    // Eject the disk
                    uint returnedBytes;
                    WindowsAPI.DeviceIoControl(fileHandle, WindowsAPI.IOCTL_STORAGE_EJECT_MEDIA,
                    IntPtr.Zero, 0,
                    IntPtr.Zero, 0,
                    out returnedBytes,
                    IntPtr.Zero);
                }
            } catch {
                throw new Exception(Marshal.GetLastWin32Error().ToString());
            } finally {
                // Close Drive Handle
                WindowsAPI.CloseHandle(fileHandle);
                fileHandle = IntPtr.Zero;
            }
        }

        public string CreateDeviceIOPath {
            get {
                return IOPathPrefix + drive.Name.Substring(0, 1) + ":";
            }
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
