using System.Drawing;
using System.Drawing.Imaging;
using System.Windows.Forms;

namespace IronAHK.Rusty
{
    partial class Core
    {
        static Bitmap GetScreen() { return GetScreen(Screen.PrimaryScreen.Bounds); }
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
                Bitmap bmp = new Bitmap(rect.Width, rect.Height, pFormat);
                Graphics g = Graphics.FromImage(bmp);
                g.CopyFromScreen(rect.Left, rect.Top, 0, 0, rect.Size);
                return bmp;
            }
            catch
            {
                Bitmap bmp2 = new Bitmap(0, 0, PixelFormat.Format24bppRgb);
                return bmp2;
            }
        }
    }
}
