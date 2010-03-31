using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using System.Text;

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
            const int WM_SYSKEYDOWN = 0x0104;
            LowLevelKeyboardProc proc;
            IntPtr hookId = IntPtr.Zero;

            const string backspace = "{BS}";

            protected override void RegisterHook()
            {
                proc = HookCallback;
                hookId = SetWindowsHookEx(WH_KEYBOARD_LL, proc, GetModuleHandle(Process.GetCurrentProcess().MainModule.ModuleName), 0);
            }

            protected override void DeregisterHook()
            {
                UnhookWindowsHookEx(hookId);
            }
            
            protected override void SendBackspace (int length)
            {
                StringBuilder Buf = new StringBuilder(backspace.Length * length);
                
                for(int i = 0; i < length; i++)
                    Buf.Append("{BS}");
                
                SendHotstring(Buf.ToString());
            }
            
            protected internal override void SendHotstring (string keys)
            {
                SendKeys.SendWait(keys);
            }

            IntPtr HookCallback(int nCode, IntPtr wParam, IntPtr lParam)
            {
                bool block = false;
                bool pressed = wParam == (IntPtr)WM_KEYDOWN || wParam == (IntPtr)WM_SYSKEYDOWN;

                if (nCode >= 0 && pressed || wParam == (IntPtr)WM_KEYUP)
                {
                    int vkCode = Marshal.ReadInt32(lParam);
                    block = KeyReceived((Keys)vkCode, pressed);
                }

                var next = CallNextHookEx(hookId, nCode, wParam, lParam);
                return block ? new IntPtr(1) : next;
            }
        }
    }
}
