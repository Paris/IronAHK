using System;
using System.Drawing;
using System.Runtime.InteropServices;

namespace IronAHK.Rusty
{
    partial class Core
    {
        /// <summary>
        /// Clicks a mouse button at the specified coordinates. It can also hold down a mouse button, turn the mouse wheel, or move the mouse.
        /// </summary>
        /// <param name="Options"></param>
        public static void Click(string[] Options)
        {
            Point pos;
            if (Environment.OSVersion.Platform == PlatformID.Win32NT)
                Windows.MouseKeyboard.GetCursorPos(out pos);
            else pos = System.Windows.Forms.Control.MousePosition;

            var opts = Formats.ParseKeys(Options);
            string button = "l";
            int x = pos.X, y = pos.Y, count = 1;
            string downup = string.Empty;
            bool rel = false;

            int[] z = new int[3] { -1, -1, -1 };
            int zc = 0;

            opts.ForEach(delegate(string name)
            {
                switch (name[0])
                {
                    case 'l':
                    case 'm':
                    //case 'r':
                    case 'x':
                    case 'w':
                        button = name;
                        break;
                    case 'r':
                        if (name[1] == 'e')
                            rel = true;
                        else button = name;
                        break;
                    default:
                        int i;
                        if (zc < 3 && int.TryParse(name, out i))
                            z[zc] = i;
                        break;
                }
            });

            if (z[0] != -1 && z[1] == -1 && z[2] == -1)
                count = z[0];
            else if (z[0] != -1 && z[1] != -1)
            {
                x = z[0];
                y = z[1];
                if (z[2] != -1)
                    count = z[2];
            }

            MouseClick(button, x, y, count, 0, downup, rel);
        }
        
        /// <summary>
        /// Sends a mouse button or mouse wheel event to a control.
        /// </summary>
        /// <param name="Target">
        /// <para>If this parameter is blank, the target window's topmost control will be clicked (or the target window itself if it has no controls). Otherwise, one of the two modes below will be used.</para>
        /// <para>Mode 1 (Position): Specify the X and Y coordinates relative to the target window's upper left corner. The X coordinate must precede the Y coordinate and there must be at least one space or tab between them. For example: X55 Y33. If there is a control at the specified coordinates, it will be sent the click-event at those exact coordinates. If there is no control, the target window itself will be sent the event (which might have no effect depending on the nature of the window). Note: In this mode, the X and Y option letters of the Options parameter are ignored.</para>
        /// <para>Mode 2 (ClassNN or Text): Specify either ClassNN (the classname and instance number of the control) or the name/text of the control, both of which can be determined via Window Spy. When using name/text, the matching behavior is determined by SetTitleMatchMode.</para>
        /// <para>By default, mode 2 takes precedence over mode 1. For example, in the unlikely event that there is a control whose text or ClassNN has the format "Xnnn Ynnn", it would be acted upon by Mode 2. To override this and use mode 1 unconditionally, specify the word Pos in Options as in the following example: ControlClick, x255 y152, WinTitle,,,, Pos</para>
        /// <para>To operate upon a control's HWND (window handle), leave this parameter blank and specify ahk_id %ControlHwnd% for the WinTitle parameter (this also works on hidden controls even when DetectHiddenWindows is Off) . The HWND of a control is typically retrieved via ControlGet Hwnd, MouseGetPos, or DllCall.</para></param>
        /// <param name="WinTitle">The title or partial title of the target window (the matching behavior is determined by SetTitleMatchMode). If this and the other 3 window parameters are omitted, the Last Found Window will be used. If this is the letter A and the other 3 window parameters are omitted, the active window will be used. To use a window class, specify ahk_class ExactClassName (shown by Window Spy). To use a process identifier (PID), specify ahk_pid %VarContainingPID%. To use a window group, specify ahk_group GroupName. To use a window's unique ID number, specify ahk_id %VarContainingID%. The search can be narrowed by specifying multiple criteria. For example: My File.txt ahk_class Notepad</param>
        /// <param name="WinText">If present, this parameter must be a substring from a single text element of the target window (as revealed by the included Window Spy utility). Hidden text elements are detected if DetectHiddenText is ON.</param>
        /// <param name="WhichButton">
        /// <para>The button to click: LEFT, RIGHT, MIDDLE (or just the first letter of each of these). If omitted or blank, the LEFT button will be used.</para>
        /// <para>WheelUp (or WU) and WheelDown (or WD) are also supported on Windows NT/2000/XP or later. In this case, ClickCount is the number of notches to turn the wheel.</para>
        /// <para>X1 (XButton1, the 4th mouse button) and X2 (XButton2, the 5th mouse button) are also supported on Windows 2000/XP or later.</para>
        /// </param>
        /// <param name="ClickCount">The number of clicks to send. If omitted or blank, 1 click is sent.</param>
        /// <param name="Options">
        /// <para>A series of zero or more of the following option letters. For example: d x50 y25</para>
        /// <para>NA: Avoids activating the window, which might also improve reliability in cases where the user is physically moving the mouse during the ControlClick. However, this mode might not work properly for all types of windows and controls.</para>
        /// <para>D: Press the mouse button down but do not release it (i.e. generate a down-event). If both the D and U options are absent, a complete click (down and up) will be sent.</para>
        /// <para>U: Release the mouse button (i.e. generate an up-event). This option should not be present if the D option is already present (and vice versa).</para>
        /// <para>Pos: Specify the word Pos anywhere in Options to unconditionally use the X/Y positioning mode as described in the Control-or-Pos parameter above. </para>
        /// <para>Xn: Specify for n the X position to click at, relative to the control's upper left corner. If unspecified, the click will occur at the horizontal-center of the control.</para>
        /// <para>Yn: Specify for n the Y position to click at, relative to the control's upper left corner. If unspecified, the click will occur at the vertical-center of the control.</para>
        /// <para>Use decimal (not hexadecimal) numbers for the X and Y options.</para>
        /// </param>
        /// <param name="ExcludeTitle">Windows whose titles include this value will not be considered.</param>
        /// <param name="ExcludeText">Windows whose text include this value will not be considered.</param>
        public static void ControlClick(string Target, string WinTitle, string WinText, string WhichButton, string ClickCount, string Options, string ExcludeTitle, string ExcludeText)
        {

        }

#if LEGACY
        /// <summary>
        /// Clicks or holds down a mouse button, or turns the mouse wheel. NOTE: The Click command is generally more flexible and easier to use.
        /// </summary>
        /// <param name="WhichButton">
        /// <para>The button to click: Left (default), Right, Middle (or just the first letter of each of these); or the fourth or fifth mouse button (X1 or X2), which are supported on Windows 2000/XP or later. For example: MouseClick, X1. This parameter may be omitted, in which case it defaults to Left.</para>
        /// <para>Rotate the mouse wheel: On Windows NT/2000/XP or later, specify WheelUp or WU to turn the wheel upward (away from you); specify WheelDown or WD to turn the wheel downward (toward you). In this case, ClickCount is the number of notches to turn the wheel.</para>
        /// <para>To compensate automatically for cases where the user has swapped the left and right mouse buttons via the system's control panel, use the Click command instead.</para></param>
        /// <param name="X">The x/y coordinates to which the mouse cursor is moved prior to clicking. Coordinates are relative to the active window unless CoordMode was used to change that. If omitted, the cursor's current position is used.</param>
        /// <param name="Y">See <paramref name="X"/>.</param>
        /// <param name="ClickCount">The number of times to click the mouse, which can be an expression.  If omitted, the button is clicked once.</param>
        /// <param name="Speed">
        /// <para>The speed to move the mouse in the range 0 (fastest) to 100 (slowest), which can be an expression.  Note: a speed of 0 will move the mouse instantly.  If omitted, the default speed (as set by SetDefaultMouseSpeed or 2 otherwise) will be used.</para>
        /// <para>Speed is ignored for SendInput/Play modes; they move the mouse instantaneously (though SetMouseDelay has a mode that applies to SendPlay). To visually move the mouse more slowly -- such as a script that performs a demonstration for an audience -- use SendEvent {Click 100, 200} or SendMode Event (optionally in conjuction with BlockInput).</para>
        /// </param>
        /// <param name="DU">
        /// <para>If this parameter is omitted, each click will consist of a "down" event followed by an "up" event. Alternatively:</para>
        /// <list type="">
        /// <item>D = Press the mouse button down but do not release it (i.e. generate a down-event).</item>
        /// <item>U = Release the mouse button (i.e. generate an up-event).</item>
        /// </list>
        /// </param>
        /// <param name="R">If this parameter is the letter R, the X and Y coordinates will be treated as offsets from the current mouse position. In other words, the cursor will be moved from its current position by X pixels to the right (left if negative) and Y pixels down (up if negative).</param>
        public static void MouseClick(string WhichButton, int X, int Y, int ClickCount, int Speed, string DU, bool R)
        {
            int WHEEL_DELTA = 120;
            MouseMove(X, Y, Speed, R);
            Core.Sleep(Settings.MouseDelay);

            var i = new Windows.MouseKeyboard.INPUT[] { new Windows.MouseKeyboard.INPUT() };
            i[0].type = Windows.MouseKeyboard.INPUT_MOUSE;
            i[0].mi = new IronAHK.Rusty.Windows.MouseKeyboard.MOUSEINPUT();

            bool pressup = true, pressdown = true;
            string mode = DU.Trim().ToLower();
            if (mode == "d") pressup = false;
            else if (mode == "u") pressdown = false;

            uint key = 0, xkey = 0;
            switch (WhichButton.Trim().ToLower())
            {
                case "l":
                case "left":
                    key = Windows.MouseKeyboard.MOUSEEVENTF_LEFTDOWN;
                    xkey = Windows.MouseKeyboard.MOUSEEVENTF_LEFTUP;
                    break;
                case "r":
                case "right":
                    key = Windows.MouseKeyboard.MOUSEEVENTF_RIGHTDOWN;
                    xkey = Windows.MouseKeyboard.MOUSEEVENTF_RIGHTUP;
                    break;
                case "m":
                case "middle":
                    key = Windows.MouseKeyboard.MOUSEEVENTF_MIDDLEDOWN;
                    xkey = Windows.MouseKeyboard.MOUSEEVENTF_MIDDLEUP;
                    break;
                case "x1":
                    i[0].mi.mouseData |= (int)Windows.MouseKeyboard.XBUTTON1;
                    goto case "x";
                case "x2":
                    i[0].mi.mouseData |= (int)Windows.MouseKeyboard.XBUTTON2;
                    goto case "x";
                case "x":
                    key = Windows.MouseKeyboard.MOUSEEVENTF_XDOWN;
                    xkey = Windows.MouseKeyboard.MOUSEEVENTF_XUP;
                    break;
                case "wu":
                case "wheelup":
                    pressdown = true;
                    pressup = false;
                    i[0].mi.mouseData = ClickCount > 0 ? ClickCount : 0;
                    i[0].mi.mouseData = i[0].mi.mouseData * WHEEL_DELTA;
                    key = Windows.MouseKeyboard.MOUSEEVENTF_WHEEL;
                    break;
                case "wd":
                case "wheeldown":
                    pressdown = true;
                    pressup = false;
                    i[0].mi.mouseData = ClickCount > 0 ? ClickCount : 0;
                    i[0].mi.mouseData = - i[0].mi.mouseData * WHEEL_DELTA;
                    key = Windows.MouseKeyboard.MOUSEEVENTF_WHEEL;
                    break;
            }

            int sz = Marshal.SizeOf(i[0]);
            uint l = (uint)i.Length;

            while (ClickCount-- > 0)
            {
                if (pressdown)
                {
                    i[0].mi.dwFlags = key;
                    Windows.MouseKeyboard.SendInput(l, i, sz);
                }

                if (pressup)
                {
                    i[0].mi.dwFlags = xkey;
                    Windows.MouseKeyboard.SendInput(l, i, sz);
                }
            }
        }
#endif
    
#if LEGACY
        /// <summary>
        /// Clicks and holds the specified mouse button, moves the mouse to the destination coordinates, then releases the button.
        /// </summary>
        /// <param name="WhichButton">
        /// <para>The button to click: Left, Right, Middle (or just the first letter of each of these). The fourth and fifth mouse buttons are supported on Windows 2000/XP or later: Specify X1 for the fourth button and X2 for the fifth. For example: MouseClickDrag, X1, ...</para>
        /// <para>To compensate automatically for cases where the user has swapped the left and right mouse buttons via the system's control panel, use the Click command instead.</para>
        /// </param>
        /// <param name="X1">The x/y coordinates of the drag's starting position, which can be expressions (the mouse will be moved to these coordinates right before the drag is started). Coordinates are relative to the active window unless CoordMode was used to change that. If omitted, the mouse's current position is used.</param>
        /// <param name="Y1">See <paramref name="X1"/>.</param>
        /// <param name="X2">The x/y coordinates to drag the mouse to (that is, while the button is held down), which can be expressions. Coordinates are relative to the active window unless CoordMode was used to change that.</param>
        /// <param name="Y2">See <paramref name="X2"/>.</param>
        /// <param name="Speed">
        /// <para>The speed to move the mouse in the range 0 (fastest) to 100 (slowest), which can be an expression.  Note: a speed of 0 will move the mouse instantly.  If omitted, the default speed (as set by SetDefaultMouseSpeed or 2 otherwise) will be used.</para>
        /// <para>Speed is ignored for SendInput/Play modes; they move the mouse instantaneously (though SetMouseDelay has a mode that applies to SendPlay). To visually move the mouse more slowly -- such as a script that performs a demonstration for an audience -- use SendEvent {Click 100, 200} or SendMode Event (optionally in conjuction with BlockInput).</para>
        /// </param>
        /// <param name="R">
        /// <para>If this parameter is the letter R, the X1 and Y1 coordinates will be treated as offsets from the current mouse position. In other words, the cursor will be moved from its current position by X1 pixels to the right (left if negative) and Y1 pixels down (up if negative).</para>
        /// <para>Similarly, the X2 and Y2 coordinates will be treated as offsets from the X1 and Y1 coordinates. For example, the following would first move the cursor down and to the right by 5 pixels from its starting position, and then drag it from that position down and to the right by 10 pixels: MouseClickDrag, Left, 5, 5, 10, 10, , R</para>
        /// </param>
        public static void MouseClickDrag(string WhichButton, int X1, int Y1, int X2, int Y2, int Speed, bool R)
        {
            MouseClick(WhichButton, X1, Y1, 1, Speed, "down", R);
            MouseClick(WhichButton, X2, Y2, 1, Speed, "up", R);
        }
#endif

        /// <summary>
        /// Retrieves the current position of the mouse cursor, and optionally which window and control it is hovering over.
        /// </summary>
        /// <param name="OutputVarX">The names of the variables in which to store the X and Y coordinates. The retrieved coordinates are relative to the active window unless CoordMode was used to change to screen coordinates.</param>
        /// <param name="OutputVarY">See <paramref name="OutputVarX"/>.</param>
        /// <param name="OutputVarWin">
        /// <para>This optional parameter is the name of the variable in which to store the unique ID number of the window under the mouse cursor. If the window cannot be determined, this variable will be made blank.</para>
        /// <para>The window does not have to be active to be detected. Hidden windows cannot be detected.</para>
        ///</param>
        /// <param name="OutputVarControl">
        /// <para>This optional parameter is the name of the variable in which to store the name (ClassNN) of the control under the mouse cursor. If the control cannot be determined, this variable will be made blank.</para>
        /// <para>The names of controls should always match those shown by the version of Window Spy distributed with v1.0.14+ (but not necessarily older versions of Window Spy). However, unlike Window Spy, the window under the mouse cursor does not have to be active for a control to be detected.</para>
        ///</param>
        /// <param name="Mode">
        /// <para>If omitted, it defaults to 0. Otherwise, specify one of the following digits:</para>
        /// <para>1: Uses a simpler method to determine OutputVarControl. This method correctly retrieves the active/topmost child window of an Multiple Document Interface (MDI) application such as SysEdit or TextPad. However, it is less accurate for other purposes such as detecting controls inside a GroupBox control.</para>
        /// <para>2: Stores the control's HWND in OutputVarControl rather than the control's ClassNN.</para>
        /// <para>3: A combination of 1 and 2 above.</para>
        /// </param>
        public static void MouseGetPos(out int OutputVarX, out int OutputVarY, out int OutputVarWin, out string OutputVarControl, int Mode)
        {
            Point pos;
            OutputVarWin = 0;
            OutputVarControl = null;

            if (Environment.OSVersion.Platform == PlatformID.Win32NT)
            {
                Windows.MouseKeyboard.GetCursorPos(out pos);
                IntPtr hwnd = Windows.Windows.WindowFromPoint(pos);

                var rect = new Windows.Windows.RECT();
                Windows.Windows.GetWindowRect(hwnd, out rect);
                IntPtr chwnd = Windows.Windows.RealChildWindowFromPoint(hwnd, new Point(pos.X - rect.Left, pos.Y - rect.Top));

                OutputVarControl = Mode == 1 ?
                    Windows.Windows.GetWindowText(chwnd) : chwnd.ToInt32().ToString();
            }
            else pos = System.Windows.Forms.Control.MousePosition;

            OutputVarX = pos.X;
            OutputVarY = pos.Y;
            if (Settings.CoordMode.Mouse)
            {
                Windows.Windows.RECT rect;
                Windows.Windows.GetWindowRect(Windows.Windows.GetForegroundWindow(), out rect);
                OutputVarX -= rect.Left;
                OutputVarY -= rect.Top;
            }
        }

#if LEGACY
        /// <summary>
        /// Moves the mouse cursor.
        /// </summary>
        /// <param name="X">The x/y coordinates to move the mouse to. Coordinates are relative to the active window unless CoordMode was used to change that.</param>
        /// <param name="Y">See <paramref name="X"/>.</param>
        /// <param name="Speed">
        /// <para>The speed to move the mouse in the range 0 (fastest) to 100 (slowest), which can be an expression.  Note: a speed of 0 will move the mouse instantly.  If omitted, the default speed (as set by SetDefaultMouseSpeed or 2 otherwise) will be used.</para>
        /// <para>Speed is ignored for SendInput/Play modes; they move the mouse instantaneously (though SetMouseDelay has a mode that applies to SendPlay). To visually move the mouse more slowly -- such as a script that performs a demonstration for an audience -- use SendEvent {Click 100, 200} or SendMode Event (optionally in conjuction with BlockInput).</para>
        /// </param>
        /// <param name="R">If this parameter is the letter R, the X and Y coordinates will be treated as offsets from the current mouse position. In other words, the cursor will be moved from its current position by X pixels to the right (left if negative) and Y pixels down (up if negative).</param>
        public static void MouseMove(int X, int Y, int Speed, bool R)
        {
            int ScreenWidth = Core.A_ScreenWidth, ScreenHeight = Core.A_ScreenHeight;
            if (Settings.CoordMode.Mouse && !R)
            {
                Windows.Windows.RECT rect;
                Windows.Windows.GetWindowRect(Windows.Windows.GetForegroundWindow(), out rect);
                X += rect.Left;
                Y += rect.Top;
            }
            if (Speed < 0)  // This can happen during script's runtime due to something like: MouseMove, X, Y, %VarContainingNegative%
                Speed = 0;  // 0 is the fastest.
            else
                if (Speed > 100)
                    Speed = 100;
            if (Speed == 0)  //Mouse is instant
            {
                var i = new Windows.MouseKeyboard.INPUT[] { new Windows.MouseKeyboard.INPUT() };
                i[0].type = Windows.MouseKeyboard.INPUT_MOUSE;
                i[0].mi = new IronAHK.Rusty.Windows.MouseKeyboard.MOUSEINPUT();
                i[0].mi.dwFlags = Windows.MouseKeyboard.MOUSEEVENTF_MOVE;
                if (!R)
                {
                    i[0].mi.dwFlags |= Windows.MouseKeyboard.MOUSEEVENTF_ABSOLUTE;
                    i[0].mi.dx = MOUSE_COORD_TO_ABS(X, ScreenWidth);
                    i[0].mi.dy = MOUSE_COORD_TO_ABS(Y, ScreenHeight);

                }
                else
                {
                    i[0].mi.dx = X;
                    i[0].mi.dy = Y;
                }

                Windows.MouseKeyboard.SendInput((uint)i.Length, i, Marshal.SizeOf(i[0]));
                return;
            }
            int mousex, mousey, mousewinid;
            string control;
            Core.MouseGetPos(out mousex, out mousey, out mousewinid, out control, 0);
            if (Settings.CoordMode.Mouse) //screen coords required for doincrementalmousemove
            {
                Windows.Windows.RECT rect;
                Windows.Windows.GetWindowRect(Windows.Windows.GetForegroundWindow(), out rect);
                mousex += rect.Left;
                mousey += rect.Top;
            }
            if (R)
            {
                X += mousex;
                Y += mousey;
            }
            DoIncrementalMouseMove(MOUSE_COORD_TO_ABS(mousex, ScreenWidth), MOUSE_COORD_TO_ABS(mousey, ScreenHeight), MOUSE_COORD_TO_ABS(X, ScreenWidth), MOUSE_COORD_TO_ABS(Y, ScreenHeight), Speed);
        }
#endif

        private static int MOUSE_COORD_TO_ABS(int coord, int width_or_height)
        {
            return ((65536 * coord) / width_or_height) + 1;
        }

        private static void DoIncrementalMouseMove(int aX1, int aY1, int aX2, int aY2, int aSpeed)
        // aX1 and aY1 are the starting coordinates, and "2" are the destination coordinates.
        // Caller has ensured that aSpeed is in the range 0 to 100, inclusive.
        {
            // AutoIt3: So, it's a more gradual speed that is needed :)
            int delta;
            int INCR_MOUSE_MIN_SPEED = 5;
            while (aX1 != aX2 || aY1 != aY2)
            {
                if (aX1 < aX2)
                {
                    delta = (aX2 - aX1) / aSpeed;
                    if (delta == 0 || delta < INCR_MOUSE_MIN_SPEED)
                        delta = INCR_MOUSE_MIN_SPEED;
                    if ((aX1 + delta) > aX2)
                        aX1 = aX2;
                    else
                        aX1 += delta;
                }
                else
                    if (aX1 > aX2)
                    {
                        delta = (aX1 - aX2) / aSpeed;
                        if (delta == 0 || delta < INCR_MOUSE_MIN_SPEED)
                            delta = INCR_MOUSE_MIN_SPEED;
                        if ((aX1 - delta) < aX2)
                            aX1 = aX2;
                        else
                            aX1 -= delta;
                    }

                if (aY1 < aY2)
                {
                    delta = (aY2 - aY1) / aSpeed;
                    if (delta == 0 || delta < INCR_MOUSE_MIN_SPEED)
                        delta = INCR_MOUSE_MIN_SPEED;
                    if ((aY1 + delta) > aY2)
                        aY1 = aY2;
                    else
                        aY1 += delta;
                }
                else
                    if (aY1 > aY2)
                    {
                        delta = (aY1 - aY2) / aSpeed;
                        if (delta == 0 || delta < INCR_MOUSE_MIN_SPEED)
                            delta = INCR_MOUSE_MIN_SPEED;
                        if ((aY1 - delta) < aY2)
                            aY1 = aY2;
                        else
                            aY1 -= delta;
                    }

                var i = new Windows.MouseKeyboard.INPUT[] { new Windows.MouseKeyboard.INPUT() };
                i[0].type = Windows.MouseKeyboard.INPUT_MOUSE;
                i[0].mi = new IronAHK.Rusty.Windows.MouseKeyboard.MOUSEINPUT();
                i[0].mi.dx = aX1;
                i[0].mi.dy = aY1;
                i[0].mi.dwFlags = Windows.MouseKeyboard.MOUSEEVENTF_MOVE;
                i[0].mi.dwFlags |= Windows.MouseKeyboard.MOUSEEVENTF_ABSOLUTE;
                Windows.MouseKeyboard.SendInput((uint)i.Length, i, Marshal.SizeOf(i[0]));
                Core.Sleep(Settings.MouseDelay);
            }
        }

#if LEGACY
        /// <summary>
        /// Sets the mouse speed that will be used if unspecified in Click and MouseMove/Click/Drag.
        /// </summary>
        /// <param name="Speed">The speed to move the mouse in the range 0 (fastest) to 100 (slowest).  Note: a speed of 0 will move the mouse instantly.</param>
        public static void SetDefaultMouseSpeed(int Speed)
        {
            Settings.DefaultMouseSpeed = Speed;
        }
#endif

#if LEGACY
        /// <summary>
        /// Sets the delay that will occur after each mouse movement or click.
        /// </summary>
        /// <param name="Delay">Time in milliseconds, which can be an expression. Use -1 for no delay at all and 0 for the smallest possible delay (however, if the Play parameter is present, both 0 and -1 produce no delay). If unset, the default delay is 10 for the traditional SendEvent mode and -1 for SendPlay mode.</param>
        /// <param name="Play">The word Play applies the delay to the SendPlay mode rather than the traditional Send/SendEvent mode. If a script never uses this parameter, the delay is always -1 for SendPlay.</param>
        public static void SetMouseDelay(int Delay, string Play)
        {
            Settings.MouseDelay = Delay;
        }
#endif
        [DllImport("user32.dll", EntryPoint = "GetCursorPos")]
        private static extern int GetCursorPos(out POINTAPI point);

        [StructLayout(LayoutKind.Sequential)]
        struct POINTAPI
        {
            public int x;
            public int y;
        }
    }
}