using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace IronAHK.Rusty
{
    partial class Core
    {
        class GuiInfo
        {
            public GuiInfo()
            {
                Controls = new Stack<Control>();
            }

            public char Delimiter { get; set; }

            public Font Font { get; set; }

            public Point Section { get; set; }

            public StatusBar StatusBar { get; set; }

            public Stack<Control> Controls { get; set; }

            public Control LastControl
            {
                get { return Controls.Peek(); }
                set { Controls.Push(value); }
            }
        }
    }
}
