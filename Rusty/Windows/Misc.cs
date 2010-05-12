using System;
using System.Runtime.InteropServices;
using System.Text;

namespace IronAHK.Rusty
{
    partial class Windows
    {
        #region DLL

        [DllImport("kernel32.dll")]
        public static extern IntPtr LoadLibrary(string lpFileName);

        [DllImport("kernel32.dll")]
        public static extern bool FreeLibrary(IntPtr hModule);

        #endregion

        #region Windows

        [DllImport("user32.dll")]
        public static extern bool FlashWindow(IntPtr hWnd, bool bInvert);

        #endregion

        #region Process

        [DllImport("kernel32.dll")]
        public static extern IntPtr GetProcAddress(IntPtr hModule, string procName);

        [DllImport("kernel32.dll")]
        public static extern IntPtr GetModuleHandle(string lpModuleName);

        #endregion

        #region INI

        [DllImport("kernel32.dll")]
        public static extern bool WritePrivateProfileString(string lpAppName, string lpKeyName, string lpString, string lpFileName);

        [DllImport("kernel32.dll")]
        public static extern uint GetPrivateProfileString(string lpAppName, string lpKeyName, string lpDefault, StringBuilder lpReturnedString, uint nSize, string lpFileName);

        #endregion

        #region Sound

        [DllImport("winmm.dll")]
        public static extern uint waveOutGetVolume(IntPtr hwo, out uint dwVolume);

        [DllImport("winmm.dll")]
        public static extern int waveOutSetVolume(IntPtr hwo, uint dwVolume);

        #endregion

        #region Hooks

        private delegate IntPtr LowLevelKeyboardProc(int nCode, IntPtr wParam, IntPtr lParam);

        [DllImport("user32.dll")]
        static extern IntPtr SetWindowsHookEx(int idHook, LowLevelKeyboardProc lpfn, IntPtr hMod, uint dwThreadId);

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        static extern bool UnhookWindowsHookEx(IntPtr hhk);

        [DllImport("user32.dll")]
        static extern IntPtr CallNextHookEx(IntPtr hhk, int nCode, IntPtr wParam, IntPtr lParam);

        #endregion
    }
}
