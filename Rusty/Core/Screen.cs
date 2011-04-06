using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using IronAHK.Rusty.Common;

namespace IronAHK.Rusty
{
    partial class Core
    {
        // TODO: organise Screen.cs

        /// <summary>
        /// Searches a region of the screen for an image.
        /// </summary>
        /// <param name="OutputVarX">
        /// The X and Y coordinates of the upper left corner of the rectangle to search, which can be expressions. Coordinates are relative to the active window unless CoordMode was used to change that.
        /// </param>
        /// <param name="OutputVarY">See <paramref name="OutputVarX"/>.</param>
        /// <param name="X1">The X and Y coordinates of the upper left corner of the rectangle to search, which can be expressions. Coordinates are relative to the active window unless CoordMode was used to change that.</param>
        /// <param name="Y1">See <paramref name="X1"/>.</param>
        /// <param name="X2">The X and Y coordinates of the lower right corner of the rectangle to search, which can be expressions. Coordinates are relative to the active window unless CoordMode was used to change that.</param>
        /// <param name="Y2">See <paramref name="X2"/>.</param>
        /// <param name="OptionsImageFile">
        /// <para>The file name of an image, which is assumed to be in %A_WorkingDir% if an absolute path isn't specified. All operating systems support GIF, JPG, BMP, ICO, CUR, and ANI images (BMP images must be 16-bit or higher). Other sources of icons include the following types of files: EXE, DLL, CPL, SCR, and other types that contain icon resources. On Windows XP or later, additional image formats such as PNG, TIF, Exif, WMF, and EMF are supported. Operating systems older than XP can be given support by copying Microsoft's free GDI+ DLL into the AutoHotkey.exe folder (but in the case of a compiled script, copy the DLL into the script's folder). To download the DLL, search for the following phrase at www.microsoft.com: gdi redistributable</para>
        /// <para>Options: Zero or more of the following strings may be also be present immediately before the name of the file. Separate each option from the next with a single space or tab. For example: *2 *w100 *h-1 C:\Main Logo.bmp</para>
        /// <para>*IconN: To use an icon group other than the first one in the file, specify *Icon followed immediately by the number of the group. For example, *Icon2 would load the default icon from the second icon group.</para>
        /// <para>*n (variation): Specify for n a number between 0 and 255 (inclusive) to indicate the allowed number of shades of variation in either direction for the intensity of the red, green, and blue components of each pixel's color. For example, *2 would allow two shades of variation. This parameter is helpful if the coloring of the image varies slightly or if ImageFile uses a format such as GIF or JPG that does not accurately represent an image on the screen. If you specify 255 shades of variation, all colors will match. The default is 0 shades.</para>
        /// <para>*TransN: This option makes it easier to find a match by specifying one color within the image that will match any color on the screen. It is most commonly used to find PNG, GIF, and TIF files that have some transparent areas (however, icons do not need this option because their transparency is automatically supported). For GIF files, *TransWhite might be most likely to work. For PNG and TIF files, *TransBlack might be best. Otherwise, specify for N some other color name or RGB value (see the color chart for guidance, or use PixelGetColor in its RGB mode). Examples: *TransBlack, *TransFFFFAA, *Trans0xFFFFAA</para>
        /// <para>*wn and *hn: Width and height to which to scale the image (this width and height also determines which icon to load from a multi-icon .ICO file). If both these options are omitted, icons loaded from ICO, DLL, or EXE files are scaled to the system's default small-icon size, which is usually 16 by 16 (you can force the actual/internal size to be used by specifying *w0 *h0). Images that are not icons are loaded at their actual size. To shrink or enlarge the image while preserving its aspect ratio, specify -1 for one of the dimensions and a positive number for the other. For example, specifying *w200 *h-1 would make the image 200 pixels wide and cause its height to be set automatically.</para>
        /// </param>
        public static void ImageSearch(out int? OutputVarX, out int? OutputVarY, string X1, string Y1, string X2, string Y2, string OptionsImageFile)
        {
            OutputVarX = null;
            OutputVarY = null;

            var optsItems = new Dictionary<string, Regex>();
            optsItems.Add(Keyword_Icon, new Regex(@"\*icon([0-9]*)"));
            optsItems.Add(Keyword_Trans, new Regex(@"\*trans([0-9]*)"));
            optsItems.Add(Keyword_Variation, new Regex(@"\*([0-9]*)"));
            optsItems.Add("w", new Regex(@"\*w([-0-9]*)"));
            optsItems.Add("h", new Regex(@"\*h([-0-9]*)"));

            var opts = ParseOptionsRegex(ref OptionsImageFile, optsItems, true);
            OptionsImageFile = OptionsImageFile.Trim();

            Point start;
            Size bound;
            Bitmap find;

            try
            {
                start = new Point(int.Parse(X1), int.Parse(Y1));
                bound = new Size(int.Parse(X2) - start.X, int.Parse(Y2) - start.Y);
                find = (Bitmap)Bitmap.FromFile(OptionsImageFile);
            }
            catch (FormatException)
            {
                ErrorLevel = 2;
                return;
            }

            var source = GetScreen(new Rectangle(start, bound));
            byte variation = 0;

            if (opts.ContainsKey(Keyword_Variation))
                byte.TryParse(opts[Keyword_Variation], out variation);

            var SearchImg = new ImageFinder(source) { Variation = variation };

            Point? location;

            try
            {
                location = SearchImg.Find(find);
            }
            catch
            {
                ErrorLevel = 2;
                return;
            }

            if (location.HasValue)
            {
                OutputVarX = location.Value.X;
                OutputVarY = location.Value.Y;
                ErrorLevel = 0;
            }
            else
                ErrorLevel = 1;
        }

        /// <summary>
        /// Retrieves the color of the pixel at the specified x,y screen coordinates.
        /// </summary>
        /// <param name="OutputVar">The name of the variable in which to store the color ID in hexadecimal blue-green-red (BGR) format. For example, the color purple is defined 0x800080 because it has an intensity of 80 for its blue and red components but an intensity of 00 for its green component.</param>
        /// <param name="X">The X coordinate of the pixel, which can be expressions. Coordinates are relative to the active window unless CoordMode was used to change that.</param>
        /// <param name="Y">The Y coordinate of the pixel, see <paramref name="X"/>.</param>
        /// <param name="Color">
        /// <para>This parameter may contain following words.</para>
        /// <para>R: Retrieve Red color</para>
        /// <para>G: Retrieve Green color</para>
        /// <para>B: Retrieves Blue color</para>
        /// <para>All: Retrieves RGB color</para>
        /// </param>
        public static void PixelGetColor(out int OutputVar, int X, int Y, string Color)
        {
            int colour = Screen.PrimaryScreen.BitsPerPixel;
            PixelFormat format;
            switch (colour)
            {
                case 8:
                case 16:
                    format = PixelFormat.Format16bppRgb565;
                    break;
                case 24: format = PixelFormat.Format24bppRgb;
                    break;
                case 32: format = PixelFormat.Format32bppArgb;
                    break;
                default: format = PixelFormat.Format32bppArgb;
                    break;
            }
            var bmp = new Bitmap(1, 1, format);
            if (coords.Mouse == CoordModeType.Relative) //if coordmode Relative using relative coords
            {
                WindowsAPI.RECT rect;
                WindowsAPI.GetWindowRect(WindowsAPI.GetForegroundWindow(), out rect);
                X += rect.Left;
                Y += rect.Top;
            }
            Graphics.FromImage(bmp).CopyFromScreen(X, Y, 0, 0, new Size(1, 1));
            switch (Color.ToLowerInvariant())
            {
                case "r": OutputVar = bmp.GetPixel(0, 0).R; break;
                case "g": OutputVar = bmp.GetPixel(0, 0).G; break;
                case "b": OutputVar = bmp.GetPixel(0, 0).B; break;
                case "all":
                default: OutputVar = bmp.GetPixel(0, 0).ToArgb() & 0xffffff; break;
            }
            return;
            //OutputVarRGB = bmp.GetPixel(0, 0).ToArgb() & 0xffffff;
        }

        /// <summary>
        /// Searches a region of the screen for a pixel of the specified color.
        /// </summary>
        /// <param name="OutputVarX">
        /// <para>The names of the variables in which to store the X and Y coordinates of the first pixel that matches ColorID (if no match is found, the variables are made blank). Coordinates are relative to the active window unless CoordMode was used to change that.</para>
        /// <para>Either or both of these parameters may be left blank, in which case ErrorLevel (see below) can be used to determine whether a match was found.</para>
        /// </param>
        /// <param name="OutputVarY">See <paramref name="OutputVarX"/>.</param>
        /// <param name="X1">The X and Y coordinates of the upper left corner of the rectangle to search, which can be expressions. Coordinates are relative to the active window unless CoordMode was used to change that.</param>
        /// <param name="Y1">See <paramref name="X1"/>.</param>
        /// <param name="X2">The X and Y coordinates of the lower right corner of the rectangle to search, which can be expressions. Coordinates are relative to the active window unless CoordMode was used to change that.</param>
        /// <param name="Y2">See <paramref name="X2"/>.</param>
        /// <param name="ColorID">The decimal or hexadecimal color ID to search for, in Blue-Green-Red (BGR) format, which can be an expression. Color IDs can be determined using Window Spy (accessible from the tray menu) or via PixelGetColor. For example: 0x9d6346</param>
        /// <param name="Variation">A number between 0 and 255 (inclusive) to indicate the allowed number of shades of variation in either direction for the intensity of the red, green, and blue components of the color (can be an expression). This parameter is helpful if the color sought is not always exactly the same shade. If you specify 255 shades of variation, all colors will match. The default is 0 shades.</param>
        /// <param name="Fast_RGB">
        /// <para>This parameter may contain the word Fast, RGB, or both (if both are present, separate them with a space; that is, Fast RGB).</para>
        /// <para>Fast: Uses a faster searching method that in most cases dramatically reduces the amount of CPU time used by the search. Although color depths as low as 8-bit (256-color) are supported, the fast mode performs much better in 24-bit or 32-bit color. If the screen's color depth is 16-bit or lower, the Variation parameter might behave slightly differently in fast mode than it does in slow mode. Finally, the fast mode searches the screen row by row (top down) instead of column by column. Therefore, it might find a different pixel than that of the slow mode if there is more than one matching pixel.</para>
        /// <para>RGB: Causes ColorID to be interpreted as an RGB value instead of BGR. In other words, the red and blue components are swapped.</para>
        /// <para>ARGB: Causes ColorID to be interpreted as an ARGB value instead of BGR. In other words, the red and blue components are swapped and also alpha chanel is recognized.</para>
        /// </param>
        public static void PixelSearch(out int? OutputVarX, out int? OutputVarY, int X1, int Y1, int X2, int Y2, int ColorID, int Variation, string Fast_RGB)
        {
            OutputVarX = null;
            OutputVarY = null;
            Variation = Math.Max(byte.MinValue, Math.Min(byte.MaxValue, Variation));
            Fast_RGB = Fast_RGB.ToLowerInvariant();
            var region = new Rectangle(X1, Y1, X2 - X1, Y2 - Y1);
            var finder = new ImageFinder(GetScreen(region)) { Variation = (byte)Variation };
            Color needle;

            if (Fast_RGB.Contains(Keyword_ARGB))
                needle = Color.FromArgb(ColorID);
            else if (Fast_RGB.Contains(Keyword_RGB))
                needle = Color.FromArgb((int)((ColorID | (0xff << 24)) & 0xffffffff));
            else
                needle = Color.FromArgb(0xff, ColorID & 0xff, (ColorID >> 8) & 0xff, (ColorID >> 16) & 0xff);

            Point? location;

            try
            {
                location = finder.Find(needle);
            }
            catch
            {
                ErrorLevel = 2;
                return;
            }

            if (location.HasValue)
            {
                OutputVarX = location.Value.X;
                OutputVarY = location.Value.Y;
                ErrorLevel = 0;
            }
            else
                ErrorLevel = 1;
        }
    }
}
