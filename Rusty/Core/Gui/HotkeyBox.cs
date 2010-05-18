using System;
using System.Text;
using System.Windows.Forms;

namespace IronAHK.Rusty
{
    partial class Core
    {
        class HotkeyBox : TextBox
        {
            Keys key, mod;
            Limits limit;

            [Flags]
            public enum Limits
            {
                None = 0,
                PreventUnmodified = 1,
                PreventShiftOnly = 2,
                PreventControlOnly = 4,
                PreventAltOnly = 8,
                PreventShiftControl = 16,
                PreventShiftAlt = 32,
                PreventShiftControlAlt = 128,
            }

            public HotkeyBox()
            {
                key = mod = Keys.None;
                limit = Limits.None;
                Multiline = false;
                ContextMenu = new ContextMenu();
                Text = Enum.GetName(typeof(Keys), key);

                KeyPress += delegate(object sender, KeyPressEventArgs e)
                                {
                                    e.Handled = true;
                                };

                KeyUp += delegate(object sender, KeyEventArgs e)
                             {
                                 if (e.KeyCode == Keys.None && e.Modifiers == Keys.None)
                                     key = Keys.None;
                             };

                KeyDown += delegate(object sender, KeyEventArgs e)
                               {
                                   if (e.KeyCode == Keys.Back || e.KeyCode == Keys.Delete)
                                       key = mod = Keys.None;
                                   else
                                   {
                                       key = e.KeyCode;
                                       mod = e.Modifiers;
                                       Validate();
                                   }

                                   SetText();
                               };
            }

            public Limits Limit
            {
                get { return limit; }
                set { limit = value; }
            }

            void Validate()
            {
                Keys[,] sym = { { Keys.Control, Keys.ControlKey }, { Keys.Shift, Keys.ShiftKey }, { Keys.Alt, Keys.Menu } };

                for (int i = 0; i < 3; i++)
                {
                    if (key == sym[i, 1] && (mod & sym[i, 0]) == sym[i, 0])
                        mod &= ~sym[i, 0];
                }

                if ((limit & Limits.PreventUnmodified) == Limits.PreventUnmodified)
                {
                    if (mod == Keys.None)
                        key = Keys.None;
                }

                if ((limit & Limits.PreventShiftOnly) == Limits.PreventShiftOnly)
                {
                    if (mod == Keys.Shift)
                        key = mod = Keys.None;
                }

                if ((limit & Limits.PreventControlOnly) == Limits.PreventControlOnly)
                {
                    if (mod == Keys.Control)
                        key = mod = Keys.None;
                }

                if ((limit & Limits.PreventAltOnly) == Limits.PreventAltOnly)
                {
                    if (mod == Keys.Alt)
                        key = mod = Keys.None;
                }

                if ((limit & Limits.PreventShiftControl) == Limits.PreventShiftControl)
                {
                    if ((mod & Keys.Shift) == Keys.Shift && (mod & Keys.Control) == Keys.Control && (mod & Keys.Alt) != Keys.Alt)
                        key = mod = Keys.None;
                }

                if ((limit & Limits.PreventShiftAlt) == Limits.PreventShiftAlt)
                {
                    if ((mod & Keys.Shift) == Keys.Shift && (mod & Keys.Control) != Keys.Control && (mod & Keys.Control) == Keys.Alt)
                        key = mod = Keys.None;
                }

                if ((limit & Limits.PreventShiftControlAlt) == Limits.PreventShiftControlAlt)
                {
                    if ((mod & Keys.Shift) == Keys.Shift && (mod & Keys.Control) == Keys.Control && (mod & Keys.Control) == Keys.Alt)
                        key = mod = Keys.None;
                }
            }

            void SetText()
            {
                var buf = new StringBuilder(45);
                const string sep = " + ";

                if ((mod & Keys.Control) == Keys.Control)
                {
                    buf.Append(Enum.GetName(typeof(Keys), Keys.Control));
                    buf.Append(sep);
                }

                if ((mod & Keys.Shift) == Keys.Shift)
                {
                    buf.Append(Enum.GetName(typeof(Keys), Keys.Shift));
                    buf.Append(sep);
                }

                if ((mod & Keys.Alt) == Keys.Alt)
                {
                    buf.Append(Enum.GetName(typeof(Keys), Keys.Alt));
                    buf.Append(sep);
                }

                buf.Append(key.ToString());
                Text = buf.ToString();
            }
        }
    }
}
