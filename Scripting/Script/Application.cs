using System.Windows.Forms;

namespace IronAHK.Scripting
{
    partial class Script
    {
        public static void Init()
        {
            Application.EnableVisualStyles();
        }

        public static void Run()
        {
            Application.Run();
        }
    }
}
