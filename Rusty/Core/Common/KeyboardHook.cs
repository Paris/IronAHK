using System;
using System.Collections.Generic;
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
            GenericFunction proc;
            bool enabled;

            [Flags]
            public enum Options { None = 0, IgnoreModifiers = 1, PassThrough = 2, Up = 4 }

            #endregion

            public HotkeyDefinition(Keys keys, Keys extra, Options options, GenericFunction proc)
            {
                this.keys = keys;
                this.extra = extra;
                this.options = options;
                this.proc = proc;
                enabled = true;
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

            public GenericFunction Proc
            {
                get { return proc; }
                set { proc = value; }
            }

            public bool Enabled
            {
                get { return enabled; }
                set { enabled = value; }
            }

            #endregion

            public static HotkeyDefinition Parse(string sequence)
            {
                Keys keys = Keys.None, extra = Keys.None;
                Options options = Options.None;

                #region Modifiers

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
                                case Keyword_ModifierCtrl: extra = Keys.RControlKey; break;
                                case Keyword_ModifierShift: extra = Keys.RShiftKey; break;
                                default: throw new ArgumentException();
                            }
                            break;

                        case Keyword_ModifierWin: keys |= Keys.LWin; break;
                        case Keyword_ModifierAlt: keys |= Keys.Alt; break;
                        case Keyword_ModifierCtrl: keys |= Keys.Control; break;
                        case Keyword_ModifierShift: keys |= Keys.Shift; break;

                        case Keyword_HotkeyIgnoreModifiers: options |= Options.IgnoreModifiers; break;
                        case Keyword_HotkeyPassThrough: options |= Options.PassThrough; break;

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
                    }
                    sequence = sequence.Substring(0, z - 1).Trim();
                }

                if (sequence.EndsWith(Keyword_Up, StringComparison.OrdinalIgnoreCase))
                {
                    sequence = sequence.Substring(0, sequence.Length - Keyword_Up.Length).Trim();
                    options |= Options.Up;
                }

                keys |= ParseKey(sequence);

                return new HotkeyDefinition(keys, extra, options, null);
            }

            static Keys ParseKey(string name)
            {
                object value = Enum.Parse(typeof(Keys), name, true);
                return value == null ? Keys.None : (Keys)value;
            }
        }

        internal class HotstringDefinition
        {
            string sequence;
            string endchars;
            Options options;
            GenericFunction proc;
            bool enabled;

            [Flags]
            public enum Options { None = 0, AutoTrigger = 1, Nested = 2, Backspace = 4, CaseSensitive = 8, OmitEnding = 16, Raw = 32, Reset = 64 }

            public HotstringDefinition(string sequence, GenericFunction proc)
            {
                this.sequence = sequence;
                this.proc = proc;
            }

            internal void PreFilter()
            {
                if ((options & Options.Backspace) == Options.Backspace)
                    Send("{BS " + sequence.Length.ToString() + "}");
            }

            internal void PostFilter()
            {
                if ((options & Options.OmitEnding) == Options.OmitEnding && (options & Options.AutoTrigger) != Options.AutoTrigger)
                    Send("{BS}");
            }

            public string Sequence
            {
                get { return sequence; }
            }

            public string EndChars
            {
                get { return endchars; }
                set { endchars = value; }
            }

            public Options EnabledOptions
            {
                get { return options; }
                set { options = value; }
            }

            public GenericFunction Proc
            {
                get { return proc; }
            }

            public bool Enabled
            {
                get { return enabled; }
                set { enabled = value; }
            }

            public static Options ParseOptions(string mode)
            {
                var options = Options.None;

                mode = mode.ToLowerInvariant();

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
        }

        #endregion

        internal abstract class KeyboardHook
        {
            #region Properties

            List<HotkeyDefinition> hotkeys;
            List<HotstringDefinition> hotstrings;
            Dictionary<Keys, bool> pressed;

            StringBuilder history;
            const int retention = 1024;

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
                const string map = "abcdefghijklmnopqrstuvwxyz";
                var list = new Keys[] { Keys.A, Keys.B, Keys.D, Keys.E, Keys.F, Keys.G, Keys.H, Keys.I, Keys.J, Keys.K, Keys.L
                    , Keys.M, Keys.N, Keys.O, Keys.P, Keys.Q, Keys.R, Keys.S, Keys.T, Keys.U, Keys.V, Keys.W, Keys.X, Keys.Y, Keys.Z };

                for (int i = 0; i < list.Length; i++)
                {
                    if ((key & list[i]) == list[i])
                    {
                        char letter = map[i];

                        if ((key & Keys.Shift) == Keys.Shift) // TODO: check capslock
                            letter = letter.ToString().ToUpperInvariant()[0];

                        return letter;
                    }
                }

                return (char)0;
            }

            #endregion

            #region Hotkey fired

            protected bool KeyReceived(Keys key, bool down)
            {
                bool block = false;
                pressed[key] = down;

                foreach (var hotkey in hotkeys)
                {
                    bool match = (hotkey.Keys & key) == key;
                    bool up = (hotkey.EnabledOptions & HotkeyDefinition.Options.Up) == HotkeyDefinition.Options.Up;

                    if (hotkey.Enabled && match && HasModifiers(hotkey) && up != down)
                        new Thread(new ThreadStart(delegate() { hotkey.Proc(new object[] { }); })).Start();

                    if (match && (hotkey.EnabledOptions & HotkeyDefinition.Options.PassThrough) != HotkeyDefinition.Options.PassThrough)
                        block = true;
                }

                if (!down)
                    return block;

                #region Sequencing

                if (hotstrings.Count > 0)
                {
                    char letter = Letter(key);

                    if (letter != 0)
                    {
                        history.Append(letter);

                        if (history.Length == retention)
                            history.Remove(0, 1); // lifo stack
                    }
                }

                #endregion

                foreach (var hotstring in hotstrings)
                {
                    if (hotstring.Enabled && HasConditions(hotstring))
                    {
                        new Thread(new ThreadStart(delegate()
                        {
                            hotstring.PreFilter();
                            hotstring.Proc(new object[] { });
                            hotstring.PostFilter();
                        })).Start();

                        if ((hotstring.EnabledOptions & HotstringDefinition.Options.Reset) == HotstringDefinition.Options.Reset)
                            history.Length = 0;
                    }
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
                                       { (hotkey.Keys & Keys.Alt) == Keys.Alt , pressed[Keys.Alt] },
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
                    StringComparison.Ordinal : StringComparison.OrdinalIgnoreCase;

                int x = hotstring.Sequence.Length - 1;

                if ((hotstring.EnabledOptions & HotstringDefinition.Options.AutoTrigger) == HotstringDefinition.Options.AutoTrigger)
                {
                    if (!history.EndsWith(hotstring.Sequence, compare))
                        return false;
                    x--;
                }
                else
                {
                    if (history.Length < hotstring.Sequence.Length + 1)
                        return false;

                    if (hotstring.EndChars.IndexOf(history[history.Length - 1]) == -1)
                        return false;

                    if (!history.Substring(x, hotstring.Sequence.Length).Equals(hotstring.Sequence, compare))
                        return false;
                }

                if ((hotstring.EnabledOptions & HotstringDefinition.Options.Nested) == HotstringDefinition.Options.Nested)
                    if (x > -1 && !char.IsLetterOrDigit(history[x]))
                        return false;

                return true;
            }

            #endregion

            #region Abstract methods

            protected abstract void RegisterHook();

            protected abstract void DeregisterHook();

            #endregion
        }
    }
}
