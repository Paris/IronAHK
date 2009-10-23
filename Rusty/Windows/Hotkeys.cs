using System;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace IronAHK.Rusty.Windows
{
    class Hotkeys : IMessageFilter
    {
        public enum KeyModifiers { None = 0, Alt = 1, Control = 2, Shift = 4, Windows = 8 }
        const int WM_HOTKEY = 0x0312;
        IntPtr handle;
        int total = 0;

        public delegate void HotkeyEvent(object sender, Message e);
        HotkeyEvent[] HotkeyPressedEvents;

        public IntPtr Handle
        {
            get { return handle; }
            set { handle = value; }
        }

        public int Total { get { return total; } }

        public Hotkeys(int Max)
        {
            HotkeyPressedEvents = new HotkeyEvent[Max];
        }

        ~Hotkeys()
        {
            Application.RemoveMessageFilter(this);
            for (int i = 0; i < total; i++)
                UnregisterHotKey(handle, i);
        }

        public bool Add(Keys key, KeyModifiers modifier, HotkeyEvent KeyPressed)
        {
            HotkeyPressedEvents[total] = KeyPressed;
            bool created = RegisterHotKey(handle, total, modifier, key);
            if (total == 0)
                Application.AddMessageFilter(this);
            total++;
            return created;
        }

        public bool PreFilterMessage(ref Message m)
        {
            if (m.Msg == WM_HOTKEY)
            {
                HotkeyPressedEvents[m.WParam.ToInt32()](this, m);
                return true;
            }
            return false;
        }

        [DllImport("user32.dll")]
        private static extern bool RegisterHotKey(IntPtr hWnd, int id, KeyModifiers fsModifiers, Keys vk);

        [DllImport("user32.dll")]
        private static extern bool UnregisterHotKey(IntPtr hWnd, int id);
    }
}
