
namespace IronAHK.Rusty
{
    partial class Core
    {
        class GuiInfo
        {
            char delimiter;
            bool owndialogs = false;

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
        }
    }
}
