using System;
using System.Drawing;
using System.Runtime.InteropServices;

namespace IronAHK.Rusty
{
    partial class Windows
    {
        [DllImport("user32.dll")]
        public static extern uint SendInput(uint nInputs, INPUT[] pInputs, int cbSize);

        [DllImport("user32.dll")]
        public static extern IntPtr GetMessageExtraInfo();

        [DllImport("user32.dll")]
        public static extern bool GetCursorPos(out Point lpPoint);

        [DllImport("user32.dll")]
        public static extern short GetKeyState(int nVirtKey);

        [DllImport("user32.dll")]
        public static extern bool BlockInput(bool fBlockIt);

        [StructLayout(LayoutKind.Sequential)]
        public struct MOUSEINPUT
        {
            public int dx;
            public int dy;
            public int mouseData;
            public uint dwFlags;
            public uint time;
            public IntPtr dwExtraInfo;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct KEYBDINPUT
        {
            public ushort wVk;
            public ushort wScan;
            public uint dwFlags;
            public uint time;
            public IntPtr dwExtraInfo;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct HARDWAREINPUT
        {
            public uint uMsg;
            public ushort wParamL;
            public ushort wParamH;
        }

        public struct INPUT
        {
            public uint type;
            public INPUTDATA i;
        }

        [StructLayout(LayoutKind.Explicit)]
        public struct INPUTDATA
        {
            [FieldOffset(0)]
            public MOUSEINPUT m;

            [FieldOffset(0)]
            public KEYBDINPUT k;

            [FieldOffset(0)]
            public HARDWAREINPUT h;
        }

        public const int INPUT_MOUSE = 0;
        public const int INPUT_KEYBOARD = 1;
        public const int INPUT_HARDWARE = 2;

        public const int KEYEVENTF_EXTENDEDKEY = 1;
        public const int KEYEVENTF_KEYUP = 2;
        public const int KEYEVENTF_UNICODE = 4;
    }
}
