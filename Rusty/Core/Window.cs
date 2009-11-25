using System;
using System.Diagnostics;

namespace IronAHK.Rusty
{
    partial class Core
    {
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
            if (Environment.OSVersion.Platform != PlatformID.Win32NT)
                return;

            IntPtr hwnd = Settings.Windows.FindWindow(WinTitle, WinText, ExcludeTitle, ExcludeText, Control);
            Windows.Windows.SetFocus(hwnd);
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
            if (Environment.OSVersion.Platform != PlatformID.Win32NT)
                throw new Win32Required();

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
            if (Environment.OSVersion.Platform != PlatformID.Win32NT)
                throw new Win32Required();

            //OutputVarWinTitle = WindowCmd.GetClassName(WindowCmd.GetFocus());

            IntPtr hwnd = Settings.Windows.FindWindow(WinTitle, WinText, ExcludeTitle, ExcludeText);
            uint thread = 0;
            Windows.Windows.GetWindowThreadProcessId(hwnd, out thread);

            Windows.Windows.GUITHREADINFO info;
            Windows.Windows.GetGUIThreadInfo(thread, out info);

            OutputVar = Windows.Windows.GetClassName(info.hwndActive);
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
            if (Environment.OSVersion.Platform != PlatformID.Win32NT)
                throw new Win32Required();

            IntPtr hwnd = Settings.Windows.FindWindow(WinTitle, WinText, ExcludeTitle, ExcludeText, Control);
            Windows.Windows.RECT pos;
            Windows.Windows.GetWindowRect(hwnd, out pos);
            X = pos.Top;
            Y = pos.Left;
            Width = pos.Right - pos.Left;
            Height = pos.Bottom - pos.Top;
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
            if (Environment.OSVersion.Platform != PlatformID.Win32NT)
                throw new Win32Required();

            IntPtr hwnd = Settings.Windows.FindWindow(WinTitle, WinText, ExcludeTitle, ExcludeText, Control);
            OutputVar = Windows.Windows.GetWindowText(hwnd);
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
            if (Environment.OSVersion.Platform != PlatformID.Win32NT)
                throw new Win32Required();

            IntPtr hwnd = Settings.Windows.FindWindow(WinTitle, WinText, ExcludeTitle, ExcludeText, Control);
            Windows.Windows.MoveWindow(hwnd, X, Y, Width, Height, true);
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
            if (Environment.OSVersion.Platform != PlatformID.Win32NT)
                throw new Win32Required();

            IntPtr hwnd = Settings.Windows.FindWindow(WinTitle, WinText, ExcludeTitle, ExcludeText, Control);
            Windows.Windows.SetWindowText(hwnd, NewText);
        }

        /// <summary>
        /// Determines whether invisible text in a window is "seen" for the purpose of finding the window. This affects commands such as IfWinExist and WinActivate.
        /// </summary>
        /// <param name="Mode">
        /// <para>On: This is the default for non-AutoIt v2 scripts (e.g. .ahk, .ini). Hidden text will be detected.</para>
        /// <para>Off: This is the default for AutoIt v2 scripts. Hidden text is not detected.</para>
        /// </param>
        [Obsolete, Conditional("LEGACY")]
        public static void DetectHiddenText(string Mode)
        {
            Formats.OnOff(ref Settings.DetectHiddenText, Mode);
        }

        /// <summary>
        /// Determines whether invisible windows are "seen" by the script.
        /// </summary>
        /// <param name="Mode">
        /// <para>On: Hidden windows are detected. </para>
        /// <para>Off: This is the default. Hidden windows are not detected, except by the WinShow command.</para>
        /// </param>
        [Obsolete, Conditional("LEGACY")]
        public static void DetectHiddenWindows(string Mode)
        {
            Formats.OnOff(ref Settings.DetectHiddenWindows, Mode);
        }

        /// <summary>
        /// Activates the next window in a window group that was defined with GroupAdd.
        /// </summary>
        /// <param name="GroupName">The name of the group to activate, as originally defined by GroupAdd.</param>
        /// <param name="R">This determines whether the oldest or the newest window is activated whenever no members of the group are currently active. If omitted, the oldest window is always activated. If it's the letter R, the newest window (the one most recently active) is activated, but only if no members of the group are active when the command is given. "R" is useful in cases where you temporarily switch to working on an unrelated task. When you return to the group via GroupActivate, GroupDeactivate, or GroupClose, the window you were most recently working with is activated rather than the oldest window.</param>
        public static void GroupActivate(string GroupName, string R)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Adds a window specification to a window group, creating the group if necessary.
        /// </summary>
        /// <param name="GroupName">The name of the group to which to add this window specification. If the group doesn't exist, it will be created. Group names are not case sensitive.</param>
        /// <param name="WinTitle">
        /// <para>The title or partial title of the target window(s). It can be blank. Note: Although SetTitleMatchMode and DetectHiddenWindows do not directly affect the behavior of this command, they do affect the other group commands such as GroupActivate and GroupClose. They also affect the use of ahk_group in any other command's WinTitle.</para>
        /// <para>To use a window class, specify ahk_class ExactClassName (shown by Window Spy). To use a process identifier (PID), specify ahk_pid %VarContainingPID%.</para>
        /// <para>To use a window's unique ID number, specify ahk_id %VarContainingID%. To use a window group, specify ahk_group GroupName (i.e. groups may contain other groups).</para>
        /// <para>The search can be narrowed by specifying multiple criteria. For example: My File.txt ahk_class Notepad</para>
        /// </param>
        /// <param name="WinText">If present, this parameter must be a substring from a single text element of the target window (as revealed by the included Window Spy utility). Hidden text elements are detected if DetectHiddenText is ON at the time that GroupActivate, GroupDeactivate, and GroupClose are used.</param>
        /// <param name="Label">The label of a subroutine to run if no windows matching this specification exist when the GroupActivate command is used. The label is jumped to as though a Gosub had been used. Omit or leave blank for none.</param>
        /// <param name="ExcludeTitle">Windows whose titles include this value will not be considered.</param>
        /// <param name="ExcludeText">Windows whose text include this value will not be considered.</param>
        public static void GroupAdd(string GroupName, string WinTitle, string WinText, string Label, string ExcludeTitle, string ExcludeText)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Closes the active window if it was just activated by GroupActivate or GroupDeactivate. It then activates the next window in the series. It can also close all windows in a group.
        /// </summary>
        /// <param name="GroupName">The name of the group as originally defined by GroupAdd.</param>
        /// <param name="A_R">
        /// <para>If it's the letter A, all members of the group will be closed. This is the same effect as WinClose ahk_group GroupName.</para>
        /// <para>Otherwise: If the command closes the active window, it will then activate the next window in the series. This parameter determines whether the oldest or the newest window is activated. If omitted, the oldest window is always activated. If it's the letter R, the newest window (the one most recently active) is activated, but only if no members of the group are active when the command is given. "R" is useful in cases where you temporarily switch to working on an unrelated task. When you return to the group via GroupActivate, GroupDeactivate, or GroupClose, the window you were most recently working with is activated rather than the oldest window.</para>
        /// </param>
        public static void GroupClose(string GroupName, string A_R)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Similar to GroupActivate except activates the next window not in the group.
        /// </summary>
        /// <param name="GroupName">The name of the target group, as originally defined by GroupAdd.</param>
        /// <param name="R">This determines whether the oldest or the newest non-member window is activated whenever a member of the group is currently active. If omitted, the oldest non-member window is always activated. If it's the letter R, the newest non-member window (the one most recently active) is activated, but only if a member of the group is active when the command is given. "R" is useful in cases where you temporarily switch to working on an unrelated task. When you return to the group via GroupActivate, GroupDeactivate, or GroupClose, the window you were most recently working with is activated rather than the oldest window.</param>
        public static void GroupDeactivate(string GroupName, string R)
        {
            throw new NotImplementedException();
        }
        
        /// <summary>
        /// Checks if the specified window exists and is currently active (foremost).
        /// </summary>
        /// <param name="WinTitle">The title or partial title of the target window (the matching behavior is determined by SetTitleMatchMode). If this and the other 3 parameters are omitted, the Last Found Window will be used. To use a window class, specify ahk_class ExactClassName (shown by Window Spy). To use a process identifier (PID), specify ahk_pid %VarContainingPID%. To use a window group, specify ahk_group GroupName. To use a window's unique ID number, specify ahk_id %VarContainingID%. The search can be narrowed by specifying multiple criteria. For example: My File.txt ahk_class Notepad</param>
        /// <param name="WinText">If present, this parameter must be a substring from a single text element of the target window (as revealed by the included Window Spy utility). Hidden text elements are detected if DetectHiddenText is ON.</param>
        /// <param name="ExcludeTitle">Windows whose titles include this value will not be considered. Note: Due to backward compatibility with .aut scripts, this parameter will be interpreted as a command if it exactly matches the name of a command. To work around this, use the WinActive() function instead.</param>
        /// <param name="ExcludeText">Windows whose text include this value will not be considered.</param>
        /// <returns></returns>
        [Obsolete, Conditional("FLOW")]
        public static void IfWinActive(string WinTitle, string WinText, string ExcludeTitle, string ExcludeText)
        {
            throw new NotSupportedException();
        }

        /// <summary>
        /// Checks if the specified window exists.
        /// </summary>
        /// <param name="WinTitle">The title or partial title of the target window (the matching behavior is determined by SetTitleMatchMode). If this and the other 3 parameters are all omitted, the Last Found Window will be used (i.e. that specific window is checked to see whether it still exists). To use a window class, specify ahk_class ExactClassName (shown by Window Spy). To use a process identifier (PID), specify ahk_pid %VarContainingPID%. To use a window group, specify ahk_group GroupName. To use a window's unique ID number, specify ahk_id %VarContainingID%. The search can be narrowed by specifying multiple criteria. For example: My File.txt ahk_class Notepad</param>
        /// <param name="WinText">If present, this parameter must be a substring from a single text element of the target window (as revealed by the included Window Spy utility). Hidden text elements are detected if DetectHiddenText is ON.</param>
        /// <param name="ExcludeTitle">Windows whose titles include this value will not be considered. Note: Due to backward compatibility with .aut scripts, this parameter will be interpreted as a command if it exactly matches the name of a command. To work around this, use the WinExist() function instead.</param>
        /// <param name="ExcludeText">Windows whose text include this value will not be considered.</param>
        /// <returns></returns>
        [Obsolete, Conditional("FLOW")]
        public static void IfWinExist(string WinTitle, string WinText, string ExcludeTitle, string ExcludeText)
        {
            throw new NotSupportedException();
        }

        /// <summary>
        /// Checks if the specified window exists and is currently active (foremost).
        /// </summary>
        /// <param name="WinTitle">The title or partial title of the target window (the matching behavior is determined by SetTitleMatchMode). If this and the other 3 parameters are omitted, the Last Found Window will be used. To use a window class, specify ahk_class ExactClassName (shown by Window Spy). To use a process identifier (PID), specify ahk_pid %VarContainingPID%. To use a window group, specify ahk_group GroupName. To use a window's unique ID number, specify ahk_id %VarContainingID%. The search can be narrowed by specifying multiple criteria. For example: My File.txt ahk_class Notepad</param>
        /// <param name="WinText">If present, this parameter must be a substring from a single text element of the target window (as revealed by the included Window Spy utility). Hidden text elements are detected if DetectHiddenText is ON.</param>
        /// <param name="ExcludeTitle">Windows whose titles include this value will not be considered. Note: Due to backward compatibility with .aut scripts, this parameter will be interpreted as a command if it exactly matches the name of a command. To work around this, use the WinActive() function instead.</param>
        /// <param name="ExcludeText">Windows whose text include this value will not be considered.</param>
        /// <returns></returns>
        [Obsolete, Conditional("FLOW")]
        public static void IfWinNotActive(string WinTitle, string WinText, string ExcludeTitle, string ExcludeText)
        {
            throw new NotSupportedException();
        }

        /// <summary>
        /// Checks if the specified window exists.
        /// </summary>
        /// <param name="WinTitle">The title or partial title of the target window (the matching behavior is determined by SetTitleMatchMode). If this and the other 3 parameters are all omitted, the Last Found Window will be used (i.e. that specific window is checked to see whether it still exists). To use a window class, specify ahk_class ExactClassName (shown by Window Spy). To use a process identifier (PID), specify ahk_pid %VarContainingPID%. To use a window group, specify ahk_group GroupName. To use a window's unique ID number, specify ahk_id %VarContainingID%. The search can be narrowed by specifying multiple criteria. For example: My File.txt ahk_class Notepad</param>
        /// <param name="WinText">If present, this parameter must be a substring from a single text element of the target window (as revealed by the included Window Spy utility). Hidden text elements are detected if DetectHiddenText is ON.</param>
        /// <param name="ExcludeTitle">Windows whose titles include this value will not be considered. Note: Due to backward compatibility with .aut scripts, this parameter will be interpreted as a command if it exactly matches the name of a command. To work around this, use the WinExist() function instead.</param>
        /// <param name="ExcludeText">Windows whose text include this value will not be considered.</param>
        /// <returns></returns>
        [Obsolete, Conditional("FLOW")]
        public static void IfWinNotExist(string WinTitle, string WinText, string ExcludeTitle, string ExcludeText)
        {
            throw new NotSupportedException();
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
            if (Environment.OSVersion.Platform != PlatformID.Win32NT)
                return;

            IntPtr hwnd = Settings.Windows.FindWindow(WinTitle, WinText, ExcludeTitle, ExcludeText, Control);
            error = Windows.Windows.PostMessage(hwnd, (uint)Msg, new IntPtr(wParam), new IntPtr(lParam)) ? 0 : 1;
        }

        /// <summary>
        /// Perform a series of commands repeatedly: either the specified number of times or until break is encountered.
        /// </summary>
        [Obsolete, Conditional("FLOW")]
        public static void Repeat()
        {
            throw new NotSupportedException();
        }

        /// <summary>
        /// Returns from a subroutine to which execution had previously jumped via function-call, Gosub, Hotkey activation, GroupActivate, or other means.
        /// </summary>
        /// <param name="Expression">This parameter should be omitted except when return is used inside a function.</param>
        [Obsolete, Conditional("FLOW")]
        public static void Return(string Expression)
        {
            throw new NotSupportedException();
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
            if (Environment.OSVersion.Platform != PlatformID.Win32NT)
                return;

            IntPtr hwnd = Settings.Windows.FindWindow(WinTitle, WinText, ExcludeTitle, ExcludeText, Control);
            error = Windows.Windows.SendMessage(hwnd, (uint)Msg, new IntPtr(wParam), new IntPtr(lParam)).ToInt32();
        }

        /// <summary>
        /// Sets the state of the Capslock key. Can also force the key to stay on or off.
        /// </summary>
        /// <param name="Mode">
        /// <para>If this parameter is omitted, the AlwaysOn/Off attribute of the key is removed (if present). Otherwise, specify one of the following words:</para>
        /// <para>On: Turns on the key and removes the AlwaysOn/Off attribute of the key (if present).</para>
        /// <para>Off: Turns off the key and removes the AlwaysOn/Off attribute of the key (if present).</para>
        /// <para>AlwaysOn: Forces the key to stay on permanently (has no effect on Windows 9x).</para>
        /// <para>AlwaysOff: Forces the key to stay off permanently (has no effect on Windows 9x).</para>
        /// </param>
        [Obsolete, Conditional("LEGACY")]
        public static void SetCapsLockState(string Mode)
        {

        }

        /// <summary>
        /// Sets the delay that will occur after each control-modifying command.
        /// </summary>
        /// <param name="Delay">Time in milliseconds. Use -1 for no delay at all and 0 for the smallest possible delay. If unset, the default delay is 20.</param>
        [Obsolete, Conditional("LEGACY")]
        public static void SetControlDelay(int Delay)
        {
            Settings.ControlDelay = Delay;
        }

        /// <summary>
        /// Sets the matching behavior of the WinTitle parameter in commands such as WinWait.
        /// </summary>
        /// <param name="Mode">
        /// <para>One of the following digits or the word RegEx:</para>
        /// <list type="">
        /// <item>1: A window's title must start with the specified WinTitle to be a match.</item>
        /// <item>2: A window's title can contain WinTitle anywhere inside it to be a match.</item>
        /// <item>3: A window's title must exactly match WinTitle to be a match.</item>
        /// </list>
        /// <para>RegEx: Changes WinTitle, WinText, ExcludeTitle, and ExcludeText to be regular expressions. Do not enclose such expressions in quotes when using them with commands. For example: WinActivate Untitled.*Notepad. RegEx also applies to ahk_group and ahk_class; for example, ahk_class IEFrame searches for any window whose class name contains IEFrame anywhere (this is because by default, regular expressions find a match anywhere in the target string). Note: For WinText, each text element (i.e. each control's text) is matched against the RegEx separately. Therefore, it is not possible to have a match span more than one text element.</para>
        /// <para>The modes above also affect ExcludeTitle in the same way as WinTitle. For example, mode 3 requires that a window's title exactly match ExcludeTitle for that window to be excluded.</para>
        /// </param>
        [Obsolete, Conditional("LEGACY")]
        public static void SetTitleMatchMode(string Mode)
        {
            switch (Mode.ToLower())
            {
                case "1": Settings.TitleMatchMode = 1; break;
                case "2": Settings.TitleMatchMode = 2; break;
                case "3": Settings.TitleMatchMode = 3; break;
                case Keywords.RegEx: Settings.TitleMatchMode = 4; break;
                case Keywords.Fast: Settings.TitleMatchModeSpeed = true; break;
                case Keywords.Slow: Settings.TitleMatchModeSpeed = false; break;
            }
        }

        /// <summary>
        /// Sets the delay that will occur after each windowing command, such as WinActivate.
        /// </summary>
        /// <param name="Delay">Time in milliseconds. Use -1 for no delay at all and 0 for the smallest possible delay. If unset, the default delay is 100.</param>
        [Obsolete, Conditional("LEGACY")]
        public static void SetWinDelay(int Delay)
        {
            Settings.WinDelay = Delay;
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
        /// <param name="WinTitle">The title or partial title of the target window (the matching behavior is determined by SetTitleMatchMode). If all parameters are omitted, the Last Found Window will be activated. To use a window class, specify ahk_class ExactClassName (shown by Window Spy). To use a process identifier (PID), specify ahk_pid %VarContainingPID%. To use a window group, specify ahk_group GroupName. To use a window's unique ID number, specify ahk_id %VarContainingID%. The search can be narrowed by specifying multiple criteria. For example: My File.txt ahk_class Notepad</param>
        /// <param name="WinText">If present, this parameter must be a substring from a single text element of the target window (as revealed by the included Window Spy utility). Hidden text elements are detected if DetectHiddenText is ON.</param>
        /// <param name="ExcludeTitle">Windows whose titles include this value will not be considered.</param>
        /// <param name="ExcludeText">Windows whose text include this value will not be considered.</param>
        public static void WinActivate(string WinTitle, string WinText, string ExcludeTitle, string ExcludeText)
        {
            if (Environment.OSVersion.Platform != PlatformID.Win32NT)
                throw new Win32Required();

            IntPtr hwnd = Settings.Windows.FindWindow(WinTitle, WinText, ExcludeTitle, ExcludeText);
            Windows.Windows.ShowWindow(hwnd, Windows.Windows.SW_SHOWNORMAL);
            Windows.Windows.SetForegroundWindow(hwnd);
            Windows.Windows.SetActiveWindow(hwnd);
            if (Settings.WinDelay>=0)
            Core.Sleep(Settings.WinDelay);
        }

        /// <summary>
        /// Same as WinActivate except that it activates the bottommost (least recently active) matching window rather than the topmost.
        /// </summary>
        /// <param name="WinTitle">The title or partial title of the target window (the matching behavior is determined by SetTitleMatchMode). To use a window class, specify ahk_class ExactClassName (shown by Window Spy). To use a process identifier (PID), specify ahk_pid %VarContainingPID%. To use a window group, specify ahk_group GroupName. To use a window's unique ID number, specify ahk_id %VarContainingID%. The search can be narrowed by specifying multiple criteria. For example: My File.txt ahk_class Notepad</param>
        /// <param name="WinText">If present, this parameter must be a substring from a single text element of the target window (as revealed by the included Window Spy utility). Hidden text elements are detected if DetectHiddenText is ON.</param>
        /// <param name="ExcludeTitle">Windows whose titles include this value will not be considered.</param>
        /// <param name="ExcludeText">Windows whose text include this value will not be considered.</param>
        public static void WinActivateBottom(string WinTitle, string WinText, string ExcludeTitle, string ExcludeText)
        {

        }

        /// <summary>
        /// Returns the Unique ID (HWND) of the active window if it matches the specified criteria. If it does not, the function returns 0. Since all non-zero numbers are seen as "true", the statement if WinActive("WinTitle") is true whenever WinTitle is active. Finally, WinTitle supports ahk_id, ahk_class, and other special strings. See IfWinActive for details about these and other aspects of window activation.
        /// </summary>
        /// <param name="WinTitle"></param>
        /// <param name="WinText"></param>
        /// <param name="ExcludeTitle"></param>
        /// <param name="ExcludeText"></param>
        /// <returns></returns>
        public static int WinActive(string WinTitle, string WinText, string ExcludeTitle, string ExcludeText)
        {
            if (Environment.OSVersion.Platform != PlatformID.Win32NT)
                throw new Win32Required();

            IntPtr hwnd = Windows.Windows.GetActiveWindow();
            return (hwnd == Settings.Windows.FindWindow(WinTitle, WinText, ExcludeTitle, ExcludeText)
                ? hwnd : IntPtr.Zero).ToInt32();
        }

        /// <summary>
        /// Closes the specified window.
        /// </summary>
        /// <param name="WinTitle">The title or partial title of the target window (the matching behavior is determined by SetTitleMatchMode). If this and the other 3 window parameters are blank or omitted, the Last Found Window will be used. If this is the letter A and the other 3 window parameters are blank or omitted, the active window will be used. To use a window class, specify ahk_class ExactClassName (shown by Window Spy). To use a process identifier (PID), specify ahk_pid %VarContainingPID%. To close a group of windows, specify ahk_group GroupName (WinText, ExcludeTitle, and ExcludeText must be blank in this case). To use a window's unique ID number, specify ahk_id %VarContainingID%. The search can be narrowed by specifying multiple criteria. For example: My File.txt ahk_class Notepad</param>
        /// <param name="WinText">If present, this parameter must be a substring from a single text element of the target window (as revealed by the included Window Spy utility). Hidden text elements are detected if DetectHiddenText is ON.</param>
        /// <param name="SecondsToWait">If omitted or blank, the command will not wait at all. If 0, it will wait 500ms. Otherwise, it will wait the indicated number of seconds (can contain a decimal point or be an expression) for the window to close. If the window does not close within that period, the script will continue. ErrorLevel is not set by this command, so use IfWinExist or WinWaitClose if you need to determine for certain that a window is closed. While the command is in a waiting state, new threads can be launched via hotkey, custom menu item, or timer.</param>
        /// <param name="ExcludeTitle">Windows whose titles include this value will not be considered.</param>
        /// <param name="ExcludeText">Windows whose text include this value will not be considered.</param>
        public static void WinClose(string WinTitle, string WinText, int SecondsToWait, string ExcludeTitle, string ExcludeText)
        {
            if (Environment.OSVersion.Platform != PlatformID.Win32NT)
                throw new Win32Required();

            IntPtr hwnd = Settings.Windows.FindWindow(WinTitle, WinText, ExcludeTitle, ExcludeText);
            Windows.Windows.CloseWindow(hwnd);

            int start = Environment.TickCount;
            SecondsToWait *= 1000;

            while (Windows.Windows.IsWindowVisible(hwnd))
            {
                System.Threading.Thread.Sleep(Settings.LoopFrequency);
                if (Environment.TickCount - start > SecondsToWait)
                    break;
            }
            if (Settings.WinDelay >= 0)
                Core.Sleep(Settings.WinDelay);
        }

        /// <summary>
        /// Returns the Unique ID (HWND) of the first matching window (0 if none) as a hexademinal integer. Since all non-zero numbers are seen as "true", the statement if WinExist("WinTitle") is true whenever WinTitle exists. Finally, WinTitle supports ahk_id, ahk_class, and other special strings. See IfWinExist for details about these and other aspects of window searching.
        /// </summary>
        /// <param name="WinTitle"></param>
        /// <param name="WinText"></param>
        /// <param name="ExcludeTitle"></param>
        /// <param name="ExcludeText"></param>
        /// <returns></returns>
        public static int WinExist(string WinTitle, string WinText, string ExcludeTitle, string ExcludeText)
        {
            if (Environment.OSVersion.Platform != PlatformID.Win32NT)
                throw new Win32Required();

            return Settings.Windows.FindWindow(WinTitle, WinText, ExcludeTitle, ExcludeText).ToInt32();
        }

        /// <summary>
        /// Retrieves the specified window's unique ID, process ID, process name, or a list of its controls. It can also retrieve a list of all windows matching the specified criteria.
        /// </summary>
        /// <param name="OutputVar">The name of the variable in which to store the result of Cmd.</param>
        /// <param name="Cmd">See list below.</param>
        /// <param name="WinTitle">The title or partial title of the target window (the matching behavior is determined by SetTitleMatchMode). If this and the other 3 parameters are omitted, the Last Found Window will be used. To use a window class, specify ahk_class ExactClassName (shown by Window Spy). To use a process identifier (PID), specify ahk_pid %VarContainingPID%. To use a window's unique ID number, specify ahk_id %VarContainingID%. The search can be narrowed by specifying multiple criteria. For example: My File.txt ahk_pid %VarContainingPID%</param>
        /// <param name="WinText">If present, this parameter must be a substring from a single text element of the target window (as revealed by the included Window Spy utility). Hidden text elements are detected if DetectHiddenText is ON.</param>
        /// <param name="ExcludeTitle">Windows whose titles include this value will not be considered.</param>
        /// <param name="ExcludeText">Windows whose text include this value will not be considered.</param>
        public static void WinGet(out string OutputVar, string Cmd, string WinTitle, string WinText, string ExcludeTitle, string ExcludeText)
        {
            OutputVar = null;
        }

        /// <summary>
        /// Combines the functions of WinGetActiveTitle and WinGetPos into one command.
        /// </summary>
        /// <param name="Title">The name of the variable in which to store the title of the active window.</param>
        /// <param name="Width">The names of the variables in which to store the width and height of the active window.</param>
        /// <param name="Height">See <paramref name="Width"/>.</param>
        /// <param name="X">The names of the variables in which to store the X and Y coordinates of the active window's upper left corner.</param>
        /// <param name="Y">See <paramref name="X"/>.</param>
        public static void WinGetActiveStats(out string Title, out int Width, out int Height, out int X, out int Y)
        {
            if (Environment.OSVersion.Platform != PlatformID.Win32NT)
                throw new Win32Required();

            IntPtr hwnd = Windows.Windows.GetActiveWindow();
            if (hwnd == IntPtr.Zero)
            {
                Title = string.Empty;
                Width = Height = X = Y = 0;
                return;
            }

            Title = Windows.Windows.GetWindowText(hwnd);

            Windows.Windows.RECT size;
            Windows.Windows.GetWindowRect(hwnd, out size);

            Width = size.Left - size.Right;
            Height = size.Bottom - size.Top;
            X = size.Left;
            Y = size.Top;
        }

        /// <summary>
        /// Retrieves the title of the active window.
        /// </summary>
        /// <param name="OutputVar">The name of the variable in which to store the title of the active window.</param>
        public static void WinGetActiveTitle(out string OutputVar)
        {
            if (Environment.OSVersion.Platform != PlatformID.Win32NT)
                throw new Win32Required();

            IntPtr hwnd = Windows.Windows.GetActiveWindow();
            OutputVar = Windows.Windows.GetWindowText(hwnd);
        }

        /// <summary>
        /// Retrieves the specified window's class name.
        /// </summary>
        /// <param name="OutputVar">The name of the variable in which to store the retrieved class name.</param>
        /// <param name="WinTitle">The title or partial title of the target window (the matching behavior is determined by SetTitleMatchMode). If this and the next 3 parameters are omitted, the Last Found Window will be used. If this is the letter A and the next 3 parameters are omitted, the active window will be used. To use a process identifier (PID), specify ahk_pid %VarContainingPID%. To use a window group, specify ahk_group GroupName. To use a window's unique ID number, specify ahk_id %VarContainingID%. The search can be narrowed by specifying multiple criteria. For example: My File.txt ahk_pid %VarContainingPID%</param>
        /// <param name="WinText">If present, this parameter must be a substring from a single text element of the target window (as revealed by the included Window Spy utility). Hidden text elements are detected if DetectHiddenText is ON.</param>
        /// <param name="ExcludeTitle">Windows whose titles include this value will not be considered.</param>
        /// <param name="ExcludeText">Windows whose text include this value will not be considered.</param>
        public static void WinGetClass(out string OutputVar, string WinTitle, string WinText, string ExcludeTitle, string ExcludeText)
        {
            if (Environment.OSVersion.Platform != PlatformID.Win32NT)
                throw new Win32Required();

            IntPtr hwnd = Settings.Windows.FindWindow(WinTitle, WinText, ExcludeTitle, ExcludeText);
            OutputVar = Windows.Windows.GetClassName(hwnd);
        }

        /// <summary>
        /// Retrieves the position and size of the specified window.
        /// </summary>
        /// <param name="X">The names of the variables in which to store the X and Y coordinates of the target window's upper left corner. If omitted, the corresponding values will not be stored.</param>
        /// <param name="Y">See <paramref name="X"/>.</param>
        /// <param name="Width">The names of the variables in which to store the width and height of the target window. If omitted, the corresponding values will not be stored.</param>
        /// <param name="Height">See <paramref name="Width"/>.</param>
        /// <param name="WinTitle">The title or partial title of the target window (the matching behavior is determined by SetTitleMatchMode). If this and the next 3 parameters are omitted, the Last Found Window will be used. If this is the letter A and the next 3 parameters are omitted, the active window will be used. To use a window class, specify ahk_class ExactClassName (shown by Window Spy). To use a process identifier (PID), specify ahk_pid %VarContainingPID%. To use a window group, specify ahk_group GroupName. To use a window's unique ID number, specify ahk_id %VarContainingID%. The search can be narrowed by specifying multiple criteria. For example: My File.txt ahk_class Notepad</param>
        /// <param name="WinText">If present, this parameter must be a substring from a single text element of the target window (as revealed by the included Window Spy utility). Hidden text elements are detected if DetectHiddenText is ON.</param>
        /// <param name="ExcludeTitle">Windows whose titles include this value will not be considered.</param>
        /// <param name="ExcludeText">Windows whose text include this value will not be considered.</param>
        public static void WinGetPos(out int X, out int Y, out int Width, out int Height, string WinTitle, string WinText, string ExcludeTitle, string ExcludeText)
        {
            if (Environment.OSVersion.Platform != PlatformID.Win32NT)
                throw new Win32Required();

            IntPtr hwnd = Settings.Windows.FindWindow(WinTitle, WinText, ExcludeTitle, ExcludeText);
            Windows.Windows.RECT pos;
            Windows.Windows.GetWindowRect(hwnd, out pos);
            X = pos.Top;
            Y = pos.Left;
            Width = pos.Right - pos.Left;
            Height = pos.Bottom - pos.Top;
        }

        /// <summary>
        /// Retrieves the text from the specified window.
        /// </summary>
        /// <param name="OutputVar">The name of the variable in which to store the retrieved text.</param>
        /// <param name="WinTitle">The title or partial title of the target window (the matching behavior is determined by SetTitleMatchMode). If this and the next 3 parameters are omitted, the Last Found Window will be used. If this is the letter A and the next 3 parameters are omitted, the active window will be used. To use a window class, specify ahk_class ExactClassName (shown by Window Spy). To use a process identifier (PID), specify ahk_pid %VarContainingPID%. To use a window group, specify ahk_group GroupName. To use a window's unique ID number, specify ahk_id %VarContainingID%. The search can be narrowed by specifying multiple criteria. For example: My File.txt ahk_class Notepad</param>
        /// <param name="WinText">If present, this parameter must be a substring from a single text element of the target window (as revealed by the included Window Spy utility). Hidden text elements are detected if DetectHiddenText is ON.</param>
        /// <param name="ExcludeTitle">Windows whose titles include this value will not be considered.</param>
        /// <param name="ExcludeText">Windows whose text include this value will not be considered.</param>
        public static void WinGetText(out string OutputVar, string WinTitle, string WinText, string ExcludeTitle, string ExcludeText)
        {
            if (Environment.OSVersion.Platform != PlatformID.Win32NT)
                throw new Win32Required();

            IntPtr hwnd = Settings.Windows.FindWindow(WinTitle, WinText, ExcludeTitle, ExcludeText);
            hwnd = Windows.Windows.GetWindow(hwnd, Windows.Windows.GW_CHILD);
            OutputVar = Windows.Windows.GetWindowText(hwnd);
        }

        /// <summary>
        /// Retrieves the title of the specified window.
        /// </summary>
        /// <param name="OutputVar">The name of the variable in which to store the retrieved title.</param>
        /// <param name="WinTitle">The title or partial title of the target window (the matching behavior is determined by SetTitleMatchMode). If this and the next 3 parameters are omitted, the Last Found Window will be used. If this is the letter A and the next 3 parameters are omitted, the active window will be used. To use a window class, specify ahk_class ExactClassName (shown by Window Spy). To use a process identifier (PID), specify ahk_pid %VarContainingPID%. To use a window group, specify ahk_group GroupName. To use a window's unique ID number, specify ahk_id %VarContainingID%. The search can be narrowed by specifying multiple criteria. For example: My File.txt ahk_class Notepad</param>
        /// <param name="WinText">If present, this parameter must be a substring from a single text element of the target window (as revealed by the included Window Spy utility). Hidden text elements are detected if DetectHiddenText is ON.</param>
        /// <param name="ExcludeTitle">Windows whose titles include this value will not be considered.</param>
        /// <param name="ExcludeText">Windows whose text include this value will not be considered.</param>
        public static void WinGetTitle(out string OutputVar, string WinTitle, string WinText, string ExcludeTitle, string ExcludeText)
        {
            if (Environment.OSVersion.Platform != PlatformID.Win32NT)
                throw new Win32Required();

            IntPtr hwnd = Settings.Windows.FindWindow(WinTitle, WinText, ExcludeTitle, ExcludeText);
            OutputVar = Windows.Windows.GetWindowText(hwnd);
        }

        /// <summary>
        /// Hides the specified window.
        /// </summary>
        /// <param name="WinTitle">The title or partial title of the target window (the matching behavior is determined by SetTitleMatchMode). If this and the other 3 parameters are omitted, the Last Found Window will be used. If this is the letter A and the other 3 parameters are omitted, the active window will be used. To use a window class, specify ahk_class ExactClassName (shown by Window Spy). To use a process identifier (PID), specify ahk_pid %VarContainingPID%. To hide a group of windows, specify ahk_group GroupName (WinText, ExcludeTitle, and ExcludeText must be blank in this case). To use a window's unique ID number, specify ahk_id %VarContainingID%. The search can be narrowed by specifying multiple criteria. For example: My File.txt ahk_class Notepad</param>
        /// <param name="WinText">If present, this parameter must be a substring from a single text element of the target window (as revealed by the included Window Spy utility). Hidden text elements are detected if DetectHiddenText is ON.</param>
        /// <param name="ExcludeTitle">Windows whose titles include this value will not be considered.</param>
        /// <param name="ExcludeText">Windows whose text include this value will not be considered.</param>
        public static void WinHide(string WinTitle, string WinText, string ExcludeTitle, string ExcludeText)
        {
            if (Environment.OSVersion.Platform != PlatformID.Win32NT)
                throw new Win32Required();

            IntPtr hwnd = Settings.Windows.FindWindow(WinTitle, WinText, ExcludeTitle, ExcludeText);
            Windows.Windows.ShowWindow(hwnd, Windows.Windows.SW_HIDE);
        }

        /// <summary>
        /// Forces the specified window to close.
        /// </summary>
        /// <param name="WinTitle">The title or partial title of the target window (the matching behavior is determined by SetTitleMatchMode). If this and the other 3 window parameters are blank or omitted, the Last Found Window will be used. If this is the letter A and the other 3 window parameters are blank or omitted, the active window will be used. To use a window class, specify ahk_class ExactClassName (shown by Window Spy). To use a process identifier (PID), specify ahk_pid %VarContainingPID%. To kill a group of windows, specify ahk_group GroupName (WinText, ExcludeTitle, and ExcludeText must be blank in this case). To use a window's unique ID number, specify ahk_id %VarContainingID%. The search can be narrowed by specifying multiple criteria. For example: My File.txt ahk_class Notepad</param>
        /// <param name="WinText">If present, this parameter must be a substring from a single text element of the target window (as revealed by the included Window Spy utility). Hidden text elements are detected if DetectHiddenText is ON.</param>
        /// <param name="SecondsToWait">If omitted or blank, the command will not wait at all. If 0, it will wait 500ms. Otherwise, it will wait the indicated number of seconds (can contain a decimal point or be an expression) for the window to close. If the window does not close within that period, the script will continue. ErrorLevel is not set by this command, so use IfWinExist or WinWaitClose if you need to determine for certain that a window is closed.</param>
        /// <param name="ExcludeTitle">Windows whose titles include this value will not be considered.</param>
        /// <param name="ExcludeText">Windows whose text include this value will not be considered.</param>
        public static void WinKill(string WinTitle, string WinText, int SecondsToWait, string ExcludeTitle, string ExcludeText)
        {
            if (Environment.OSVersion.Platform != PlatformID.Win32NT)
                throw new Win32Required();

            IntPtr hwnd = Settings.Windows.FindWindow(WinTitle, WinText, ExcludeTitle, ExcludeText);
            Windows.Windows.SendMessage(hwnd, Windows.Windows.WM_SYSCOMMAND, new IntPtr(Windows.Windows.SC_CLOSE), IntPtr.Zero);

            int start = Environment.TickCount;
            SecondsToWait *= 1000;

            while (Windows.Windows.IsWindowVisible(hwnd))
            {
                System.Threading.Thread.Sleep(Settings.LoopFrequency);
                if (Environment.TickCount - start > SecondsToWait)
                    break;
            }
        }

        /// <summary>
        /// Enlarges the specified window to its maximum size.
        /// </summary>
        /// <param name="WinTitle">The title or partial title of the target window (the matching behavior is determined by SetTitleMatchMode). If this and the next 3 parameters are omitted, the Last Found Window will be used. If this is the letter A and the next 3 parameters are omitted, the active window will be used. To use a window class, specify ahk_class ExactClassName (shown by Window Spy). To use a process identifier (PID), specify ahk_pid %VarContainingPID%. To maximize a group of windows, specify ahk_group GroupName (WinText, ExcludeTitle, and ExcludeText must be blank in this case). To use a window's unique ID number, specify ahk_id %VarContainingID%. The search can be narrowed by specifying multiple criteria. For example: My File.txt ahk_class Notepad</param>
        /// <param name="WinText">If present, this parameter must be a substring from a single text element of the target window (as revealed by the included Window Spy utility). Hidden text elements are detected if DetectHiddenText is ON.</param>
        /// <param name="ExcludeTitle">Windows whose titles include this value will not be considered.</param>
        /// <param name="ExcludeText">Windows whose text include this value will not be considered.</param>
        public static void WinMaximize(string WinTitle, string WinText, string ExcludeTitle, string ExcludeText)
        {
            if (Environment.OSVersion.Platform != PlatformID.Win32NT)
                throw new Win32Required();

            IntPtr hwnd = Settings.Windows.FindWindow(WinTitle, WinText, ExcludeTitle, ExcludeText);
            Windows.Windows.ShowWindow(hwnd, Windows.Windows.SW_MAXIMIZE);
        }

        /// <summary>
        /// Invokes a menu item from the menu bar of the specified window.
        /// </summary>
        /// <param name="WinTitle">The title or partial title of the target window (the matching behavior is determined by SetTitleMatchMode). If this and the other 3 window parameters are blank or omitted, the Last Found Window will be used. If this is the letter A and the other 3 window parameters are blank or omitted, the active window will be used. To use a window class, specify ahk_class ExactClassName (shown by Window Spy). To use a process identifier (PID), specify ahk_pid %VarContainingPID%. To use a window group, specify ahk_group GroupName. To use a window's unique ID number, specify ahk_id %VarContainingID%. The search can be narrowed by specifying multiple criteria. For example: My File.txt ahk_class Notepad</param>
        /// <param name="WinText">If present, this parameter must be a substring from a single text element of the target window (as revealed by the included Window Spy utility). Hidden text elements are detected if DetectHiddenText is ON.</param>
        /// <param name="Menu">The name of the top-level menu, e.g. File, Edit, View. It can also be the position of the desired menu item by using 1&amp; to represent the first menu, 2&amp; the second, and so on.</param>
        /// <param name="SubMenu1">The name of the menu item to select or its position (see above).</param>
        /// <param name="SubMenu2">If SubMenu1 itself contains a menu, this is the name of the menu item inside, or its position.</param>
        /// <param name="SubMenu3">Same as above.</param>
        /// <param name="SubMenu4">Same as above.</param>
        /// <param name="SubMenu5">Same as above.</param>
        /// <param name="SubMenu6">Same as above.</param>
        /// <param name="ExcludeTitle">Windows whose titles include this value will not be considered.</param>
        /// <param name="ExcludeText">Windows whose text include this value will not be considered.</param>
        public static void WinMenuSelectItem(string WinTitle, string WinText, string Menu, string SubMenu1, string SubMenu2, string SubMenu3, string SubMenu4, string SubMenu5, string SubMenu6, string ExcludeTitle, string ExcludeText)
        {
            
        }

        /// <summary>
        /// Collapses the specified window into a button on the task bar.
        /// </summary>
        /// <param name="WinTitle">The title or partial title of the target window (the matching behavior is determined by SetTitleMatchMode). If this and the next 3 parameters are omitted, the Last Found Window will be used. If this is the letter A and the next 3 parameters are omitted, the active window will be used. To use a window class, specify ahk_class ExactClassName (shown by Window Spy). To use a process identifier (PID), specify ahk_pid %VarContainingPID%. To minimize a group of windows, specify ahk_group GroupName (WinText, ExcludeTitle, and ExcludeText must be blank in this case). To use a window's unique ID number, specify ahk_id %VarContainingID%. The search can be narrowed by specifying multiple criteria. For example: My File.txt ahk_class Notepad</param>
        /// <param name="WinText">If present, this parameter must be a substring from a single text element of the target window (as revealed by the included Window Spy utility). Hidden text elements are detected if DetectHiddenText is ON.</param>
        /// <param name="ExcludeTitle">Windows whose titles include this value will not be considered.</param>
        /// <param name="ExcludeText">Windows whose text include this value will not be considered.</param>
        public static void WinMinimize(string WinTitle, string WinText, string ExcludeTitle, string ExcludeText)
        {
            if (Environment.OSVersion.Platform != PlatformID.Win32NT)
                throw new Win32Required();

            IntPtr hwnd = Settings.Windows.FindWindow(WinTitle, WinText, ExcludeTitle, ExcludeText);
            Windows.Windows.ShowWindow(hwnd, Windows.Windows.SW_MINIMIZE);
        }

        /// <summary>
        /// Minimizes all windows.
        /// </summary>
        public static void WinMinimizeAll()
        {
            if (Environment.OSVersion.Platform != PlatformID.Win32NT)
                throw new Win32Required();

            foreach (IntPtr hwnd in Settings.Windows.FindAllWindows())
                Windows.Windows.ShowWindow(hwnd, Windows.Windows.SW_MINIMIZE);
        }

        /// <summary>
        /// Unminimizes all windows.
        /// </summary>
        public static void WinMinimizeAllUndo()
        {
            if (Environment.OSVersion.Platform != PlatformID.Win32NT)
                throw new Win32Required();

            foreach (IntPtr hwnd in Settings.Windows.FindAllWindows())
                Windows.Windows.ShowWindow(hwnd, Windows.Windows.SW_RESTORE);
        }

        /// <summary>
        /// Changes the position and/or size of the specified window.
        /// </summary>
        /// <param name="WinTitle">The title or partial title of the target window (the matching behavior is determined by SetTitleMatchMode). If this and the other 3 window parameters are blank or omitted, the Last Found Window will be used. If this is the letter A and the other 3 window parameters are blank or omitted, the active window will be used. To use a window class, specify ahk_class ExactClassName (shown by Window Spy). To use a process identifier (PID), specify ahk_pid %VarContainingPID%. To use a window group, specify ahk_group GroupName. To use a window's unique ID number, specify ahk_id %VarContainingID%. The search can be narrowed by specifying multiple criteria. For example: My File.txt ahk_class Notepad</param>
        /// <param name="WinText">If present, this parameter must be a substring from a single text element of the target window (as revealed by the included Window Spy utility). Hidden text elements are detected if DetectHiddenText is ON.</param>
        /// <param name="X">
        /// <para>The X and Y coordinates (in pixels) of the upper left corner of the target window's new location. The upper-left pixel of the screen is at 0, 0.</para>
        /// <para>If these are the only parameters given with the command, the Last Found Window will be used as the target window.</para>
        /// </param>
        /// <param name="Y">See <paramref name="X"/>.</param>
        /// <param name="Width">The new width and height of the window (in pixels). If either is omitted, blank, or the word DEFAULT, the size in that dimension will not be changed.</param>
        /// <param name="Height">See <paramref name="Width"/>.</param>
        /// <param name="ExcludeTitle">Windows whose titles include this value will not be considered.</param>
        /// <param name="ExcludeText">Windows whose text include this value will not be considered.</param>
        public static void WinMove(string WinTitle, string WinText, int X, int Y, int Width, int Height, string ExcludeTitle, string ExcludeText)
        {
            if (Environment.OSVersion.Platform != PlatformID.Win32NT)
                throw new Win32Required();

            IntPtr hwnd = Settings.Windows.FindWindow(WinTitle, WinText, ExcludeTitle, ExcludeText);
            Windows.Windows.MoveWindow(hwnd, X, Y, Width, Height, true);
        }

        /// <summary>
        /// Unminimizes or unmaximizes the specified window if it is minimized or maximized.
        /// </summary>
        /// <param name="WinTitle">The title or partial title of the target window (the matching behavior is determined by SetTitleMatchMode). If this and the next 3 parameters are omitted, the Last Found Window will be used. If this is the letter A and the next 3 parameters are omitted, the active window will be used. To use a window class, specify ahk_class ExactClassName (shown by Window Spy). To use a process identifier (PID), specify ahk_pid %VarContainingPID%. To restore a group of windows, specify ahk_group GroupName (WinText, ExcludeTitle, and ExcludeText must be blank in this case). To use a window's unique ID number, specify ahk_id %VarContainingID%. The search can be narrowed by specifying multiple criteria. For example: My File.txt ahk_class Notepad</param>
        /// <param name="WinText">If present, this parameter must be a substring from a single text element of the target window (as revealed by the included Window Spy utility). Hidden text elements are detected if DetectHiddenText is ON.</param>
        /// <param name="ExcludeTitle">Windows whose titles include this value will not be considered.</param>
        /// <param name="ExcludeText">Windows whose text include this value will not be considered.</param>
        public static void WinRestore(string WinTitle, string WinText, string ExcludeTitle, string ExcludeText)
        {
            if (Environment.OSVersion.Platform != PlatformID.Win32NT)
                throw new Win32Required();

            IntPtr hwnd = Settings.Windows.FindWindow(WinTitle, WinText, ExcludeTitle, ExcludeText);
            Windows.Windows.ShowWindow(hwnd, Windows.Windows.SW_RESTORE);
        }

        /// <summary>
        /// Makes a variety of changes to the specified window, such as "always on top" and transparency.
        /// </summary>
        /// <param name="Attribute">See list below.</param>
        /// <param name="Value">See list below.</param>
        /// <param name="WinTitle">The title or partial title of the target window (the matching behavior is determined by SetTitleMatchMode). If this and the next 3 parameters are omitted, the Last Found Window will be used. If this is the letter A and the next 3 parameters are omitted, the active window will be used. To use a window class, specify ahk_class ExactClassName (shown by Window Spy). To use a process identifier (PID), specify ahk_pid %VarContainingPID%. To use a window group, specify ahk_group GroupName. To use a window's unique ID number, specify ahk_id %VarContainingID%. The search can be narrowed by specifying multiple criteria. For example: My File.txt ahk_class Notepad</param>
        /// <param name="WinText">If present, this parameter must be a substring from a single text element of the target window (as revealed by the included Window Spy utility). Hidden text elements are detected if DetectHiddenText is ON.</param>
        /// <param name="ExcludeTitle">Windows whose titles include this value will not be considered.</param>
        /// <param name="ExcludeText">Windows whose text include this value will not be considered.</param>
        public static void WinSet(string Attribute, string Value, string WinTitle, string WinText, string ExcludeTitle, string ExcludeText)
        {
            
        }

        /// <summary>
        /// Changes the title of the specified window.
        /// </summary>
        /// <param name="WinTitle">The title or partial title of the target window (the matching behavior is determined by SetTitleMatchMode). If this and the next 3 parameters are omitted, the Last Found Window will be used. If this is the letter A and the next 3 parameters are omitted, the active window will be used. To use a window class, specify ahk_class ExactClassName (shown by Window Spy). To use a process identifier (PID), specify ahk_pid %VarContainingPID%. To use a window group, specify ahk_group GroupName. To use a window's unique ID number, specify ahk_id %VarContainingID%. The search can be narrowed by specifying multiple criteria. For example: My File.txt ahk_class Notepad</param>
        /// <param name="WinText">If present, this parameter must be a substring from a single text element of the target window (as revealed by the included Window Spy utility). Hidden text elements are detected if DetectHiddenText is ON.</param>
        /// <param name="NewTitle">The new title for the window. If this is the only parameter given, the Last Found Window will be used.</param>
        /// <param name="ExcludeTitle">Windows whose titles include this value will not be considered.</param>
        /// <param name="ExcludeText">Windows whose text include this value will not be considered.</param>
        public static void WinSetTitle(string WinTitle, string WinText, string NewTitle, string ExcludeTitle, string ExcludeText)
        {
            if (Environment.OSVersion.Platform != PlatformID.Win32NT)
                throw new Win32Required();

            IntPtr hwnd = Settings.Windows.FindWindow(WinTitle, WinText, ExcludeTitle, ExcludeText);
            Windows.Windows.SetWindowText(hwnd, NewTitle);
        }

        /// <summary>
        /// Unhides the specified window.
        /// </summary>
        /// <param name="WinTitle">The title or partial title of the target window (the matching behavior is determined by SetTitleMatchMode). If this and the other 3 parameters are omitted, the Last Found Window will be used. To use a window class, specify ahk_class ExactClassName (shown by Window Spy). To use a process identifier (PID), specify ahk_pid %VarContainingPID%. To show a group of windows, specify ahk_group GroupName (WinText, ExcludeTitle, and ExcludeText must be blank in this case). To use a window's unique ID number, specify ahk_id %VarContainingID%. The search can be narrowed by specifying multiple criteria. For example: My File.txt ahk_class Notepad</param>
        /// <param name="WinText">If present, this parameter must be a substring from a single text element of the target window (as revealed by the included Window Spy utility). Hidden text elements are detected if DetectHiddenText is ON.</param>
        /// <param name="ExcludeTitle">Windows whose titles include this value will not be considered.</param>
        /// <param name="ExcludeText">Windows whose text include this value will not be considered.</param>
        public static void WinShow(string WinTitle, string WinText, string ExcludeTitle, string ExcludeText)
        {
            if (Environment.OSVersion.Platform != PlatformID.Win32NT)
                throw new Win32Required();

            IntPtr hwnd = Settings.Windows.FindWindow(WinTitle, WinText, ExcludeTitle, ExcludeText);
            Windows.Windows.ShowWindow(hwnd, Windows.Windows.SW_SHOW);
        }

        /// <summary>
        /// Waits until the specified window exists.
        /// </summary>
        /// <param name="WinTitle">
        /// <para>The title or partial title of the target window (the matching behavior is determined by SetTitleMatchMode). To use a window class, specify ahk_class ExactClassName (shown by Window Spy). To use a process identifier (PID), specify ahk_pid %VarContainingPID%. To use a window group, specify ahk_group GroupName. The search can be narrowed by specifying multiple criteria. For example: My File.txt ahk_class Notepad</para>
        /// <para>WinTitle may be blank only when WinText, ExcludeTitle, or ExcludeText is present.</para>
        /// </param>
        /// <param name="WinText">If present, this parameter must be a substring from a single text element of the target window (as revealed by the included Window Spy utility). Hidden text elements are detected if DetectHiddenText is ON.</param>
        /// <param name="Seconds">How many seconds to wait before timing out and setting ErrorLevel to 1. Leave blank to wait indefinitely. Specifying 0 is the same as specifying 0.5. This parameter can be an expression.</param>
        /// <param name="ExcludeTitle">Windows whose titles include this value will not be considered.</param>
        /// <param name="ExcludeText">Windows whose text include this value will not be considered.</param>
        public static void WinWait(string WinTitle, string WinText, int Seconds, string ExcludeTitle, string ExcludeText)
        {
            if (Environment.OSVersion.Platform != PlatformID.Win32NT)
                throw new Win32Required();

            int start = Environment.TickCount;
            Seconds *= 1000;
            error = 0;

            while (Settings.Windows.FindWindow(WinTitle, WinText, ExcludeTitle, ExcludeText) == IntPtr.Zero)
            {
                System.Threading.Thread.Sleep(Settings.LoopFrequency);
                if (Environment.TickCount - start > Seconds)
                {
                    error = 1;
                    break;
                }
            }
        }

        /// <summary>
        /// Waits until the specified window is active.
        /// </summary>
        /// <param name="WinTitle">
        /// <para>The title or partial title of the target window (the matching behavior is determined by SetTitleMatchMode). To use a window class, specify ahk_class ExactClassName (shown by Window Spy). To use a process identifier (PID), specify ahk_pid %VarContainingPID%. To use a window group, specify ahk_group GroupName. The search can be narrowed by specifying multiple criteria. For example: My File.txt ahk_class Notepad</para>
        /// <para>WinTitle may be blank only when WinText, ExcludeTitle, or ExcludeText is present.</para>
        /// </param>
        /// <param name="WinText">If present, this parameter must be a substring from a single text element of the target window (as revealed by the included Window Spy utility). Hidden text elements are detected if DetectHiddenText is ON.</param>
        /// <param name="Seconds">How many seconds to wait before timing out and setting ErrorLevel to 1. Leave blank to wait indefinitely. Specifying 0 is the same as specifying 0.5. This parameter can be an expression.</param>
        /// <param name="ExcludeTitle">Windows whose titles include this value will not be considered.</param>
        /// <param name="ExcludeText">Windows whose text include this value will not be considered.</param>
        public static void WinWaitActive(string WinTitle, string WinText, int Seconds, string ExcludeTitle, string ExcludeText)
        {
            if (Environment.OSVersion.Platform != PlatformID.Win32NT)
                throw new Win32Required();

            int start = Environment.TickCount;
            Seconds *= 1000;
            error = 0;
            
            while (Settings.Windows.FindWindow(WinTitle, WinText, ExcludeTitle, ExcludeText) != Windows.Windows.GetForegroundWindow())
            {
                System.Threading.Thread.Sleep(Settings.LoopFrequency);
                if (Environment.TickCount - start > Seconds)
                {
                    error = 1;
                    break;
                }
            }
            if (Settings.WinDelay >= 0)
                Core.Sleep(Settings.WinDelay);
        }

        /// <summary>
        /// Waits until the specified window does not exist.
        /// </summary>
        /// <param name="WinTitle">
        /// <para>The title or partial title of the target window (the matching behavior is determined by SetTitleMatchMode). To use a window class, specify ahk_class ExactClassName (shown by Window Spy). To use a process identifier (PID), specify ahk_pid %VarContainingPID%. To use a window group, specify ahk_group GroupName. The search can be narrowed by specifying multiple criteria. For example: My File.txt ahk_class Notepad</para>
        /// <para>WinTitle may be blank only when WinText, ExcludeTitle, or ExcludeText is present.</para>
        /// </param>
        /// <param name="WinText">If present, this parameter must be a substring from a single text element of the target window (as revealed by the included Window Spy utility). Hidden text elements are detected if DetectHiddenText is ON.</param>
        /// <param name="Seconds">How many seconds to wait before timing out and setting ErrorLevel to 1. Leave blank to wait indefinitely. Specifying 0 is the same as specifying 0.5. This parameter can be an expression.</param>
        /// <param name="ExcludeTitle">Windows whose titles include this value will not be considered.</param>
        /// <param name="ExcludeText">Windows whose text include this value will not be considered.</param>
        public static void WinWaitClose(string WinTitle, string WinText, int Seconds, string ExcludeTitle, string ExcludeText)
        {
            if (Environment.OSVersion.Platform != PlatformID.Win32NT)
                throw new Win32Required();

            int start = Environment.TickCount;
            Seconds *= 1000; 
            error = 0;

            while (Settings.Windows.FindWindow(WinTitle, WinText, ExcludeTitle, ExcludeText) != IntPtr.Zero)
            {
                System.Threading.Thread.Sleep(Settings.LoopFrequency);
                if (Environment.TickCount - start > Seconds)
                {
                    error = 1;
                    break;
                }
            }
        }

        /// <summary>
        /// Waits until the specified window is not active.
        /// </summary>
        /// <param name="WinTitle">
        /// <para>The title or partial title of the target window (the matching behavior is determined by SetTitleMatchMode). To use a window class, specify ahk_class ExactClassName (shown by Window Spy). To use a process identifier (PID), specify ahk_pid %VarContainingPID%. To use a window group, specify ahk_group GroupName. The search can be narrowed by specifying multiple criteria. For example: My File.txt ahk_class Notepad</para>
        /// <para>WinTitle may be blank only when WinText, ExcludeTitle, or ExcludeText is present.</para>
        /// </param>
        /// <param name="WinText">If present, this parameter must be a substring from a single text element of the target window (as revealed by the included Window Spy utility). Hidden text elements are detected if DetectHiddenText is ON.</param>
        /// <param name="Seconds">How many seconds to wait before timing out and setting ErrorLevel to 1. Leave blank to wait indefinitely. Specifying 0 is the same as specifying 0.5. This parameter can be an expression.</param>
        /// <param name="ExcludeTitle">Windows whose titles include this value will not be considered.</param>
        /// <param name="ExcludeText">Windows whose text include this value will not be considered.</param>
        public static void WinWaitNotActive(string WinTitle, string WinText, int Seconds, string ExcludeTitle, string ExcludeText)
        {
            if (Environment.OSVersion.Platform != PlatformID.Win32NT)
                throw new Win32Required();

            int start = Environment.TickCount;
            Seconds *= 1000; 
            error = 0;

            while (Settings.Windows.FindWindow(WinTitle, WinText, ExcludeTitle, ExcludeText) != Windows.Windows.GetActiveWindow())
            {
                System.Threading.Thread.Sleep(Settings.LoopFrequency);
                if (Environment.TickCount - start > Seconds)
                {
                    error = 1;
                    break;
                }
            }
        }
    }
}