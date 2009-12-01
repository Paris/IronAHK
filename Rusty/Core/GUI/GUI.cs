using System.Diagnostics;
using System.Windows.Forms;
using System;
using System.Runtime.InteropServices;

namespace IronAHK.Rusty
{
    partial class Core
    {
        /// <summary>
        /// Creates and manages windows and controls. Such windows can be used as data entry forms or custom user interfaces.
        /// </summary>
        /// <param name="Command"></param>
        /// <param name="Param2"></param>
        /// <param name="Param3"></param>
        /// <param name="Param4"></param>
        public static void Gui(string Command, string Param2, string Param3, string Param4)
        {

        }

        /// <summary>
        /// Makes a variety of changes to a control in a GUI window.
        /// </summary>
        /// <param name="Command">See list below.</param>
        /// <param name="ControlID">
        /// <para>If the target control has an associated variable, specify the variable's name as the ControlID (this method takes precedence over the ones described next). For this reason, it is usually best to assign a variable to any control that will later be accessed via GuiControl or GuiControlGet, even if that control is not an input-capable type (such as GroupBox or Text).</para>
        /// <para>Otherwise, ControlID can be either ClassNN (the classname and instance number of the control) or the name/text of the control, both of which can be determined via Window Spy. When using name/text, the matching behavior is determined by SetTitleMatchMode. Note: a picture control's file name (as it was specified at the time the control was created) may be used as its ControlID.</para>
        /// </param>
        /// <param name="Param3">This parameter is omitted except where noted in the list of sub-commands below.</param>
        public static void GuiControl(string Command, string ControlID, string Param3)
        {

        }

        /// <summary>
        /// Retrieves various types of information about a control in a GUI window.
        /// </summary>
        /// <param name="OutputVar">The name of the variable in which to store the result. If the command cannot complete (see ErrorLevel below), this variable is made blank.</param>
        /// <param name="Sub_command">See list below.</param>
        /// <param name="ControlID">
        /// <para>If blank or omitted, it behaves as though the name of the output variable was specified. For example, GuiControlGet, MyEdit is the same as GuiControlGet, MyEdit,, MyEdit.</para>
        /// <para>If the target control has an associated variable, specify the variable's name as the ControlID (this method takes precedence over the ones described next). For this reason, it is usually best to assign a variable to any control that will later be accessed via GuiControl or GuiControlGet, even if that control is not input-capable (such as GroupBox or Text).</para>
        /// <para>Otherwise, ControlID can be either ClassNN (the classname and instance number of the control) or the name/text of the control, both of which can be determined via Window Spy. When using name/text, the matching behavior is determined by SetTitleMatchMode. Note: a picture control's file name (as it was specified at the time the control was created) may be used as its ControlID.</para>
        /// </param>
        /// <param name="Param4">This parameter is omitted except where noted in the list of sub-commands below.</param>
        public static void GuiControlGet(out string OutputVar, string Sub_command, string ControlID, string Param4)
        {
            OutputVar = null;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="Mode"></param>
        [Obsolete, Conditional("FLOW")]
        public static void IfMsgBox(string Mode)
        {
            throw new NotSupportedException();
        }

        /// <summary>
        /// Displays an input box to ask the user to enter a string.
        /// </summary>
        /// <param name="OutputVar">The name of the variable in which to store the text entered by the user.</param>
        /// <param name="Title">The title of the input box. If blank or omitted, it defaults to the name of the script.</param>
        /// <param name="Prompt">The text of the input box, which is usually a message to the user indicating what kind of input is expected.</param>
        /// <param name="HIDE">If this parameter is the word HIDE, the user's input will be masked, which is useful for passwords.</param>
        /// <param name="Width">If this parameter is blank or omitted, the starting width of the window will be 375.</param>
        /// <param name="Height">If this parameter is blank or omitted, the starting height of the window will be 189.</param>
        /// <param name="X">The X coordinate of the window (use 0,0 to move it to the upper left corner of the desktop). If either coordinate is blank or omitted, the dialog will be centered in that dimension. Either coordinate can be negative to position the window partially or entirely off the desktop.</param>
        /// <param name="Y">The Y coordinate of the window (see <paramref name="X"/>).</param>
        /// <param name="Font">Not yet implemented (leave blank). In the future it might accept something like verdana:8</param>
        /// <param name="Timeout">Timeout in seconds (can contain a decimal point).  If this value exceeds 2147483 (24.8 days), it will be set to 2147483. After the timeout has elapsed, the InputBox window will be automatically closed and ErrorLevel will be set to 2. OutputVar will still be set to what the user entered.</param>
        /// <param name="Default">A string that will appear in the InputBox's edit field when the dialog first appears. The user can change it by backspacing or other means.</param>
        public static void InputBox(out string OutputVar, string Title, string Prompt, string HIDE, string Width, string Height, string X, string Y, string Font, string Timeout, string Default)
        {
            OutputVar = null;
        }

        /// <summary>
        /// Show a message box.
        /// </summary>
        /// <param name="Text">The text to show in the prompt.</param>
        public static void MsgBox(string Text)
        {
            MessageBox.Show(Text);
        }

        /// <summary>
        /// Displays the specified text in a small window containing one or more buttons (such as Yes and No).
        /// </summary>
        /// <param name="Options">
        /// <para>Indicates the type of message box and the possible button combinations. If blank or omitted, it defaults to 0. See the table below for allowed values.</para>
        /// <para>This parameter will not be recognized if it contains an expression or a variable reference such as %option%. Instead, use a literal numeric value.</para>
        /// </param>
        /// <param name="Title">The title of the message box window. If omitted or blank, it defaults to the name of the script (without path).</param>
        /// <param name="Text">
        /// <para>If all the parameters are omitted, the MsgBox will display the text "Press OK to continue." Otherwise, this parameter is the text displayed inside the message box to instruct the user what to do, or to present information.</para>
        /// <para>Escape sequences can be used to denote special characters. For example, `n indicates a linefeed character, which ends the current line and begins a new one. Thus, using text1`n`ntext2 would create a blank line between text1 and text2.</para>
        /// <para>If Text is long, it can be broken up into several shorter lines by means of a continuation section, which might improve readability and maintainability.</para>
        /// </param>
        /// <param name="Timeout">
        /// <para>(optional) Timeout in seconds (can contain a decimal point but cannot be an expression).  If this value exceeds 2147483 (24.8 days), it will be set to 2147483.  After the timeout has elapsed the message box will be automatically closed and the IfMsgBox command will see the value TIMEOUT.</para>
        /// <para>Known limitation: If the MsgBox contains only an OK button, IfMsgBox will think that the OK button was pressed if the MsgBox times out while its own thread is interrupted by another.</para>
        /// </param>
        public static void MsgBox(int Options, string Title, string Text, int Timeout)
        {
            MessageBoxButtons buttons = MessageBoxButtons.OK;
            switch (Options & 0xf)
            {
                case 0: buttons = MessageBoxButtons.OK; break;
                case 1: buttons = MessageBoxButtons.OKCancel; break;
                case 2: buttons = MessageBoxButtons.AbortRetryIgnore; break;
                case 3: buttons = MessageBoxButtons.YesNoCancel; break;
                case 4: buttons = MessageBoxButtons.YesNo; break;
                case 5: buttons = MessageBoxButtons.RetryCancel; break;
                //case 6: /* Cancel/Try Again/Continue */ ; break;
                //case 7: /* Adds a Help button */ ; break; // help done differently
            }

            MessageBoxIcon icon = MessageBoxIcon.None;
            switch (Options & 0xf0)
            {
                case 16: icon = MessageBoxIcon.Hand; break;
                case 32: icon = MessageBoxIcon.Question; break;
                case 48: icon = MessageBoxIcon.Exclamation; break;
                case 64: icon = MessageBoxIcon.Asterisk; break;
            }

            MessageBoxDefaultButton defaultbutton = MessageBoxDefaultButton.Button1;
            switch (Options & 0xf00)
            {
                case 256: defaultbutton = MessageBoxDefaultButton.Button2; break;
                case 512: defaultbutton = MessageBoxDefaultButton.Button3; break;
            }

            MessageBoxOptions? options = null;
            switch (Options & 0xf0000)
            {
                case 131072: options = MessageBoxOptions.DefaultDesktopOnly; break;
                case 262144: options = MessageBoxOptions.ServiceNotification; break;
                case 524288: options = MessageBoxOptions.RightAlign; break;
                case 1048576: options = MessageBoxOptions.RtlReading; break;
            }

            Settings.MsgBox = MessageBox.Show(Text, Title, buttons, icon, defaultbutton,
                options ?? default(MessageBoxOptions),
                (Options & 0xf000) == 16384);
        }

        /// <summary>
        /// Creates or updates a window containing a progress bar or an image.
        /// </summary>
        /// <param name="ProgressParam1">
        /// <para>If the progress window already exists: If Param1 is the word OFF, the window is destroyed. If Param1 is the word SHOW, the window is shown if it is currently hidden.</para>
        /// <para>Otherwise, if Param1 is an pure number, its bar's position is changed to that value. If Param1 is blank, its bar position will be unchanged but its text will be updated to reflect any new strings provided in SubText, MainText, and WinTitle. In both of these modes, if the window doesn't yet exist, it will be created with the defaults for all options.</para>
        /// <para>If the progress window does not exist: A new progress window is created (replacing any old one), and Param1 is a string of zero or more options from the list below.</para>
        /// </param>
        /// <param name="SubText">The text to display below the image or bar indicator. Although word-wrapping will occur, to begin a new line explicitly, use linefeed (`n). To set an existing window's text to be blank, specify %A_Space%. For the purpose of auto-calculating the window's height, blank lines can be reserved in a way similar to MainText below.</param>
        /// <param name="MainText">
        /// <para>The text to display above the image or bar indicator (its font is semi-bold). Although word-wrapping will occur, to begin a new line explicitly, use linefeed (`n).</para>
        /// <para>If blank or omitted, no space will be reserved in the window for MainText. To reserve space for single line to be added later, or to set an existing window's text to be blank, specify %A_Space%. To reserve extra lines beyond the first, append one or more linefeeds (`n).</para>
        /// <para>Once the height of MainText's control area has been set, it cannot be changed without recreating the window.</para>
        /// </param>
        /// <param name="WinTitle">The text to display below the image or bar indicator. Although word-wrapping will occur, to begin a new line explicitly, use linefeed (`n). To set an existing window's text to be blank, specify %A_Space%. For the purpose of auto-calculating the window's height, blank lines can be reserved in a way similar to MainText below.</param>
        /// <param name="FontName">
        /// <para>The name of the font to use for both MainText and SubText. The font table lists the fonts included with the various versions of Windows. If unspecified or if the font cannot be found, the system's default GUI font will be used.</para>
        /// <para>See the options section below for how to change the size, weight, and color of the font.</para>
        /// </param>
        [Obsolete, Conditional("LEGACY")]
        public static void Progress(string ProgressParam1, string SubText, string MainText, string WinTitle, string FontName)
        {

        }

        /// <summary>
        /// Creates or updates a window containing a progress bar or an image.
        /// </summary>
        /// <param name="ImageFile">
        /// <para>If this is the word OFF, the window is destroyed. If this is the word SHOW, the window is shown if it is currently hidden.</para>
        /// <para>Otherwise, this is the file name of the BMP, GIF, or JPG image to display (to display other file formats such as PNG, TIF, and ICO, consider using the Gui command to create a window containing a picture control).</para>
        /// <para>ImageFile is assumed to be in %A_WorkingDir% if an absolute path isn't specified. If ImageFile and Options are blank and the window already exists, its image will be unchanged but its text will be updated to reflect any new strings provided in SubText, MainText, and WinTitle.</para>
        /// <para>For newly created windows, if ImageFile is blank or there is a problem loading the image, the window will be displayed without the picture.</para>
        /// </param>
        /// <param name="Options">A string of zero or more options from the list further below.</param>
        /// <param name="SubText">The text to display below the image or bar indicator. Although word-wrapping will occur, to begin a new line explicitly, use linefeed (`n). To set an existing window's text to be blank, specify %A_Space%. For the purpose of auto-calculating the window's height, blank lines can be reserved in a way similar to MainText below.</param>
        /// <param name="MainText">
        /// <para>The text to display above the image or bar indicator (its font is semi-bold). Although word-wrapping will occur, to begin a new line explicitly, use linefeed (`n).</para>
        /// <para>If blank or omitted, no space will be reserved in the window for MainText. To reserve space for single line to be added later, or to set an existing window's text to be blank, specify %A_Space%. To reserve extra lines beyond the first, append one or more linefeeds (`n).</para>
        /// <para>Once the height of MainText's control area has been set, it cannot be changed without recreating the window.</para>
        /// </param>
        /// <param name="WinTitle">The title of the window. If omitted and the window is being newly created, the title defaults to the name of the script (without path). If the B (borderless) option has been specified, there will be no visible title bar but the window can still be referred to by this title in commands such as WinMove.</param>
        /// <param name="FontName">
        /// <para>The name of the font to use for both MainText and SubText. The font table lists the fonts included with the various versions of Windows. If unspecified or if the font cannot be found, the system's default GUI font will be used.</para>
        /// <para>See the options section below for how to change the size, weight, and color of the font.</para>
        /// </param>
        public static void SplashImage(string ImageFile, string Options, string SubText, string MainText, string WinTitle, string FontName)
        {

        }

        private static IntPtr[] g_hWndToolTip = new IntPtr[50]; //pointers to 50 tooltips
        /// <summary>
        /// Creates an always-on-top window anywhere on the screen.
        /// </summary>
        /// <param name="aText">
        /// <para>If blank or omitted, the existing tooltip (if any) will be hidden. Otherwise, this parameter is the text to display in the tooltip. To create a multi-line tooltip, use the linefeed character (`n) in between each line, e.g. Line1`nLine2.</para>
        /// <para>If Text is long, it can be broken up into several shorter lines by means of a continuation section, which might improve readability and maintainability.</para>
        /// </param>
        /// <param name="aX">The X position of the tooltip relative to the active window (use "CoordMode, ToolTip" to change to screen coordinates). If the coordinates are omitted, the tooltip will be shown near the mouse cursor. X and Y can be expressions.</param>
        /// <param name="aY">The Y position (see <paramref name="X"/>).</param>
        /// <param name="aID">Omit this parameter if you don't need multiple tooltips to appear simultaneously. Otherwise, this is a number between 1 and 20 to indicate which tooltip window to operate upon. If unspecified, that number is 1 (the first).</param>
        public static bool ToolTip(string aText, System.Nullable<int> aX, System.Nullable<int> aY, System.Nullable<int> aID)
        {
            
            int window_index = 0;
            if (aID.HasValue) { window_index = (int)(aID) - 1; }
            if (window_index < 0 | window_index >= 50) return false;
            IntPtr tip_hwnd = g_hWndToolTip[window_index];

            if (aText == "")
            {
                if (tip_hwnd != IntPtr.Zero & Windows.Windows.IsWindow(tip_hwnd))
                {
                    Windows.Windows.SendMessage(tip_hwnd, 0x10, IntPtr.Zero, IntPtr.Zero); //WM_Close
                }
                g_hWndToolTip[window_index] = IntPtr.Zero;
                return true;
            }

            Windows.Windows.RECT dtw = new Windows.Windows.RECT();
            Windows.Windows.GetVirtualDesktopRect(out dtw);

            bool one_or_both_coords_unspecified = (!aX.HasValue | !aY.HasValue);
            POINTAPI pt = new POINTAPI();
            POINTAPI pt_cursor = new POINTAPI();

            if (one_or_both_coords_unspecified)
            {
                GetCursorPos(out pt_cursor);
                pt.x = pt_cursor.x + 16;  // Set default spot to be near the mouse cursor.
                pt.y = pt_cursor.y + 16;  // Use 16 to prevent the tooltip from overlapping large cursors.
            }
            Windows.Windows.RECT rect = new Windows.Windows.RECT();

            if ((aX.HasValue | aY.HasValue) & Settings.CoordMode.ToolTip)
            {
                if (!Windows.Windows.GetWindowRect(Windows.Windows.GetForegroundWindow(), out rect))
                    return true;  // Don't bother setting ErrorLevel with this command.
            }
            //else leave all of rect's members initialized to zero.

            // This will also convert from relative to screen coordinates if rect contains non-zero values:
            if (aX.HasValue)
                pt.x = (int)aX + (int)rect.Left;
            if (aY.HasValue)
                pt.y = (int)aY + (int)rect.Top;

            System.Windows.Forms.CreateParams cp = new System.Windows.Forms.CreateParams();
            // fill in our create params
            cp.ClassName = API.TOOLTIPS_CLASS;
            cp.Style = API.TTS_NOPREFIX | API.TTS_ALWAYSTIP;

            // create and fill in the tool tip info
            API.TOOLINFO ti = new API.TOOLINFO();
            
            ti.uFlags = API.TTF_TRANSPARENT | API.TTF_TRACK | API.TTF_ABSOLUTE;
            ti.lpszText = aText;
            ti.cbSize = Marshal.SizeOf(ti);

            if (tip_hwnd == IntPtr.Zero | !Windows.Windows.IsWindow(tip_hwnd))
            {
                // create the tooltip window
                NativeWindow toolTip = new NativeWindow();
                toolTip.CreateHandle(cp);
                tip_hwnd = g_hWndToolTip[window_index] = toolTip.Handle;
                API.SendMessage(tip_hwnd, API.TTM_SETMAXTIPWIDTH, 0, Windows.Windows.GetSystemMetrics(0));
                //API.SendMessage(tip_hwnd, API.TTM_ADDTOOL, 0, ref ti);

                if (Marshal.SystemDefaultCharSize == 1)
                    API.SendMessage(tip_hwnd, API.TTM_ADDTOOLA, 0, ref ti); //Win9x machines
                else
                    API.SendMessage(tip_hwnd, API.TTM_ADDTOOLW, 0, ref ti); //WinNT machines

                API.SendMessage(tip_hwnd, API.TTM_TRACKPOSITION, 0, (pt.y << 16) + pt.x);
                API.SendMessage(tip_hwnd, API.TTM_TRACKACTIVATE, 1, ref ti);
            }
            if (Marshal.SystemDefaultCharSize == 1)
                API.SendMessage(tip_hwnd, API.TTM_UPDATETIPTEXTA, 0, ref ti); //Win9x machines
            else
                API.SendMessage(tip_hwnd, API.TTM_UPDATETIPTEXTW, 0, ref ti); //WinNT machines

            Windows.Windows.RECT ttw = new Windows.Windows.RECT();
            Windows.Windows.GetWindowRect(tip_hwnd, out ttw); // Must be called this late to ensure the tooltip has been created by above.
            int tt_width = ttw.Right - ttw.Left;
            int tt_height = ttw.Bottom - ttw.Top;

            // v1.0.21: Revised for multi-monitor support.  I read somewhere that dtw.left can be negative (perhaps
            // if the secondary monitor is to the left of the primary).  So it seems best to assume it is possible:
            if (pt.x + tt_width >= dtw.Right)
                pt.x = dtw.Right - tt_width - 1;
            if (pt.y + tt_height >= dtw.Bottom)
                pt.y = dtw.Bottom - tt_height - 1;

            if (one_or_both_coords_unspecified)
            {
                // Since Tooltip is being shown at the cursor's coordinates, try to ensure that the above
                // adjustment doesn't result in the cursor being inside the tooltip's window boundaries,
                // since that tends to cause problems such as blocking the tray area (which can make a
                // tootip script impossible to terminate).  Normally, that can only happen in this case
                // (one_or_both_coords_unspecified == true) when the cursor is near the buttom-right
                // corner of the screen (unless the mouse is moving more quickly than the script's
                // ToolTip update-frequency can cope with, but that seems inconsequential since it
                // will adjust when the cursor slows down):
                ttw.Left = pt.x;
                ttw.Top = pt.y;
                ttw.Right = ttw.Left + tt_width;
                ttw.Bottom = ttw.Top + tt_height;
                if ((pt_cursor.x >= ttw.Left) & (pt_cursor.x <= ttw.Right) & (pt_cursor.y >= ttw.Top) & (pt_cursor.y <= ttw.Bottom))
                {
                    // Push the tool tip to the upper-left side, since normally the only way the cursor can
                    // be inside its boundaries (when one_or_both_coords_unspecified == true) is when the
                    // cursor is near the bottom right corner of the screen.
                    pt.x = pt_cursor.x - tt_width - 3;    // Use a small offset since it can't overlap the cursor
                    pt.y = pt_cursor.y - tt_height - 3;   // when pushed to the the upper-left side of it.
                }
            }
            API.SendMessage(tip_hwnd, API.TTM_TRACKPOSITION, 0, (pt.y << 16) + pt.x);
            //And do a TTM_TRACKACTIVATE even if the tooltip window already existed upon entry to this function,
            //so that in case it was hidden or dismissed while its HWND still exists, it will be shown again:
            API.SendMessage(tip_hwnd, API.TTM_TRACKACTIVATE, 1, ref ti);
            return true;
        }

        /// <summary>
        /// Creates a balloon message window near the tray icon. Requires Windows 2000/XP or later.
        /// </summary>
        /// <param name="Title">
        /// <para>If all parameters are omitted, any TrayTip window currently displayed will be removed.</para>
        /// <para>Otherwise, this parameter is the title of the window, which can be up to 73 characters long (characters beyond this length are not shown).</para>
        /// <para>If Title is blank, the title line will be entirely omitted from the balloon window, making it vertically shorter.</para>
        /// </param>
        /// <param name="Text">
        /// <para>If this parameter is omitted or blank, any TrayTip window currently displayed will be removed.</para>
        /// <para>Otherwise, specify the message to display, which appears beneath Title. Only the first 265 characters of Text will be displayed. Carriage return (`r) or linefeed (`n) may be used to create multiple lines of text. For example: Line1`nLine2</para>
        /// <para>If Text is long, it can be broken up into several shorter lines by means of a continuation section, which might improve readability and maintainability.</para>
        /// </param>
        /// <param name="Seconds">
        /// <para>The approximate number of seconds to display the window, after which it will be automatically removed by the OS. Specifying a number less than 10 or greater than 30 will usually cause the minimum (10) or maximum (30) display time to be used instead. If blank or omitted, the minimum time will usually be used. This parameter can be an expression.</para>
        /// <para>The actual timeout may vary from the one specified. Microsoft explains, "if the user does not appear to be using the computer, the system does not count this time towards the timeout." (Technical details here.) Therefore, to have precise control over how long the TrayTip is displayed, use the Sleep command followed by TrayTip with no parameters, or use SetTimer as illustrated in the Examples section below.</para>
        /// </param>
        /// <param name="Options">
        /// <para>Specify one of the following digits to have a small icon appear to the left of Title:</para>
        /// <list type="">
        /// <item>1: Info icon</item>
        /// <item>2: Warning icon</item>
        /// <item>3: Error icon</item>
        /// <para>If omitted, it defaults to 0, which is no icon.</para>
        /// </list>
        /// </param>
        public static void TrayTip(string Title, string Text, int Seconds, int Options)
        {
            Settings.Tray.ShowBalloonTip(Seconds * 1000, Title, Text,
                Options == 1 ? ToolTipIcon.Info : Options == 2 ? ToolTipIcon.Warning :
                Options == 3 ? ToolTipIcon.Error : ToolTipIcon.None);
        }

        #region API class

        internal sealed class API
        {
            public const int WM_USER = 0x0400;
            public const int TTM_SETDELAYTIME = API.WM_USER + 3;
            public const int TTM_RELAYEVENT = WM_USER + 7;
            public const int TTM_TRACKACTIVATE = WM_USER + 17;
            public const int TTM_TRACKPOSITION = WM_USER + 18;
            public const int TTM_SETMAXTIPWIDTH = 0x418;

            public const string TOOLTIPS_CLASS = "tooltips_class32";
            public const int TTS_ALWAYSTIP = 0x01;
            public const int TTS_NOPREFIX = 0x02;
            public const int TTS_BALLOON = 0x40;
            public const int WS_POPUP = unchecked((int)0x80000000);
            public const int TTF_SUBCLASS = 0x0010;
            public const int TTF_TRANSPARENT = 0x0100;
            public const int TTF_CENTERTIP = 0x0002;
            public const int TTF_ABSOLUTE = 0x0080;
            public const int TTF_TRACK = 0x0020;
            public const int TTM_ADDTOOLA = 1028;
            public const int TTM_ADDTOOLW = 1074;
            public const int TTM_UPDATETIPTEXTA = 0x40C;
            public const int TTM_UPDATETIPTEXTW = 0x439;
            public const int ICC_WIN95_CLASSES = 0x000000FF;
            public const int SWP_NOSIZE = 0x0001;
            public const int SWP_NOMOVE = 0x0002;
            public const int SWP_NOACTIVATE = 0x0010;
            public static readonly IntPtr HWND_TOPMOST = new IntPtr(-1);

            [StructLayout(LayoutKind.Sequential)]
            public struct TOOLINFO
            {
                public int cbSize;
                public int uFlags;
                public IntPtr hwnd;
                public IntPtr uId;
                public IronAHK.Rusty.Windows.Windows.RECT rect;
                public IntPtr hinst;
                [MarshalAs(UnmanagedType.LPTStr)]
                public string lpszText;
                public uint lParam;
            }

            [DllImport("User32", SetLastError = true)]
            public static extern int SendMessage(
            IntPtr hWnd,
            int Msg,
            int wParam,
            ref TOOLINFO lParam);

            [DllImport("User32", SetLastError = true)]
            public static extern int SendMessage(
            IntPtr hWnd,
            int Msg,
            int wParam,
            int lParam);

            [DllImport("User32", SetLastError = true)]
            public static extern bool SetWindowPos(
            IntPtr hWnd,
            IntPtr hWndInsertAfter,
            int X,
            int Y,
            int cx,
            int cy,
            int uFlags);
        }
        #endregion
    }
}