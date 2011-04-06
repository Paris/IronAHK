using System;
using System.Collections.Generic;
using System.Text;

namespace IronAHK.Rusty.Common
{
    partial class Keyboard
    {
        internal class HotstringDefinition
        {
            string sequence;
            Core.GenericFunction proc;
            string name;

            [Flags]
            public enum Options { None = 0, AutoTrigger = 1, Nested = 2, Backspace = 4, CaseSensitive = 8, OmitEnding = 16, Raw = 32, Reset = 64 }

            public HotstringDefinition(string sequence, Core.GenericFunction proc)
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

            public Core.GenericFunction Proc
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
                        case Core.Keyword_HotstringAuto: change = Options.AutoTrigger; break;
                        case Core.Keyword_HotstringNested: change = Options.Nested; break;
                        case Core.Keyword_HotstringBackspace: change = Options.Backspace; break;
                        case Core.Keyword_HotstringCase: change = Options.CaseSensitive; break;
                        case Core.Keyword_HotstringOmitEnding: change = Options.OmitEnding; break;
                        case Core.Keyword_HotstringReset: change = Options.Reset; break;
                    }

                    if (change == Options.None)
                        continue;

                    int n = i + 1;
                    bool off = n < mode.Length && mode[n] == Core.Keyword_HotstringOff;

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
    }
}
