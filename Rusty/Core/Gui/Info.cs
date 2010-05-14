using System.Drawing;

namespace IronAHK.Rusty
{
    partial class Core
    {
        class GuiInfo
        {
            char delimiter;
            bool owndialogs = false;
            Point section;

            public char Delimiter
            {
                get { return delimiter; }
                set { delimiter = value; }
            }

            public bool OwnDialogs
            {
                get { return owndialogs; }
                set { owndialogs = value; }
            }

            public Point Section
            {
                get { return section; }
                set { section = value; }
            }
        }
    }
}
