using System;
using System.Drawing;
using System.Threading;

namespace IronAHK.Rusty.Common
{
    /// <summary>
    /// Class which provides common search Methods to find a Color or a subimage in given Image.
    /// </summary>
    class ImageFinder
    {
        #region Fields

        private ImageData sourceImage, findImage;
        private PixelMask findPixel;
        private CoordProvider Provider;
        private Point? match;
        private object matchLocker = new object();
        private ManualResetEvent[] resets;
        private int threads = Environment.ProcessorCount;   

        #endregion

        #region Constructor

        /// <summary>
        /// Creates a new Image Finder Instance
        /// </summary>
        /// <param name="source">Source Image where to search in</param>
        public ImageFinder(Bitmap source)
        {
            sourceImage = new ImageData(source);
        }

        #endregion

        #region Propertys

        public byte Variation { get; set; }

        #endregion

        #region Public Methods

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
                Provider = new CoordProvider(sourceImage.Size, NeedleRect.Size);

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
            Provider = new CoordProvider(sourceImage.Size, new Size(1, 1));

            for (int i = 0; i < threads; i++)
            {
                resets[i] = new ManualResetEvent(false);
                ThreadPool.QueueUserWorkItem(new WaitCallback(PixelWorker), i);
            }

            WaitHandle.WaitAll(resets);
            return match;
        }

        #endregion

        #region Private Methods

        void PixelWorker(object state)
        {
            Point? Location;
            Color pix = new Color();
            int index = (int)state;

            if(Provider == null)
                throw new ArgumentNullException();

            while((Location = Provider.Next()) != null && !match.HasValue) {

                pix = sourceImage.Pixel[Location.Value.X, Location.Value.Y];
                if(findPixel.Equals(pix)) {
                    lock(matchLocker) {
                        if(!match.HasValue)
                            match = Location.Value;
                    }
                    break;
                }
            }

            resets[index].Set();
        }

        void ImageWorker(object state)
        {
            Point? Location;
            int index = (int)state;

            if(Provider == null)
                throw new ArgumentNullException();

            while((Location = Provider.Next()) != null && !match.HasValue) {
                if(CompareAt(Location.Value)) {
                    lock(matchLocker) {
                        if(!match.HasValue)
                            match = Location.Value;
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

        #endregion

        #region Nested Helper Classes

        class ImageData
        {
            readonly Size _size;
            readonly Color[,] _pixel;

            public ImageData(Bitmap bmp)
            {
                _size = bmp.Size;
                _pixel = ColorTable(bmp);
            }

            public PixelMask[,] PixelMask { get; set; }

            public Size Size
            {
                get { return _size; }
            }

            public Color[,] Pixel
            {
                get { return _pixel; }
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
