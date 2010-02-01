using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace IronAHK.Rusty
{
    partial class Windows
    {
        public class KeyboardHook : Core.KeyboardHook
        {
            // credit to http://blogs.msdn.com/toub/archive/2006/05/03/589423.aspx for saving me time

            const int WH_KEYBOARD_LL = 13;
            const int WM_KEYDOWN = 0x0100;
            LowLevelKeyboardProc proc;
            IntPtr hookId = IntPtr.Zero;

            protected override void RegisterHook()
            {
                proc = HookCallback;
                hookId = SetWindowsHookEx(WH_KEYBOARD_LL, proc, GetModuleHandle(Process.GetCurrentProcess().MainModule.ModuleName), 0);
            }

            protected override void DeregisterHook()
            {
                UnhookWindowsHookEx(hookId);
            }

            IntPtr HookCallback(int nCode, IntPtr wParam, IntPtr lParam)
            {
                if (nCode >= 0 && wParam == (IntPtr)WM_KEYDOWN)
                {
                    int vkCode = Marshal.ReadInt32(lParam);
                    KeyReceived((Keys)vkCode);
                }
                return CallNextHookEx(hookId, nCode, wParam, lParam);
            }
        }
    }
}
