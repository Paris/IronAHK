using System;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Text;

namespace IronAHK.Rusty
{
    [CLSCompliant(false)]
    public partial class WindowsAPI
    {
        const string kernel32 = "kernel32.dll", shell32 = "shell32.dll", user32 = "user32.dll", version = "version.dll", winmm = "winmm.dll";

        #region Functions

        #region DLL

        [DllImport(kernel32)]
        public static extern IntPtr LoadLibrary(string lpFileName);

        [DllImport(kernel32)]
        public static extern bool FreeLibrary(IntPtr hModule);

        #endregion

        #region Windows

        [DllImport(user32)]
        public static extern bool FlashWindow(IntPtr hWnd, bool bInvert);

        [DllImport(user32)]
        public static extern bool InvalidateRect(IntPtr hWnd, IntPtr lpRect, bool bErase);

        [DllImport(user32)]
        public static extern int EnumWindows(EnumWindowsProc lpEnumFunc, int lParam);

        [DllImport(user32)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool EnumChildWindows(IntPtr hwndParent, EnumWindowsProc lpEnumFunc, int lParam);

        [DllImport(user32)]
        public static extern IntPtr SendMessage(IntPtr hWnd, uint Msg, IntPtr wParam, IntPtr lParam);

        [DllImport(user32)]
        static extern int SendMessage(IntPtr hwnd, uint msg, int wParam, StringBuilder sb);

        [DllImport(user32)]
        public static extern bool PostMessage(IntPtr hWnd, uint Msg, IntPtr wParam, IntPtr lParam);

        [DllImport(kernel32)]
        public static extern void OutputDebugString(string lpOutputString);

        [DllImport(user32)]
        public static extern int GetWindowText(IntPtr hWnd, StringBuilder lpString, int nMaxCount);

        [DllImport(user32)]
        public static extern int GetWindowTextLength(IntPtr hWnd);

        [DllImport(user32)]
        public static extern bool SetWindowText(IntPtr hWnd, string lpString);

        [DllImport(user32)]
        public static extern IntPtr SetFocus(IntPtr hWnd);

        [DllImport(user32)]
        public static extern IntPtr GetFocus();

        [DllImport(user32)]
        public static extern int GetClassName(IntPtr hWnd, StringBuilder lpClassName, int nMaxCount);

        [DllImport(user32)]
        public static extern IntPtr GetActiveWindow(IntPtr hWnd);

        [DllImport(user32)]
        public static extern IntPtr GetActiveWindow();

        [DllImport(user32)]
        public static extern IntPtr FindWindow(string className, string windowName);

        [DllImport(user32)]
        public static extern IntPtr FindWindowEx(IntPtr parentHandle, IntPtr childAfter, string className, string windowTitle);

        [DllImport(user32)]
        public static extern IntPtr GetWindow(IntPtr hWnd, uint uCmd);

        [DllImport(user32, CharSet = CharSet.Auto, ExactSpelling = true)]
        public static extern IntPtr GetAncestor(IntPtr hWnd, gaFlags gaFlags);

        [DllImport(user32)]
        public static extern int CloseWindow(IntPtr hWnd);

        [DllImport(user32)]
        public static extern bool DestroyWindow(IntPtr hwnd);

        [DllImport(user32)]
        public static extern bool MoveWindow(IntPtr hWnd, int X, int Y, int nWidth, int nHeight, bool bRepaint);

        [DllImport(user32)]
        public static extern bool IsWindowVisible(IntPtr hWnd);

        [DllImport(user32)]
        public static extern bool GetGUIThreadInfo(uint idThread, out GUITHREADINFO lpgui);

        [DllImport(user32)]
        public static extern IntPtr WindowFromPoint(Point Point);

        [DllImport(user32)]
        public static extern IntPtr RealChildWindowFromPoint(IntPtr hwndParent, Point ptParentClientCoords);

        [DllImport(user32)]
        public static extern IntPtr GetDesktopWindow();

        [DllImport(user32)]
        public static extern IntPtr GetForegroundWindow();

        [DllImport(user32)]
        public static extern bool SetForegroundWindow(IntPtr hWnd);

        [DllImport(user32)]
        public static extern int SetActiveWindow(IntPtr handle);

        [DllImport(user32)]
        public static extern bool IsWindow(IntPtr hWnd);

        [DllImport(user32)]
        public static extern bool GetWindowRect(IntPtr hWnd, out RECT lpRect);

        [DllImport(user32)]
        public static extern int GetSystemMetrics(int sysMetric);

        [DllImport(user32)]
        public static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);

        [DllImport(version)]
        public static extern bool GetFileVersionInfo(string sFileName, int handle, int size, byte[] infoBuffer);

        [DllImport(version)]
        public static extern int GetFileVersionInfoSize(string sFileName, out int handle);

        [DllImport(version)]
        public static extern bool VerQueryValue(byte[] pBlock, string pSubBlock, out string pValue, out uint len);

        [DllImport(shell32)]
        public static extern int SHEmptyRecycleBin(IntPtr hWnd, string pszRootPath, uint dwFlags);

        [DllImport(user32)]
        public static extern bool ExitWindowsEx(uint uFlags, uint dwReason);

        [DllImport(user32)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool IsWindowEnabled(IntPtr hWnd);

        [DllImport(user32)]
        public static extern bool EnableWindow(IntPtr hWnd, bool bEnable);

        [DllImport(user32)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool GetWindowPlacement(IntPtr hWnd, out WINDOWPLACEMENT lpwndpl);

        [DllImport(user32)]
        public static extern int SetWindowLong(IntPtr hWnd, int nIndex, int dwNewLong);

        [DllImport(user32)]
        public static extern int GetWindowLong(IntPtr hWnd, int nIndex);

        [DllImport(user32)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool SetWindowPos(IntPtr hWnd, IntPtr hWndInsertAfter, int X, int Y, int cx, int cy, uint uFlags);

        #region Menu

        [DllImport(user32)]
        public static extern IntPtr GetMenu(IntPtr hWnd);

        [DllImport(user32)]
        public static extern int GetMenuItemCount(IntPtr hMenu);

        [DllImport(user32)]
        public static extern IntPtr GetSubMenu(IntPtr hMenu, int nPos);

        [DllImport(user32)]
        public static extern int GetMenuString(IntPtr hMenu, uint uIDItem, [Out, MarshalAs(UnmanagedType.LPStr)] StringBuilder lpString, int nMaxCount, uint uFlag);

        [DllImport(user32)]
        public static extern bool SetLayeredWindowAttributes(IntPtr hwnd, uint crKey, byte bAlpha, uint dwFlags);

        #endregion

        #endregion

        #region Process

        [DllImport(kernel32)]
        public static extern IntPtr GetProcAddress(IntPtr hModule, string procName);

        [DllImport(kernel32)]
        public static extern IntPtr GetModuleHandle(string lpModuleName);

        [DllImport(kernel32)]
        public static extern bool TerminateProcess(IntPtr hProcess, uint uExitCode);

        [DllImport(kernel32)]
        public static extern bool CloseHandle(IntPtr hObject);

        [DllImport(kernel32)]
        public static extern IntPtr OpenProcess(uint dwDesiredAccess, bool bInheritHandle, uint dwProcessId);

        [DllImport(user32)]
        public static extern uint GetWindowThreadProcessId(IntPtr hWnd, out uint lpdwProcessId);

        #endregion

        #region INI

        [DllImport(kernel32)]
        public static extern bool WritePrivateProfileString(string lpAppName, string lpKeyName, string lpString, string lpFileName);

        [DllImport(kernel32)]
        public static extern uint GetPrivateProfileString(string lpAppName, string lpKeyName, string lpDefault, StringBuilder lpReturnedString, uint nSize, string lpFileName);

        #endregion

        #region Sound

        [DllImport(winmm)]
        public static extern uint waveOutGetVolume(IntPtr hwo, out uint dwVolume);

        [DllImport(winmm)]
        public static extern int waveOutSetVolume(IntPtr hwo, uint dwVolume);

        #endregion

        #region Hooks

        [DllImport(user32)]
        public static extern IntPtr SetWindowsHookEx(int idHook, LowLevelKeyboardProc lpfn, IntPtr hMod, uint dwThreadId);

        [DllImport(user32)]
        public static extern bool UnhookWindowsHookEx(IntPtr hhk);

        [DllImport(user32)]
        public static extern IntPtr CallNextHookEx(IntPtr hhk, int nCode, IntPtr wParam, IntPtr lParam);

        #endregion

        #region Icons

        [DllImport(shell32)]
        public static extern IntPtr ExtractIcon(IntPtr hInst, string lpszExeFileName, int nIconIndex);

        #endregion

        #region File / Device

        [DllImport(kernel32, SetLastError = true)]
        public static extern IntPtr CreateFile(string fileName,
        uint desiredAccess,
        uint shareMode,
        IntPtr attributes,
        uint creationDisposition,
        uint flagsAndAttributes,
        IntPtr templateFile);

        [DllImport(kernel32, SetLastError = true)]
        public static extern bool DeviceIoControl(IntPtr driveHandle,
        uint IoControlCode,
        IntPtr lpInBuffer,
        uint inBufferSize,
        IntPtr lpOutBuffer,
        uint outBufferSize,
        out uint lpBytesReturned,
        IntPtr lpOverlapped);

        #endregion

        #endregion

        #region Constants

        public enum gaFlags : uint
        {
            GA_PARENT = 1,
            GA_ROOT = 2,
            GA_ROOTOWNER = 3
        }

        // File / Device IO
        public const uint GENERICREAD = 0x80000000;
        public const uint OPENEXISTING = 3;
        public const uint IOCTL_STORAGE_EJECT_MEDIA = 2967560;
        public const int INVALID_HANDLE = -1;

        public const uint GW_CHILD = 5;
        public const uint GW_HWNDPREV = 3;
        public const int GWL_STYLE = -16;
        public const int GWL_EXSTYLE = -20;

        public const int WS_EX_LAYERED = 0x80000;
        public const int LWA_ALPHA = 0x2;
        public const int LWA_COLORKEY = 0x1;

        public const int SW_HIDE = 0;
        public const int SW_SHOWNORMAL = 1;
        public const int SW_NORMAL = 1;
        public const int SW_SHOWMINIMIZED = 2;
        public const int SW_SHOWMAXIMIZED = 3;
        public const int SW_MAXIMIZE = 3;
        public const int SW_SHOWNOACTIVATE = 4;
        public const int SW_SHOW = 5;
        public const int SW_MINIMIZE = 6;
        public const int SW_SHOWMINNOACTIVE = 7;
        public const int SW_SHOWNA = 8;
        public const int SW_RESTORE = 9;
        public const int SW_SHOWDEFAULT = 10;
        public const int SW_FORCEMINIMIZE = 11;
        public const int SW_MAX = 11;

        public const int SWP_NOSIZE = 1;
        public const int SWP_NOMOVE = 2;
        public const int SWP_NOACTIVATE = 0x10;

        public const int SHERB_NOCONFIRMATION = 0x1;
        public const int SHERB_NOPROGRESSUI = 0x2;
        public const int SHERB_NOSOUND = 0x4;

        public const int HWND_BROADCAST = 0xffff;
        public const uint WM_SETTINGCHANGE = 0x001A;

        public const int HWND_TOP = 0;
        public const int HWND_BOTTOM = 1;
        public const int HWND_TOPMOST = -1;
        public const int HWND_NOTOPMOST = -2;

        public const int WM_CLOSE = 0x0010;
        public const int WM_GETTEXT = 0x000D;
        public const int WM_GETTEXTLENGTH = 0x000E;
        public const int WM_COMMAND = 0x0111;

        public const int WS_EX_TOPMOST = 8;

        public const uint WM_SYSCOMMAND = 0x0112;
        public const int SC_CLOSE = 0xF060;

        public const int STANDARD_RIGHTS_REQUIRED = 0x000F0000;
        public const int SYNCHRONIZE = 0x00100000;
        public const int PROCESS_ALL_ACCESS = STANDARD_RIGHTS_REQUIRED | SYNCHRONIZE | 0xFFF;

        public const int MF_BYCOMMAND = 0;
        public const int MF_BYPOSITION = 0x400;

        #endregion

        #region Structures

        [Serializable, StructLayout(LayoutKind.Sequential)]
        public struct GUITHREADINFO
        {
            public uint cbSize;
            public uint flags;
            public IntPtr hwndActive;
            public IntPtr hwndFocus;
            public IntPtr hwndCapture;
            public IntPtr hwndMenuOwner;
            public IntPtr hwndMoveSize;
            public IntPtr hwndCaret;
            public RECT rcCaret;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct RECT
        {
            public int Left;
            public int Top;
            public int Right;
            public int Bottom;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct POINT
        {
            public int x;
            public int y;
        }

        [Serializable]
        [StructLayout(LayoutKind.Sequential)]
        public struct WINDOWPLACEMENT
        {
            public uint length;
            public uint flags;
            public uint showCmd;
            public POINT ptMinPosition;
            public POINT ptMaxPosition;
            public RECT rcNormalPosition;

            public static WINDOWPLACEMENT Default
            {
                get
                {
                    var result = new WINDOWPLACEMENT();
                    result.length = (uint)Marshal.SizeOf(result);
                    return result;
                }
            }
        }

        #endregion

        #region Delegates

        public delegate IntPtr LowLevelKeyboardProc(int nCode, IntPtr wParam, IntPtr lParam);

        public delegate bool EnumWindowsProc(IntPtr hwnd, int lParam);

        #endregion

        #region Helpers

        public static int MakeInt(short lowPart, short highPart) {
            return (int)(((ushort)lowPart) | (uint)(highPart << 16));
        }

        public static string GetWindowText(IntPtr hwnd)
        {
            var length = SendMessage(hwnd, WM_GETTEXTLENGTH, IntPtr.Zero, IntPtr.Zero).ToInt32();
            var buf = new StringBuilder(length + 1);
            SendMessage(hwnd, WM_GETTEXT, buf.Capacity, buf);
            return buf.ToString();
        }

        public static string GetClassName(IntPtr hwnd)
        {
            var buf = new StringBuilder(64);
            GetClassName(hwnd, buf, buf.Capacity);
            return buf.ToString();
        }

        #endregion
    }
}
