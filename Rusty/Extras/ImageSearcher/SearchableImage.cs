using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.IO;
using System.Threading;

namespace IronAHK.Rusty
{

    /// <summary>
    /// Provides easy to use multi threaded image comparisation and search Algorythms
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
        private int[,] NeedlePixelMask = null;
        private CoordProvider Provider;
        private Point? FoundLocation = null;

        int ThreadCount = Environment.ProcessorCount;
        ManualResetEvent[] ResetEvents;

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

            //Is the needle image conatined (physical size) in the source image?
            var SourceRect = new Rectangle(new Point(0, 0), mSourceImageData.Size);
            var NeedleRect = new Rectangle(new Point(0, 0), mNeedleImageData.Size);
            if(SourceRect.Contains(NeedleRect)) {

                ResetEvents = new ManualResetEvent[ThreadCount];
                Provider = new CoordProvider(mSourceImageData.Size, NeedleRect.Size);

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
        public Point? SearchPixel(int ColorId) {

            NeedlePixelMask = CreateComparerMask(ColorId);

            ResetEvents = new ManualResetEvent[ThreadCount];
            Provider = new CoordProvider(mSourceImageData.Size, new Size(1,1));

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
            Point? Location;
            Color pix;
            int index = (int)StateInfo;
            var locker = new object();

            if(Provider == null)
                throw new ArgumentNullException();

            while((Location = Provider.Next()) != null && !FoundLocation.HasValue) {

                pix = mSourceImageData.Pixel[Location.Value.X, Location.Value.Y];
                if(CompareWithMask(pix, NeedlePixelMask)) {
                    lock(locker) {
                        if(!FoundLocation.HasValue)
                            FoundLocation = Location.Value;
                    }
                    break;
                }
            }
            ResetEvents[index].Set();
        }

        private void SearchImageWorker(object StateInfo) {
            Point? Location;
            int index = (int)StateInfo;

            if(Provider == null)
                throw new ArgumentNullException();

            while((Location = Provider.Next()) != null && !FoundLocation.HasValue) {
                if(CompareImage(Location.Value)) {
                    FoundLocation = Location.Value;
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
            Color NeedlePix;
            Color SourcePix;

            /* This Code is obsolete as this is checked before
             * 
                // Check if the Needleimage at given Position fits into SourceImage
                var SourceBox = new Rectangle(new Point(0, 0), mSourceImageData.Size);
                var NeedleBox = new Rectangle(NeedleImageLocation, mNeedleImageData.Size);
                if(!SourceBox.Contains(NeedleBox))
                return false;
             * */

            //If so, perform a comarisation for each pixel from needle:
            for(int row = 0; row < mNeedleImageData.Size.Height; row++)
                for(int col = 0; col < mNeedleImageData.Size.Width; col++) {
                    NeedlePix = mNeedleImageData.Pixel[col, row];
                    SourcePix = mSourceImageData.Pixel[col + NeedleImageLocation.X, row + NeedleImageLocation.Y];
                    if(!CompareWithMask(SourcePix, CreateComparerMask(NeedlePix.ToArgb()))) {
                        return false; //they don't match
                    }
                }
            return true;
        }




        /// <summary>Create a Comarer Mask with given Variation
        /// 
        /// </summary>
        /// <param name="ColorID">Source Color</param>
        /// <returns></returns>
        private int[,] CreateComparerMask(int ColorID) {
            var c = new int[3, 2];

            for(int i = 0; i < 3; i++) {
                int t = ColorID >> (2 - i) * 8 & 0xff;
                c[i, 0] = t - this.Variation; c[i, 1] = t + this.Variation;
            }
            return c;
        }

        /// <summary>Compares given Color with Comparer Mask
        /// 
        /// </summary>
        /// <param name="GivenColor"></param>
        /// <param name="c"></param>
        /// <returns></returns>
        private bool CompareWithMask(Color GivenColor, int[,] c) {
            return (GivenColor.R >= c[0, 0] && GivenColor.R <= c[0, 1] &&
                         GivenColor.G >= c[1, 0] && GivenColor.G <= c[1, 1] &&
                         GivenColor.B >= c[2, 0] && GivenColor.B <= c[2, 1]);
        }

        #endregion

        #region Class ImageData

        private sealed class ImageData
        {
            public readonly Size Size;
            public readonly Color[,] Pixel;

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
        }

        #endregion
    }
}