using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using System.Globalization;

namespace IronAHK.Rusty.Common
{
    partial class Keyboard
    {
        internal class KeyParser
        {
            internal static IEnumerable<Keys> ParseKeyStream(string sequence)
            {
                var keys = new List<Keys>();
                var buf = new[] { new StringBuilder(sequence.Length), new StringBuilder(16) };
                var scan = false;

                for (var i = 0; i < sequence.Length; i++)
                {
                    var sym = sequence[i];

                    switch (sym)
                    {
                        case Core.Keyword_KeyNameOpen:
                            if (scan)
                                goto default;
                            scan = true;
                            break;

                        case Core.Keyword_KeyNameClose:
                            {
                                if (!scan)
                                    goto default;

                                var n = i + 1;

                                if (buf[1].Length == 0 && n < sequence.Length && sequence[n] == sym)
                                    goto default;

                                scan = false;

                                if (buf[1].Length == 1)
                                {
                                    buf[0].Append(buf[1][0]);
                                    buf[1].Length = 0;
                                }

                                if (buf[1].Length == 0)
                                    continue;

                                switch (buf[1].ToString().ToLowerInvariant())
                                {
                                    case Core.Keyword_Raw:
                                        buf[0].Append(sequence, i, sequence.Length - i);
                                        i = sequence.Length;
                                        break;
                                }
                                keys.Add(ParseKey(buf[1].ToString()));
                                buf[1].Length = 0;
                            }
                            break;

                        default:
                            if (scan)
                                buf[1].Append(sym);
                            else
                                keys.Add(ParseKey(sym.ToString()));
                            break;
                    }
                }
                return keys;
            }


            /// <summary>
            /// Parses a single Key to its Keys Representation
            /// </summary>
            /// <param name="name"></param>
            /// <returns></returns>
            internal static Keys ParseKey(string name)
            {
                var value = Keys.None;
                int n;

                if (name.StartsWith(Core.Keyword_HotkeyVK, StringComparison.OrdinalIgnoreCase))
                {
                    name = name.Substring(Core.Keyword_HotkeyVK.Length);

                    if (int.TryParse(name, out n) && n > -1)
                        value = (Keys)n;
                }
                else if (name.StartsWith(Core.Keyword_HotkeySC, StringComparison.OrdinalIgnoreCase) && Environment.OSVersion.Platform == PlatformID.Win32NT)
                {
                    name = name.Substring(Core.Keyword_HotkeySC.Length);

                    if (int.TryParse(name, NumberStyles.HexNumber, System.Threading.Thread.CurrentThread.CurrentCulture, out n) && n > -1)
                        value = (Keys)WindowsAPI.MapVirtualKeyEx((uint)n, WindowsAPI.MAPVK_VSC_TO_VK_EX, WindowsAPI.GetKeyboardLayout(0));
                }

                try
                {
                    if (value == Keys.None)
                        value = (Keys)Enum.Parse(typeof(Keys), name, true);
                }
                catch (ArgumentException)
                {
                    switch (name.ToLowerInvariant())
                    {
                        case "esc": value = Keys.Escape; break;
                        case "backspace": value = Keys.Back; break;
                        case "bs": value = Keys.Back; break;
                        case "del": value = Keys.Delete; break;
                        case "ins": value = Keys.Insert; break;
                        case "pgup": value = Keys.PageUp; break;
                        case "pgdn": value = Keys.PageDown; break;
                        case "scrolllock": value = Keys.Scroll; break;
                        case "appskey": value = Keys.Apps; break;
                        case "ctrl": value = Keys.Control; break;
                        case "lcontrol": value = Keys.LControlKey; break;
                        case "lctrl": value = Keys.LControlKey; break;
                        case "rcontrol": value = Keys.RControlKey; break;
                        case "rctrl": value = Keys.RControlKey; break;
                        case "lshift": value = Keys.LShiftKey; break;
                        case "rshift": value = Keys.RShiftKey; break;
                        case "lalt": value = Keys.LMenu; break;
                        case "ralt": value = Keys.RMenu; break;
                        case "break": value = Keys.Pause; break;
                        case "numpad1": value = Keys.Oem1; break;
                        case "numpad2": value = Keys.Oem2; break;
                        case "numpad3": value = Keys.Oem3; break;
                        case "numpad4": value = Keys.Oem4; break;
                        case "numpad5": value = Keys.Oem5; break;
                        case "numpad6": value = Keys.Oem6; break;
                        case "numpad7": value = Keys.Oem7; break;
                        case "numpad8": value = Keys.Oem8; break;
                        case ";": value = Keys.OemSemicolon; break;
                        case "=": value = Keys.Oemplus; break;
                        case ",": value = Keys.Oemcomma; break;
                        case "-": value = Keys.OemMinus; break;
                        case ".": value = Keys.OemPeriod; break;
                        case "/": value = Keys.OemQuestion; break;
                        case "'": value = Keys.Oemtilde; break;
                        case "[": value = Keys.OemOpenBrackets; break;
                        case "\\": value = Keys.OemPipe; break;
                        case "]": value = Keys.OemCloseBrackets; break;
                        case "#": value = Keys.Oem7; break;
                        case "`": value = Keys.Oem8; break;
                    }
                }

                switch (value)
                {
                    case Keys.Control: value = Keys.ControlKey; break;
                    case Keys.Shift: value = Keys.ShiftKey; break;
                    case Keys.Alt: value = Keys.LMenu; break;
                }

                return value;
            }
        }
    }
}
