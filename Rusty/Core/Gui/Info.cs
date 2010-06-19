using System.Drawing;
using System.Windows.Forms;

namespace IronAHK.Rusty
{
    partial class Core
    {
        class GuiInfo
        {
            public char Delimiter { get; set; }

            public Point Section { get; set; }

            public StatusBar StatusBar { get; set; }
        }
    }
}
