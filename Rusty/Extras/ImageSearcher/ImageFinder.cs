using System;
using System.Drawing;
using System.Threading;

namespace IronAHK.Rusty
{
    class ImageFinder
    {
        ImageData sourceImage, findImage;
        PixelMask findPixel;
        Size[] regions;
        Point? match;
        ManualResetEvent[] resets;
        int threads = Environment.ProcessorCount;

        public ImageFinder(Bitmap source)
        {
            sourceImage = new ImageData(source);
        }

        public byte Variation { get; set; }

        public Point? Find(Bitmap findBitmap)
        {
            if (sourceImage == null || findBitmap == null)
                throw new InvalidOperationException();

            findImage = new ImageData(findBitmap);
            findImage.PixelMaskTable(this.Variation);

            var SourceRect = new Rectangle(new Point(0, 0), sourceImage.Size);
            var NeedleRect = new Rectangle(new Point(0, 0), findImage.Size);

            if (SourceRect.Contains(NeedleRect))
            {
                resets = new ManualResetEvent[threads];
                regions = new[] { sourceImage.Size, NeedleRect.Size };

                for (int i = 0; i < threads; i++)
                {
                    resets[i] = new ManualResetEvent(false);
                    ThreadPool.QueueUserWorkItem(new WaitCallback(ImageWorker), i);
                }

                WaitHandle.WaitAll(resets);

                return match;
            }

            return null;
        }

        public Point? Find(Color ColorId)
        {
            findPixel = new PixelMask(ColorId, Variation);

            resets = new ManualResetEvent[threads];
            regions = new[] { sourceImage.Size, new Size(1, 1) };

            for (int i = 0; i < threads; i++)
            {
                resets[i] = new ManualResetEvent(false);
                ThreadPool.QueueUserWorkItem(new WaitCallback(PixelWorker), i);
            }

            WaitHandle.WaitAll(resets);
            return match;
        }

        void PixelWorker(object state)
        {
            int index = (int)state;

            if (regions == null)
                throw new ArgumentNullException();

            foreach (Point Location in Core.MapTraverse(regions[0], regions[1]))
            {
                if (match.HasValue)
                    continue;

                var pix = sourceImage.Pixel[Location.X, Location.Y];

                if (findPixel.Equals(pix))
                {
                    lock (this)
                    {
                        if (!match.HasValue)
                            match = Location;
                    }
                    break;
                }
            }

            resets[index].Set();
        }

        void ImageWorker(object state)
        {
            int index = (int)state;

            if (regions == null)
                throw new ArgumentNullException();

            foreach (Point Location in Core.MapTraverse(regions[0], regions[1]))
            {
                if (match.HasValue)
                    continue;

                if (CompareAt(Location))
                {
                    lock (this)
                    {
                        if (!match.HasValue)
                            match = Location;
                    }
                    break;
                }
            }

            resets[index].Set();
        }

        bool CompareAt(Point findLocation)
        {
            for (int row = 0; row < findImage.Size.Height; row++)
            {
                for (int col = 0; col < findImage.Size.Width; col++)
                {
                    var SourcePix = sourceImage.Pixel[col + findLocation.X, row + findLocation.Y];
                    if (!findImage.PixelMask[col, row].Equals(SourcePix))
                        return false;
                }
            }

            return true;
        }

        #region Helpers

        class ImageData
        {
            readonly Size size;
            readonly Color[,] pixel;

            public ImageData(Bitmap bmp)
            {
                size = bmp.Size;
                pixel = ColorTable(bmp);
            }

            public PixelMask[,] PixelMask { get; set; }

            public Size Size
            {
                get { return Size; }
            }

            public Color[,] Pixel
            {
                get { return Pixel; }
            }

            Color[,] ColorTable(Bitmap bmp)
            {
                Color[,] colors = new Color[bmp.Width, bmp.Height];

                for (int row = 0; row < bmp.Height; row++)
                    for (int col = 0; col < bmp.Width; col++)
                        colors[col, row] = bmp.GetPixel(col, row);

                return colors;
            }

            public void PixelMaskTable(byte variation)
            {
                PixelMask = new PixelMask[Size.Width, Size.Height];

                for (int row = 0; row < Size.Height; row++)
                    for (int col = 0; col < Size.Width; col++)
                        PixelMask[col, row] = new PixelMask(Pixel[col, row], variation);
            }
        }

        class PixelMask
        {
            public PixelMask(Color color, byte variation)
            {
                Color = color;
                Variation = variation;
            }

            public PixelMask() : this(Color.Black, 0) { }

            public Color Color { get; set; }

            public byte Variation { get; set; }

            public bool Transparent
            {
                get { return Color.A == 0; }
            }

            public bool Exact
            {
                get { return Variation == 0; }
            }

            public bool Equals(Color match)
            {
                if (Transparent)
                    return true;

                if (Exact)
                    return Color == match;

                var r = match.R >= Color.R - Variation && match.R <= Color.R + Variation;
                var g = match.G >= Color.G - Variation && match.G <= Color.G + Variation;
                var b = match.B >= Color.B - Variation && match.B <= Color.B + Variation;

                return r && g && b;
            }
        }

        #endregion
    }
}
