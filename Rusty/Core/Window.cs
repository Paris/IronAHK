using System;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Drawing;
using IronAHK.Rusty.Common;

namespace IronAHK.Rusty
{
    partial class Core
    {
        // TODO: organise Windows.cs


        /// <summary>
        /// easy access to the window groups
        /// </summary>
        private static Dictionary<string, Stack<Window.WindowItemBase>> windowGroups
        {
            get { return Window.WindowItemProvider.Instance.Groups; }
        }

        /// <summary>
        /// easy access to the windowmanager
        /// </summary>
        private static Window.WindowManagerBase windowManager
        {
            get { return Window.WindowItemProvider.Instance; }
        }




        /// <summary>
        /// Makes a variety of changes to a control.
        /// </summary>
        /// <param name="Cmd">See list below.</param>
        /// <param name="Value">See list below.</param>
        /// <param name="ControlID">
        /// <para>Can be either ClassNN (the classname and instance number of the control) or the name/text of the control, both of which can be determined via Window Spy. When using name/text, the matching behavior is determined by SetTitleMatchMode. If this parameter is blank, the target window's topmost control will be used.</para>
        /// <para>To operate upon a control's HWND (window handle), leave the Control parameter blank and specify ahk_id %ControlHwnd% for the WinTitle parameter (this also works on hidden controls even when DetectHiddenWindows is Off) . The HWND of a control is typically retrieved via ControlGet Hwnd, MouseGetPos, or DllCall.</para>
        /// </param>
        /// <param name="WinTitle">The title or partial title of the target window (the matching behavior is determined by SetTitleMatchMode). If this and the next 3 parameters are omitted, the Last Found Window will be used. If this is the letter A and the next 3 parameters are omitted, the active window will be used. To use a window class, specify ahk_class ExactClassName (shown by Window Spy). To use a process identifier (PID), specify ahk_pid %VarContainingPID%. To use a window group, specify ahk_group GroupName. To use a window's unique ID number, specify ahk_id %VarContainingID%. The search can be narrowed by specifying multiple criteria. For example: My File.txt ahk_class Notepad</param>
        /// <param name="WinText">If present, this parameter must be a substring from a single text element of the target window (as revealed by the included Window Spy utility). Hidden text elements are detected if DetectHiddenText is ON.</param>
        /// <param name="ExcludeTitle">Windows whose titles include this value will not be considered.</param>
        /// <param name="ExcludeText">Windows whose text include this value will not be considered.</param>
        public static void Control(string Cmd, string Value, string ControlID, string WinTitle, string WinText, string ExcludeTitle, string ExcludeText)
        {

        }

        /// <summary>
        /// Sends a mouse button or mouse wheel event to a control.
        /// </summary>
        /// <param name="mode">
        /// <para>If this parameter is blank, the target window's topmost control will be clicked (or the target window itself if it has no controls). Otherwise, one of the two modes below will be used.</para>
        /// <para>Mode 1 (Position): Specify the X and Y coordinates relative to the target window's upper left corner. The X coordinate must precede the Y coordinate and there must be at least one space or tab between them. For example: X55 Y33. If there is a control at the specified coordinates, it will be sent the click-event at those exact coordinates. If there is no control, the target window itself will be sent the event (which might have no effect depending on the nature of the window). Note: In this mode, the X and Y option letters of the Options parameter are ignored.</para>
        /// <para>Mode 2 (ClassNN or Text): Specify either ClassNN (the classname and instance number of the control) or the name/text of the control, both of which can be determined via Window Spy. When using name/text, the matching behavior is determined by SetTitleMatchMode.</para>
        /// <para>By default, mode 2 takes precedence over mode 1. For example, in the unlikely event that there is a control whose text or ClassNN has the format "Xnnn Ynnn", it would be acted upon by Mode 2. To override this and use mode 1 unconditionally, specify the word Pos in Options as in the following example: ControlClick, x255 y152, WinTitle,,,, Pos</para>
        /// <para>To operate upon a control's HWND (window handle), leave this parameter blank and specify ahk_id %ControlHwnd% for the WinTitle parameter (this also works on hidden controls even when DetectHiddenWindows is Off) . The HWND of a control is typically retrieved via ControlGet Hwnd, MouseGetPos, or DllCall.</para></param>
        /// <param name="title">The title or partial title of the target window (the matching behavior is determined by SetTitleMatchMode). If this and the other 3 window parameters are omitted, the Last Found Window will be used. If this is the letter A and the other 3 window parameters are omitted, the active window will be used. To use a window class, specify ahk_class ExactClassName (shown by Window Spy). To use a process identifier (PID), specify ahk_pid %VarContainingPID%. To use a window group, specify ahk_group GroupName. To use a window's unique ID number, specify ahk_id %VarContainingID%. The search can be narrowed by specifying multiple criteria. For example: My File.txt ahk_class Notepad</param>
        /// <param name="text">If present, this parameter must be a substring from a single text element of the target window (as revealed by the included Window Spy utility). Hidden text elements are detected if DetectHiddenText is ON.</param>
        /// <param name="button">
        /// <para>The button to click: LEFT, RIGHT, MIDDLE (or just the first letter of each of these). If omitted or blank, the LEFT button will be used.</para>
        /// <para>WheelUp (or WU) and WheelDown (or WD) are also supported on Windows NT/2000/XP or later. In this case, ClickCount is the number of notches to turn the wheel.</para>
        /// <para>X1 (XButton1, the 4th mouse button) and X2 (XButton2, the 5th mouse button) are also supported on Windows 2000/XP or later.</para>
        /// </param>
        /// <param name="clickCount">The number of clicks to send. If omitted or blank, 1 click is sent.</param>
        /// <param name="options">
        /// <para>A series of zero or more of the following option letters. For example: d x50 y25</para>
        /// <para>NA: Avoids activating the window, which might also improve reliability in cases where the user is physically moving the mouse during the ControlClick. However, this mode might not work properly for all types of windows and controls.</para>
        /// <para>D: Press the mouse button down but do not release it (i.e. generate a down-event). If both the D and U options are absent, a complete click (down and up) will be sent.</para>
        /// <para>U: Release the mouse button (i.e. generate an up-event). This option should not be present if the D option is already present (and vice versa).</para>
        /// <para>Pos: Specify the word Pos anywhere in Options to unconditionally use the X/Y positioning mode as described in the Control-or-Pos parameter above. </para>
        /// <para>Xn: Specify for n the X position to click at, relative to the control's upper left corner. If unspecified, the click will occur at the horizontal-center of the control.</para>
        /// <para>Yn: Specify for n the Y position to click at, relative to the control's upper left corner. If unspecified, the click will occur at the vertical-center of the control.</para>
        /// <para>Use decimal (not hexadecimal) numbers for the X and Y options.</para>
        /// </param>
        /// <param name="excludeTitle">Windows whose titles include this value will not be considered.</param>
        /// <param name="excludeText">Windows whose text include this value will not be considered.</param>
        public static void ControlClick(string mode, string title, string text, string button, int clickCount, string options, string excludeTitle, string excludeText)
        {
            Point _click;
            Point? click = null;
            var criteria = Window.SearchCriteria.FromString(title, text, excludeTitle, excludeText);
            var target = windowManager.FindWindow(criteria);

            if(!target.IsSpecified)
                return; // window/control not found.

            if(string.IsNullOrEmpty(mode)) {
                // click topmost control/window
            } else if(TryParseCoordinate(mode, out _click)) {
                click = _click;
            } else {
                // controlOrPos must be control identifier
                // ToDo: set target to spec. control!
                throw new NotImplementedException();
            }

            // ToDo: Parse Options and send specific Mousekey 
            // For now, just send single Click
            for(int i = 0; i < clickCount; i++)
                target.Click(click);
        }




        /// <summary>
        /// Sets input focus to a given control on a window.
        /// </summary>
        /// <param name="Control">
        /// <para>Can be either ClassNN (the classname and instance number of the control) or the name/text of the control, both of which can be determined via Window Spy. When using name/text, the matching behavior is determined by SetTitleMatchMode. If this parameter is blank or omitted, the target window's topmost control will be used.</para>
        /// <para>To operate upon a control's HWND (window handle), leave the Control parameter blank and specify ahk_id %ControlHwnd% for the WinTitle parameter (this also works on hidden controls even when DetectHiddenWindows is Off) . The HWND of a control is typically retrieved via ControlGet Hwnd, MouseGetPos, or DllCall.</para>
        /// </param>
        /// <param name="WinTitle">The title or partial title of the target window (the matching behavior is determined by SetTitleMatchMode). If this and the next 3 parameters are omitted, the Last Found Window will be used. If this is the letter A and the next 3 parameters are omitted, the active window will be used. To use a window class, specify ahk_class ExactClassName (shown by Window Spy). To use a process identifier (PID), specify ahk_pid %VarContainingPID%. To use a window group, specify ahk_group GroupName. To use a window's unique ID number, specify ahk_id %VarContainingID%. The search can be narrowed by specifying multiple criteria. For example: My File.txt ahk_class Notepad</param>
        /// <param name="WinText">If present, this parameter must be a substring from a single text element of the target window (as revealed by the included Window Spy utility). Hidden text elements are detected if DetectHiddenText is ON.</param>
        /// <param name="ExcludeTitle">Windows whose titles include this value will not be considered.</param>
        /// <param name="ExcludeText">Windows whose text include this value will not be considered.</param>
        public static void ControlFocus(string Control, string WinTitle, string WinText, string ExcludeTitle, string ExcludeText)
        {

        }

        /// <summary>
        /// Retrieves various types of information about a control.
        /// </summary>
        /// <param name="OutputVar">The name of the variable in which to store the result of Cmd.</param>
        /// <param name="Cmd">See list below.</param>
        /// <param name="Value">See list below.</param>
        /// <param name="ControlID">
        /// <para>Can be either ClassNN (the classname and instance number of the control) or the name/text of the control, both of which can be determined via Window Spy. When using name/text, the matching behavior is determined by SetTitleMatchMode. If this parameter is blank, the target window's topmost control will be used.</para>
        /// <para>To operate upon a control's HWND (window handle), leave the Control parameter blank and specify ahk_id %ControlHwnd% for the WinTitle parameter (this also works on hidden controls even when DetectHiddenWindows is Off) . The HWND of a control is typically retrieved via ControlGet Hwnd, MouseGetPos, or DllCall.</para>
        /// </param>
        /// <param name="WinTitle">The title or partial title of the target window (the matching behavior is determined by SetTitleMatchMode). If this and the next 3 parameters are omitted, the Last Found Window will be used. If this is the letter A and the next 3 parameters are omitted, the active window will be used. To use a window class, specify ahk_class ExactClassName (shown by Window Spy). To use a process identifier (PID), specify ahk_pid %VarContainingPID%. To use a window group, specify ahk_group GroupName. To use a window's unique ID number, specify ahk_id %VarContainingID%. The search can be narrowed by specifying multiple criteria. For example: My File.txt ahk_class Notepad</param>
        /// <param name="WinText">If present, this parameter must be a substring from a single text element of the target window (as revealed by the included Window Spy utility). Hidden text elements are detected if DetectHiddenText is ON.</param>
        /// <param name="ExcludeTitle">Windows whose titles include this value will not be considered.</param>
        /// <param name="ExcludeText">Windows whose text include this value will not be considered.</param>
        public static void ControlGet(out string OutputVar, string Cmd, string Value, string ControlID, string WinTitle, string WinText, string ExcludeTitle, string ExcludeText)
        {
            OutputVar = null;
        }

        /// <summary>
        /// Retrieves which control of the target window has input focus, if any.
        /// </summary>
        /// <param name="OutputVar">The name of the variable in which to store the identifier of the control, which consists of its classname followed by its sequence number within its parent window, e.g. Button12.</param>
        /// <param name="WinTitle">The title or partial title of the target window (the matching behavior is determined by SetTitleMatchMode). If this and the next 3 parameters are omitted, the Last Found Window will be used. If this is the letter A and the next 3 parameters are omitted, the active window will be used. To use a window class, specify ahk_class ExactClassName (shown by Window Spy). To use a process identifier (PID), specify ahk_pid %VarContainingPID%. To use a window group, specify ahk_group GroupName. To use a window's unique ID number, specify ahk_id %VarContainingID%. The search can be narrowed by specifying multiple criteria. For example: My File.txt ahk_class Notepad</param>
        /// <param name="WinText">If present, this parameter must be a substring from a single text element of the target window (as revealed by the included Window Spy utility). Hidden text elements are detected if DetectHiddenText is ON.</param>
        /// <param name="ExcludeTitle">Windows whose titles include this value will not be considered.</param>
        /// <param name="ExcludeText">Windows whose text include this value will not be considered.</param>
        public static void ControlGetFocus(out string OutputVar, string WinTitle, string WinText, string ExcludeTitle, string ExcludeText)
        {
            OutputVar = null;
        }

        /// <summary>
        /// Retrieves the position and size of a control.
        /// </summary>
        /// <param name="X">
        /// <para>The names of the variables in which to store the X and Y coordinates (in pixels) of Control's upper left corner. These coordinates are relative to the target window's upper-left corner and thus are the same as those used by ControlMove.</para>
        /// <para>If either X or Y is omitted, the corresponding values will not be stored.</para>
        /// </param>
        /// <param name="Y">See <paramref name="X"/>.</param>
        /// <param name="Width">The names of the variables in which to store Control's width and height (in pixels). If omitted, the corresponding values will not be stored.</param>
        /// <param name="Height">See <paramref name="Width"/>.</param>
        /// <param name="Control">
        /// <para>Can be either ClassNN (the classname and instance number of the control) or the name/text of the control, both of which can be determined via Window Spy. When using name/text, the matching behavior is determined by SetTitleMatchMode. If this parameter is blank, the target window's topmost control will be used.</para>
        /// <para>To operate upon a control's HWND (window handle), leave the Control parameter blank and specify ahk_id %ControlHwnd% for the WinTitle parameter (this also works on hidden controls even when DetectHiddenWindows is Off) . The HWND of a control is typically retrieved via ControlGet Hwnd, MouseGetPos, or DllCall.</para>
        /// </param>
        /// <param name="WinTitle">The title or partial title of the target window (the matching behavior is determined by SetTitleMatchMode). If this and the next 3 parameters are omitted, the Last Found Window will be used. If this is the letter A and the next 3 parameters are omitted, the active window will be used. To use a window class, specify ahk_class ExactClassName (shown by Window Spy). To use a process identifier (PID), specify ahk_pid %VarContainingPID%. To use a window group, specify ahk_group GroupName. To use a window's unique ID number, specify ahk_id %VarContainingID%. The search can be narrowed by specifying multiple criteria. For example: My File.txt ahk_class Notepad</param>
        /// <param name="WinText">If present, this parameter must be a substring from a single text element of the target window (as revealed by the included Window Spy utility). Hidden text elements are detected if DetectHiddenText is ON.</param>
        /// <param name="ExcludeTitle">Windows whose titles include this value will not be considered.</param>
        /// <param name="ExcludeText">Windows whose text include this value will not be considered.</param>
        public static void ControlGetPos(out int X, out int Y, out int Width, out int Height, string Control, string WinTitle, string WinText, string ExcludeTitle, string ExcludeText)
        {
            X = Y = Width = Height = 0;
        }

        /// <summary>
        /// Retrieves text from a control.
        /// </summary>
        /// <param name="OutputVar">The name of the variable in which to store the retrieved text.</param>
        /// <param name="Control">
        /// <para>Can be either ClassNN (the classname and instance number of the control) or the name/text of the control, both of which can be determined via Window Spy. When using name/text, the matching behavior is determined by SetTitleMatchMode. If this parameter is blank or omitted, the target window's topmost control will be used.</para>
        /// <para>To operate upon a control's HWND (window handle), leave the Control parameter blank and specify ahk_id %ControlHwnd% for the WinTitle parameter (this also works on hidden controls even when DetectHiddenWindows is Off) . The HWND of a control is typically retrieved via ControlGet Hwnd, MouseGetPos, or DllCall.</para>
        /// </param>
        /// <param name="WinTitle">The title or partial title of the target window (the matching behavior is determined by SetTitleMatchMode). If this and the next 3 parameters are omitted, the Last Found Window will be used. If this is the letter A and the next 3 parameters are omitted, the active window will be used. To use a window class, specify ahk_class ExactClassName (shown by Window Spy). To use a process identifier (PID), specify ahk_pid %VarContainingPID%. To use a window group, specify ahk_group GroupName. To use a window's unique ID number, specify ahk_id %VarContainingID%. The search can be narrowed by specifying multiple criteria. For example: My File.txt ahk_class Notepad</param>
        /// <param name="WinText">If present, this parameter must be a substring from a single text element of the target window (as revealed by the included Window Spy utility). Hidden text elements are detected if DetectHiddenText is ON.</param>
        /// <param name="ExcludeTitle">Windows whose titles include this value will not be considered.</param>
        /// <param name="ExcludeText">Windows whose text include this value will not be considered.</param>
        public static void ControlGetText(out string OutputVar, string Control, string WinTitle, string WinText, string ExcludeTitle, string ExcludeText)
        {
            OutputVar = null;
        }

        /// <summary>
        /// Moves or resizes a control.
        /// </summary>
        /// <param name="Control">
        /// <para>Can be either ClassNN (the classname and instance number of the control) or the name/text of the control, both of which can be determined via Window Spy. When using name/text, the matching behavior is determined by SetTitleMatchMode. If this parameter is blank, the target window's topmost control will be used.</para>
        /// <para>To operate upon a control's HWND (window handle), leave the Control parameter blank and specify ahk_id %ControlHwnd% for the WinTitle parameter (this also works on hidden controls even when DetectHiddenWindows is Off) . The HWND of a control is typically retrieved via ControlGet Hwnd, MouseGetPos, or DllCall.</para>
        /// </param>
        /// <param name="X">The X and Y coordinates (in pixels) of the upper left corner of Control's new location, which can be expressions. If either coordinate is blank, Control's position in that dimension will not be changed. The coordinates are relative to the upper-left corner of the Control's parent window; ControlGetPos or Window Spy can be used to determine them.</param>
        /// <param name="Y">See <paramref name="Y"/>.</param>
        /// <param name="Width">The new width and height of Control (in pixels), which can be expressions. If either parameter is blank or omitted, Control's size in that dimension will not be changed.</param>
        /// <param name="Height">See <paramref name="Width"/>.</param>
        /// <param name="WinTitle">The title or partial title of the target window (the matching behavior is determined by SetTitleMatchMode). If this and the next 3 parameters are omitted, the Last Found Window will be used. If this is the letter A and the next 3 parameters are omitted, the active window will be used. To use a window class, specify ahk_class ExactClassName (shown by Window Spy). To use a process identifier (PID), specify ahk_pid %VarContainingPID%. To use a window group, specify ahk_group GroupName. To use a window's unique ID number, specify ahk_id %VarContainingID%. The search can be narrowed by specifying multiple criteria. For example: My File.txt ahk_class Notepad</param>
        /// <param name="WinText">If present, this parameter must be a substring from a single text element of the target window (as revealed by the included Window Spy utility). Hidden text elements are detected if DetectHiddenText is ON.</param>
        /// <param name="ExcludeTitle">Windows whose titles include this value will not be considered.</param>
        /// <param name="ExcludeText">Windows whose text include this value will not be considered.</param>
        public static void ControlMove(string Control, int X, int Y, int Width, int Height, string WinTitle, string WinText, string ExcludeTitle, string ExcludeText)
        {

        }

        /// <summary>
        /// Sends simulated keystrokes to a window or control.
        /// </summary>
        /// <param name="Control">
        /// <para>Can be either ClassNN (the classname and instance number of the control) or the name/text of the control, both of which can be determined via Window Spy. When using name/text, the matching behavior is determined by SetTitleMatchMode. If this parameter is blank or omitted, the target window's topmost control will be used. If this parameter is ahk_parent, the keystrokes will be sent directly to the control's parent window (see Automating Winamp for an example).</para>
        /// <para>To operate upon a control's HWND (window handle), leave the Control parameter blank and specify ahk_id %ControlHwnd% for the WinTitle parameter (this also works on hidden controls even when DetectHiddenWindows is Off) . The HWND of a control is typically retrieved via ControlGet Hwnd, MouseGetPos, or DllCall.</para>
        /// </param>
        /// <param name="Keys">
        /// <para>The sequence of keys to send (see the Send command for details). To send a literal comma, escape it (`,). The rate at which characters are sent is determined by SetKeyDelay.</para>
        /// <para>Unlike the Send command, mouse clicks cannot be sent by ControlSend. Use ControlClick for that.</para>
        /// </param>
        /// <param name="WinTitle">The title or partial title of the target window (the matching behavior is determined by SetTitleMatchMode). If this and the next 3 parameters are omitted, the Last Found Window will be used. If this is the letter A and the next 3 parameters are omitted, the active window will be used. To use a window class, specify ahk_class ExactClassName (shown by Window Spy). To use a process identifier (PID), specify ahk_pid %VarContainingPID%. To use a window group, specify ahk_group GroupName. To use a window's unique ID number, specify ahk_id %VarContainingID%. The search can be narrowed by specifying multiple criteria. For example: My File.txt ahk_class Notepad</param>
        /// <param name="WinText">If present, this parameter must be a substring from a single text element of the target window (as revealed by the included Window Spy utility). Hidden text elements are detected if DetectHiddenText is ON.</param>
        /// <param name="ExcludeTitle">Windows whose titles include this value will not be considered.</param>
        /// <param name="ExcludeText">Windows whose text include this value will not be considered.</param>
        public static void ControlSend(string Control, string Keys, string WinTitle, string WinText, string ExcludeTitle, string ExcludeText)
        {

        }

        /// <summary>
        /// Changes the text of a control.
        /// </summary>
        /// <param name="Control">
        /// <para>Can be either ClassNN (the classname and instance number of the control) or the name/text of the control, both of which can be determined via Window Spy. When using name/text, the matching behavior is determined by SetTitleMatchMode. If this parameter is blank, the target window's topmost control will be used.</para>
        /// <para>To operate upon a control's HWND (window handle), leave the Control parameter blank and specify ahk_id %ControlHwnd% for the WinTitle parameter (this also works on hidden controls even when DetectHiddenWindows is Off) . The HWND of a control is typically retrieved via ControlGet Hwnd, MouseGetPos, or DllCall.</para>
        /// </param>
        /// <param name="NewText">The new text to set into the control. If blank or omitted, the control is made blank.</param>
        /// <param name="WinTitle">The title or partial title of the target window (the matching behavior is determined by SetTitleMatchMode). If this and the next 3 parameters are omitted, the Last Found Window will be used. If this is the letter A and the next 3 parameters are omitted, the active window will be used. To use a window class, specify ahk_class ExactClassName (shown by Window Spy). To use a process identifier (PID), specify ahk_pid %VarContainingPID%. To use a window group, specify ahk_group GroupName. To use a window's unique ID number, specify ahk_id %VarContainingID%. The search can be narrowed by specifying multiple criteria. For example: My File.txt ahk_class Notepad</param>
        /// <param name="WinText">If present, this parameter must be a substring from a single text element of the target window (as revealed by the included Window Spy utility). Hidden text elements are detected if DetectHiddenText is ON.</param>
        /// <param name="ExcludeTitle">Windows whose titles include this value will not be considered.</param>
        /// <param name="ExcludeText">Windows whose text include this value will not be considered.</param>
        public static void ControlSetText(string Control, string NewText, string WinTitle, string WinText, string ExcludeTitle, string ExcludeText)
        {

        }

        /// <summary>
        /// Activates the next window in a window group that was defined with <see cref="GroupAdd"/>.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="mode"></param>
        public static void GroupActivate(string name, string mode)
        {
            name = (name ?? string.Empty).ToLowerInvariant();

            if(name == null || !windowGroups.ContainsKey(name) || windowGroups[name].Count == 0)
                return;

            Window.WindowItemBase next = null;

            if (mode.Equals(Keyword_R, StringComparison.OrdinalIgnoreCase))
                next = windowGroups[name].Peek();
            else if (windowGroups[name].Count != 0)
                next = windowGroups[name].ToArray()[windowGroups[name].Count - 1];

            if (next != null)
                next.Active = true;
        }

        /// <summary>
        /// Adds a window specification to a window group, creating the group if necessary.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="winTitle"></param>
        /// <param name="winText"></param>
        /// <param name="label"></param>
        /// <param name="excludeTitle"></param>
        /// <param name="excludeText"></param>
        public static void GroupAdd(string name, string winTitle = null, string winText = null, string label = null, string excludeTitle = null, string excludeText = null)
        {
            var win = windowManager.FindWindow(winTitle, winText, excludeTitle, excludeText);

            if (string.IsNullOrEmpty(name) || win == null)
                return;

            name = name.ToLowerInvariant();

            if (!windowGroups.ContainsKey(name))
                windowGroups.Add(name, new Stack<Window.WindowItemBase>());

            windowGroups[name].Push(win);
        }

        /// <summary>
        /// Closes the active window if it was just activated by <see cref="GroupActivate"/> or <see cref="GroupDeactivate"/>.
        /// It then activates the next window in the series. It can also close all windows in a group.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="mode"></param>
        public static void GroupClose(string name, string mode = null)
        {
            name = (name ?? string.Empty).ToLowerInvariant();

            if (name == null || !windowGroups.ContainsKey(name) || windowGroups[name].Count == 0)
                return;

            switch (mode.ToLowerInvariant())
            {
                case Keyword_A:
                    while (windowGroups[name].Count != 0)
                        windowGroups[name].Pop().Close();
                    windowGroups.Remove(name);
                    break;

                case Keyword_R:
                    windowGroups[name].Pop().Close();
                    windowGroups[name].Peek().Active = true;
                    break;

                case "":
                    windowGroups[name].Pop().Close();
                    if (windowGroups[name].Count != 0)
                        windowGroups[name].ToArray()[windowGroups[name].Count - 1].Active = true;
                    break;
            }
        }

        /// <summary>
        /// Similar to <see cref="GroupActivate"/> except activates the next window not in the group.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="mode"></param>
        public static void GroupDeactivate(string name, string mode)
        {
            // TODO: EnumWindows for GroupDeactivate
        }

        /// <summary>
        /// Sends a message to a window or control (SendMessage additionally waits for acknowledgement).
        /// </summary>
        /// <param name="Msg">The message number to send. See the message list to determine the number.</param>
        /// <param name="wParam">The first component of the message. If blank or omitted, 0 will be sent.</param>
        /// <param name="lParam">
        /// <para>If this parameter is blank or omitted, the message will be sent directly to the target window rather than one of its controls. Otherwise, this parameter can be either ClassNN (the classname and instance number of the control) or the name/text of the control, both of which can be determined via Window Spy. When using name/text, the matching behavior is determined by SetTitleMatchMode.</para>
        /// <para>To operate upon a control's HWND (window handle), leave the Control parameter blank and specify ahk_id %ControlHwnd% for the WinTitle parameter (this also works on hidden controls even when DetectHiddenWindows is Off). The HWND of a control is typically retrieved via ControlGet Hwnd, MouseGetPos, or DllCall.</para>
        /// </param>
        /// <param name="Control">The title or partial title of the target window (the matching behavior is determined by SetTitleMatchMode). If this and the next 3 parameters are omitted, the Last Found Window will be used. If this is the letter A and the next 3 parameters are omitted, the active window will be used. To use a window class, specify ahk_class ExactClassName (shown by Window Spy). To use a process identifier (PID), specify ahk_pid %VarContainingPID%. To use a window group, specify ahk_group GroupName. To use a window's unique ID number, specify ahk_id %VarContainingID% (also accepts a control's HWND). The search can be narrowed by specifying multiple criteria. For example: My File.txt ahk_class Notepad</param>
        /// <param name="WinTitle">If present, this parameter must be a substring from a single text element of the target window (as revealed by the included Window Spy utility). Hidden text elements are detected if DetectHiddenText is ON.</param>
        /// <param name="WinText">Windows whose titles include this value will not be considered.</param>
        /// <param name="ExcludeTitle">Windows whose text include this value will not be considered.</param>
        /// <param name="ExcludeText"></param>
        public static void PostMessage(int Msg, int wParam, int lParam, string Control, string WinTitle, string WinText, string ExcludeTitle, string ExcludeText)
        {

        }

        /// <summary>
        /// Sends a message to a window or control (SendMessage additionally waits for acknowledgement).
        /// </summary>
        /// <param name="Msg">The message number to send. See the message list to determine the number.</param>
        /// <param name="wParam">The first component of the message. If blank or omitted, 0 will be sent.</param>
        /// <param name="lParam">The second component of the message. If blank or omitted, 0 will be sent.</param>
        /// <param name="Control">
        /// <para>If this parameter is blank or omitted, the message will be sent directly to the target window rather than one of its controls. Otherwise, this parameter can be either ClassNN (the classname and instance number of the control) or the name/text of the control, both of which can be determined via Window Spy. When using name/text, the matching behavior is determined by SetTitleMatchMode.</para>
        /// <para>To operate upon a control's HWND (window handle), leave the Control parameter blank and specify ahk_id %ControlHwnd% for the WinTitle parameter (this also works on hidden controls even when DetectHiddenWindows is Off). The HWND of a control is typically retrieved via ControlGet Hwnd, MouseGetPos, or DllCall.</para>
        /// </param>
        /// <param name="WinTitle">The title or partial title of the target window (the matching behavior is determined by SetTitleMatchMode). If this and the next 3 parameters are omitted, the Last Found Window will be used. If this is the letter A and the next 3 parameters are omitted, the active window will be used. To use a window class, specify ahk_class ExactClassName (shown by Window Spy). To use a process identifier (PID), specify ahk_pid %VarContainingPID%. To use a window group, specify ahk_group GroupName. To use a window's unique ID number, specify ahk_id %VarContainingID% (also accepts a control's HWND). The search can be narrowed by specifying multiple criteria. For example: My File.txt ahk_class Notepad</param>
        /// <param name="WinText">If present, this parameter must be a substring from a single text element of the target window (as revealed by the included Window Spy utility). Hidden text elements are detected if DetectHiddenText is ON.</param>
        /// <param name="ExcludeTitle">Windows whose titles include this value will not be considered.</param>
        /// <param name="ExcludeText">Windows whose text include this value will not be considered.</param>
        public static void SendMessage(int Msg, int wParam, int lParam, string Control, string WinTitle, string WinText, string ExcludeTitle, string ExcludeText)
        {

        }

        /// <summary>
        /// Creates a customizable text popup window.
        /// </summary>
        public static void SplashTextOff()
        {

        }

        /// <summary>
        /// Creates a customizable text popup window.
        /// </summary>
        /// <param name="Width">The width in pixels of the Window. Default 200.</param>
        /// <param name="Height">The height in pixels of the window (not including its title bar if the script's file extension isn't .aut). Default 0 (i.e. just the title bar will be shown). This parameter can be an expression.</param>
        /// <param name="Title">The title of the window. Default empty (blank).</param>
        /// <param name="Text">The text of the window. Default empty (blank). If Text is long, it can be broken up into several shorter lines by means of a continuation section, which might improve readability and maintainability.</param>
        public static void SplashTextOn(string Width, string Height, string Title, string Text)
        {

        }

        /// <summary>
        /// Retrieves the text from a standard status bar control.
        /// </summary>
        /// <param name="OutputVar">The name of the variable in which to store the retrieved text.</param>
        /// <param name="Part">Which part number of the bar to retrieve. Default 1, which is usually the part that contains the text of interest.</param>
        /// <param name="WinTitle">The title or partial title of the target window (the matching behavior is determined by SetTitleMatchMode). If this and the other 3 window parameters are blank or omitted, the Last Found Window will be used. If this is the letter A and the other 3 window parameters are blank or omitted, the active window will be used. To use a window class, specify ahk_class ExactClassName (shown by Window Spy). To use a process identifier (PID), specify ahk_pid %VarContainingPID%. To use a window group, specify ahk_group GroupName. To use a window's unique ID number, specify ahk_id %VarContainingID%. The search can be narrowed by specifying multiple criteria. For example: My File.txt ahk_class Notepad</param>
        /// <param name="WinText">If present, this parameter must be a substring from a single text element of the target window (as revealed by the included Window Spy utility). Hidden text elements are detected if DetectHiddenText is ON.</param>
        /// <param name="ExcludeTitle">Windows whose titles include this value will not be considered.</param>
        /// <param name="ExcludeText">Windows whose text include this value will not be considered.</param>
        public static void StatusBarGetText(out string OutputVar, string Part, string WinTitle, string WinText, string ExcludeTitle, string ExcludeText)
        {
            OutputVar = null;
        }

        /// <summary>
        /// Waits until a window's status bar contains the specified string.
        /// </summary>
        /// <param name="BarText">
        /// <para>The text or partial text for the which the command will wait to appear. Default is blank (empty), which means to wait for the status bar to become blank. The text is case sensitive and the matching behavior is determined by SetTitleMatchMode, similar to WinTitle below.</para>
        /// <para>To instead wait for the bar's text to change, either use StatusBarGetText in a loop, or use the RegEx example at the bottom of this page.</para>
        /// </param>
        /// <param name="Seconds">The number of seconds (can contain a decimal point) to wait before timing out, in which case ErrorLevel will be set to 1. Default is blank, which means wait indefinitely. Specifying 0 is the same as specifying 0.5.</param>
        /// <param name="Part">Which part number of the bar to retrieve. Default 1, which is usually the part that contains the text of interest.</param>
        /// <param name="WinTitle">The title or partial title of the target window (the matching behavior is determined by SetTitleMatchMode). If this and the other 3 window parameters are blank or omitted, the Last Found Window will be used. If this is the letter A and the other 3 window parameters are blank or omitted, the active window will be used. To use a window class, specify ahk_class ExactClassName (shown by Window Spy). To use a process identifier (PID), specify ahk_pid %VarContainingPID%. To use a window group, specify ahk_group GroupName. To use a window's unique ID number, specify ahk_id %VarContainingID%. The search can be narrowed by specifying multiple criteria. For example: My File.txt ahk_class Notepad</param>
        /// <param name="WinText">If present, this parameter must be a substring from a single text element of the target window (as revealed by the included Window Spy utility). Hidden text elements are detected if DetectHiddenText is ON.</param>
        /// <param name="Interval">How often the status bar should be checked while the command is waiting (in milliseconds), which can be an expression. Default is 50.</param>
        /// <param name="ExcludeTitle">Windows whose titles include this value will not be considered.</param>
        /// <param name="ExcludeText">Windows whose text include this value will not be considered.</param>
        public static void StatusBarWait(string BarText, string Seconds, string Part, string WinTitle, string WinText, string Interval, string ExcludeTitle, string ExcludeText)
        {

        }

        /// <summary>
        /// Activates the specified window (makes it foremost).
        /// </summary>
        /// <param name="title"></param>
        /// <param name="text"></param>
        /// <param name="excludeTitle"></param>
        /// <param name="excludeText"></param>
        public static void WinActivate(string title = null, string text = null, string excludeTitle = null, string excludeText = null)
        {
            var win = windowManager.FindWindow(title, text, excludeTitle, excludeText);
            if(win != null)
                win.Active = true;
        }

        /// <summary>
        /// Activates the bottommost (least recently active) matching window rather than the topmost.
        /// </summary>
        /// <param name="title"></param>
        /// <param name="text"></param>
        /// <param name="excludeTitle"></param>
        /// <param name="excludeText"></param>
        public static void WinActivateBottom(string title = null, string text = null, string excludeTitle = null, string excludeText = null)
        {
            var criteria = Window.SearchCriteria.FromString(title, text, excludeTitle, excludeText);

            var window = windowManager.FindWindow(criteria);
            
            while(window != null && window.IsSpecified){
                if(window.Equals(criteria)) {
                    window.Active = true;
                    break;
                }
                window = window.PreviousWindow;
            }
        }

        /// <summary>
        /// Returns the Unique ID (HWND) of the active window if it matches the specified criteria.
        /// </summary>
        /// <param name="title"></param>
        /// <param name="text"></param>
        /// <param name="excludeTitle"></param>
        /// <param name="excludeText"></param>
        /// <returns></returns>
        public static long WinActive(string title = null, string text = null, string excludeTitle = null, string excludeText = null)
        {
            long id = 0;
            var criteria = Window.SearchCriteria.FromString(title, text, excludeTitle, excludeText);
            var window = windowManager.ActiveWindow;

            if(window.Equals(criteria)) {
                id = window.Handle.ToInt64();
            }

            return id;
        }

        /// <summary>
        /// Closes the specified window.
        /// </summary>
        /// <param name="title"></param>
        /// <param name="text"></param>
        /// <param name="excludeTitle"></param>
        /// <param name="excludeText"></param>
        public static void WinClose(string title = null, string text = null, string excludeTitle = null, string excludeText = null)
        {
            var win = windowManager.FindWindow(title, text, excludeTitle, excludeText);
            if(win != null)
                win.Close();
        }

        /// <summary>
        /// Returns the Unique ID (HWND) of the first matching window (0 if none) as a hexademinal integer.
        /// </summary>
        /// <param name="title"></param>
        /// <param name="text"></param>
        /// <param name="excludeTitle"></param>
        /// <param name="excludeText"></param>
        /// <returns></returns>
        public static long WinExist(string title = null, string text = null, string excludeTitle = null, string excludeText = null)
        {
            if (title == string.Empty)
            {
                if (LastFoundForm != 0)
                    return LastFoundForm;
            }
            var win = windowManager.FindWindow(title, text, excludeTitle, excludeText);
            if(win != null)
                return win.Handle.ToInt64();
            else
                return 0;
        }

        /// <summary>
        /// Retrieves the specified window's unique ID, process ID, process name, or a list of its controls.
        /// </summary>
        /// <param name="result"></param>
        /// <param name="command"></param>
        /// <param name="title"></param>
        /// <param name="text"></param>
        /// <param name="excludeTitle"></param>
        /// <param name="excludeText"></param>
        public static void WinGet(out string result, string command, string title = null, string text = null, string excludeTitle = null, string excludeText = null)
        {
            result = null;

            // TODO: WinGet
        }

        /// <summary>
        /// Combines the functions of <see cref="WinGetActiveTitle"/> and <see cref="WinGetPos"/> into one command.
        /// </summary>
        /// <param name="title"></param>
        /// <param name="width"></param>
        /// <param name="height"></param>
        /// <param name="x"></param>
        /// <param name="y"></param>
        public static void WinGetActiveStats(out string title, out int width, out int height, out int x, out int y)
        {
            var active = windowManager.ActiveWindow;

            if (!active.IsSpecified){
                title = string.Empty;
                width = height = x = y = 0;
                return;
            }
            title = active.Title;
            width = active.Size.Width;
            height = active.Size.Height;
            x = active.Location.X;
            y = active.Location.Y;
        }

        /// <summary>
        /// Retrieves the title of the active window.
        /// </summary>
        /// <param name="result"></param>
        public static void WinGetActiveTitle(out string result)
        {
            result = windowManager.ActiveWindow.Title;
        }

        /// <summary>
        /// Retrieves the specified window's class name.
        /// </summary>
        /// <param name="result"></param>
        /// <param name="title"></param>
        /// <param name="text"></param>
        /// <param name="excludeTitle"></param>
        /// <param name="excludeText"></param>
        public static void WinGetClass(out string result, string title = null, string text = null, string excludeTitle = null, string excludeText = null)
        {
            result = "";

            var win = windowManager.FindWindow(title, text, excludeTitle, excludeText);
            if(win != null)
                result = win.ClassName;
        }

        /// <summary>
        /// Retrieves the position and size of the specified window.
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="width"></param>
        /// <param name="height"></param>
        /// <param name="title"></param>
        /// <param name="text"></param>
        /// <param name="excludeTitle"></param>
        /// <param name="excludeText"></param>
        public static void WinGetPos(out int x, out int y, out int width, out int height, string title = null, string text = null, string excludeTitle = null, string excludeText = null)
        {
            var win = windowManager.FindWindow(title, text, excludeTitle, excludeText);
            if(win != null) {
                x = win.Location.X;
                y = win.Location.Y;
                width = win.Size.Width;
                height = win.Size.Height;
            } else {
                x = 0;
                y = 0;
                width = 0;
                height = 0;
            }
        }

        /// <summary>
        /// Retrieves the text from the specified window.
        /// </summary>
        /// <param name="result"></param>
        /// <param name="title"></param>
        /// <param name="text"></param>
        /// <param name="excludeTitle"></param>
        /// <param name="excludeText"></param>
        public static void WinGetText(out string result, string title = null, string text = null, string excludeTitle = null, string excludeText = null)
        {
            result = "";
            var win = windowManager.FindWindow(title, text, excludeTitle, excludeText);
            if(win != null)
                result = string.Join(Keyword_Linefeed, win.Text);
        }

        /// <summary>
        /// Retrieves the title of the specified window.
        /// </summary>
        /// <param name="result"></param>
        /// <param name="title"></param>
        /// <param name="text"></param>
        /// <param name="excludeTitle"></param>
        /// <param name="excludeText"></param>
        public static void WinGetTitle(out string result, string title = null, string text = null, string excludeTitle = null, string excludeText = null)
        {
            result = "";
            var win = windowManager.FindWindow(title, text, excludeTitle, excludeText);
            if(win != null)
                result = win.Title;
        }

        /// <summary>
        /// Hides the specified window.
        /// </summary>
        /// <param name="title"></param>
        /// <param name="text"></param>
        /// <param name="excludeTitle"></param>
        /// <param name="excludeText"></param>
        public static void WinHide(string title = null, string text = null, string excludeTitle = null, string excludeText = null)
        {
            var win = windowManager.FindWindow(title, text, excludeTitle, excludeText);
            if(win != null)
                win.Hide();
        }

        /// <summary>
        /// Forces the specified window to close.
        /// </summary>
        /// <param name="title"></param>
        /// <param name="text"></param>
        /// <param name="excludeTitle"></param>
        /// <param name="excludeText"></param>
        public static void WinKill(string title = null, string text = null, string excludeTitle = null, string excludeText = null)
        {
            var win = windowManager.FindWindow(title, text, excludeTitle, excludeText);
            if(win != null)
                win.Kill();
        }

        /// <summary>
        /// Enlarges the specified window to its maximum size.
        /// </summary>
        /// <param name="title"></param>
        /// <param name="text"></param>
        /// <param name="excludeTitle"></param>
        /// <param name="excludeText"></param>
        public static void WinMaximize(string title = null, string text = null, string excludeTitle = null, string excludeText = null)
        {
            var win = windowManager.FindWindow(title, text, excludeTitle, excludeText);
            if(win != null)
                win.WindowState = FormWindowState.Maximized;
        }

        /// <summary>
        /// Invokes a menu item from the menu bar of the specified window.
        /// </summary>
        /// <param name="title"></param>
        /// <param name="text"></param>
        /// <param name="menu"></param>
        /// <param name="subMenu1"></param>
        /// <param name="subMenu2"></param>
        /// <param name="subMenu3"></param>
        /// <param name="subMenu4"></param>
        /// <param name="subMenu5"></param>
        /// <param name="subMenu6"></param>
        /// <param name="excludeTitle"></param>
        /// <param name="excludeText"></param>
        public static void WinMenuSelectItem(string title = null, string text = null, string menu = null, string subMenu1 = null, string subMenu2 = null, string subMenu3 = null, string subMenu4 = null, string subMenu5 = null, string subMenu6 = null, string excludeTitle = null, string excludeText = null)
        {
            ErrorLevel = 1;
            var win = windowManager.FindWindow(title, text, excludeTitle, excludeText);
            if(win != null)
                ErrorLevel = win.SelectMenuItem(menu, subMenu1, subMenu2, subMenu3, subMenu4, subMenu5, subMenu6) ? 1 : 0;
        }

        /// <summary>
        /// Collapses the specified window into a button on the task bar.
        /// </summary>
        /// <param name="title"></param>
        /// <param name="text"></param>
        /// <param name="excludeTitle"></param>
        /// <param name="excludeText"></param>
        public static void WinMinimize(string title = null, string text = null, string excludeTitle = null, string excludeText = null)
        {
            var win = windowManager.FindWindow(title, text, excludeTitle, excludeText);
            if(win != null)
                win.WindowState = FormWindowState.Normal;
        }

        /// <summary>
        /// Minimizes all windows.
        /// </summary>
        public static void WinMinimizeAll()
        {
            windowManager.MinimizeAll();
        }

        /// <summary>
        /// Unminimizes all windows.
        /// </summary>
        public static void WinMinimizeAllUndo()
        {
            windowManager.MinimizeAllUndo();
        }

        /// <summary>
        /// Changes the position and/or size of the specified window.
        /// </summary>
        /// <param name="title"></param>
        /// <param name="text"></param>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="width"></param>
        /// <param name="height"></param>
        /// <param name="excludeTitle"></param>
        /// <param name="excludeText"></param>
        public static void WinMove(string title = null, string text = null, int x = -1, int y = -1, int width = -1, int height = -1, string excludeTitle = null, string excludeText = null)
        {
            var win = windowManager.FindWindow(title, text, excludeTitle, excludeText);
            if(win == null)
                return;

            var location = win.Location;

            if (x != -1)
                location.X = x;

            if (y != -1)
                location.Y = y;

            win.Location = location;

            var size = win.Size;

            if (width != -1)
                size.Width = width;

            if (height != -1)
                size.Height = height;

            win.Size = size;
        }

        /// <summary>
        /// Unminimizes or unmaximizes the specified window if it is minimized or maximized.
        /// </summary>
        /// <param name="title"></param>
        /// <param name="text"></param>
        /// <param name="excludeTitle"></param>
        /// <param name="excludeText"></param>
        public static void WinRestore(string title = null, string text = null, string excludeTitle = null, string excludeText = null)
        {
            var win = windowManager.FindWindow(title, text, excludeTitle, excludeText);
            if(win != null)
                win.WindowState = FormWindowState.Normal;
        }

        /// <summary>
        /// Makes a variety of changes to the specified window, such as "always on top" and transparency.
        /// </summary>
        /// <param name="attribute"></param>
        /// <param name="value"></param>
        /// <param name="title"></param>
        /// <param name="text"></param>
        /// <param name="excludeTitle"></param>
        /// <param name="excludeText"></param>
        public static void WinSet(string attribute, string value = null, string title = null, string text = null, string excludeTitle = null, string excludeText = null)
        {
            var win = windowManager.FindWindow(title, text, excludeTitle, excludeText);
            if(win != null) {

                // TODO: winset

            }
        }

        /// <summary>
        /// Changes the title of the specified window.
        /// </summary>
        /// <param name="title"></param>
        /// <param name="text"></param>
        /// <param name="newTitle"></param>
        /// <param name="excludeTitle"></param>
        /// <param name="excludeText"></param>
        public static void WinSetTitle(string title = null, string text = null, string newTitle = null, string excludeTitle = null, string excludeText = null)
        {
            var win = windowManager.FindWindow(title, text, excludeTitle, excludeText);
            if(win != null) 
                win.Title = newTitle;
        }

        /// <summary>
        /// Unhides the specified window.
        /// </summary>
        /// <param name="title"></param>
        /// <param name="text"></param>
        /// <param name="excludeTitle"></param>
        /// <param name="excludeText"></param>
        public static void WinShow(string title = null, string text = null, string excludeTitle = null, string excludeText = null)
        {
            var win = windowManager.FindWindow(title, text, excludeTitle, excludeText);
            if(win != null) 
                win.Show();
        }

        /// <summary>
        /// Waits until the specified window exists.
        /// </summary>
        /// <param name="title"></param>
        /// <param name="text"></param>
        /// <param name="seconds"></param>
        /// <param name="excludeTitle"></param>
        /// <param name="excludeText"></param>
        public static void WinWait(string title = null, string text = null, int seconds = -1, string excludeTitle = null, string excludeText = null)
        {
            var win = windowManager.FindWindow(title, text, excludeTitle, excludeText);
            if(win != null)
                ErrorLevel = win.Wait(seconds == -1 ? seconds : seconds * 1000) ? 0 : 1;
            else
                ErrorLevel = 1;
        }

        /// <summary>
        /// Waits until the specified window is active.
        /// </summary>
        /// <param name="title"></param>
        /// <param name="text"></param>
        /// <param name="seconds"></param>
        /// <param name="excludeTitle"></param>
        /// <param name="excludeText"></param>
        public static void WinWaitActive(string title = null, string text = null, int seconds = -1, string excludeTitle = null, string excludeText = null)
        {
            var win = windowManager.FindWindow(title, text, excludeTitle, excludeText);
            if(win != null)
                ErrorLevel = win.WaitActive(seconds == -1 ? seconds : seconds * 1000) ? 0 : 1;
            else
                ErrorLevel = 1;
        }

        /// <summary>
        /// Waits until the specified window does not exist.
        /// </summary>
        /// <param name="title"></param>
        /// <param name="text"></param>
        /// <param name="seconds"></param>
        /// <param name="excludeTitle"></param>
        /// <param name="excludeText"></param>
        public static void WinWaitClose(string title = null, string text = null, int seconds = -1, string excludeTitle = null, string excludeText = null)
        {
            var win = windowManager.FindWindow(title, text, excludeTitle, excludeText);
            if(win != null)
                ErrorLevel = win.WaitClose(seconds == -1 ? seconds : seconds * 1000) ? 0 : 1;
            else
                ErrorLevel = 1;
        }

        /// <summary>
        /// Waits until the specified window is not active.
        /// </summary>
        /// <param name="title"></param>
        /// <param name="text"></param>
        /// <param name="seconds"></param>
        /// <param name="excludeTitle"></param>
        /// <param name="excludeText"></param>
        public static void WinWaitNotActive(string title = null, string text = null, int seconds = -1, string excludeTitle = null, string excludeText = null)
        {
            var win = windowManager.FindWindow(title, text, excludeTitle, excludeText);
            if(win != null)
                ErrorLevel = win.WaitNotActive(seconds == -1 ? seconds : seconds * 1000) ? 0 : 1;
            else
                ErrorLevel = 1;
        }
    }
}
