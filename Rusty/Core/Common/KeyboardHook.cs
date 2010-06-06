using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace IronAHK.Rusty
{
    partial class Core
    {
        #region Events

        internal delegate void HotkeyEvent(object sender, HotkeyEventArgs e);

        internal delegate void HotstringEvent(object sender, HotstringEventArgs e);

        internal class HotkeyEventArgs : EventArgs
        {
            Keys keys;

            public HotkeyEventArgs(Keys keys)
            {
                this.keys = keys;
            }

            public Keys Keys
            {
                get { return keys; }
            }
        }

        internal class HotstringEventArgs : EventArgs
        {
            string sequence;

            public HotstringEventArgs() : this(string.Empty) { }

            public HotstringEventArgs(string sequence)
            {
                this.sequence = sequence;
            }

            public string Sequence
            {
                get { return sequence; }
            }
        }

        #endregion

        #region Definitions

        internal class HotkeyDefinition
        {
            #region Properties

            Keys keys, extra;
            Options options;
            GenericFunction precondition;
            string name;

            [Flags]
            public enum Options { None = 0, IgnoreModifiers = 1, PassThrough = 2, Up = 4 }

            #endregion

            public HotkeyDefinition(Keys keys, Keys extra, Options options, GenericFunction proc)
            {
                this.keys = keys;
                this.extra = extra;
                this.options = options;
                this.Proc = proc;
                Enabled = true;
            }

            #region Accessors

            public Keys Keys
            {
                get { return keys; }
            }

            public Keys Extra
            {
                get { return extra; }
            }

            public Options EnabledOptions
            {
                get { return options; }
            }

            public GenericFunction Proc { get; set; }

            public GenericFunction Precondition
            {
                get { return precondition; }
                set { precondition = value; }
            }

            public bool Enabled { get; set; }

            public string Name
            {
                get { return name; }
                set { name = value; }
            }

            public string Typed { get; set; }

            #endregion

            public bool Condition()
            {
                if (precondition == null)
                    return true;

                var result = precondition.Invoke(new object[] { });
                return result is bool ? (bool)result : result != null;
            }

            public static HotkeyDefinition Parse(string sequence)
            {
                Keys keys = Keys.None, extra = Keys.None;
                Options options = Options.None;
                string typed = string.Empty;

                #region Modifiers

                sequence = sequence.Replace(Keyword_ModifierAltGr, new string(new[] { Keyword_ModifierCtrl, Keyword_ModifierAlt }));

                for (int i = 0; i < sequence.Length; i++)
                {
                    switch (sequence[i])
                    {
                        case Keyword_ModifierLeftPair:
                            i++;
                            if (i == sequence.Length)
                                throw new ArgumentException();
                            switch (sequence[i])
                            {
                                case Keyword_ModifierWin: extra = Keys.LWin; break;
                                case Keyword_ModifierAlt: extra = Keys.LMenu; break;
                                case Keyword_ModifierCtrl: extra = Keys.LControlKey; break;
                                case Keyword_ModifierShift: extra = Keys.LShiftKey; break;
                                default: throw new ArgumentException();
                            }
                            break;

                        case Keyword_ModifierRightPair:
                            i++;
                            if (i == sequence.Length)
                                throw new ArgumentException();
                            switch (sequence[i])
                            {
                                case Keyword_ModifierWin: extra = Keys.RWin; break;
                                case Keyword_ModifierAlt: extra = Keys.RMenu; break;
                                case Keyword_ModifierCtrl: extra = Keys.RControlKey; break;
                                case Keyword_ModifierShift: extra = Keys.RShiftKey; break;
                                default: throw new ArgumentException();
                            }
                            break;

                        case Keyword_ModifierWin: extra = Keys.LWin; break;
                        case Keyword_ModifierAlt: keys |= Keys.Alt; break;
                        case Keyword_ModifierCtrl: keys |= Keys.Control; break;
                        case Keyword_ModifierShift: keys |= Keys.Shift; break;

                        case Keyword_HotkeyIgnoreModifiers: options |= Options.IgnoreModifiers; break;
                        case Keyword_HotkeyPassThrough: options |= Options.PassThrough; break;

                        case Keyword_HotkeyNoRecurse: continue;

                        default:
                            if (i > 0)
                                sequence = sequence.Substring(i);
                            i = sequence.Length;
                            break;
                    }
                }

                #endregion

                int z = sequence.IndexOf(Keyword_HotkeyCombination);
                
                if (z != -1)
                {
                    z++;
                    if (z < sequence.Length)
                    {
                        string alt = sequence.Substring(z).Trim();
                        extra = ParseKey(alt);

                        if (alt.Length == 1)
                            typed = alt;
                    }
                    sequence = sequence.Substring(0, z - 1).Trim();
                }

                z = sequence.LastIndexOf(Keyword_Up, StringComparison.OrdinalIgnoreCase);

                if (z > 0 && char.IsWhiteSpace(sequence, z - 1))
                {
                    sequence = sequence.Substring(0, z).Trim();
                    options |= Options.Up;
                }

                keys |= ParseKey(sequence);

                if (typed.Length == 0 && sequence.Length == 1)
                    typed = sequence;

                return new HotkeyDefinition(keys, extra, options, null) { Typed = typed };
            }

            static Keys ParseKey(string name)
            {
                var value = Keys.None;
                int n;

                if (name.StartsWith(Keyword_HotkeyVK, StringComparison.OrdinalIgnoreCase))
                {
                    name = name.Substring(Keyword_HotkeyVK.Length);

                    if (int.TryParse(name, out n) && n > -1)
                        value = (Keys)n;
                }
                else if (name.StartsWith(Keyword_HotkeySC, StringComparison.OrdinalIgnoreCase) && Environment.OSVersion.Platform == PlatformID.Win32NT)
                {
                    name = name.Substring(Keyword_HotkeySC.Length);

                    if (int.TryParse(name, NumberStyles.HexNumber, System.Threading.Thread.CurrentThread.CurrentCulture, out n) && n > -1)
                        value = (Keys)Windows.MapVirtualKeyEx((uint)n, Windows.MAPVK_VSC_TO_VK_EX, Windows.GetKeyboardLayout(0));
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

                return value;
            }

            public override string ToString()
            {
                return name;
            }
        }

        internal class HotstringDefinition
        {
            string sequence;
            GenericFunction proc;
            string name;

            [Flags]
            public enum Options { None = 0, AutoTrigger = 1, Nested = 2, Backspace = 4, CaseSensitive = 8, OmitEnding = 16, Raw = 32, Reset = 64 }

            public HotstringDefinition(string sequence, GenericFunction proc)
            {
                this.sequence = sequence;
                this.proc = proc;

                EndChars = "-()[]{}:;'\"/\\,.?!\r\n \t";
            }

            public string Sequence
            {
                get { return sequence; }
            }

            public string EndChars { get; set; }

            public Options EnabledOptions { get; set; }

            public GenericFunction Proc
            {
                get { return proc; }
            }

            public bool Enabled { get; set; }

            public string Name
            {
                get { return name; }
                set { name = value; }
            }

            public static Options ParseOptions(string mode)
            {
                var options = Options.Backspace;

                mode = mode.ToUpperInvariant();

                for (int i = 0; i < mode.Length; i++)
                {
                    char sym = mode[i];
                    var change = Options.None;

                    switch (sym)
                    {
                        case Keyword_HotstringAuto: change = Options.AutoTrigger; break;
                        case Keyword_HotstringNested: change = Options.Nested; break;
                        case Keyword_HotstringBackspace: change = Options.Backspace; break;
                        case Keyword_HotstringCase: change = Options.CaseSensitive; break;
                        case Keyword_HotstringOmitEnding: change = Options.OmitEnding; break;
                        case Keyword_HotstringReset: change = Options.Reset; break;
                    }

                    if (change == Options.None)
                        continue;

                    int n = i + 1;
                    bool off = n < mode.Length && mode[n] == Keyword_HotstringOff;

                    if (off)
                        options &= ~change;
                    else
                        options |= change;
                }

                return options;
            }

            public override string ToString()
            {
                return name;
            }
        }

        #endregion

        internal abstract class KeyboardHook
        {
            #region Properties

            List<HotkeyDefinition> hotkeys;
            List<HotstringDefinition> hotstrings;
            Dictionary<Keys, bool> pressed;
            string current, previous;
            int currentTime, priorTime;

            StringBuilder history;
            const int retention = 1024;

            public string CurrentHotkey
            {
                get { return current; }
            }

            public string PriorHotkey
            {
                get { return previous; }
            }

            public int CurrentHotkeyTime
            {
                get { return currentTime; }
            }

            public int PriorHotkeyTime
            {
                get { return priorTime; }
            }

            #endregion

            #region Constructor/destructor

            public KeyboardHook()
            {
                hotkeys = new List<HotkeyDefinition>();
                hotstrings = new List<HotstringDefinition>();
                history = new StringBuilder(retention);
                pressed = new Dictionary<Keys, bool>();

                foreach (int i in Enum.GetValues(typeof(Keys)))
                    if (!pressed.ContainsKey((Keys)i))
                        pressed.Add((Keys)i, false);

                RegisterHook();
            }

            ~KeyboardHook()
            {
                DeregisterHook();
            }

            #endregion

            #region Add/remove

            public HotkeyDefinition Add(HotkeyDefinition hotkey)
            {
                hotkeys.Add(hotkey);
                return hotkey;
            }

            public HotstringDefinition Add(HotstringDefinition hotstring)
            {
                hotstrings.Add(hotstring);
                return hotstring;
            }

            public void Remove(HotkeyDefinition hotkey)
            {
                hotkeys.Remove(hotkey);
            }

            public void Remove(HotstringDefinition hotstring)
            {
                hotstrings.Remove(hotstring);
            }

            #endregion

            #region Conversions

            char Letter(Keys key)
            {
                // HACK: remove Keys translation (and the overload) since it should be passed from the native handler

                bool caps = (key & Keys.Shift) == Keys.Shift || pressed[Keys.ShiftKey] || pressed[Keys.LShiftKey] || pressed[Keys.RShiftKey];
                key &= ~Keys.Modifiers;

                switch (key)
                {
                    case Keys.Space: return ' ';
                    case Keys.Enter: return '\n';
                }

                string letter = key.ToString();

                if (!caps)
                    letter = letter.ToLower();

                return letter.Length == 1 ? letter[0] : (char)0;
            }

            #endregion

            #region Hotkey fired

            [Obsolete]
            protected bool KeyReceived(Keys key, bool down)
            {
                return KeyReceived(key, Letter(key).ToString(), down);
            }

            protected bool KeyReceived(Keys key, string typed, bool down)
            {
                bool block = false;

                if (suspended)
                    goto next;

                pressed[key] = down;

                var exec = new List<HotkeyDefinition>();

                foreach (var hotkey in hotkeys)
                {
                    bool match = (hotkey.Keys & ~Keys.Modifiers) == key ||
                        hotkey.Typed.Length != 0 && hotkey.Typed.Equals(typed, StringComparison.CurrentCultureIgnoreCase);
                    bool up = (hotkey.EnabledOptions & HotkeyDefinition.Options.Up) == HotkeyDefinition.Options.Up;

                    if (hotkey.Enabled && match && HasModifiers(hotkey) && up != down)
                    {
                        exec.Add(hotkey);

                        if ((hotkey.EnabledOptions & HotkeyDefinition.Options.PassThrough) != HotkeyDefinition.Options.PassThrough)
                            block = true;
                    }
                }

                new Thread(delegate()
                               {
                                   foreach (var hotkey in exec)
                                   {
                                       priorTime = currentTime;
                                       currentTime = Environment.TickCount;
                                       previous = current;
                                       current = hotkey.ToString();

                                       if (hotkey.Condition())
                                           hotkey.Proc(new object[] { });
                                   }
                               }).Start();

            next:

                if (!down)
                    return block;

                #region Sequencing

                if (hotstrings.Count > 0)
                {
                    if (key == Keys.Back && history.Length > 0)
                        history.Remove(history.Length - 1, 1);

                    switch (key)
                    {
                        case Keys.Left:
                        case Keys.Right:
                        case Keys.Down:
                        case Keys.Up:
                        case Keys.Next:
                        case Keys.Prior:
                        case Keys.Home:
                        case Keys.End:
                            history.Length = 0;
                            break;

                        case Keys.Alt:
                        case Keys.LMenu:
                        case Keys.RMenu:
                        case Keys.LControlKey:
                        case Keys.RControlKey:
                        case Keys.LShiftKey:
                        case Keys.RShiftKey:
                            break;

                        default:
                            int d = retention - history.Length;
                            if (d < 0)
                                history.Remove(history.Length + d, -d);
                            history.Append(typed);
                            break;
                    }
                }

                if (suspended)
                    return block;

                #endregion

                var expand = new List<HotstringDefinition>();

                foreach (var hotstring in hotstrings)
                {
                    if (hotstring.Enabled && HasConditions(hotstring))
                    {
                        expand.Add(hotstring);

                        if ((hotstring.EnabledOptions & HotstringDefinition.Options.Reset) == HotstringDefinition.Options.Reset)
                            history.Length = 0;
                    }
                }

                string trigger = history.Length > 0 ? history[history.Length - 1].ToString() : null;

                foreach (var hotstring in expand)
                {
                    block = true;

                    new Thread(delegate()
                                   {
                                       priorTime = currentTime;
                                       currentTime = Environment.TickCount;
                                       previous = current;
                                       current = hotstring.ToString();
                                       int length = hotstring.Sequence.Length;
                                       bool auto = (hotstring.EnabledOptions & HotstringDefinition.Options.AutoTrigger) == HotstringDefinition.Options.AutoTrigger;

                                       if (auto)
                                           length--;

                                       if ((hotstring.EnabledOptions & HotstringDefinition.Options.Backspace) == HotstringDefinition.Options.Backspace && length > 0)
                                       {
                                           int n = length + 1;
                                           history.Remove(history.Length - n, n);
                                           Backspace(length);
                                       }

                                       hotstring.Proc(new object[] { });

                                       if ((hotstring.EnabledOptions & HotstringDefinition.Options.OmitEnding) == HotstringDefinition.Options.OmitEnding)
                                       {
                                           if ((hotstring.EnabledOptions & HotstringDefinition.Options.Backspace) == HotstringDefinition.Options.Backspace &&
                                               (hotstring.EnabledOptions & HotstringDefinition.Options.AutoTrigger) != HotstringDefinition.Options.AutoTrigger)
                                           {
                                               history.Remove(history.Length - 1, 1);
                                               Backspace(1);
                                           }
                                       }
                                       else if (trigger != null && !auto)
                                           Send(trigger);
                                   }).Start();
                }

                return block;
            }

            bool HasModifiers(HotkeyDefinition hotkey)
            {
                if (hotkey.Extra != Keys.None && !pressed[hotkey.Extra])
                    return false;

                if ((hotkey.EnabledOptions & HotkeyDefinition.Options.IgnoreModifiers) == HotkeyDefinition.Options.IgnoreModifiers)
                    return true;

                bool[,] modifiers = { 
                                       { (hotkey.Keys & Keys.Alt) == Keys.Alt, pressed[Keys.Alt] || pressed[Keys.LMenu] || pressed[Keys.RMenu] },
                                       { (hotkey.Keys & Keys.Control) == Keys.Control, pressed[Keys.Control] || pressed[Keys.LControlKey] || pressed[Keys.RControlKey] },
                                       { (hotkey.Keys & Keys.Shift) == Keys.Shift, pressed[Keys.Shift] || pressed[Keys.LShiftKey] || pressed[Keys.RShiftKey] }
                                   };

                for (int i = 0; i < 3; i++)
                    if ((modifiers[i, 0] && !modifiers[i, 1]) || (modifiers[i, 1] && !modifiers[i, 0]))
                        return false;

                return true;
            }

            bool HasConditions(HotstringDefinition hotstring)
            {
                string history = this.history.ToString();

                if (history.Length == 0)
                    return false;

                var compare = (hotstring.EnabledOptions & HotstringDefinition.Options.CaseSensitive) == HotstringDefinition.Options.CaseSensitive ?
                    StringComparison.CurrentCulture : StringComparison.CurrentCultureIgnoreCase;

                int x = history.Length - hotstring.Sequence.Length - 1;

                if ((hotstring.EnabledOptions & HotstringDefinition.Options.AutoTrigger) == HotstringDefinition.Options.AutoTrigger)
                {
                    if (!history.EndsWith(hotstring.Sequence, compare))
                        return false;
                }
                else
                {
                    if (history.Length < hotstring.Sequence.Length + 1)
                        return false;
                    
                    if (hotstring.EndChars.IndexOf(history[history.Length - 1]) == -1)
                        return false;
                                        
                    if (!history.Substring(x--, hotstring.Sequence.Length).Equals(hotstring.Sequence, compare))
                        return false;
                }

                if ((hotstring.EnabledOptions & HotstringDefinition.Options.Nested) != HotstringDefinition.Options.Nested)
                    if (x > -1 && char.IsLetterOrDigit(history[x]))
                        return false;

                return true;
            }

            #endregion

            #region Abstract methods

            protected abstract void RegisterHook();

            protected abstract void DeregisterHook();
            
            protected internal abstract void Send(string keys);
            
            protected abstract void Backspace(int n);

            #endregion
        }
    }
}
