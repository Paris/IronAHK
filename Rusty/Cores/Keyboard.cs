using System;
using IronAHK.Rusty.Cores.Common.Keyboard;
using System.Text.RegularExpressions;
using System.Collections.Generic;

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
            #region Conditions

            int win = -1;

            switch (KeyName.ToLowerInvariant())
            {
                case Keyword_IfWinActive: win = 0; break;
                case Keyword_IfWinExist: win = 1; break;
                case Keyword_IfWinNotActive: win = 2; break;
                case Keyword_IfWinNotExit: win = 3; break;
            }

            if (win != -1)
            {
                var cond = new string[4, 2];

                cond[win, 0] = Label; // title
                cond[win, 1] = Options; // text

                keyCondition = new GenericFunction(delegate {
                    return HotkeyPrecondition(cond);
                });
                
                return;
            }

            #endregion

            #region Options

            InitKeyboardHook();

            bool? enabled = true;

            foreach (var option in ParseOptions(Options))
            {
                switch (option.ToLowerInvariant())
                {
                    case Keyword_On: enabled = true; break;
                    case Keyword_Off: enabled = false; break;
                    case Keyword_Toggle: enabled = null; break;
                    case Keyword_UseErrorLevel: break;

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
            catch (ArgumentException)
            {
                ErrorLevel = 2;
                return;
            }

            string id = KeyName;
            key.Name = id;

            if (keyCondition != null)
                id += "_" + keyCondition.GetHashCode().ToString("X");

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
                var method = FindLocalMethod(Label);

                if (method == null)
                {
                    ErrorLevel = 1;
                    return;
                }
                
                key.Proc = (GenericFunction)Delegate.CreateDelegate(typeof(GenericFunction), method);
                key.Precondition = keyCondition;

                hotkeys.Add(id, key);
                keyboardHook.Add(key);
            }

            #endregion
        }

        static bool HotkeyPrecondition(string[,] win)
        {
            if (!string.IsNullOrEmpty(win[0, 0]) || !string.IsNullOrEmpty(win[0, 1]))
                if (WinActive(win[0, 0], win[0, 1], string.Empty, string.Empty) == 0)
                    return false;

            if (!string.IsNullOrEmpty(win[1, 0]) || !string.IsNullOrEmpty(win[1, 1]))
                if (WinExist(win[1, 0], win[1, 1], string.Empty, string.Empty) == 0)
                    return false;

            if (!string.IsNullOrEmpty(win[2, 0]) || !string.IsNullOrEmpty(win[2, 1]))
                if (WinActive(win[2, 0], win[2, 1], string.Empty, string.Empty) != 0)
                    return false;

            if (!string.IsNullOrEmpty(win[3, 0]) || !string.IsNullOrEmpty(win[3, 1]))
                if (WinExist(win[3, 0], win[3, 1], string.Empty, string.Empty) != 0)
                    return false;

            return true;
        }

        /// <summary>
        /// Creates a hotstring.
        /// </summary>
        /// <param name="Sequence"></param>
        /// <param name="Label"></param>
        /// <param name="Options"></param>
        public static void Hotstring(string Sequence, string Label, string Options)
        {
            InitKeyboardHook();

            GenericFunction proc;

            try
            {
                var method = FindLocalMethod(Label);
                if (method == null)
                    throw new ArgumentNullException();
                proc = (GenericFunction)Delegate.CreateDelegate(typeof(GenericFunction), method);
            }
            catch (Exception)
            {
                ErrorLevel = 1;
                throw new ArgumentException();
            }

            var options = HotstringDefinition.ParseOptions(Options);
            var key = new HotstringDefinition(Sequence, proc) { Name = Sequence, Enabled = true, EnabledOptions = options };
            hotstrings.Add(Sequence, key);
            keyboardHook.Add(key);
        }

        /// <summary>
        /// Disables or enables the user's ability to interact with the computer via keyboard and mouse.
        /// </summary>
        /// <param name="Mode">
        /// <list type="bullet">
        /// <item>On: the user is prevented from interacting with the computer (mouse and keyboard input has no effect).</item>
        /// <item>Off: input is re-enabled.</item>
        /// </list>
        /// </param>
        public static void BlockInput(string Mode)
        {
            bool state = OnOff(Mode) ?? false;

            if (Environment.OSVersion.Platform == PlatformID.Win32NT)
                WindowsAPI.BlockInput(state);

            InitKeyboardHook();
            keyboardHook.Block = state;
        }

        /// <summary>
        /// Unlike the GetKeyState command -- which returns D for down and U for up -- this function returns (1) if the key is down and (0) if it is up.
        /// If <paramref name="KeyName"/> is invalid, an empty string is returned.
        /// </summary>
        /// <param name="KeyName">Use autohotkey definition or virtual key starting from "VK"</param>
        /// <param name="Mode"></param>
        public static string GetKeyState(string KeyName, string Mode)
        {
            return null;
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
        public static void Input(out string OutputVar, string Options, string EndKeys, string matchList)
        {
            OutputVar = null;
            InitKeyboardHook();

            var optsItems = new Dictionary<string, Regex>();
            var inputHandler = IAInputCommand.Instance;

            if(inputHandler.IsBusy) { // is there another Thread using this command already?
                inputHandler.AbortCatching(); // force to stop it
            }
            inputHandler.Reset();

            #region RegEx: Setup Options

            optsItems.Add(Keyword_LimitS, new Regex(Keyword_LimitS + @"(\d*)"));
            optsItems.Add(Keyword_TimeOutS, new Regex(Keyword_TimeOutS + @"([\d|\.]*)"));

            optsItems.Add(Keyword_BackSpaceS, new Regex("(" + Keyword_BackSpaceS + ")"));
            optsItems.Add(Keyword_IgnoreS, new Regex("(" + Keyword_IgnoreS + ")"));
            optsItems.Add(Keyword_ModifiedS, new Regex("(" + Keyword_ModifiedS + ")"));
            optsItems.Add(Keyword_VisibleS, new Regex("(" + Keyword_VisibleS + ")"));
            optsItems.Add(Keyword_FindAnyWhereS, new Regex("(\\" + Keyword_FindAnyWhereS + ")"));


            var dicOptions = ParseOptionsRegex(ref Options, optsItems, true);

            #endregion

            #region Apply Options

            try {
                var limit = Int32.Parse(dicOptions[Keyword_LimitS]);
                inputHandler.KeyLimit = limit;
            } catch {
                inputHandler.KeyLimit = 0;
            }

            try {
                var timeout = float.Parse(dicOptions[Keyword_TimeOutS]);
                inputHandler.TimeOutVal = (int)(timeout * 1000);
            } catch {
                inputHandler.TimeOutVal = 0;
            }

            if(!String.IsNullOrEmpty(dicOptions[Keyword_BackSpaceS])){
                inputHandler.IgnoreBackSpace = true;
            }

            if(!String.IsNullOrEmpty(dicOptions[Keyword_ModifiedS])) {
                inputHandler.RecognizeModifiedKeystrockes = true;
            }

            if(!String.IsNullOrEmpty(dicOptions[Keyword_IgnoreS])) {
                inputHandler.IgnoreIAGeneratedInput = true;
            }
            if(!String.IsNullOrEmpty(dicOptions[Keyword_VisibleS])) {
                inputHandler.Visible = true;
            }
            if(!String.IsNullOrEmpty(dicOptions[Keyword_FindAnyWhereS])) {
                inputHandler.FindAnyWhere = true;
            }

            // Parse EndKeys
            var endkeys = KeyParser.ParseKeyStream(EndKeys);
            foreach(var key in endkeys)
                inputHandler.Endkeys.Add(key);

            // Parse MatchList
            foreach(var matchStr in matchList.Split(','))
                inputHandler.EndMatches.Add(matchStr);


            #endregion

            var abortReason = inputHandler.StartCatching();
            OutputVar = abortReason.CatchedText;
            ErrorLevel = (int)abortReason.Reason;
        }

        /// <summary>
        /// Sends simulated keystrokes and mouse clicks to the active window.
        /// </summary>
        /// <param name="Keys">The sequence of keys to send.</param>
        public static void Send(string Keys)
        {
            InitKeyboardHook();
            keyboardHook.SendMixed(Keys);
        }

        /// <summary>
        /// Sets the state of the NumLock, ScrollLock or CapsLock keys.
        /// </summary>
        /// <param name="Key">
        /// <list type="bullet">
        /// <item>NumLock</item>
        /// <item>ScrollLock</item>
        /// <item>CapsLock</item>
        /// </list>
        /// </param>
        /// <param name="Mode">
        /// <list type="bullet">
        /// <item><term>On</term></item>
        /// <item><term>Off</term></item>
        /// <item><term>AlwaysOn</term>: <description>forces the key to stay on permanently.</description></item>
        /// <item><term>AlwaysOn</term>: <description>forces the key to stay off permanently.</description></item>
        /// <item><term>(blank)</term>: turn off the <c>AlwaysOn</c> or <c>Off</c> states if present.</item>
        /// </list>
        /// </param>
        public static void SetLockState(string Key, string Mode)
        {

        }
    }
}