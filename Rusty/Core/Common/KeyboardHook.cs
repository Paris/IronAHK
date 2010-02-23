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
            Keys keys;
            HotkeyEvent proc;
            bool enabled;

            public HotkeyDefinition(Keys keys, HotkeyEvent proc)
            {
                this.keys = keys;
                this.proc = proc;
                enabled = true;
            }

            public Keys Keys
            {
                get { return keys; }
            }

            public HotkeyEvent Proc
            {
                get { return proc; }
            }

            public bool Enabled
            {
                get { return enabled; }
                set { enabled = value; }
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
            List<HotkeyDefinition> hotkeys;
            List<HotstringDefinition> hotstrings;

            StringBuilder history;
            const int retention = 1024;

            public KeyboardHook()
            {
                hotkeys = new List<HotkeyDefinition>();
                hotstrings = new List<HotstringDefinition>();
                history = new StringBuilder(retention);

                RegisterHook();
            }

            ~KeyboardHook()
            {
                DeregisterHook();
            }

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

            protected void KeyReceived(Keys key)
            {
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

                foreach (var hotkey in hotkeys)
                {
                    if (hotkey.Keys == key && hotkey.Enabled)
                        hotkey.Proc(this, new HotkeyEventArgs(key));
                }

                foreach (var hotstring in hotstrings)
                {
                    if (sequence.EndsWith(hotstring.Sequence, StringComparison.OrdinalIgnoreCase) && hotstring.Enabled) // TODO: hotstring case sensitive matching
                        hotstring.Proc(this, new HotstringEventArgs(sequence));
                }
            }

            protected abstract void RegisterHook();

            protected abstract void DeregisterHook();
        }
    }
}
