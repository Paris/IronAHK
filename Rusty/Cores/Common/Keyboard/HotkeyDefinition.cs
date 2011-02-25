using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using System.Globalization;

namespace IronAHK.Rusty.Cores.Common.Keyboard
{
    internal class HotkeyDefinition
    {
        #region Properties

        Keys keys, extra;
        Options options;
        Core.GenericFunction precondition;
        string name;

        [Flags]
        public enum Options { None = 0, IgnoreModifiers = 1, PassThrough = 2, Up = 4 }

        #endregion

        public HotkeyDefinition(Keys keys, Keys extra, Options options, Core.GenericFunction proc) {
            this.keys = keys;
            this.extra = extra;
            this.options = options;
            Proc = proc;
            Enabled = true;
        }

        #region Accessors

        public Keys Keys {
            get { return keys; }
        }

        public Keys Extra {
            get { return extra; }
        }

        public Options EnabledOptions {
            get { return options; }
        }

        public IronAHK.Rusty.Core.GenericFunction Proc { get; set; }

        public IronAHK.Rusty.Core.GenericFunction Precondition {
            get { return precondition; }
            set { precondition = value; }
        }

        public bool Enabled { get; set; }

        public string Name {
            get { return name; }
            set { name = value; }
        }

        public string Typed { get; set; }

        #endregion

        public bool Condition() {
            if(precondition == null)
                return true;

            var result = precondition.Invoke(new object[] { });
            return result is bool ? (bool)result : result != null;
        }

        public static HotkeyDefinition Parse(string sequence) {
            Keys keys = Keys.None, extra = Keys.None;
            Options options = Options.None;
            string typed = string.Empty;

            #region Modifiers

            sequence = sequence.Replace(Core.Keyword_ModifierAltGr, new string(new[] { Core.Keyword_ModifierCtrl, Core.Keyword_ModifierAlt }));

            for(int i = 0; i < sequence.Length; i++) {
                switch(sequence[i]) {
                    case Core.Keyword_ModifierLeftPair:
                        i++;
                        if(i == sequence.Length)
                            throw new ArgumentException();
                        switch(sequence[i]) {
                            case Core.Keyword_ModifierWin: extra = Keys.LWin; break;
                            case Core.Keyword_ModifierAlt: extra = Keys.LMenu; break;
                            case Core.Keyword_ModifierCtrl: extra = Keys.LControlKey; break;
                            case Core.Keyword_ModifierShift: extra = Keys.LShiftKey; break;
                            default: throw new ArgumentException();
                        }
                        break;

                    case Core.Keyword_ModifierRightPair:
                        i++;
                        if(i == sequence.Length)
                            throw new ArgumentException();
                        switch(sequence[i]) {
                            case Core.Keyword_ModifierWin: extra = Keys.RWin; break;
                            case Core.Keyword_ModifierAlt: extra = Keys.RMenu; break;
                            case Core.Keyword_ModifierCtrl: extra = Keys.RControlKey; break;
                            case Core.Keyword_ModifierShift: extra = Keys.RShiftKey; break;
                            default: throw new ArgumentException();
                        }
                        break;

                    case Core.Keyword_ModifierWin: extra = Keys.LWin; break;
                    case Core.Keyword_ModifierAlt: keys |= Keys.Alt; break;
                    case Core.Keyword_ModifierCtrl: keys |= Keys.Control; break;
                    case Core.Keyword_ModifierShift: keys |= Keys.Shift; break;

                    case Core.Keyword_HotkeyIgnoreModifiers: options |= Options.IgnoreModifiers; break;
                    case Core.Keyword_HotkeyPassThrough: options |= Options.PassThrough; break;

                    case Core.Keyword_HotkeyNoRecurse: continue;

                    default:
                        if(i > 0)
                            sequence = sequence.Substring(i);
                        i = sequence.Length;
                        break;
                }
            }

            #endregion

            int z = sequence.IndexOf(Core.Keyword_HotkeyCombination);

            if(z != -1) {
                z++;
                if(z < sequence.Length) {
                    string alt = sequence.Substring(z).Trim();
                    extra = ParseKey(alt);

                    if(alt.Length == 1)
                        typed = alt;
                }
                sequence = sequence.Substring(0, z - 1).Trim();
            }

            z = sequence.LastIndexOf(Core.Keyword_Up, StringComparison.OrdinalIgnoreCase);

            if(z > 0 && char.IsWhiteSpace(sequence, z - 1)) {
                sequence = sequence.Substring(0, z).Trim();
                options |= Options.Up;
            }

            keys |= ParseKey(sequence);

            if(typed.Length == 0 && sequence.Length == 1)
                typed = sequence;

            return new HotkeyDefinition(keys, extra, options, null) { Typed = typed };
        }

        internal static Keys ParseKey(string name) {
            var value = Keys.None;
            int n;

            if(name.StartsWith(Core.Keyword_HotkeyVK, StringComparison.OrdinalIgnoreCase)) {
                name = name.Substring(Core.Keyword_HotkeyVK.Length);

                if(int.TryParse(name, out n) && n > -1)
                    value = (Keys)n;
            } else if(name.StartsWith(Core.Keyword_HotkeySC, StringComparison.OrdinalIgnoreCase) && Environment.OSVersion.Platform == PlatformID.Win32NT) {
                name = name.Substring(Core.Keyword_HotkeySC.Length);

                if(int.TryParse(name, NumberStyles.HexNumber, System.Threading.Thread.CurrentThread.CurrentCulture, out n) && n > -1)
                    value = (Keys)WindowsAPI.MapVirtualKeyEx((uint)n, WindowsAPI.MAPVK_VSC_TO_VK_EX, WindowsAPI.GetKeyboardLayout(0));
            }

            try {
                if(value == Keys.None)
                    value = (Keys)Enum.Parse(typeof(Keys), name, true);
            } catch(ArgumentException) {
                switch(name.ToLowerInvariant()) {
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

            switch(value) {
                case Keys.Control: value = Keys.ControlKey; break;
                case Keys.Shift: value = Keys.ShiftKey; break;
                case Keys.Alt: value = Keys.LMenu; break;
            }

            return value;
        }

        public override string ToString() {
            return name;
        }
    }
}
