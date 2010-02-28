using System;
using System.Collections.Generic;
using System.Threading;
using System.Windows.Forms;

namespace IronAHK.Rusty
{
    partial class Core
    {
        // TODO: organise Keyboard.cs

        /// <summary>
        /// Creates or modifies a hotkey.
        /// </summary>
        /// <param name="KeyName">Name of the hotkey's activation key including any modifier symbols.</param>
        /// <param name="Label">The name of the function or label whose contents will be executed when the hotkey is pressed.
        /// This parameter can be left blank if <paramref name="KeyName"/> already exists as a hotkey,
        /// in which case its label will not be changed. This is useful for changing only the <paramref name="Options"/>.
        /// </param>
        /// <param name="Options">
        /// <list type="bullet">
        /// <item><term>UseErrorLevel</term>: <description>skips the warning dialog and sets <see cref="ErrorLevel"/> if there was a problem.</description></item>
        /// <item><term>On</term>: <description>the hotkey becomes enabled.</description></item>
        /// <item><term>Off</term>: <description>the hotkey becomes disabled.</description></item>
        /// <item><term>Toggle</term>: <description>the hotkey is set to the opposite state (enabled or disabled).</description></item>
        /// </list>
        /// </param>
        public static void Hotkey(string KeyName, string Label, string Options)
        {
            #region Initialise

            if (keyboardHook == null)
            {
                if (Environment.OSVersion.Platform == PlatformID.Win32NT)
                    keyboardHook = new Windows.KeyboardHook();
                else
                    throw new NotImplementedException(); // TODO: Linux hotkeys
            }

            if (hotkeys == null)
                hotkeys = new Dictionary<string, HotkeyDefinition>();

            #endregion

            #region Options

            bool? enabled = true;
            bool error = false;

            foreach (string option in ParseOptions(Options))
            {
                switch (option.ToLowerInvariant())
                {
                    case Keyword_On: enabled = true; break;
                    case Keyword_Off: enabled = false; break;
                    case Keyword_Toggle: enabled = null; break;
                    case Keyword_UseErrorLevel: error = true; break;

                    default:
                        switch (option[0])
                        {
                            case 'B':
                            case 'b':
                            case 'P':
                            case 'p':
                            case 'T':
                            case 't':
                                break;

                            default:
                                ErrorLevel = 10;
                                break;
                        }
                        break;
                }
            }

            #endregion

            #region Modify

            HotkeyDefinition key;

            try { key = HotkeyDefinition.Parse(KeyName); }
#if !DEBUG
            catch (Exception)
            {
                ErrorLevel = 2;
                if (!error)
                    throw new ArgumentException();
                return;
            }
#endif
            finally { }

            string id = KeyName;

            if (hotkeys.ContainsKey(id))
            {
                if (enabled == null)
                    hotkeys[id].Enabled = !hotkeys[id].Enabled;
                else
                    hotkeys[id].Enabled = enabled == true;

                switch (Label.ToLowerInvariant())
                {
                    case Keyword_On: hotkeys[id].Enabled = true; break;
                    case Keyword_Off: hotkeys[id].Enabled = true; break;
                    case Keyword_Toggle: hotkeys[id].Enabled = !hotkeys[id].Enabled; break;
                }
            }
            else
            {
                try
                {
                    var method = FindLocalMethod(Label);
                    if (method == null)
                        throw new ArgumentNullException();
                    key.Proc = (GenericFunction)Delegate.CreateDelegate(typeof(GenericFunction), method);
                }
                catch (Exception)
                {
                    ErrorLevel = 1;
                    if (!error)
                        throw new ArgumentException();
                    return;
                }

                hotkeys.Add(id, key);
                keyboardHook.Add(key);
            }

            #endregion
        }

        /// <summary>
        /// Creates a hotstring.
        /// </summary>
        /// <param name="Options"></param>
        /// <param name="Sequence"></param>
        public static void Hotstring(string Options, string Sequence)
        {

        }

        /// <summary>
        /// Disables or enables the user's ability to interact with the computer via keyboard and mouse.
        /// </summary>
        /// <param name="Mode">
        /// <para>Mode 1: One of the following words:</para>
        /// <list type="">
        /// <item>On: The user is prevented from interacting with the computer (mouse and keyboard input has no effect).</item>
        /// <item>Off: Input is re-enabled.</item>
        /// </list>
        /// <para>Mode 2 (has no effect on Windows 9x): This mode operates independently of the other two. For example, BlockInput On will continue to block input until BlockInput Off is used, even if one of the below is also in effect.</para>
        /// <list type="">
        /// <item>Send: The user's keyboard and mouse input is ignored while a Send or SendRaw is in progress (the traditional SendEvent mode only). This prevents the user's keystrokes from disrupting the flow of simulated keystrokes. When the Send finishes, input is re-enabled (unless still blocked by a previous use of BlockInput On).</item>
        /// <item>Mouse: The user's keyboard and mouse input is ignored while a Click, MouseMove, MouseClick, or MouseClickDrag is in progress (the traditional SendEvent mode only). This prevents the user's mouse movements and clicks from disrupting the simulated mouse events. When the mouse command finishes, input is re-enabled (unless still blocked by a previous use of BlockInput On).</item>
        /// <item>SendAndMouse: A combination of the above two modes.</item>
        /// <item>Default: Turns off both the Send and the Mouse modes, but does not change the current state of input blocking. For example, if BlockInput On is currently in effect, using BlockInput Default will not turn it off.</item>
        /// </list>
        /// <para>Mode 3 (has no effect on Windows 9x; requires v1.0.43.11+): This mode operates independently of the other two. For example, if BlockInput On and BlockInput MouseMove are both in effect, mouse movement will be blocked until both are turned off.</para>
        /// <list type="">
        /// <item>MouseMove: The mouse cursor will not move in response to the user's physical movement of the mouse (DirectInput applications are a possible exception). When a script first uses this command, the mouse hook is installed (if it is not already). In addition, the script becomes persistent, meaning that ExitApp should be used to terminate it. The mouse hook will stay installed until the next use of the Suspend or Hotkey command, at which time it is removed if not required by any hotkeys or hotstrings (see #Hotstring NoMouse).</item>
        /// <item>MouseMoveOff: Allows the user to move the mouse cursor.</item>
        /// </list>
        /// </param>
        public static void BlockInput(string Mode)
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
        /// <see cref="Control"/>
        /// </summary>
        public static void ControlSendRaw(string Control, string Keys, string WinTitle, string WinText, string ExcludeTitle, string ExcludeText)
        {

        }

        /// <summary>
        /// Unlike the GetKeyState command -- which returns D for down and U for up -- this function returns (1) if the key is down and (0) if it is up.
        /// If <paramref name="KeyName"/> is invalid, an empty string is returned.
        /// </summary>
        /// <param name="KeyName">Use autohotkey definition or virtual key starting from "VK"</param>
        /// <param name="Mode"></param>
        public static string GetKeyState(string KeyName, string Mode)
        {
            int VK = 0;
            if (KeyName.Contains("VK"))
            {
                try
                {
                    VK = Convert.ToInt32(Core.SubStr(KeyName.Trim(), 3, KeyName.Trim().Length - 2));
                }
                catch
                {
                    return string.Empty;
                }
            }
            else if (KeyName.Trim().Length == 1)
            {
                VK = Windows.VkKeyScan(Convert.ToChar(KeyName.Trim().ToLowerInvariant()));
            }
            else
            {
                switch (KeyName.Trim().ToLowerInvariant())
                {
                    //keyboard
                    case "space": VK = (int)System.Windows.Forms.Keys.Space; break;
                    case "tab": VK = (int)System.Windows.Forms.Keys.Tab; break;
                    case "enter":
                    case "return": VK = (int)System.Windows.Forms.Keys.Return; break;
                    case "escape":
                    case "esc": VK = (int)System.Windows.Forms.Keys.Escape; break;
                    case "backspace":
                    case "bs": VK = (int)System.Windows.Forms.Keys.Back; break;
                    case "delete":
                    case "del": VK = (int)System.Windows.Forms.Keys.Delete; break;
                    case "insert":
                    case "ins": VK = (int)System.Windows.Forms.Keys.Insert; break;
                    case "home": VK = (int)System.Windows.Forms.Keys.Home; break;
                    case "end": VK = (int)System.Windows.Forms.Keys.End; break;
                    case "pgup":
                    case "pageup": VK = (int)System.Windows.Forms.Keys.PageUp; break;
                    case "pgdn":
                    case "pagedown": VK = (int)System.Windows.Forms.Keys.PageDown; break;
                    case "up": VK = (int)System.Windows.Forms.Keys.Up; break;
                    case "down": VK = (int)System.Windows.Forms.Keys.Down; break;
                    case "left": VK = (int)System.Windows.Forms.Keys.Left; break;
                    case "right": VK = (int)System.Windows.Forms.Keys.Right; break;
                    case "scrolllock": VK = (int)System.Windows.Forms.Keys.Scroll; break;
                    case "capslock": VK = (int)System.Windows.Forms.Keys.CapsLock; break;
                    case "numlock": VK = (int)System.Windows.Forms.Keys.NumLock; break;
                    case "numpad0": VK = (int)System.Windows.Forms.Keys.NumPad0; break;
                    case "numpadins": VK = (int)System.Windows.Forms.Keys.Insert; break;
                    case "numpad1": VK = (int)System.Windows.Forms.Keys.NumPad1; break;
                    case "numpadend": VK = (int)System.Windows.Forms.Keys.End; break;
                    case "numpad2": VK = (int)System.Windows.Forms.Keys.NumPad2; break;
                    case "numpaddown": VK = (int)System.Windows.Forms.Keys.Down; break;
                    case "numpad3": VK = (int)System.Windows.Forms.Keys.NumPad3; break;
                    case "numpadpgdn": VK = (int)System.Windows.Forms.Keys.PageDown; break;
                    case "numpad4": VK = (int)System.Windows.Forms.Keys.NumPad4; break;
                    case "numpadleft": VK = (int)System.Windows.Forms.Keys.Left; break;
                    case "numpad5": VK = (int)System.Windows.Forms.Keys.NumPad5; break;
                    case "numpadclear": VK = (int)System.Windows.Forms.Keys.Clear; break;
                    case "numpad6": VK = (int)System.Windows.Forms.Keys.NumPad6; break;
                    case "numpadright": VK = (int)System.Windows.Forms.Keys.Right; break;
                    case "numpad7": VK = (int)System.Windows.Forms.Keys.NumPad7; break;
                    case "numpadhome": VK = (int)System.Windows.Forms.Keys.Home; break;
                    case "numpad8": VK = (int)System.Windows.Forms.Keys.NumPad8; break;
                    case "numpadup": VK = (int)System.Windows.Forms.Keys.Up; break;
                    case "numpad9": VK = (int)System.Windows.Forms.Keys.NumPad9; break;
                    case "numpadpgup": VK = (int)System.Windows.Forms.Keys.PageUp; break;
                    case "numpaddot": VK = (int)System.Windows.Forms.Keys.Decimal; break;
                    case "numpaddel": VK = (int)System.Windows.Forms.Keys.Delete; break;
                    case "numpaddiv": VK = (int)System.Windows.Forms.Keys.Divide; break;
                    case "numpadmult": VK = (int)System.Windows.Forms.Keys.Multiply; break;
                    case "numpadadd": VK = (int)System.Windows.Forms.Keys.Add; break;
                    case "numpadsub": VK = (int)System.Windows.Forms.Keys.Subtract; break;
                    case "numpadenter": VK = (int)System.Windows.Forms.Keys.Enter; break;
                    case "f1": VK = (int)System.Windows.Forms.Keys.F1; break;
                    case "f2": VK = (int)System.Windows.Forms.Keys.F2; break;
                    case "f3": VK = (int)System.Windows.Forms.Keys.F3; break;
                    case "f4": VK = (int)System.Windows.Forms.Keys.F4; break;
                    case "f5": VK = (int)System.Windows.Forms.Keys.F5; break;
                    case "f6": VK = (int)System.Windows.Forms.Keys.F6; break;
                    case "f7": VK = (int)System.Windows.Forms.Keys.F7; break;
                    case "f8": VK = (int)System.Windows.Forms.Keys.F8; break;
                    case "f9": VK = (int)System.Windows.Forms.Keys.F9; break;
                    case "f10": VK = (int)System.Windows.Forms.Keys.F10; break;
                    case "f11": VK = (int)System.Windows.Forms.Keys.F11; break;
                    case "f12": VK = (int)System.Windows.Forms.Keys.F12; break;
                    case "f13": VK = (int)System.Windows.Forms.Keys.F13; break;
                    case "f14": VK = (int)System.Windows.Forms.Keys.F14; break;
                    case "f15": VK = (int)System.Windows.Forms.Keys.F15; break;
                    case "f16": VK = (int)System.Windows.Forms.Keys.F16; break;
                    case "f17": VK = (int)System.Windows.Forms.Keys.F17; break;
                    case "f18": VK = (int)System.Windows.Forms.Keys.F18; break;
                    case "f19": VK = (int)System.Windows.Forms.Keys.F19; break;
                    case "f20": VK = (int)System.Windows.Forms.Keys.F20; break;
                    case "f21": VK = (int)System.Windows.Forms.Keys.F21; break;
                    case "f22": VK = (int)System.Windows.Forms.Keys.F22; break;
                    case "f23": VK = (int)System.Windows.Forms.Keys.F23; break;
                    case "f24": VK = (int)System.Windows.Forms.Keys.F24; break;
                    case "appskey": VK = (int)System.Windows.Forms.Keys.Apps; break;
                    case "lwin": VK = (int)System.Windows.Forms.Keys.LWin; break;
                    case "rwin": VK = (int)System.Windows.Forms.Keys.RWin; break;
                    case "ctrl":
                    case "control": VK = (int)System.Windows.Forms.Keys.ControlKey; break;
                    case "alt": VK = (int)System.Windows.Forms.Keys.Menu; break;
                    case "shift": VK = (int)System.Windows.Forms.Keys.ShiftKey; break;
                    case "lctrl":
                    case "lcontrol": VK = (int)System.Windows.Forms.Keys.LControlKey; break;
                    case "rctrl":
                    case "rcontrol": VK = (int)System.Windows.Forms.Keys.RControlKey; break;
                    case "lshift": VK = (int)System.Windows.Forms.Keys.LShiftKey; break;
                    case "rshift": VK = (int)System.Windows.Forms.Keys.RShiftKey; break;
                    case "lalt": VK = (int)System.Windows.Forms.Keys.LMenu; break;
                    case "ralt": VK = (int)System.Windows.Forms.Keys.RMenu; break;
                    case "printscreen": VK = (int)System.Windows.Forms.Keys.PrintScreen; break;
                    case "ctrlbreak": VK = (int)System.Windows.Forms.Keys.Cancel; break;
                    case "pause":
                    case "break": VK = (int)System.Windows.Forms.Keys.Pause; break;
                    case "help": VK = (int)System.Windows.Forms.Keys.Help; break;
                    case "sleep": VK = (int)System.Windows.Forms.Keys.Sleep; break;
                    case "browser_back": VK = (int)System.Windows.Forms.Keys.BrowserBack; break;
                    case "browser_forward": VK = (int)System.Windows.Forms.Keys.BrowserForward; break;
                    case "browser_refresh": VK = (int)System.Windows.Forms.Keys.BrowserRefresh; break;
                    case "browser_stop": VK = (int)System.Windows.Forms.Keys.BrowserStop; break;
                    case "browser_search": VK = (int)System.Windows.Forms.Keys.BrowserSearch; break;
                    case "browser_favorites": VK = (int)System.Windows.Forms.Keys.BrowserFavorites; break;
                    case "browser_home": VK = (int)System.Windows.Forms.Keys.BrowserHome; break;
                    case "volume_mute": VK = (int)System.Windows.Forms.Keys.VolumeMute; break;
                    case "volume_down": VK = (int)System.Windows.Forms.Keys.VolumeDown; break;
                    case "volume_up": VK = (int)System.Windows.Forms.Keys.VolumeUp; break;
                    case "media_next": VK = (int)System.Windows.Forms.Keys.MediaNextTrack; break;
                    case "media_prev": VK = (int)System.Windows.Forms.Keys.MediaPreviousTrack; break;
                    case "media_stop": VK = (int)System.Windows.Forms.Keys.MediaStop; break;
                    case "media_play_pause": VK = (int)System.Windows.Forms.Keys.MediaPlayPause; break;
                    case "launch_mail": VK = (int)System.Windows.Forms.Keys.LaunchMail; break;
                    case "launch_media": break; //??
                    case "launch_app1": VK = (int)System.Windows.Forms.Keys.LaunchApplication1; break;
                    case "launch_app2": VK = (int)System.Windows.Forms.Keys.LaunchApplication2; break;

                    //mouse
                    case "lbutton": VK = (int)System.Windows.Forms.Keys.LButton; break;
                    case "rbutton": VK = (int)System.Windows.Forms.Keys.RButton; break;
                    case "wd":
                    case "wheeldown":
                    case "wu":
                    case "wheelup":
                    case "wheelleft":
                    case "wheelright": break; //??
                    case "xbutton1": VK = (int)System.Windows.Forms.Keys.XButton1; break;
                    case "xbutton2": VK = (int)System.Windows.Forms.Keys.XButton2; break;

                }
            }

            if (VK == 0) return string.Empty;

            short result = Windows.GetKeyState(VK);

            switch (result)
            {
                case 0:
                    // Not pressed and not toggled on.
                    return "0";

                case 1:
                    // Not pressed, but toggled on
                    if (Mode.Trim().ToLowerInvariant() == "t") return "1";
                    return "0";

                default:
                    // Pressed (and may be toggled on)
                    if (Mode.Trim().ToLowerInvariant() == "t")
                    {
                        switch (Convert.ToByte(result))
                        {
                            case 0: return "0";
                            case 1: return "1";
                        }
                    }
                    else
                        return "1";
                    break;
            }
            return string.Empty;
        }

        /// <summary>
        /// See <see cref="GetKeyState"/>.
        /// </summary>
        /// <param name="OutputVar"></param>
        /// <param name="WhichKey"></param>
        /// <param name="Mode"></param>
        public static void GetKeyStateX(out string OutputVar, string WhichKey, string Mode)
        {
            OutputVar = null;
        }

        /// <summary>
        /// Waits for a key or mouse/joystick button to be released or pressed down.
        /// </summary>
        /// <param name="KeyName">
        /// <para>This can be just about any single character from the keyboard or one of the key names from the key list, such as a mouse/joystick button. Joystick attributes other than buttons are not supported.</para>
        /// <para>An explicit virtual key code such as vkFF may also be specified. This is useful in the rare case where a key has no name and produces no visible character when pressed. Its virtual key code can be determined by following the steps at the bottom fo the key list page.</para>
        /// </param>
        /// <param name="Options">
        /// <para>If this parameter is blank, the command will wait indefinitely for the specified key or mouse/joystick button to be physically released by the user. However, if the keyboard hook is not installed and KeyName is a keyboard key released artificially by means such as the Send command, the key will be seen as having been physically released. The same is true for mouse buttons when the mouse hook is not installed.</para>
        /// <para>Options: A string of one or more of the following letters (in any order, with optional spaces in between):</para>
        /// <list type="">
        /// <item>D: Wait for the key to be pushed down.</item>
        /// <item>L: Check the logical state of the key, which is the state that the OS and the active window believe the key to be in (not necessarily the same as the physical state). This option is ignored for joystick buttons.</item>
        /// <item>T: Timeout (e.g. T3). The number of seconds to wait before timing out and setting ErrorLevel to 1. If the key or button achieves the specified state, the command will not wait for the timeout to expire. Instead, it will immediately set ErrorLevel to 0 and the script will continue executing.</item>
        /// </list>
        /// <para>The timeout value can be a floating point number such as 2.5, but it should not be a hexadecimal value such as 0x03.</para>
        /// </param>
        public static void KeyWait(string KeyName, string Options)
        {

        }

        /// <summary>
        /// Waits for the user to type a string (not supported on Windows 9x: it does nothing).
        /// </summary>
        /// <param name="OutputVar">
        /// <para>The name of the variable in which to store the text entered by the user (by default, artificial input is also captured).</para>
        /// <para>If this and the other parameters are omitted, any Input in progress in another thread is terminated and its ErrorLevel is set to the word NewInput. By contrast, the ErrorLevel of the current command will be set to 0 if it terminated a prior Input, or 1 if there was no prior Input to terminate.</para>
        /// <para>OutputVar does not store keystrokes per se. Instead, it stores characters produced by keystrokes according to the active window's keyboard layout/language. Consequently, keystrokes that do not produce characters (such as PageUp and Escape) are not stored (though they can be recognized via the EndKeys parameter below).</para>
        /// <para>Whitespace characters such as TAB (`t) are stored literally. ENTER is stored as linefeed (`n).</para>
        /// </param>
        /// <param name="Options">
        /// <para>A string of zero or more of the following letters (in any order, with optional spaces in between):</para>
        /// <para>B: Backspace is ignored. Normally, pressing backspace during an Input will remove the most recently pressed character from the end of the string. Note: If the input text is visible (such as in an editor) and the arrow keys or other means are used to navigate within it, backspace will still remove the last character rather than the one behind the caret (insertion point).</para>
        /// <para>C: Case sensitive. Normally, MatchList is not case sensitive (in versions prior to 1.0.43.03, only the letters A-Z are recognized as having varying case, not letters like ü/Ü).</para>
        /// <para>I: Ignore input generated by any AutoHotkey script, such as the SendEvent command. However, the SendInput and SendPlay methods are always ignored, regardless of this setting.</para>
        /// <para>L: Length limit (e.g. L5). The maximum allowed length of the input. When the text reaches this length, the Input will be terminated and ErrorLevel will be set to the word Max unless the text matches one of the MatchList phrases, in which case ErrorLevel is set to the word Match. If unspecified, the length limit is 16383, which is also the absolute maximum.</para>
        /// <para>M: Modified keystrokes such as Control-A through Control-Z are recognized and transcribed if they correspond to real ASCII characters. Consider this example, which recognizes Control-C:</para>
        /// <code>Transform, CtrlC, Chr, 3 ; Store the character for Ctrl-C in the CtrlC var. 
        /// Input, OutputVar, L1 M
        /// if OutputVar = %CtrlC%
        /// MsgBox, You pressed Control-C.</code>
        /// <para>ExitAppNote: The characters Ctrl-A through Ctrl-Z correspond to Chr(1) through Chr(26). Also, the M option might cause some keyboard shortcuts such as Ctrl-LeftArrow to misbehave while an Input is in progress.</para>
        /// <para>T: Timeout (e.g. T3). The number of seconds to wait before terminating the Input and setting ErrorLevel to the word Timeout. If the Input times out, OutputVar will be set to whatever text the user had time to enter. This value can be a floating point number such as 2.5.</para>
        /// <para>V: Visible. Normally, the user's input is blocked (hidden from the system). Use this option to have the user's keystrokes sent to the active window.</para>
        /// <para>*: Wildcard (find anywhere). Normally, what the user types must exactly match one of the MatchList phrases for a match to occur. Use this option to find a match more often by searching the entire length of the input text.</para>
        /// </param>
        /// <param name="EndKeys">
        /// <para>A list of zero or more keys, any one of which terminates the Input when pressed (the EndKey itself is not written to OutputVar). When an Input is terminated this way, ErrorLevel is set to the word EndKey followed by a colon and the name of the EndKey. Examples: <code>EndKey:.
        /// EndKey:Escape</code></para>
        /// <para>The EndKey list uses a format similar to the Send command. For example, specifying {Enter}.{Esc} would cause either ENTER, period (.), or ESCAPE to terminate the Input. To use the braces themselves as end keys, specify {{} and/or {}}.</para>
        /// <para>To use Control, Alt, or Shift as end-keys, specify the left and/or right version of the key, not the neutral version. For example, specify {LControl}{RControl} rather than {Control}.</para>
        /// <para>Although modified keys such as Control-C (^c) are not supported, certain keys that require the shift key to be held down -- namely punctuation marks such as ?!:@&amp;{} -- are supported in v1.0.14+.</para>
        /// <para>An explicit virtual key code such as {vkFF} may also be specified. This is useful in the rare case where a key has no name and produces no visible character when pressed. Its virtual key code can be determined by following the steps at the bottom fo the key list page.</para>
        /// </param>
        /// <param name="MatchList">
        /// <para>A comma-separated list of key phrases, any of which will cause the Input to be terminated (in which case ErrorLevel will be set to the word Match). The entirety of what the user types must exactly match one of the phrases for a match to occur (unless the * option is present). In addition, any spaces or tabs around the delimiting commas are significant, meaning that they are part of the match string. For example, if MatchList is "ABC , XYZ ", the user must type a space after ABC or before XYZ to cause a match.</para>
        /// <para>Two consecutive commas results in a single literal comma. For example, the following would produce a single literal comma at the end of string: "string1,,,string2". Similarly, the following list contains only a single item with a literal comma inside it: "single,,item".</para>
        /// <para>Because the items in MatchList are not treated as individual parameters, the list can be contained entirely within a variable. In fact, all or part of it must be contained in a variable if its length exceeds 16383 since that is the maximum length of any script line. For example, MatchList might consist of %List1%,%List2%,%List3% -- where each of the variables contains a large sub-list of match phrases.</para>
        /// </param>
        public static void Input(out string OutputVar, string Options, string EndKeys, string MatchList)
        {
            OutputVar = null;
        }

        /// <summary>
        /// Sends simulated keystrokes and mouse clicks to the active window.
        /// </summary>
        /// <param name="Keys">The sequence of keys to send.</param>
        public static void Send(string Keys)
        {
            SendKeys.Send(Keys);
        }

        /// <summary>
        /// Sends simulated keystrokes and mouse clicks to the active window.
        /// </summary>
        /// <param name="Keys">The sequence of keys to send.</param>
        public static void SendEvent(string Keys)
        {
            SendKeys.Send(Keys);
        }

        /// <summary>
        /// Sends simulated keystrokes and mouse clicks to the active window.
        /// </summary>
        /// <param name="Keys">The sequence of keys to send.</param>
        /// <remarks>
        /// <para>SendInput is generally the preferred method to send keystrokes and mouse clicks because of its superior speed and reliability. Under most conditions, SendInput is nearly instantaneous, even when sending long strings. Since SendInput is so fast, it is also more reliable because there is less opportunity for some other window to pop up unexpectedly and intercept the keystrokes. Reliability is further improved by the fact that anything the user types during a SendInput is postponed until afterward.</para>
        /// <para>Unlike the other sending modes, the operating system limits SendInput to about 5000 characters (this may vary depending on the operating system's version and performance settings). Characters and events beyond this limit are not sent.</para>
        /// <para>Note: SendInput ignores SetKeyDelay because the operating system does not support a delay in this mode. However, when SendInput reverts to SendEvent under the conditions described below, it uses SetKeyDelay -1, 0 (unless SendEvent's KeyDelay is "-1,-1", in which case "-1,-1" is used). When SendInput reverts to SendPlay, it uses SendPlay's KeyDelay.</para>
        /// <para>If a script other than the one executing SendInput has a low-level keyboard hook installed, SendInput automatically reverts to SendEvent (or SendPlay if SendMode InputThenPlay is in effect). This is done because the presence of an external hook disables all of SendInput's advantages, making it inferior to both SendPlay and SendEvent. However, since SendInput is unable to detect a low-level hook in programs other than AutoHotkey v1.0.43+, it will not revert in these cases, making it less reliable than SendPlay/Event.</para>
        /// <para>When SendInput sends mouse clicks by means such as {Click}, and CoordMode Mouse, Relative is in effect (the default), every click will be relative to the window that was active at the start of the send. Therefore, if SendInput intentionally activates another window (by means such as alt-tab), the coordinates of subsequent clicks within the same command will be wrong because they will still be relative to the old window rather than the new one.</para>
        /// <para>Windows 95 (and NT4 pre-SP3): SendInput is not supported and will automatically revert to SendEvent (or SendPlay if SendMode InputThenPlay is in effect).</para>
        /// </remarks>
        public static void SendInput(string Keys)
        {
            SendKeys.Send(Keys);
        }

        /// <summary>
        /// Makes Send synonymous with SendInput or SendPlay rather than the default (SendEvent). Also makes Click and MouseMove/Click/Drag use the specified method.
        /// </summary>
        /// <param name="Mode">
        /// <para>Event: This is the starting default used by all scripts. It uses the SendEvent method for Send, SendRaw, Click, and MouseMove/Click/Drag.</para>
        /// <para>Input: Switches to the SendInput method for Send, SendRaw, Click, and MouseMove/Click/Drag. Known limitations:</para>
        /// <list type="">
        /// <item>Windows Explorer ignores SendInput's simulation of certain navigational hotkeys such as Alt+LeftArrow. To work around this, use either SendEvent !{Left} or SendInput {Backspace}.</item>
        /// </list>
        /// <para>InputThenPlay: Same as above except that rather than falling back to Event mode when SendInput is unavailable, it reverts to Play mode (below). This also causes the SendInput command itself to revert to Play mode when SendInput is unavailable.</para>
        /// <para>Play: Switches to the SendPlay method for Send, SendRaw, Click, and MouseMove/Click/Drag.
        /// Known limitations:</para>
        /// <list type="">
        /// <item>Characters that do not exist in the current keyboard layout (such as Ô in English) cannot be sent. To work around this, use SendEvent.</item>
        /// <item>Simulated mouse dragging might have no effect in RichEdit controls (and possibly others) such as those of WordPad and Metapad. To use an alternate mode for a particular drag, follow this example: SendEvent {Click 6, 52, down}{Click 45, 52, up}</item>
        /// <item>Simulated mouse wheel rotation produces movement in only one direction (usually downward, but upward in some applications). Also, wheel rotation might have no effect in applications such as MS Word and Notepad. To use an alternate mode for a particular rotation, follow this example: SendEvent {WheelDown 5}</item>
        /// <item>When using SendMode Play in the auto-execute section (top part of the script), all remapped keys are affected and might lose some of their functionality. See SendPlay remapping limitations for details.</item>
        /// </list>
        /// </param>
        public static void SendMode(string Mode)
        {
            char m = 'e';
            switch (Mode.Trim().ToLowerInvariant())
            {
                case "event": m = 'e'; break;
                case "input": m = 'i'; break;
                case "inputthenplay": m = 't'; break;
                case "play": m = 'p'; break;
            }
            _SendMode = m;
        }

        /// <summary>
        /// Sends simulated keystrokes and mouse clicks to the active window.
        /// </summary>
        /// <param name="Keys">The sequence of keys to send.</param>
        /// <remarks>
        /// <para>SendPlay's biggest advantage is its ability to "play back" keystrokes and mouse clicks in a broader variety of games than the other modes. For example, a particular game may accept hotstrings only when they have the SendPlay option.</para>
        /// <para>Of the three sending modes, SendPlay is the most unusual because it does not simulate keystrokes and mouse clicks per se. Instead, it creates a series of events (messages) that flow directly to the active window (similar to ControlSend, but at a lower level).</para>
        /// <para>Like SendInput, SendPlay's keystrokes do not get interspersed with keystrokes typed by the user. Thus, if the user happens to type something during a SendPlay, those keystrokes are postponed until afterward.</para>
        /// <para>Although SendPlay is considerably slower than SendInput, it is usually faster than the traditional SendEvent mode (even when KeyDelay is -1).</para>
        /// <para>SendPlay is unable to trigger system hotkeys that involve the two Windows keys (LWin and RWin). For example, it cannot display the Start Menu or use Win-R to show the Run dialog.</para>
        /// <para>The Windows keys (LWin and RWin) are automatically blocked during a SendPlay if the keyboard hook is installed. This prevents the Start Menu from appearing if the user accidentally presses a Windows key during the send. By contrast, keys other than LWin and RWin do not need to be blocked because the operating system automatically postpones them until after the SendPlay (via buffering).</para>
        /// <para>SendPlay does not use the standard settings of SetKeyDelay and SetMouseDelay. Instead, it defaults to no delay at all, which can be changed as shown in the following examples:</para>
        /// <code>
        /// SetKeyDelay, 0, 10, Play  ; Note that both 0 and -1 are the same in SendPlay mode.
        /// SetMouseDelay, 10, Play
        /// </code>
        /// <para>SendPlay is unable to turn on or off the Capslock, Numlock, or Scroll-lock keys. Similarly, it is unable to change a key's state as seen by GetKeyState unless the keystrokes are sent to one of the script's own windows. Even then, any changes to the left/right modifier keys (e.g. RControl) can be detected only via their neutral counterparts (e.g. Control). Also, SendPlay has other limitations described on the SendMode page.</para>
        /// <para>Unlike SendInput and SendEvent, the user may interrupt a SendPlay by pressing Control-Alt-Del or Control-Escape. When this happens, the remaining keystrokes are not sent but the script continues executing as though the SendPlay had completed normally.</para>
        /// <para>Although SendPlay can send LWin and RWin events, they are sent directly to the active window rather than performing their native operating system function. To work around this, use SendEvent. For example, SendEvent #r would show the Start Menu's Run dialog.</para>
        /// <para>Unlike SendInput, SendPlay works even on Windows 95 and NT4-pre-SP3.</para>
        /// </remarks>
        public static void SendPlay(string Keys)
        {
            SendKeys.Send(Keys);
        }

        /// <summary>
        /// Sends simulated keystrokes and mouse clicks to the active window.
        /// </summary>
        /// <param name="Keys">The sequence of keys to send.</param>
        public static void SendRaw(string Keys)
        {
            SendKeys.Send(Keys);
        }

        /// <summary>
        /// Sets the delay that will occur after each keystroke sent by Send and ControlSend.
        /// </summary>
        /// <param name="Delay">Time in milliseconds. Use -1 for no delay at all and 0 for the smallest possible delay (however, if the Play parameter is present, both 0 and -1 produce no delay). Leave this parameter blank to retain the current Delay.</param>
        /// <param name="PressDuration">
        /// <para>Certain games and other specialized applications may require a delay inside each keystroke; that is, after the press of the key but before its release.</para>
        /// <para>Use -1 for no delay at all (default) and 0 for the smallest possible delay (however, if the Play parameter is present, both 0 and -1 produce no delay). Omit this parameter to leave the current PressDuration unchanged.</para>
        /// <para>Note: PressDuration also produces a delay after any change to the modifier key state (CTRL, ALT, SHIFT, and WIN) needed to support the keys being sent.</para>
        /// </param>
        /// <param name="Play">The word Play applies the above settings to the SendPlay mode rather than the traditional SendEvent mode. If a script never uses this parameter, the delay is always -1/-1 for SendPlay.</param>
        public static void SetKeyDelay(int Delay, int PressDuration, bool Play)
        {
            _KeyDelay = Delay;
            _KeyPressDuration = PressDuration;
        }

        /// <summary>
        /// Sets the state of the NumLock key. Can also force the key to stay on or off.
        /// </summary>
        /// <param name="Mode">
        /// <para>If this parameter is omitted, the AlwaysOn/Off attribute of the key is removed (if present). Otherwise, specify one of the following words:</para>
        /// <list type="">
        /// <item>On: Turns on the key and removes the AlwaysOn/Off attribute of the key (if present).</item>
        /// <item>Off: Turns off the key and removes the AlwaysOn/Off attribute of the key (if present).</item>
        /// <item>AlwaysOn: Forces the key to stay on permanently (has no effect on Windows 9x).</item>
        /// <item>AlwaysOff: Forces the key to stay off permanently (has no effect on Windows 9x).</item>
        /// </list>
        /// </param>
        public static void SetNumLockState(string Mode)
        {

        }

        /// <summary>
        /// Sets the state of the ScrollLock key. Can also force the key to stay on or off.
        /// </summary>
        /// <param name="Mode">
        /// <para>If this parameter is omitted, the AlwaysOn/Off attribute of the key is removed (if present). Otherwise, specify one of the following words:</para>
        /// <list type="">
        /// <item>On: Turns on the key and removes the AlwaysOn/Off attribute of the key (if present).</item>
        /// <item>Off: Turns off the key and removes the AlwaysOn/Off attribute of the key (if present).</item>
        /// <item>AlwaysOn: Forces the key to stay on permanently (has no effect on Windows 9x).</item>
        /// <item>AlwaysOff: Forces the key to stay off permanently (has no effect on Windows 9x).</item>
        /// </list>
        /// </param>
        public static void SetScrollLockState(string Mode)
        {

        }

        /// <summary>
        /// Sets the state of the Capslock key. Can also force the key to stay on or off.
        /// </summary>
        /// <param name="Mode">
        /// <para>If this parameter is omitted, the AlwaysOn/Off attribute of the key is removed (if present). Otherwise, specify one of the following words:</para>
        /// <list type="">
        /// <item>On: Turns on the key and removes the AlwaysOn/Off attribute of the key (if present).</item>
        /// <item>Off: Turns off the key and removes the AlwaysOn/Off attribute of the key (if present).</item>
        /// <item>AlwaysOn: Forces the key to stay on permanently (has no effect on Windows 9x).</item>
        /// <item>AlwaysOff: Forces the key to stay off permanently (has no effect on Windows 9x).</item>
        /// </list>
        /// </param>
        public static void SetStoreCapslockMode(string Mode)
        {

        }
    }
}