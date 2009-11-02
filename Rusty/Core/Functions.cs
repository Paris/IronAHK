using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;

namespace IronAHK.Rusty
{
    partial class Core
    {
        /// <summary>
        /// Returns the absolute value of a number.
        /// </summary>
        /// <param name="Number">Any number.</param>
        /// <returns></returns>
        public static decimal Abs(decimal Number)
        {
            return Math.Abs(Number);
        }

        /// <summary>
        /// Returns the arccosine of a number in radians.
        /// </summary>
        /// <param name="Number">-1 &lt; n &lt; 1</param>
        /// <returns></returns>
        public static decimal ACos(decimal Number)
        {
            return (decimal)Math.Acos((double)Number);
        }

        /// <summary>
        /// Returns the ASCII code (a number between 1 and 255) for the first character in a string.
        /// </summary>
        /// <param name="String">A string.</param>
        /// <returns>The ASCII code. If String is empty, 0 is returned.</returns>
        public static decimal Asc(string String)
        {
            return string.IsNullOrEmpty(String) ? 0 : (decimal)String[0];
        }

        /// <summary>
        /// Returns the arcsine of a number in radians.
        /// </summary>
        /// <param name="Number">-1 &lt; n &lt; 1</param>
        /// <returns></returns>
        public static decimal ASin(decimal Number)
        {
            return (decimal)Math.Asin((double)Number);
        }

        /// <summary>
        /// Returns the arctangent of a number in radians.
        /// </summary>
        /// <param name="Number">-1 &lt; n &lt; 1</param>
        /// <returns></returns>
        public static decimal ATan(decimal Number)
        {
            return (decimal)Math.Atan((double)Number);
        }

        /// <summary>
        /// Returns a number rounded up to the nearest integer.
        /// </summary>
        /// <param name="Number">Any number.</param>
        /// <returns></returns>
        public static decimal Ceil(decimal Number)
        {
            return Math.Ceiling(Number);
        }

        /// <summary>
        /// Returns the single character corresponding to the Unicode value indicated by a number.
        /// </summary>
        /// <param name="Number">A positive integer.</param>
        /// <returns></returns>
        public static string Chr(decimal Number)
        {
            return ((char)Number).ToString();
        }

        /// <summary>
        /// Returns the cosent of a number in radians.
        /// </summary>
        /// <param name="Number">-1 &lt; n &lt; 1</param>
        /// <returns></returns>
        public static decimal Cos(decimal Number)
        {
            return (decimal)Math.Cos((double)Number);
        }

        /// <summary>
        /// Returns e (which is approximately 2.71828182845905) raised to the Nth power. N may be negative and may contain a decimal point. To raise numbers other than e to a power, use the ** operator.
        /// </summary>
        /// <param name="Number"></param>
        /// <returns></returns>
        public static decimal Exp(decimal Number)
        {
            return (decimal)Math.Exp((double)Number);
        }

        /// <summary>
        /// Returns a blank value (empty string) if FilePattern does not exist (FilePattern is assumed to be in A_WorkingDir if an absolute path isn't specified). Otherwise, it returns the attribute string (a subset of "RASHNDOCT") of the first matching file or folder. If the file has no attributes (rare), "X" is returned. FilePattern may be the exact name of a file or folder, or it may contain wildcards (* or ?). Since an empty string is seen as "false", the function's return value can always be used as a quasi-boolean value. For example, the statement if FileExist("C:\My File.txt") would be true if the file exists and false otherwise. Similarly, the statement if InStr(FileExist("C:\My Folder"), "D") would be true only if the file exists and is a directory. Corresponding commands: IfExist and FileGetAttrib.
        /// </summary>
        /// <param name="FilePattern"></param>
        /// <returns></returns>
        public static string FileExist(string FilePattern)
        {
            try { return Formats.FromFileAttribs(File.GetAttributes(FilePattern)); }
            catch (Exception) { return string.Empty; }
        }

        /// <summary>
        /// Returns Number rounded down to the nearest integer (without any .00 suffix). For example, Floor(1.2) is 1 and Floor(-1.2) is -2.
        /// </summary>
        /// <param name="Number"></param>
        /// <returns></returns>
        public static decimal Floor(decimal Number)
        {
            return Math.Floor(Number);
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
            if (Core.IfInString(KeyName,"VK"))
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
                VK = Windows.MouseKeyboard.VkKeyScan(Convert.ToChar(KeyName.Trim().ToLower()));
            }
            else
            {
                switch (KeyName.Trim().ToLower())
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

            short result = Windows.MouseKeyboard.GetKeyState(VK);

            switch( result )
            {
                case 0:
                // Not pressed and not toggled on.
                return "0";

                case 1:
                // Not pressed, but toggled on
                if (Mode.Trim().ToLower() == "t") return "1";
                return "0";

                default:
                // Pressed (and may be toggled on)
                if (Mode.Trim().ToLower() == "t")
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
        /// Returns the position of the first occurrence of the string Needle in the string Haystack. Unlike StringGetPos, position 1 is the first character; this is because 0 is synonymous with "false", making it an intuitive "not found" indicator. If the parameter CaseSensitive is omitted or false, the search is not case sensitive (the method of insensitivity depends on StringCaseSense); otherwise, the case must match exactly. If StartingPos is omitted, it defaults to 1 (the beginning of Haystack). Otherwise, specify 2 to start at Haystack's second character, 3 to start at the third, etc. If StartingPos is beyond the length of Haystack, 0 is returned. If StartingPos is 0, the search is conducted in reverse (right-to-left) so that the rightmost match is found. Regardless of the value of StartingPos, the returned position is always relative to the first character of Haystack. For example, the position of "abc" in "123abc789" is always 4. Related items: RegExMatch(), IfInString, and StringGetPos.
        /// </summary>
        /// <param name="Haystack"></param>
        /// <param name="Needle"></param>
        /// <param name="CaseSensitive"></param>
        /// <param name="StartingPos"></param>
        /// <returns></returns>
        public static int InStr(string Haystack, string Needle, string CaseSensitive, int StartingPos)
        {
            StringComparison type = CaseSensitive == null ?
                StringComparison.OrdinalIgnoreCase : StringComparison.Ordinal;

            return StartingPos == 0 ? Haystack.LastIndexOf(Needle, 0, type) :
                Haystack.IndexOf(Needle, StartingPos, type);
        }

        /// <summary>
        /// Returns a non-zero number if LabelName exists in the script as a subroutine, hotkey, or hotstring (do not include the trailing colon(s) in LabelName). For example, the statement if IsLabel(VarContainingLabelName) would be true if the label exists, and false otherwise. This is useful to avoid runtime errors when specifying a dynamic label in commands such as Gosub, Hotkey, Menu, and Gui.
        /// </summary>
        /// <param name="LabelName"></param>
        /// <returns></returns>
        public static bool IsLabel(object LabelName)
        {
            return LabelName.GetType() == typeof(PseudoLabel);
        }

        /// <summary>
        /// Returns the natural logarithm (base e) of Number. The result is formatted as floating point. If Number is negative, an empty string is returned.
        /// </summary>
        /// <param name="Number"></param>
        /// <returns></returns>
        public static decimal Ln(decimal Number)
        {
            return (decimal)Math.Log((double)Number, Math.E);
        }

        /// <summary>
        /// Returns the logarithm (base 10) of Number. The result is formatted as floating point. If Number is negative, an empty string is returned.
        /// </summary>
        /// <param name="Number"></param>
        /// <returns></returns>
        public static decimal Log(decimal Number)
        {
            return (decimal)Math.Log10((double)Number);
        }

        /// <summary>
        /// Modulo. Returns the remainder when Dividend is divided by Divisor. The sign of the result is always the same as the sign of the first parameter. For example, both mod(5, 3) and mod(5, -3) yield 2, but mod(-5, 3) and mod(-5, -3) yield -2. If either input is a floating point number, the result is also a floating point number. For example, mod(5.0, 3) is 2.0 and mod(5, 3.5) is 1.5. If the second parameter is zero, the function yields a blank result (empty string).
        /// </summary>
        /// <param name="Dividend"></param>
        /// <param name="Divisor"></param>
        /// <returns></returns>
        public static decimal Mod(decimal Dividend, decimal Divisor)
        {
            return Dividend % Divisor;
        }

        /// <summary>
        /// Returns the binary number stored at the specified address+offset. For VarOrAddress, passing MyVar is equivalent to passing &amp;MyVar. However, omitting the "&amp;" performs better and ensures that the target address is valid (invalid addresses return ""). By contrast, anything other than a naked variable passed to VarOrAddress is treated as a raw address; consequently, specifying MyVar+0 forces the number in MyVar to be used instead of the address of MyVar itself. For Type, specify UInt, Int, Int64, Short, UShort, Char, UChar, Double, or Float (though unlike DllCall, these must be enclosed in quotes when used as literal strings); for details see DllCall Types.
        /// </summary>
        /// <param name="VarOrAddress"></param>
        /// <param name="Offset"></param>
        /// <param name="Type"></param>
        public static void NumGet(int VarOrAddress, int Offset, string Type)
        {
            char[] type = Type.Trim().ToLower().ToCharArray();
            IntPtr adr = new IntPtr(VarOrAddress);

            switch (type[1])
            {
                case 's': Marshal.ReadInt16(adr, Offset); break; // short
                case 'c': Marshal.ReadByte(adr, Offset); break; // char
                default: // double, int, int64
                    if (Array.Exists<char>(type, delegate(char match) { return match == '6'; }))
                        Marshal.ReadInt64(adr, Offset);
                    else Marshal.ReadInt32(adr, Offset);
                    break;
            }
        }

        /// <summary>
        /// Stores Number in binary format at the specified address+offset and returns the address to the right of the item just written. For VarOrAddress, passing MyVar is equivalent to passing &amp;MyVar. However, omitting the "&amp;" performs better and ensures that the target address is valid (invalid addresses return ""). By contrast, anything other than a naked variable passed to VarOrAddress is treated as a raw address; consequently, specifying MyVar+0 forces the number in MyVar to be used instead of the address of MyVar itself. For Type, specify UInt, Int, Int64, Short, UShort, Char, UChar, Double, or Float (though unlike DllCall, these must be enclosed in quotes when used as literal strings); for details see DllCall Types. If an integer is too large to fit in the specified Type, its most significant bytes are ignored; e.g. NumPut(257, var, 0, "Char") would store the number 1.
        /// </summary>
        /// <param name="Number"></param>
        /// <param name="VarOrAddress"></param>
        /// <param name="Offset"></param>
        /// <param name="Type"></param>
        public static void NumPut(int Number, int VarOrAddress, int Offset, string Type)
        {
            char[] type = (Type).Trim().ToLower().ToCharArray();
            IntPtr adr = new IntPtr(VarOrAddress);

            switch (type[1])
            {
                case 's': Marshal.WriteInt16(adr, Offset, (char)Number); break; // short
                case 'c': Marshal.WriteByte(adr, Offset, (byte)Number); break; // char
                default: // double, int, int64
                    if (Array.Exists<char>(type, delegate(char match) { return match == '6'; }))
                        Marshal.WriteInt64(adr, Offset, (long)Number);
                    else Marshal.WriteInt32(adr, Offset, Number);
                    break;
            }
        }

        /// <summary>
        /// Specifies a function to call automatically when the script receives the specified message.
        /// </summary>
        /// <param name="MsgNumber">The number of the message to monitor or query, which should be between 0 and 4294967295 (0xFFFFFFFF). If you do not wish to monitor a system message (that is, one below 0x400), it is best to choose a number greater than 4096 (0x1000) to the extent you have a choice. This reduces the chance of interfering with messages used internally by current and future versions of AutoHotkey.</param>
        /// <param name="FunctionName">A function's name, which must be enclosed in quotes if it is a literal string. This function will be called automatically when the script receives MsgNumber. Omit this parameter and the next one to retrieve the name of the function currently monitoring MsgNumber (blank if none). Specify an empty string ("") or an empty variable to turn off the monitoring of MsgNumber.</param>
        /// <param name="MaxThreads">This integer is normally omitted, in which case the monitor function is limited to one thread at a time. This is usually best because otherwise, the script would process messages out of chronological order whenever the monitor function interrupts itself. Therefore, as an alternative to MaxThreads, consider using Critical as described below.</param>
        public static void OnMessage(string MsgNumber, string FunctionName, string MaxThreads)
        {

        }

        /// <summary>
        /// Determines whether a string contains a pattern (regular expression).
        /// </summary>
        /// <param name="Haystack">The string whose content is searched.</param>
        /// <param name="NeedleRegEx">The pattern to search for, which is a Perl-compatible regular expression (PCRE). The pattern's options (if any) must be included at the beginning of the string followed by an close-parenthesis. For example, the pattern "i)abc.*123" would turn on the case-insensitive option and search for "abc", followed by zero or more occurrences of any character, followed by "123". If there are no options, the ")" is optional; for example, ")abc" is equivalent to "abc".</param>
        /// <param name="UnquotedOutputVar">
        /// <para>Mode 1 (default): OutputVar is the unquoted name of a variable in which to store the part of Haystack that matched the entire pattern. If the pattern is not found (that is, if the function returns 0), this variable and all array elements below are made blank.</para>
        /// <para>If any capturing subpatterns are present inside NeedleRegEx, their matches are stored in an array whose base name is OutputVar. For example, if the variable's name is Match, the substring that matches the first subpattern would be stored in Match1, the second would be stored in Match2, and so on. The exception to this is named subpatterns: they are stored by name instead of number. For example, the substring that matches the named subpattern (?P&lt;Year&gt;\d{4}) would be stored in MatchYear. If a particular subpattern does not match anything (or if the function returns zero), the corresponding variable is made blank.</para>
        /// <para>Within a function, to create an array that is global instead of local, declare the base name of the array (e.g. Match) as a global variable prior to using it. The converse is true for assume-global functions.</para>
        /// <para>Mode 2 (position-and-length): If a capital P is present in the RegEx's options -- such as "P)abc.*123" -- the length of the entire-pattern match is stored in OutputVar (or 0 if no match). If any capturing subpatterns are present, their positions and lengths are stored in two arrays: OutputVarPos and OutputVarLen. For example, if the variable's base name is Match, the one-based position of the first subpattern's match would be stored in MatchPos1, and its length in MatchLen1 (zero is stored in both if the subpattern was not matched or the function returns 0). The exception to this is named subpatterns: they are stored by name instead of number (e.g. MatchPosYear and MatchLenYear).</para>
        /// </param>
        /// <param name="StartingPos">
        /// <para>If StartingPosition is omitted, it defaults to 1 (the beginning of Haystack). Otherwise, specify 2 to start at the second character, 3 to start at the third, and so on. If StartingPosition is beyond the length of Haystack, the search starts at the empty string that lies at the end of Haystack (which typically results in no match).</para>
        /// <para>If StartingPosition is less than 1, it is considered to be an offset from the end of Haystack. For example, 0 starts at the last character and -1 starts at the next-to-last character. If StartingPosition tries to go beyond the left end of Haystack, all of Haystack is searched.</para>
        /// <para>Regardless of the value of StartingPosition, the return value is always relative to the first character of Haystack. For example, the position of "abc" in "123abc789" is always 4.</para>
        /// </param>
        /// <returns>RegExMatch() returns the position of the leftmost occurrence of NeedleRegEx in the string Haystack. Position 1 is the first character. Zero is returned if the pattern is not found. If an error occurs (such as a syntax error inside NeedleRegEx), an empty string is returned and ErrorLevel is set to one of the values below instead of 0.</returns>
        public static int RegExMatch(string Haystack, string NeedleRegEx, out string[] UnquotedOutputVar, int StartingPos)
        {
            Regex re = Formats.ParseRegEx(NeedleRegEx);
            Match res = re.Match(Haystack, StartingPos);

            string[] matches = new string[res.Groups.Count];
            for (int i = 0; i < res.Groups.Count; i++)
                matches[i] = res.Groups[i].Value;

            UnquotedOutputVar = matches;

            return 0;
        }

        /// <summary>
        /// Replaces occurrences of a pattern (regular expression) inside a string.
        /// </summary>
        /// <param name="Haystack">The string whose content is searched and replaced.</param>
        /// <param name="NeedleRegEx">The pattern to search for, which is a Perl-compatible regular expression (PCRE). The pattern's options (if any) must be included at the beginning of the string followed by an close-parenthesis. For example, the pattern "i)abc.*123" would turn on the case-insensitive option and search for "abc", followed by zero or more occurrences of any character, followed by "123". If there are no options, the ")" is optional; for example, ")abc" is equivalent to "abc".</param>
        /// <param name="Replacement">
        /// <para>The string to be substituted for each match, which is plain text (not a regular expression). It may include backreferences like $1, which brings in the substring from Haystack that matched the first subpattern. The simplest backreferences are $0 through $9, where $0 is the substring that matched the entire pattern, $1 is the substring that matched the first subpattern, $2 is the second, and so on. For backreferences above 9 (and optionally those below 9), enclose the number in braces; e.g. ${10}, ${11}, and so on. For named subpatterns, enclose the name in braces; e.g. ${SubpatternName}. To specify a literal $, use $$ (this is the only character that needs such special treatment; backslashes are never needed to escape anything).</para>
        /// <para>To convert the case of a subpattern, follow the $ with one of the following characters: U or u (uppercase), L or l (lowercase), T or t (title case, in which the first letter of each word is capitalized but all others are made lowercase). For example, both $U1 and $U{1} transcribe an uppercase version of the first subpattern.</para>
        /// <para>Nonexistent backreferences and those that did not match anything in Haystack -- such as one of the subpatterns in (abc)|(xyz) -- are transcribed as empty strings.</para>
        /// </param>
        /// <param name="OutputVarCount">The unquoted name of a variable in which to store the number of replacements that occurred (0 if none).</param>
        /// <param name="Limit">If Limit is omitted, it defaults to -1, which replaces all occurrences of the pattern found in Haystack. Otherwise, specify the maximum number of replacements to allow. The part of Haystack to the right of the last replacement is left unchanged.</param>
        /// <param name="StartingPos">
        /// <para>If StartingPosition is omitted, it defaults to 1 (the beginning of Haystack). Otherwise, specify 2 to start at the second character, 3 to start at the third, and so on. If StartingPosition is beyond the length of Haystack, the search starts at the empty string that lies at the end of Haystack (which typically results in no replacements).</para>
        /// <para>If StartingPosition is less than 1, it is considered to be an offset from the end of Haystack. For example, 0 starts at the last character and -1 starts at the next-to-last character. If StartingPosition tries to go beyond the left end of Haystack, all of Haystack is searched.</para>
        /// <para>Regardless of the value of StartingPosition, the return value is always a complete copy of Haystack -- the only difference is that more of its left side might be unaltered compared to what would have happened with a StartingPosition of 1.</para>
        /// </param>
        /// <returns>RegExReplace() returns a version of Haystack whose contents have been replaced by the operation. If no replacements are needed, Haystack is returned unaltered. If an error occurs (such as a syntax error inside NeedleRegEx), Haystack is returned unaltered (except in versions prior to 1.0.46.06, which return "") and ErrorLevel is set to one of the values below instead of 0.</returns>
        public static string RegExReplace(string Haystack, string NeedleRegEx, string Replacement, out int OutputVarCount, int Limit, int StartingPos)
        {
            Regex re = Formats.ParseRegEx(NeedleRegEx);
            int total = re.Matches(Haystack, StartingPos).Count;
            OutputVarCount = Math.Min(Limit, total);
            return re.Replace(Haystack, Replacement, Limit, StartingPos);
        }

        /// <summary>
        /// Creates a machine-code address that when called, redirects the call to a function in the script.
        /// </summary>
        /// <param name="FunctionName">A function's name, which must be enclosed in quotes if it is a literal string. This function is called automatically whenever Address is called. The function also receives the parameters that were passed to Address.</param>
        /// <param name="Options">
        /// <para>Specify zero or more of the following words. Separate each option from the next with a space (e.g. "C Fast").</para>
        /// <para>Fast or F: Avoids starting a new thread each time FunctionName is called. Although this performs much better, it must be avoided whenever the thread from which Address is called varies (e.g. when the callback is triggered by an incoming message). This is because FunctionName will be able to change global settings such as ErrorLevel, A_LastError, and the last-found window for whichever thread happens to be running at the time it is called. For more information, see Remarks.</para>
        /// <para>CDecl or C : Makes Address conform to the "C" calling convention. This is typically omitted because the standard calling convention is much more common for callbacks.</para>
        /// </param>
        /// <param name="ParamCount">The number of parameters that Address's caller will pass to it. If entirely omitted, it defaults to the number of mandatory parameters in the definition of FunctionName. In either case, ensure that the caller passes exactly this number of parameters.</param>
        /// <param name="EventInfo">An integer between 0 and 4294967295 that FunctionName will see in A_EventInfo whenever it is called via this Address. This is useful when FunctionName is called by more than one Address. If omitted, it defaults to Address. Note: Unlike other global settings, the current thread's A_EventInfo is not disturbed by the fast mode.</param>
        /// <returns>Upon success, RegisterCallback() returns a numeric address that may be called by DllCall() or anything else capable of calling a machine-code function. Upon failure, it returns an empty string. Failure occurs when FunctionName: 1) does not exist; 2) accepts too many or too few parameters according to ParamCount; or 3) accepts any ByRef parameters.</returns>
        public static int RegisterCallback(string FunctionName, string Options, string ParamCount, string EventInfo)
        {
            return 0;
        }

        /// <summary>
        /// If N is omitted or 0, Number is rounded to the nearest integer. If N is positive number, Number is rounded to N decimal places. If N is negative, Number is rounded by N digits to the left of the decimal point. For example, Round(345, -1) is 350 and Round (345, -2) is 300. Unlike Transform Round, the result has no .000 suffix whenever N is omitted or less than 1. In v1.0.44.01+, a value of N greater than zero displays exactly N decimal places rather than obeying SetFormat. To avoid this, perform another math operation on Round()'s return value; for example: Round(3.333, 1)+0.
        /// </summary>
        /// <param name="Number"></param>
        /// <param name="Places"></param>
        /// <returns></returns>
        public static decimal Round(decimal Number, decimal Places)
        {
            return Math.Round(Number, (int)Places);
        }

        /// <summary>
        /// Returns the trigonometric sine Number. Number must be expressed in radians.
        /// </summary>
        /// <param name="Number"></param>
        /// <returns></returns>
        public static decimal Sin(decimal Number)
        {
            return (decimal)Math.Sin((double)Number);
        }

        /// <summary>
        /// Returns the square root of Number. The result is formatted as floating point. If Number is negative, the function yields a blank result (empty string).
        /// </summary>
        /// <param name="Number"></param>
        /// <returns></returns>
        public static decimal Sqrt(decimal Number)
        {
            return (decimal)Math.Sqrt((double)Number);
        }

        /// <summary>
        /// Returns the length of String. If String is a variable to which ClipboardAll was previously assigned, its total size is returned. Corresponding command: StringLen.
        /// </summary>
        /// <param name="String"></param>
        /// <returns></returns>
        public static decimal StrLen(string String)
        {
            return (decimal)String.Length;
        }

        /// <summary>
        /// Copies a substring from String starting at StartingPos and proceeding rightward to include at most Length characters (if Length is omitted, it defaults to "all characters"). For StartingPos, specify 1 to start at the first character, 2 to start at the second, and so on (if StartingPos is beyond String's length, an empty string is returned). If StartingPos is less than 1, it is considered to be an offset from the end of the string. For example, 0 extracts the last character and -1 extracts the two last characters (but if StartingPos tries to go beyond the left end of the string, the extraction starts at the first character). Length is the maximum number of characters to retrieve (fewer than the maximum are retrieved whenever the remaining part of the string too short). Specify a negative Length to omit that many characters from the end of the returned string (an empty string is returned if all or too many characters are omitted). Related items: RegExMatch(), StringMid, StringLeft/Right, StringTrimLeft/Right.
        /// </summary>
        /// <param name="String"></param>
        /// <param name="StartingPos"></param>
        /// <param name="Length"></param>
        /// <returns></returns>
        public static string SubStr(string String, decimal StartingPos, decimal Length)
        {
            return String.Substring((int)(StartingPos < 1 ? String.Length - StartingPos : StartingPos + 1), (int)Length);
        }

        /// <summary>
        /// Returns the trigonometric tangent of Number. Number must be expressed in radians.
        /// </summary>
        /// <param name="Number"></param>
        /// <returns></returns>
        public static decimal Tan(decimal Number)
        {
            return (decimal)Math.Tan((double)Number);
        }

        /// <summary>
        /// Enlarges a variable's holding capacity or frees its memory. Normally, this is necessary only for unusual circumstances such as DllCall.
        /// </summary>
        /// <param name="Var">The name of the variable (not in quotes). For example: VarSetCapacity(MyVar, 1000). This can also be a dynamic variable such as Array%i% or a function's ByRef parameter.</param>
        /// <param name="RequestedCapacity">
        /// <para>If omitted, the variable's current capacity will be returned and its contents will not be altered. Otherwise, anything currently in the variable is lost (the variable becomes blank).</para>
        /// <para>Specify for RequestedCapacity the length of string that the variable should be able to hold after the adjustment. This length does not include the internal zero terminator. For example, specifying 1 would allow the variable to hold up to one character in addition to its internal terminator. Note: the variable will auto-expand if the script assigns it a larger value later.</para>
        /// <para>Since this function is often called simply to ensure the variable has a certain minimum capacity, for performance reasons, it shrinks the variable only when RequestedCapacity is 0. In other words, if the variable's capacity is already greater than RequestedCapacity, it will not be reduced (but the variable will still made blank for consistency).</para>
        /// <para>Therefore, to explicitly shrink a variable, first free its memory with VarSetCapacity(Var, 0) and then use VarSetCapacity(Var, NewCapacity) -- or simply let it auto-expand from zero as needed.</para>
        /// <para>For performance reasons, freeing a variable whose previous capacity was between 1 and 63 might have no effect because its memory is of a permanent type. In this case, the current capacity will be returned rather than 0.</para>
        /// <para>For performance reasons, the memory of a variable whose capacity is under 4096 is not freed by storing an empty string in it (e.g. Var := ""). However, VarSetCapacity(Var, 0) does free it.</para>
        /// <para>Specify -1 for RequestedCapacity to update the variable's internally-stored length to the length of its current contents. This is useful in cases where the variable has been altered indirectly, such as by passing its address via DllCall(). In this mode, VarSetCapacity() returns the length rather than the capacity.</para>
        /// </param>
        /// <param name="FillByte">This parameter is normally omitted, in which case the memory of the target variable is not initialized (instead, the variable is simply made blank as described above). Otherwise, specify a number between 0 and 255. Each byte in the target variable's memory area (its current capacity) is set to that number. Zero is by far the most common value, which is useful in cases where the variable will hold raw binary data such as a DllCall structure.</param>
        /// <returns>The length of string that Var can now hold, which will be greater or equal to RequestedCapacity. If VarName is not a valid variable name (such as a literal string or number), 0 is returned. If the system has insufficient memory to make the change (very rare), an error dialog will be displayed and the current thread will exit.</returns>
        public static int VarSetCapacity(out byte[] Var, int RequestedCapacity, int FillByte)
        {
            Var = new byte[RequestedCapacity];

            byte fill = (byte)FillByte;
            if (fill != 0)
                for (int i = 0; i < Var.Length; i++)
                    Var[i] = fill;

            return Var.Length;
        }
    }
}