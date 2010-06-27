using System;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace IronAHK.Rusty
{
    partial class Core
    {
        /// <summary>
        /// Clicks a mouse button at the specified coordinates. It can also hold down a mouse button, turn the mouse wheel, or move the mouse.
        /// </summary>
        /// <param name="options">Can be one or more of the following options:
        /// <list type="bullet">
        /// <item><term><c>X</c> or <c>Y</c></term>: <description>the coordinates;</description></item>
        /// <item><term>Button</term>: <description><c>Left</c> (default), <c>Middle</c>, <c>Right</c>, <c>X1</c> or <c>X2</c>;</description></item>
        /// <item><term>Wheel</term>: <description><c>WheelUp</c>, <c>WheelDown</c>, <c>WheelLeft</c> or <c>WheelRight</c>;</description></item>
        /// <item><term>Count</term>: <description>number of times to send the click;</description></item>
        /// <item><term><c>Down</c> or <c>Up</c></term>: <description>if omitted send a down click followed by an up release;</description></item>
        /// <item><term><c>Relative</c></term>: <description>treat the coordinates as relative offsets from the current mouse position.</description></item>
        /// </list>
        /// </param>
        public static void Click(object[] options)
        {
            string ParamLine = string.Empty;
            var aInput = new Windows.INPUT[2];
            var MousePos = new Point(0, 0);
            int ClickCount = 1;
            const string delimiter = ",";

            var RE_Coord = new Regex(@"(\d*?)\s*,\s*(\d*)[\s,\,]*", RegexOptions.IgnoreCase);
            var RE_Num = new Regex(@"\d+", RegexOptions.IgnoreCase);
            CaptureCollection Out;
            Match Match;

            //rebuild Argument string, as we have to parse this in a special way 
            foreach (var option in options)
            {
                if (option is string)
                    ParamLine += (string)option + delimiter;
                else if (option is double)
                    ParamLine += ((int)(double)option) + delimiter;
            }
            ParamLine = ParamLine.ToLower().Substring(0, ParamLine.Length - 1);

            //search coordinates, move mouse, remove them
            if (RE_Coord.IsMatch(ParamLine))
            {
                Match = RE_Coord.Match(ParamLine);
                MousePos.X = Convert.ToInt32(Match.Groups[1].Value);
                MousePos.Y = Convert.ToInt32(Match.Groups[2].Value);
                ParamLine = RE_Coord.Replace(ParamLine, string.Empty); //remove coord
                var CurrentCursor = new Cursor(Cursor.Current.Handle);
                if (coords.Mouse == CoordModeType.Relative)
                {
                    Windows.RECT rect;
                    Windows.GetWindowRect(Windows.GetForegroundWindow(), out rect);
                    MousePos.X += rect.Left;
                    MousePos.Y += rect.Top;
                }
                Cursor.Position = MousePos;
            }
            //click count
            if (RE_Num.IsMatch(ParamLine))
            {
                Out = RE_Num.Match(ParamLine).Captures;
                ClickCount = Convert.ToInt32(Out[0].Value);
                if (ClickCount <= 0)
                    return;
            }
            if (Environment.OSVersion.Platform == PlatformID.Win32NT)
            {
                //right or left mouse
                if (ParamLine.Contains(Keyword_Right))
                {
                    aInput[0].i.m.dwFlags = (uint)Windows.MOUSEEVENTF.RIGHTDOWN;
                    aInput[1].i.m.dwFlags = (uint)Windows.MOUSEEVENTF.RIGHTUP;
                }
                else
                {
                    aInput[0].i.m.dwFlags = (uint)Windows.MOUSEEVENTF.LEFTDOWN;
                    aInput[1].i.m.dwFlags = (uint)Windows.MOUSEEVENTF.LEFTUP;
                }
                //down event
                aInput[0].type = Windows.INPUT_MOUSE;
                aInput[0].i.m.dwExtraInfo = IntPtr.Zero;
                aInput[0].i.m.mouseData = 0;
                aInput[0].i.m.time = 0;
                aInput[0].i.m.dx = MousePos.X;
                aInput[0].i.m.dy = MousePos.Y;
                //up event
                aInput[1].type = Windows.INPUT_MOUSE;
                aInput[1].i.m.dwExtraInfo = IntPtr.Zero;
                aInput[1].i.m.mouseData = 0;
                aInput[1].i.m.time = 0;
                aInput[1].i.m.dx = MousePos.X;
                aInput[1].i.m.dy = MousePos.Y;

                if (ParamLine.Contains(Keyword_Up))
                { //just send the up event:
                    aInput[0] = aInput[1];
                    for (int i = 1; ClickCount >= i; i++)
                        Windows.SendInput(1, aInput, Marshal.SizeOf(typeof(Windows.INPUT)));
                }
                else if (ParamLine.Contains(Keyword_Down))
                {
                    //just send the down event:
                    for (int i = 1; ClickCount >= i; i++)
                        Windows.SendInput(1, aInput, Marshal.SizeOf(typeof(Windows.INPUT)));
                }
                else //send both events:
                    for (int i = 1; ClickCount >= i; i++)
                        Windows.SendInput((uint)aInput.Length, aInput, Marshal.SizeOf(typeof(Windows.INPUT)));
            }
        }

        /// <summary>
        /// Change the coordinate modes.
        /// </summary>
        /// <param name="item">
        /// <list type="bullet">
        /// <item><term>ToolTip</term></item>
        /// <item><term>Pixel</term></item>
        /// <item><term>Mouse</term></item>
        /// <item><term>Caret</term></item>
        /// <item><term>Menu</term></item>
        /// </list>
        /// </param>
        /// <param name="mode">
        /// <list type="bullet">
        /// <item><term>Screen</term>: <description>retrieve absolute coordinates;</description></item>
        /// <item><term>Relative</term>: <description>coordinates relative to the upper left corner of the active window.</description></item>
        /// </list>
        /// </param>
        public static void CoordMode(string item, string mode)
        {
            var target = IsOption(mode, Keyword_Screen) ? CoordModeType.Screen : CoordModeType.Relative;

            switch (item.ToLowerInvariant())
            {
                case Keyword_ToolTip: coords.Tooltip = target; break;
                case Keyword_Pixel: coords.Pixel = target; break;
                case Keyword_Mouse: coords.Mouse = target; break;
                case Keyword_Caret: coords.Caret = target; break;
                case Keyword_Menu: coords.Menu = target; break;
            }
        }

        /// <summary>
        /// Retrieves the current position of the mouse cursor, and optionally which window and control it is hovering over.
        /// </summary>
        /// <param name="x">The x-coordinate.</param>
        /// <param name="y">The y-coordinate.</param>
        /// <param name="win">The window ID.</param>
        /// <param name="control">The control ID.</param>
        /// <param name="mode">
        /// <list type="bullet">
        /// <item><term>2</term>: retrieve the <paramref name="control"/> ID rather than its class name.</item>
        /// </list>
        /// </param>
        public static void MouseGetPos(out int x, out int y, out long win, out string control, int mode = 0)
        {
            Point pos;
            win = 0;
            control = null;
            var cid = (mode & 2) == 2;

            if (Environment.OSVersion.Platform == PlatformID.Win32NT)
            {
                Windows.GetCursorPos(out pos);
                var hwnd = Windows.WindowFromPoint(pos);

                win = hwnd.ToInt32();

                var rect = new Windows.RECT();
                Windows.GetWindowRect(hwnd, out rect);
                var chwnd = Windows.RealChildWindowFromPoint(hwnd, new Point(pos.X - rect.Left, pos.Y - rect.Top));

                control = cid ? Windows.GetWindowText(chwnd) : chwnd.ToInt64().ToString();
            }
            else
                pos = System.Windows.Forms.Control.MousePosition;

            x = pos.X;
            y = pos.Y;

            if (coords.Mouse == CoordModeType.Relative)
            {
                if (Environment.OSVersion.Platform == PlatformID.Win32NT)
                {
                    Windows.RECT rect;
                    Windows.GetWindowRect(Windows.GetForegroundWindow(), out rect);
                    x -= rect.Left;
                    y -= rect.Top;
                }
                else
                {
                    // TODO: X11 get topstack window Rect(last in XQueryTree)  
                }
            }
        }
    }
}
