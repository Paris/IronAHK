using System;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Text;

namespace IronAHK.Rusty.Windows
{
    class Windows
    {
        WindowType find, exclude;
        IntPtr[] matches = new IntPtr[31];
        int count = 0;

        public IntPtr[] Matches { get { return matches; } }

        public IntPtr FindWindow(string WinTitle, string WinText, string ExcludeTitle, string ExcludeText)
        {
            return FindAllWindows(WinText, WinText, ExcludeText, ExcludeText, string.Empty)[0];
        }

        public IntPtr FindWindow(string WinTitle, string WinText, string ExcludeTitle, string ExcludeText, string Control)
        {
            return FindAllWindows(WinTitle, WinText, ExcludeTitle, ExcludeText, Control)[0];
        }

        public IntPtr FindWindow()
        {
            return FindWindow(string.Empty, string.Empty, string.Empty, string.Empty, string.Empty);
        }

        public IntPtr[] FindAllWindows()
        {
            return FindAllWindows(string.Empty, string.Empty, string.Empty, string.Empty, string.Empty);
        }

        public IntPtr[] FindAllWindows(string WinTitle, string WinText, string ExcludeTitle, string ExcludeText, string Control)
        {
            if (WinTitle.Length == 0 && WinText.Length == 0 && ExcludeTitle.Length == 0 && ExcludeText.Length == 0)
                return matches;

            find = new WindowType(WinTitle);
            find.Text = WinText;
            find.Control = Control;
            exclude = new WindowType(ExcludeTitle);
            exclude.Text = ExcludeText;

            matches.Initialize();
            for (int i = 0; i < matches.Length; i++)
                matches[i] = IntPtr.Zero;
            count = 0;

            if (find.Title == "A")
                matches[0] = Windows.GetActiveWindow();
            else EnumWindows(new EnumFunc(FilterWindow), 0);

            if (find.Control.Length != 0)
            {
                for (int i = 0; i < matches.Length; i++)
                {
                    if (matches[i] != IntPtr.Zero)
                        matches[i] = Windows.FindWindowEx(matches[i],
                            Windows.GetWindow(matches[i], Windows.GW_CHILD),
                            find.Control, string.Empty);
                }
            }

            return matches;
        }

        bool FilterWindow(IntPtr hwnd, int lParam)
        {
            string title = Windows.GetWindowText(hwnd).ToLower();
            StringBuilder sb = new StringBuilder(Math.Max(find.Class.Length, exclude.Class.Length));
            string classname = Windows.GetClassName(hwnd, sb, sb.Capacity).ToString();

            int id = hwnd.ToInt32();
            uint pid;
            Windows.GetWindowThreadProcessId(hwnd, out pid);
            bool pass = false;

            if (find.Title.Length != 0)
                pass = find.Title == title;
            if (exclude.Title.Length != 0)
                pass = exclude.Title != title;
            if (find.ID != 0)
                pass = find.ID == id;
            if (exclude.ID != 0)
                pass = exclude.ID != id;
            if (find.PID != 0)
                pass = find.PID == pid;
            if (exclude.PID != 0)
                pass = exclude.PID != pid;
            if (find.Class.Length != 0)
                pass = find.Class == classname;
            if (exclude.Class.Length != 0)
                pass = exclude.Class != classname;

            IntPtr child = Windows.GetWindow(hwnd, Windows.GW_CHILD);

            if (find.Text.Length != 0)
                pass = Windows.FindWindowEx(hwnd, child, string.Empty, find.Text) != IntPtr.Zero;
            if (exclude.Text.Length != 0)
                pass = Windows.FindWindowEx(hwnd, child, string.Empty, exclude.Text) == IntPtr.Zero;

            if (pass)
            {
                matches[count++] = hwnd;
                if (count == matches.Length)
                    return false;
            }

            return true;
        }

        public delegate bool EnumFunc(IntPtr hwnd, int lParam);

        [DllImport("user32.dll")]
        public static extern int EnumWindows(EnumFunc lpEnumFunc, int lParam);

        public static string GetWindowText(IntPtr hwnd)
        {
            StringBuilder sb = new StringBuilder(GetWindowTextLength(hwnd) + 1);
            Windows.GetWindowText(hwnd, sb, sb.Capacity);
            return sb.ToString();
        }

        public static string GetClassName(IntPtr hwnd)
        {
            StringBuilder sb = new StringBuilder(1024);
            return GetClassName(hwnd, sb, sb.Capacity).ToString();
        }

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = false)]
        public static extern IntPtr SendMessage(IntPtr hWnd, uint Msg, IntPtr wParam, IntPtr lParam);

        [return: MarshalAs(UnmanagedType.Bool)]
        [DllImport("user32.dll", SetLastError = true)]
        public static extern bool PostMessage(IntPtr hWnd, uint Msg, IntPtr wParam, IntPtr lParam);

        [DllImport("kernel32.dll")]
        public static extern void OutputDebugString(string lpOutputString);

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        public static extern int GetWindowText(IntPtr hWnd, StringBuilder lpString, int nMaxCount);

        [DllImport("user32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        public static extern int GetWindowTextLength(IntPtr hWnd);

        [DllImport("user32.dll")]
        public static extern bool SetWindowText(IntPtr hWnd, string lpString);

        [DllImport("user32.dll")]
        public static extern IntPtr SetFocus(IntPtr hWnd);

        [DllImport("user32.dll")]
        public static extern IntPtr GetFocus();

        [DllImport("user32.dll")]
        public static extern uint GetWindowThreadProcessId(IntPtr hWnd, out uint lpdwProcessId);

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        public static extern int GetClassName(IntPtr hWnd, StringBuilder lpClassName, int nMaxCount);

        [DllImport("user32.dll", CharSet = CharSet.Auto, ExactSpelling = true)]
        public static extern IntPtr GetActiveWindow(IntPtr hWnd);

        [DllImport("user32.dll", CharSet = CharSet.Auto, ExactSpelling = true)]
        public static extern IntPtr GetActiveWindow();

        /*
         * find the handle for a client
         * classname - should be IntPtr.Zero
         * windowName - name of the window as found in title bar
         */
        [DllImport("user32.dll", EntryPoint = "FindWindow")]
        public static extern IntPtr FindWindow(IntPtr className, String windowName);

        [DllImport("user32.dll")]
        public static extern IntPtr FindWindowEx(IntPtr parentHandle, IntPtr childAfter, string className, string windowTitle);

        [DllImport("user32.dll")]
        public static extern IntPtr GetWindow(IntPtr hWnd, uint uCmd);

        [DllImport("user32.dll")]
        public static extern int CloseWindow(IntPtr hWnd);

        public static void DestroyWindow(IntPtr hWnd)
        {
            SendMessage(hWnd, 0x0002, IntPtr.Zero, IntPtr.Zero); //WM_DESTROY message
        }

        [DllImport("user32.dll")]
        public static extern bool MoveWindow(IntPtr hWnd, int X, int Y, int nWidth, int nHeight, bool bRepaint);

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool IsWindowVisible(IntPtr hWnd);

        [DllImport("user32.dll")]
        public static extern bool GetGUIThreadInfo(uint idThread, out GUITHREADINFO lpgui);

        [DllImport("user32.dll")]
        public static extern IntPtr WindowFromPoint(Point Point);

        [DllImport("user32.dll")]
        public static extern IntPtr RealChildWindowFromPoint(IntPtr hwndParent, Point ptParentClientCoords);

        [DllImport("user32.dll", EntryPoint = "GetDesktopWindow")]
        public static extern IntPtr GetDesktopWindow();

        [DllImport("user32.dll", EntryPoint = "GetForegroundWindow")]
        public static extern IntPtr GetForegroundWindow();

        [DllImport("user32.dll", EntryPoint = "SetForegroundWindow")]
        public static extern Int32 SetForegroundWindow(IntPtr handle);

        [DllImport("user32.dll", EntryPoint = "SetActiveWindow")]
        public static extern int SetActiveWindow(IntPtr handle);

        [DllImport("user32.dll", EntryPoint = "IsWindow")]
        public static extern bool IsWindow(IntPtr hWnd);

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


        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool GetWindowRect(IntPtr hWnd, out RECT lpRect);

        [DllImport("user32.dll", EntryPoint = "GetSystemMetrics")]
        public static extern int GetSystemMetrics(int sysMetric);

        public static void GetVirtualDesktopRect(out RECT aRect)
        {
            aRect.Right = GetSystemMetrics(78); //SM_CXVIRTUALSCREEN
            if (aRect.Right != 0) // A non-zero value indicates the OS supports multiple monitors or at least SM_CXVIRTUALSCREEN.
            {
                aRect.Left = GetSystemMetrics(76);  // SM_XVIRTUALSCREEN; Might be negative or greater than zero.
                aRect.Right += aRect.Left;
                aRect.Top = GetSystemMetrics(77);   //SM_YVIRTUALSCREEN; Might be negative or greater than zero.
                aRect.Bottom = aRect.Top + GetSystemMetrics(79); //SM_CYVIRTUALSCREEN
            }
            else // Win95/NT do not support SM_CXVIRTUALSCREEN and such, so zero was returned.
                GetWindowRect(GetDesktopWindow(), out aRect);
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct RECT
        {
            public int Left;
            public int Top;
            public int Right;
            public int Bottom;
        }

        [DllImport("user32.dll", SetLastError = false)]
        public static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);

        public const uint GW_CHILD = 5;
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

        [DllImport("version.dll")]
        public static extern bool GetFileVersionInfo(string sFileName, int handle, int size, byte[] infoBuffer);
        [DllImport("version.dll")]
        public static extern int GetFileVersionInfoSize(string sFileName, out int handle);

        // The third parameter - "out string pValue" - is automatically marshaled from ANSI to Unicode:
        [DllImport("version.dll")]
        public static extern bool VerQueryValue(byte[] pBlock, string pSubBlock, out string pValue, out uint len);

        [DllImport("shell32.dll")]
        public static extern int SHEmptyRecycleBin(IntPtr hWnd, string pszRootPath, uint dwFlags);

        public const int SHERB_NOCONFIRMATION = 0x1;
        public const int SHERB_NOPROGRESSUI = 0x2;
        public const int SHERB_NOSOUND = 0x4;

        public const int HWND_BROADCAST = 0xffff;
        public const uint WM_SETTINGCHANGE = 0x001A;

        public const uint WM_SYSCOMMAND = 0x0112;
        public const int SC_CLOSE = 0xF060;

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool ExitWindowsEx(uint uFlags, uint dwReason);
    }

    class WindowType
    {
        string title = string.Empty, classname = string.Empty, group = string.Empty, text = string.Empty, control = string.Empty;
        int id = 0;
        uint pid = 0;
        public string[] Bounds = new string[] { "ahk" };

        enum Mode { Title, Class, ID, PID, Group };

        public WindowType(string Criteria)
        {
            foreach (string criterion in Criteria.Trim().Split(Bounds, StringSplitOptions.RemoveEmptyEntries))
            {
                if (criterion.Substring(0, 1) == "_")
                {
                    int i = 0;
                    char[] letters = criterion.ToCharArray();
                    while (!char.IsWhiteSpace(letters[++i])) ;
                    string phrase = criterion.Substring(i).Trim();
                    switch (criterion.Substring(1, i - 1).ToLower())
                    {
                        case "class": classname = phrase.ToLower(); break;
                        case "id": id = int.Parse(phrase); break;
                        case "pid": pid = uint.Parse(phrase); break;
                        case "group": group = phrase.ToLower(); break;
                    }
                }
                else title = criterion.ToLower();
            }
        }

        public string Title { get { return title; } }
        public string Class { get { return classname; } }
        public int ID { get { return id; } }
        public uint PID { get { return pid; } }
        public string Group { get { return group; } }

        public string Text
        {
            get { return text; }
            set { text = value; }
        }

        public string Control
        {
            get { return control; }
            set { control = value; }
        }
    }
}
