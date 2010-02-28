using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows.Forms;

namespace IronAHK.Rusty
{
    partial class Windows
    {
        public class KeyboardHook : Core.KeyboardHook
        {
            // credit to http://blogs.msdn.com/toub/archive/2006/05/03/589423.aspx for saving me time

            const int WM_HOTKEY = 0x0312;
            const int WH_KEYBOARD_LL = 13;
            const int WM_KEYDOWN = 0x0100;
            const int WM_KEYUP = 0x0101;
            LowLevelKeyboardProc proc;
            IntPtr hookId = IntPtr.Zero;

            protected override void PassThrough(Keys keys)
            {
                // TODO: send back keys with SendInput
            }

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
                if (nCode >= 0 && wParam == (IntPtr)WM_KEYDOWN || wParam == (IntPtr)WM_KEYUP)
                {
                    int vkCode = Marshal.ReadInt32(lParam);
                    new Thread(new ThreadStart(delegate() { KeyReceived((Keys)vkCode, wParam == (IntPtr)WM_KEYDOWN); })).Start();
                    
                }
                return CallNextHookEx(hookId, nCode, wParam, lParam);
            }
        }
    }
}
