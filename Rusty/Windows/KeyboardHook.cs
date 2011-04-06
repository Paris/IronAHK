using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;

namespace IronAHK.Rusty.Windows
{
    internal class KeyboardHook : Common.Keyboard.KeyboardHook
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
        WindowsAPI.LowLevelKeyboardProc proc;
        IntPtr hookId = IntPtr.Zero;
        bool ignore;
        bool dead;
        List<int> deadkeys;

        protected override void RegisterHook() {
            ScanDeadKeys();
            proc = HookCallback;
            hookId = WindowsAPI.SetWindowsHookEx(WH_KEYBOARD_LL, proc, WindowsAPI.GetModuleHandle(Process.GetCurrentProcess().MainModule.ModuleName), 0);
        }

        protected override void DeregisterHook() {
            WindowsAPI.UnhookWindowsHookEx(hookId);
        }

        protected internal override void Send(string keys) {
            if(keys.Length == 0)
                return;

            var len = keys.Length * 2;
            var inputs = new WindowsAPI.INPUT[len];

            for(int i = 0; i < keys.Length; i++) {
                uint flag = WindowsAPI.KEYEVENTF_UNICODE;

                if((keys[i] & 0xff00) == 0xe000)
                    flag |= WindowsAPI.KEYEVENTF_EXTENDEDKEY;

                var down = new WindowsAPI.INPUT { type = WindowsAPI.INPUT_KEYBOARD };
                down.i.k = new WindowsAPI.KEYBDINPUT { wScan = keys[i], dwFlags = flag };

                var up = new WindowsAPI.INPUT { type = WindowsAPI.INPUT_KEYBOARD };
                up.i.k = new WindowsAPI.KEYBDINPUT { wScan = keys[i], dwFlags = flag | WindowsAPI.KEYEVENTF_KEYUP };

                int x = i * 2;
                inputs[x] = down;
                inputs[x + 1] = up;
            }

            ignore = true;
            WindowsAPI.SendInput((uint)len, inputs, Marshal.SizeOf(typeof(WindowsAPI.INPUT)));
            ignore = false;
        }

        protected internal override void Send(Keys key) {
            key &= ~Keys.Modifiers;

            if(key == Keys.None)
                return;

            uint flag = WindowsAPI.KEYEVENTF_UNICODE;
            var vk = (ushort)key;

            if((vk & 0xff00) == 0xe000)
                flag |= WindowsAPI.KEYEVENTF_EXTENDEDKEY;

            var down = new WindowsAPI.INPUT { type = WindowsAPI.INPUT_KEYBOARD };
            down.i.k = new WindowsAPI.KEYBDINPUT { wVk = vk, dwFlags = flag };

            var up = new WindowsAPI.INPUT { type = WindowsAPI.INPUT_KEYBOARD };
            up.i.k = new WindowsAPI.KEYBDINPUT { wVk = vk, dwFlags = flag | WindowsAPI.KEYEVENTF_KEYUP };

            var inputs = new[] { down, up };

            ignore = true;
            WindowsAPI.SendInput((uint)inputs.Length, inputs, Marshal.SizeOf(typeof(WindowsAPI.INPUT)));
            ignore = false;
        }

        protected override void Backspace(int length) {
            length *= 2;
            var inputs = new WindowsAPI.INPUT[length];

            for(int i = 0; i < length; i += 2) {
                var down = new WindowsAPI.INPUT { type = WindowsAPI.INPUT_KEYBOARD };
                down.i.k = new WindowsAPI.KEYBDINPUT { wVk = VK_BACK };

                var up = new WindowsAPI.INPUT { type = WindowsAPI.INPUT_KEYBOARD };
                up.i.k = new WindowsAPI.KEYBDINPUT { wVk = VK_BACK, dwFlags = WindowsAPI.KEYEVENTF_KEYUP };

                inputs[i] = down;
                inputs[i + 1] = up;
            }

            ignore = true;
            WindowsAPI.SendInput((uint)length, inputs, Marshal.SizeOf(typeof(WindowsAPI.INPUT)));
            ignore = false;
        }

        void ScanDeadKeys() {
            const int vkmax = 256;
            var state = new byte[vkmax];
            var kbd = WindowsAPI.GetKeyboardLayout(0);
            deadkeys = new List<int>();

            for(int i = 0; i < vkmax; i++) {
                var buf = new StringBuilder(4);
                var result = WindowsAPI.ToUnicodeEx((uint)i, 0, state, buf, buf.Capacity, 0, kbd);

                if(result == -1)
                    deadkeys.Add(i);
            }
        }

        string MapKey(uint vk, uint sc) {
            var state = new byte[256];
            WindowsAPI.GetKeyboardState(state);

            foreach(var key in new[] { VK_SHIFT, VK_CONTROL, VK_MENU }) {
                const byte d = 0x80;
                const byte u = d - 1;

                bool s = WindowsAPI.GetKeyState(key) >> 8 != 0;
                state[key] &= s ? d : u;
            }

            var buf = new StringBuilder(4);
            WindowsAPI.ToUnicodeEx(vk, sc, state, buf, buf.Capacity, 0, WindowsAPI.GetKeyboardLayout(0));
            return buf.ToString();
        }

        IntPtr HookCallback(int nCode, IntPtr wParam, IntPtr lParam) {
            bool block = false;

            if(ignore)
                goto chain;

            bool pressed = wParam == (IntPtr)WM_KEYDOWN || wParam == (IntPtr)WM_SYSKEYDOWN;

            if(nCode >= 0 && pressed || wParam == (IntPtr)WM_KEYUP) {
                int vkCode = Marshal.ReadInt32(lParam);
                int sc = Marshal.ReadInt32(lParam, 8);

                if(dead || deadkeys.Contains(vkCode)) {
                    dead = !dead;
                    goto chain;
                }

                string typed = MapKey((uint)vkCode, (uint)sc);
                block = KeyReceived((Keys)vkCode, typed, pressed);
            }

        chain:
            var next = WindowsAPI.CallNextHookEx(hookId, nCode, wParam, lParam);
            return block ? new IntPtr(1) : next;
        }
    }
}
