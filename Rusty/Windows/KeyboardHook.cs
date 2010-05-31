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
            const int VK_BACK = 0x08;
            const int VK_SHIFT = 0x10;
            const int VK_CONTROL = 0x11;
            const int VK_MENU = 0x12;
            LowLevelKeyboardProc proc;
            IntPtr hookId = IntPtr.Zero;
            bool ignore;

            protected override void RegisterHook()
            {
                proc = HookCallback;
                hookId = SetWindowsHookEx(WH_KEYBOARD_LL, proc, GetModuleHandle(Process.GetCurrentProcess().MainModule.ModuleName), 0);
            }

            protected override void DeregisterHook()
            {
                UnhookWindowsHookEx(hookId);
            }

            protected internal override void Send(string keys)
            {
                if (keys.Length == 0)
                    return;

                var len = keys.Length * 2;
                var inputs = new INPUT[len];

                for (int i = 0; i < keys.Length; i++)
                {
                    uint flag = KEYEVENTF_UNICODE;

                    if ((keys[i] & 0xff00) == 0xe000)
                        flag |= KEYEVENTF_EXTENDEDKEY;

                    var down = new INPUT { type = INPUT_KEYBOARD };
                    down.i.k = new KEYBDINPUT { wScan = keys[i], dwFlags = flag };

                    var up = new INPUT { type = INPUT_KEYBOARD };
                    up.i.k = new KEYBDINPUT { wScan = keys[i], dwFlags = flag | KEYEVENTF_KEYUP };

                    int x = i * 2;
                    inputs[x] = down;
                    inputs[x + 1] = up;
                }

                ignore = true;
                SendInput((uint)len, inputs, Marshal.SizeOf(typeof(INPUT)));
                ignore = false;
            }

            protected override void Backspace(int length)
            {
                length *= 2;
                var inputs = new INPUT[length];

                for (int i = 0; i < length; i += 2)
                {
                    var down = new INPUT { type = INPUT_KEYBOARD };
                    down.i.k = new KEYBDINPUT { wVk = VK_BACK };

                    var up = new INPUT { type = INPUT_KEYBOARD };
                    up.i.k = new KEYBDINPUT { wVk = VK_BACK, dwFlags = KEYEVENTF_KEYUP };

                    inputs[i] = down;
                    inputs[i + 1] = up;
                }

                ignore = true;
                SendInput((uint)length, inputs, Marshal.SizeOf(typeof(INPUT)));
                ignore = false;
            }

            string MapKey(uint vk, uint sc)
            {
                var state = new byte[256];
                GetKeyboardState(state);

                foreach (var key in new[] { VK_SHIFT, VK_CONTROL, VK_MENU })
                {
                    const byte d = 0x80;
                    const byte u = d - 1;

                    bool s = GetKeyState(key) >> 8 != 0;
                    state[key] &= s ? d : u;
                }

                var buf = new StringBuilder(4);
                ToUnicodeEx(vk, sc, state, buf, buf.Capacity, 0, GetKeyboardLayout(0));
                return buf.ToString();
            }

            IntPtr HookCallback(int nCode, IntPtr wParam, IntPtr lParam)
            {
                bool block = false;

                if (ignore)
                    goto chain;

                bool pressed = wParam == (IntPtr)WM_KEYDOWN || wParam == (IntPtr)WM_SYSKEYDOWN;

                if (nCode >= 0 && pressed || wParam == (IntPtr)WM_KEYUP)
                {
                    int vkCode = Marshal.ReadInt32(lParam);
                    string typed = MapKey((uint)vkCode, (uint)Marshal.ReadInt32(lParam, 8));
                    block = KeyReceived((Keys)vkCode, typed, pressed);
                }

            chain:
                var next = CallNextHookEx(hookId, nCode, wParam, lParam);
                return block ? new IntPtr(1) : next;
            }
        }
    }
}
