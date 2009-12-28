using System.Windows.Forms;

namespace IronAHK.Rusty
{
    partial class Core
    {
        class GUI : Form
        {
            StatusBar _StatusBar;
            public StatusBar StatusBar
            {
                get { return _StatusBar; }
                set { _StatusBar = value; }
            }

            ListView _ListView;
            public ListView ListView
            {
                get { return _ListView; }
                set { _ListView = value; }
            }

            TreeView _TreeView;
            public TreeView TreeView
            {
                get { return _TreeView; }
                set { _TreeView = value; }
            }
        }
    }
}
