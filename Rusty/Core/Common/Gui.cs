using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Windows.Forms;

namespace IronAHK.Rusty
{
    partial class Core
    {
        static string GuiId(ref string command)
        {
            string id = DefaultGuiId;

            if (command.Length == 0)
                return id;

            int z = command.IndexOf(':');
            string pre = string.Empty;

            if (z != -1)
            {
                pre = command.Substring(0, z).Trim();
                z++;
                command = z == command.Length ? string.Empty : command.Substring(z);
            }

            return pre.Length == 0 ? id : pre;
        }

        static Icon GetIcon(string source, int n)
        {
            if (Environment.OSVersion.Platform == PlatformID.Win32NT)
            {
                var prc = System.Diagnostics.Process.GetCurrentProcess().Handle;
                var icon = Windows.ExtractIcon(prc, source, n);

                if (icon != IntPtr.Zero)
                    return Icon.FromHandle(icon);
            }

            return Icon.ExtractAssociatedIcon(source);
        }

        static void LV_RowOptions(ref ListViewItem row, string options)
        {
            string[] opts = options.Split(new[] { ' ', '\t' });

            for (int i = 0; i < opts.Length; i++)
            {
                bool enable = true;
                char state = opts[i][0];
                if (state == '-')
                    enable = false;
                if (!enable || state == '+')
                    opts[i] = opts[i].Substring(1);

                string mode = opts[i].Trim().ToLowerInvariant();
                switch (opts[i].Trim().ToLowerInvariant())
                {
                    case Keyword_Check: row.Checked = enable; break;
                    case Keyword_Focus: row.Focused = enable; break;
                    case Keyword_Icon: row.ImageIndex = int.Parse(mode.Substring(4)); break;
                    case Keyword_Select: row.Selected = enable; break;
                    case Keyword_Vis: row.EnsureVisible(); break;
                }
            }
        }

        static void LV_ColOptions(ref ColumnHeader col, string options)
        {
            
        }

        static void TV_NodeOptions(ref TreeNode node, string options)
        {

        }

        static TreeNode TV_FindNode(TreeView parent, long id)
        {
            var match = parent.Nodes.Find(id.ToString(), true);
            return match.Length == 0 ? null : match[0];
        }

        static Bitmap GetScreen(Rectangle rect)
        {
            int color = Screen.PrimaryScreen.BitsPerPixel;
            PixelFormat pFormat;
            switch (color)
            {
                case 8:
                case 16:
                    pFormat = PixelFormat.Format16bppRgb565;
                    break;
                case 24: pFormat = PixelFormat.Format24bppRgb; break;
                case 32: pFormat = PixelFormat.Format32bppArgb; break;
                default: pFormat = PixelFormat.Format32bppArgb; break;
            }
            try
            {
                var bmp = new Bitmap(rect.Width, rect.Height, pFormat);
                Graphics g = Graphics.FromImage(bmp);
                g.CopyFromScreen(rect.Left, rect.Top, 0, 0, rect.Size);
                return bmp;
            }
            catch
            {
                var bmp2 = new Bitmap(0, 0, PixelFormat.Format24bppRgb);
                return bmp2;
            }
        }
    }
}
