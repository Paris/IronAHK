using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.IO;
using System.Threading;

namespace IronAHK.Rusty
{

    /// <summary>
    /// Creation:       02.08.2010 - IsNull
    /// Last Changes:   07.08.2010 - IsNull
    /// -----------------------------------
    /// 
    /// Provides easy to use multi threaded image comparisation and search Algorythms.
    /// 
    /// ToDo:
    /// 
    /// Implement Alpha Canal
    /// Speed it up! :)
    /// 
    /// </summary>
    public class SearchableImage
    {

        #region Private Data

        private int mVariation = 0; // 0-255
        private ImageData mSourceImageData = null;
        private ImageData mNeedleImageData = null;

        private PixelMask NeedlePixelMask;
        private Size[] Provider;
        private Point? FoundLocation = null;

        int ThreadCount = Environment.ProcessorCount;
        ManualResetEvent[] ResetEvents;
        object Locker = new object();

        #endregion

        #region Constructors

        /// <summary>
        /// Creates a new instance of a Searchable Image
        /// </summary>
        /// <param name="uSourceImage">Image to search withhin</param>
        public SearchableImage(Bitmap uSourceImage) {
            mSourceImageData = new ImageData(uSourceImage);
        }

        #endregion

        #region Public Propertys

        /// <summary>Variation of matcher (0 = exact match, 255 = every color matches)
        /// 
        /// </summary>
        public int Variation {
            get { return mVariation; }
            set {
                if(value >= 0 && value <= 255) {
                    mVariation = value;
                } else
                    throw new ArgumentException("Submitted variation is invalid!");
            }
        }

        #endregion

        #region Pubic Methods

        /// <summary>Searches the image.
        /// Searches a image in antoher image
        /// 
        /// 1. search the first pixel of the needle in the source
        /// 2. if found --> lookup required size for performance optimisation
        /// 3. if step 2. passed --> begin pixel per pixel comparisation, with (Variation)
        /// 
        /// </summary>
        /// <returns>If nothing is found [Point?]=null is returned, otherwise upperleft corner of found position.</returns>
        public Point? SearchImage(Bitmap uNeedleImage) {

            if(mSourceImageData == null || uNeedleImage == null)
                throw new InvalidOperationException();

            mNeedleImageData = new ImageData(uNeedleImage);
            mNeedleImageData.CreatePixelMaskTable(this.Variation);

            //Is the needle image conatined (physical size) in the source image?
            var SourceRect = new Rectangle(new Point(0, 0), mSourceImageData.Size);
            var NeedleRect = new Rectangle(new Point(0, 0), mNeedleImageData.Size);
            if(SourceRect.Contains(NeedleRect)) {

                ResetEvents = new ManualResetEvent[ThreadCount];
                Provider = new[] { mSourceImageData.Size, NeedleRect.Size };

                for(int i = 0; i < ThreadCount; i++) {
                    ResetEvents[i] = new ManualResetEvent(false);
                    ThreadPool.QueueUserWorkItem(new WaitCallback(SearchImageWorker), i);
                }

                //Wait until the work is complete!
                WaitHandle.WaitAll(ResetEvents);

                //Return FoundLocation (can be null in case if nothing was found!)
                return FoundLocation;
            }
            return null;
        }

        /// <summary>Searches a Pixel in a Image.
        /// 
        /// </summary>
        /// <param name="ColorId"></param>
        /// <returns>If nothing is found [Point?]=null is returned, otherwise upperleft corner of found position.</returns>
        public Point? SearchPixel(Color ColorId) {

            NeedlePixelMask = new PixelMask(ColorId, Variation);

            ResetEvents = new ManualResetEvent[ThreadCount];
            Provider = new[] { mSourceImageData.Size, new Size(1, 1) };

            for(int i = 0; i < ThreadCount; i++) {
                ResetEvents[i] = new ManualResetEvent(false);
                ThreadPool.QueueUserWorkItem(new WaitCallback(SearchPixelWorker), i);
            }
            WaitHandle.WaitAll(ResetEvents);
            return FoundLocation;
        }

        #endregion

        #region Private Methods

        private void SearchPixelWorker(object StateInfo) {
            Color pix = new Color();
            int index = (int)StateInfo;

            if(Provider == null)
                throw new ArgumentNullException();

            foreach (Point Location in Core.MapTraverse(Provider[0], Provider[1]))
            {
                if (FoundLocation.HasValue)
                    continue;
                pix = mSourceImageData.Pixel[Location.X, Location.Y];
                if(NeedlePixelMask.Compare(pix)) {
                    lock(Locker) {
                        if(!FoundLocation.HasValue)
                            FoundLocation = Location;
                    }
                    break;
                }
            }
            ResetEvents[index].Set();
        }

        private void SearchImageWorker(object StateInfo) {
            int index = (int)StateInfo;

            if(Provider == null)
                throw new ArgumentNullException();

            foreach (Point Location in Core.MapTraverse(Provider[0], Provider[1]))
            {
                if (FoundLocation.HasValue)
                    continue;
                if(CompareImage(Location)) {
                    lock(Locker) {
                        if(!FoundLocation.HasValue)
                            FoundLocation = Location;
                    }
                    break;
                }
            }
            ResetEvents[index].Set();
        }

        /// <summary>Searches the Needle Image withhin the Source Image at given Point. Variation and other Parameters are taken into account.
        /// 
        /// </summary>
        /// <param name="NeedleImageLocation"></param>
        /// <returns></returns>
        private bool CompareImage(Point NeedleImageLocation) {
            Color SourcePix;

            //If so, perform a comarisation for each pixel from needle:
            for(int row = 0; row < mNeedleImageData.Size.Height; row++)
                for(int col = 0; col < mNeedleImageData.Size.Width; col++) {
                    SourcePix = mSourceImageData.Pixel[col + NeedleImageLocation.X, row + NeedleImageLocation.Y];
                    if(!mNeedleImageData.MaskedPixel[col, row].Compare(SourcePix)) {
                        return false; //they don't match
                    }

                }
            return true;
        }
        #endregion

        #region Class ImageData

        private sealed class ImageData
        {
            public readonly Size Size;
            public readonly Color[,] Pixel;
            public PixelMask[,] MaskedPixel;

            public ImageData(Bitmap bmp) {
                Size = bmp.Size;
                Pixel = ColorTableFromBmp(bmp);
            }

            private Color[,] ColorTableFromBmp(Bitmap bmp) {
                Color[,] colors = new Color[bmp.Width, bmp.Height];

                for(int row = 0; row < bmp.Height; row++)
                    for(int col = 0; col < bmp.Width; col++) {
                        colors[col, row] = bmp.GetPixel(col, row);
                    }
                return colors;
            }

            /// <summary>Creates a PixelMask table from the given Pixel Data.
            /// 
            /// </summary>
            /// <param name="Variation"></param>
            public void CreatePixelMaskTable(int Variation){

                MaskedPixel = new PixelMask[Size.Width, Size.Height];

                 for(int row = 0; row < Size.Height; row++)
                    for(int col = 0; col < Size.Width; col++) {
                        MaskedPixel[col, row] = new PixelMask(Pixel[col, row], Variation);
                    }
            }

        }

        #endregion

        #region Struct  PixelMask

        private struct PixelMask
        {
            private bool FullTransparent;
            private bool ExactMatch;
            private Color ColorLower;
            private Color ColorHigher;


            public PixelMask(Color Color, int Variation) {

                FullTransparent = (Color.A == 0x00);
                ExactMatch = (Variation == 0);

                if(ExactMatch) {
                    ColorLower = Color;
                    ColorHigher = new Color();
                } else {
                    ColorLower = Color.FromArgb(Color.A, Color.R - Variation, Color.B - Variation, Color.G - Variation);
                    ColorHigher = Color.FromArgb(Color.A, Color.R + Variation, Color.B + Variation, Color.G + Variation);
                }
            }

            public bool Compare(Color SourceColor) {

                if(this.FullTransparent)
                    return true; //Full Transparent matches every pixel

                if(ExactMatch) {
                    //ToDo: we may compare only RGB without A chanel, as this doesn't care here and may give false results
                    return (ColorLower.ToArgb() == SourceColor.ToArgb());
                } else {
                    return (SourceColor.R >= ColorLower.R && SourceColor.R <= ColorHigher.R &&
                            SourceColor.G >= ColorLower.G && SourceColor.G <= ColorHigher.G &&
                            SourceColor.B >= ColorLower.B && SourceColor.B <= ColorHigher.B);
                }
            }
        }

        #endregion

    }
}