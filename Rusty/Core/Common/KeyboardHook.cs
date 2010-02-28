using System;
using System.Collections.Generic;
using System.Text;
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

            public enum Options { None, IgnoreModifiers, PassThrough, Up }

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
            string sequence, replace;
            HotstringEvent proc;
            bool enabled;

            public HotstringDefinition(string sequence, HotstringEvent proc)
            {
                this.sequence = sequence;
                this.proc = proc;
            }

            public HotstringDefinition(string sequence, string replace)
            {
                this.sequence = sequence;
                this.replace = replace;
                proc = Replace;
            }

            void Replace(object sender, EventArgs e)
            {
                if (replace != null)
                    Send(replace);
            }

            public string Sequence
            {
                get { return sequence; }
            }

            public HotstringEvent Proc
            {
                get { return proc; }
            }

            public bool Enabled
            {
                get { return enabled; }
                set { enabled = value; }
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

            protected void KeyReceived(Keys key, bool down)
            {
                pressed[key] = down;

                foreach (var hotkey in hotkeys)
                {
                    if (hotkey.Enabled && (hotkey.Keys & key) == key && HasModifiers(hotkey) &&
                        ((hotkey.EnabledOptions & HotkeyDefinition.Options.Up) == HotkeyDefinition.Options.Up ? !down : true))
                    {
                        if ((hotkey.EnabledOptions & HotkeyDefinition.Options.PassThrough) == HotkeyDefinition.Options.PassThrough)
                            PassThrough(hotkey.Keys);
                        hotkey.Proc(new object[] { });
                    }
                }

                if (!down)
                    return;

                string sequence = null;

                if (hotstrings.Count > 0)
                {
                    char letter = Letter(key);

                    if (letter != 0)
                    {
                        history.Append(letter);

                        if (history.Length > retention)
                            history.Remove(0, retention / 4);
                    }

                    sequence = history.ToString();
                }

                foreach (var hotstring in hotstrings)
                {
                    if (sequence.EndsWith(hotstring.Sequence, StringComparison.OrdinalIgnoreCase) && hotstring.Enabled) // TODO: hotstring case sensitive matching
                        hotstring.Proc(this, new HotstringEventArgs(sequence));
                }
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

            #endregion

            #region Abstract methods

            protected abstract void RegisterHook();

            protected abstract void DeregisterHook();

            protected abstract void PassThrough(Keys keys);

            #endregion
        }
    }
}
