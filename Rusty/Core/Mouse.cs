using System;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using IronAHK.Rusty.Common;

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
                if (coords.Mouse == CoordModeType.Relative)
                {
                    var foreGroundWindow = Window.WindowItemProvider.Instance.ActiveWindow;
                    if(foreGroundWindow != null) {
                        var location = foreGroundWindow.Location;
                        MousePos.X += location.X;
                        MousePos.Y += location.Y;
                    }
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
                var aInput = new WindowsAPI.INPUT[2];
                //right or left mouse
                if (ParamLine.Contains(Keyword_Right))
                {
                    aInput[0].i.m.dwFlags = (uint)WindowsAPI.MOUSEEVENTF.RIGHTDOWN;
                    aInput[1].i.m.dwFlags = (uint)WindowsAPI.MOUSEEVENTF.RIGHTUP;
                }
                else
                {
                    aInput[0].i.m.dwFlags = (uint)WindowsAPI.MOUSEEVENTF.LEFTDOWN;
                    aInput[1].i.m.dwFlags = (uint)WindowsAPI.MOUSEEVENTF.LEFTUP;
                }
                //down event
                aInput[0].type = WindowsAPI.INPUT_MOUSE;
                aInput[0].i.m.dwExtraInfo = IntPtr.Zero;
                aInput[0].i.m.mouseData = 0;
                aInput[0].i.m.time = 0;
                aInput[0].i.m.dx = MousePos.X;
                aInput[0].i.m.dy = MousePos.Y;
                //up event
                aInput[1].type = WindowsAPI.INPUT_MOUSE;
                aInput[1].i.m.dwExtraInfo = IntPtr.Zero;
                aInput[1].i.m.mouseData = 0;
                aInput[1].i.m.time = 0;
                aInput[1].i.m.dx = MousePos.X;
                aInput[1].i.m.dy = MousePos.Y;

                if (ParamLine.Contains(Keyword_Up))
                { //just send the up event:
                    aInput[0] = aInput[1];
                    for (int i = 1; ClickCount >= i; i++)
                        WindowsAPI.SendInput(1, aInput, Marshal.SizeOf(typeof(WindowsAPI.INPUT)));
                }
                else if (ParamLine.Contains(Keyword_Down))
                {
                    //just send the down event:
                    for (int i = 1; ClickCount >= i; i++)
                        WindowsAPI.SendInput(1, aInput, Marshal.SizeOf(typeof(WindowsAPI.INPUT)));
                }
                else //send both events:
                    for (int i = 1; ClickCount >= i; i++)
                        WindowsAPI.SendInput((uint)aInput.Length, aInput, Marshal.SizeOf(typeof(WindowsAPI.INPUT)));
            }
        }

        /// <summary>
        /// Clicks and holds the specified mouse button, moves the mouse to the destination coordinates, then releases the button.
        /// </summary>
        /// <param name="button">Either <c>Left</c> (default), <c>Middle</c>, <c>Right</c>, <c>X1</c> or <c>X2</c>.</param>
        /// <param name="x1">The starting x-coordinate.</param>
        /// <param name="y1">The starting y-coordinate.</param>
        /// <param name="x2">The final x-coordinate.</param>
        /// <param name="y2">The final y-coordinate.</param>
        /// <param name="speed">The speed to move the mouse from 0 (fastest) to 100 (slowest).
        /// The default speed is determined by <see cref="A_DefaultMouseSpeed"/>.</param>
        /// <param name="relative"><c>true</c> to treat the first set of coordinates as relative offsets from the current mouse position
        /// and the second set as offsets from the first, <c>false</c> otherwise.</param>
        public static void MouseClickDrag(string button, int x1, int y1, int x2, int y2, int? speed = null, bool relative = false)
        {
            var oldSpeed = A_DefaultMouseSpeed;

            if (speed != null)
                A_DefaultMouseSpeed = (int)speed;

            var opts = new[] { x1.ToString(), y1.ToString(), relative ? Keyword_Relative : string.Empty };
            Click(opts);

            opts[0] = x2.ToString();
            opts[1] = y2.ToString();
            Click(opts);

            if (speed != null)
                A_DefaultMouseSpeed = oldSpeed;
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
            win = 0;
            control = null;
            var cid = (mode & 2) == 2;

            var pos = Cursor.Position;
            var found = windowManager.WindowFromPoint(pos);
            win = found.Handle.ToInt32();

            var foundLocation = found.Location;
            var child = found.RealChildWindowFromPoint(new Point(pos.X - foundLocation.X, pos.Y - foundLocation.Y));

            control = cid ? child.Handle.ToInt64().ToString() : child.ClassNN;

            x = pos.X;
            y = pos.Y;
            if (coords.Mouse == CoordModeType.Relative)
            {
                var location = windowManager.GetForeGroundWindow().Location;
                x -= location.X;
                y -= location.Y;
            }
        }
    }
}
